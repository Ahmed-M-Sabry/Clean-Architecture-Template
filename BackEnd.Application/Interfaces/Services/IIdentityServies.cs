using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Domain.Entities.Identity;
using BackEnd.Domain.Entities.Identity.AuthenticationHepler;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using System.Threading.Tasks;

namespace BackEnd.Application.Interfaces.Services
{
    public interface IIdentityService
    {
        Task<Result<ResponseAuthModel>> RefreshTokenAsunc(string token);

        Task<Result<ResponseAuthModel>> GenerateRefreshTokenAsync(ApplicationUser user, bool rememberMe, CancellationToken cancellationToken = default);

        Task<Result<bool>> RevokeRefreshTokenFromCookiesAsync();

        Task<Result<bool>> IsInRole(string userId, string role);

        Task<Result<ApplicationUser>> IsUserExist(string userId);

        Task<Result<ApplicationUser?>> GetUserByEmailAsync(string email, CancellationToken cancellationToken = default);

        Task<Result<bool>> IsEmailExist(string email, CancellationToken cancellationToken = default);

        Task<Result<bool>> IsPasswordExist(ApplicationUser user, string Password, CancellationToken cancellationToken = default);
        Task<Result<IdentityResult>> CreateUserAsync(ApplicationUser user, string password,string Role, CancellationToken cancellationToken = default);

        Task<Result<string>> CreateJwtToken(ApplicationUser user, CancellationToken cancellationToken = default);
        Task<IdentityResult> ResetPasswordAsync(ApplicationUser user, string token, string newPassword);
        Task<string> GetRestPasswordTokenAsync(ApplicationUser user, CancellationToken cancellationToken = default);
        string? GetUserId();

    }
}
