using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Interfaces.Services;
using BackEnd.Domain.Entities.Identity;
using BackEnd.Domain.Entities.Identity.AuthenticationHepler;
using BackEnd.Infrastructure.Persistence.DbContext;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace BackEnd.Infrastructure.Services
{
    public class IdentityServies : IIdentityService
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly JwtSittings _jwt;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ApplicationDbContext _context;
        public IdentityServies(UserManager<ApplicationUser> userManager,
            JwtSittings jwt, IHttpContextAccessor httpContextAccessor,
            ApplicationDbContext context)
        {
            _userManager = userManager;
            _jwt = jwt;
            _httpContextAccessor = httpContextAccessor;
            _context = context;
        }

        public async Task<Result<string>> CreateJwtToken(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            if (user == null)
                return Result<string>.Failure("User is null", ErrorType.BadRequest);

            var userClaims = await _userManager.GetClaimsAsync(user);
            var roles = await _userManager.GetRolesAsync(user);
            var roleClaims = roles.Select(role => new Claim("roles", role)).ToList();

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.UserName ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim("uid", user.Id),
            }
            .Union(userClaims)
            .Union(roleClaims);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _jwt.Issuer,
                audience: _jwt.Audience,
                claims: claims,
                expires: DateTime.Now.AddHours(_jwt.DurationInHours),
                signingCredentials: creds);

            return Result<string>.Success(new JwtSecurityTokenHandler().WriteToken(token));
        }

        public async Task<Result<ResponseAuthModel>> RefreshTokenAsunc(string token)
        {
            var user = await _userManager.Users
                .Include(u => u.refreshTokens)
                .SingleOrDefaultAsync(u => u.refreshTokens.Any(t => t.token == token));

            if (user == null)
                return Result<ResponseAuthModel>.Failure("User Not Found", ErrorType.NotFound);

            var refreshToken = user.refreshTokens.Single(t => t.token == token);

            if (!refreshToken.IsActive)
                return Result<ResponseAuthModel>.Failure("Inactive Token", ErrorType.Unauthorized);

            refreshToken.RevokedOn = DateTime.UtcNow;
            var newRefreshToken = GenerateRefreshToken();
            user.refreshTokens.Add(newRefreshToken);
            await _userManager.UpdateAsync(user);

            var jwtToken = await CreateJwtToken(user);
            if (!jwtToken.IsSuccess)
                return Result<ResponseAuthModel>.Failure(jwtToken.Message, jwtToken.ErrorType);

            var roles = await _userManager.GetRolesAsync(user);

            return Result<ResponseAuthModel>.Success(new ResponseAuthModel
            {
                Token = jwtToken.Value,
                Roles = roles.ToList(),
                RefreshToken = newRefreshToken.token,
                RefreshTokenExpiration = newRefreshToken.ExpireOn,
                CookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = newRefreshToken.ExpireOn,
                    Path = "/"
                }
            });
        }

        public async Task<Result<bool>> RevokeRefreshTokenFromCookiesAsync()
        {
            var refreshToken = _httpContextAccessor.HttpContext?.Request.Cookies["RefreshToken"];

            if (string.IsNullOrEmpty(refreshToken))
                return Result<bool>.Failure("Refresh token not found in cookies", ErrorType.NotFound);

            var user = await _userManager.Users
                .Include(u => u.refreshTokens)
                .FirstOrDefaultAsync(u => u.refreshTokens.Any(t => t.token == refreshToken));

            if (user == null)
                return Result<bool>.Failure("User not found", ErrorType.NotFound);

            var token = user.refreshTokens.SingleOrDefault(t => t.token == refreshToken);

            if (token == null || !token.IsActive)
                return Result<bool>.Failure("Token not active", ErrorType.Unauthorized);

            token.RevokedOn = DateTime.UtcNow;

            var result = await _userManager.UpdateAsync(user);
            if (!result.Succeeded)
                return Result<bool>.Failure("Failed to revoke token", ErrorType.InternalServerError);

            _httpContextAccessor.HttpContext.Response.Cookies.Delete("RefreshToken", new CookieOptions
            {
                Path = "/",
                HttpOnly = true,
                Secure = true,
                SameSite = SameSiteMode.Strict
            });

            return Result<bool>.Success(true);
        }

        public RefreshToken GenerateRefreshToken()
        {
            var randomNumber = new byte[32];
            using var generator = new RNGCryptoServiceProvider();
            generator.GetBytes(randomNumber);

            return new RefreshToken
            {
                token = Convert.ToBase64String(randomNumber),
                CreatedOn = DateTime.UtcNow,
                ExpireOn = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<Result<ResponseAuthModel>> GenerateRefreshTokenAsync(ApplicationUser user, bool rememberMe, CancellationToken cancellationToken = default)
        {
            if (user == null)
                return Result<ResponseAuthModel>.Failure("User is null", ErrorType.BadRequest);

            var jwtToken = await CreateJwtToken(user);
            if (!jwtToken.IsSuccess)
                return Result<ResponseAuthModel>.Failure(jwtToken.Message, jwtToken.ErrorType);

            RefreshToken refreshToken;
            var existingActiveToken = user.refreshTokens.FirstOrDefault(r => r.IsActive);

            if (existingActiveToken != null)
            {
                refreshToken = existingActiveToken;
            }
            else
            {
                refreshToken = GenerateRefreshToken();
                if (rememberMe)
                    refreshToken.ExpireOn = DateTime.UtcNow.AddDays(30);

                user.refreshTokens.Add(refreshToken);
                await _userManager.UpdateAsync(user);
            }

            var roles = await _userManager.GetRolesAsync(user);

            return Result<ResponseAuthModel>.Success(new ResponseAuthModel
            {
                Message = "Login successful.",
                Token = jwtToken.Value,
                Roles = roles.ToList(),
                RefreshToken = refreshToken.token,
                RefreshTokenExpiration = refreshToken.ExpireOn,
                CookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = refreshToken.ExpireOn,
                    Path = "/"
                }
            });
        }

        public async Task<Result<bool>> IsEmailExist(string email, CancellationToken cancellationToken = default)
        {
            var user = await _userManager.FindByEmailAsync(email);
            return Result<bool>.Success(user != null);
        }

        public async Task<Result<bool>> IsInRole(string userId, string role)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<bool>.Failure("User not found", ErrorType.NotFound);

            var inRole = await _userManager.IsInRoleAsync(user, role);
            return Result<bool>.Success(inRole);
        }

        public async Task<Result<ApplicationUser>> IsUserExist(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
                return Result<ApplicationUser>.Failure("User not found", ErrorType.NotFound);

            return Result<ApplicationUser>.Success(user);
        }

        public async Task<Result<IdentityResult>> CreateUserAsync(ApplicationUser user, string password,string Role ,CancellationToken cancellationToken = default)
        {
            var createResult = await _userManager.CreateAsync(user, password);
            if (!createResult.Succeeded)
                return Result<IdentityResult>.Failure("Failed to create user", ErrorType.BadRequest);

            var roleResult = await _userManager.AddToRoleAsync(user, Role);
            if (!roleResult.Succeeded)
                return Result<IdentityResult>.Failure("Failed to assign role", ErrorType.InternalServerError);

            return Result<IdentityResult>.Success(roleResult);
        }

        public async Task<Result<bool>> IsPasswordExist(ApplicationUser user, string Password, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.CheckPasswordAsync(user, Password);
            return Result<bool>.Success(result);
        }

        public async Task<Result<ApplicationUser?>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            var result = await _userManager.FindByEmailAsync(email);
            if (result == null)
                return Result<ApplicationUser?>.Failure("User not found", ErrorType.NotFound);

            return Result<ApplicationUser?>.Success(result);
        }

        public async Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword)
        {
            return await _userManager.ResetPasswordAsync(user, token, newPassword);
        }
        public string? GetUserId()
        {
            return _httpContextAccessor.HttpContext?.User?.FindFirst("uid")?.Value;
        }

        public async Task<string> GetRestPasswordTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default)
        {
            var token = await _userManager.GeneratePasswordResetTokenAsync(user);
            var tokenBytes = Encoding.UTF8.GetBytes(token);
            return WebEncoders.Base64UrlEncode(tokenBytes);
        }

    }
}
