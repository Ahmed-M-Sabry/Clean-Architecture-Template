using BackEnd.Application.Common.ResponseFormat;
using BackEnd.Application.Interfaces.Services;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Infrastructure.Services
{
    public class FileService : IFileService
    {
        private readonly IWebHostEnvironment _webHostEnvironment;

        public FileService(IWebHostEnvironment webHostEnvironment)
        {
            _webHostEnvironment = webHostEnvironment;
        }

        public async Task<Result<string>> UploadFileAsync(IFormFile file, string targetFolder, string expectedType)
        {
            if (file == null || file.Length == 0)
                return Result<string>.Failure("File is empty.", ErrorType.BadRequest);

            var extValidation = ValidateExtension(file, expectedType);
            if (!extValidation.IsSuccess)
                return extValidation;

            var mimeValidation = ValidateMimeType(file, expectedType);
            if (!mimeValidation.IsSuccess)
                return mimeValidation;

            var sizeValidation = ValidateFileSize(file, 10 * 1024 * 1024); // 10MB
            if (!sizeValidation.IsSuccess)
                return Result<string>.Failure(sizeValidation.Message, ErrorType.BadRequest);

            string webRoot = _webHostEnvironment.WebRootPath;
            if (string.IsNullOrWhiteSpace(webRoot))
            {
                webRoot = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot");
            }

            string uploadsFolder = Path.Combine(webRoot, "Uploads", targetFolder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            string uniqueFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            return Result<string>.Success($"/Uploads/{targetFolder}/{uniqueFileName}");
        }

        public async Task<Result<List<string>>> UploadMultipleFilesAsync(IFormFileCollection files, string targetFolder, string expectedType)
        {
            if (files == null || files.Count == 0)
                return Result<List<string>>.Failure("No files were provided.", ErrorType.BadRequest);

            var uploadedPaths = new List<string>();
            string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "Uploads", targetFolder);

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            foreach (var file in files)
            {
                var extValidation = ValidateExtension(file, expectedType);
                if (!extValidation.IsSuccess)
                    return Result<List<string>>.Failure($"File '{file.FileName}' failed: {extValidation.Message}", ErrorType.BadRequest);

                var mimeValidation = ValidateMimeType(file, expectedType);
                if (!mimeValidation.IsSuccess)
                    return Result<List<string>>.Failure($"File '{file.FileName}' failed: {mimeValidation.Message}", ErrorType.BadRequest);

                var sizeValidation = ValidateFileSize(file, 10 * 1024 * 1024);
                if (!sizeValidation.IsSuccess)
                    return Result<List<string>>.Failure($"File '{file.FileName}' failed: {sizeValidation.Message}", ErrorType.BadRequest);

                string uniqueFileName = $"{Guid.NewGuid()}_{DateTime.UtcNow:yyyyMMddHHmmss}{Path.GetExtension(file.FileName)}";
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                uploadedPaths.Add($"/Uploads/{targetFolder}/{uniqueFileName}");
            }

            return Result<List<string>>.Success(uploadedPaths);
        }

        public Result<bool> DeleteFile(string relativePath)
        {
            try
            {
                string fullPath = Path.Combine(_webHostEnvironment.WebRootPath, relativePath.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString()));

                if (File.Exists(fullPath))
                {
                    File.Delete(fullPath);
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Failure("File not found.", ErrorType.NotFound);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure($"Error deleting file: {ex.Message}", ErrorType.InternalServerError);
            }
        }

        public async Task<string> CalculateFileHashAsync(IFormFile file)
        {
            using (var md5 = MD5.Create())
            using (var stream = file.OpenReadStream())
            {
                var hash = await md5.ComputeHashAsync(stream);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private Result<string> ValidateExtension(IFormFile file, string expectedType)
        {
            var allowedImageExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
            var allowedVideoExtensions = new[] { ".mp4", ".avi", ".mov" };
            var fileExtension = Path.GetExtension(file.FileName).ToLower();

            if (expectedType == "image" && !allowedImageExtensions.Contains(fileExtension))
                return Result<string>.Failure("Invalid file type. Only JPG, JPEG, PNG, GIF, or WEBP are allowed.", ErrorType.BadRequest);

            if (expectedType == "video" && !allowedVideoExtensions.Contains(fileExtension))
                return Result<string>.Failure("Invalid file type. Only MP4, AVI, or MOV are allowed.", ErrorType.BadRequest);

            return Result<string>.Success("Extension is valid.");
        }

        private Result<string> ValidateMimeType(IFormFile file, string expectedType)
        {
            var allowedImageMimeTypes = new[] { "image/jpeg", "image/png", "image/jpg", "image/gif", "image/webp" };
            var allowedVideoMimeTypes = new[] { "video/mp4", "video/avi", "video/quicktime" };
            var mimeType = file.ContentType.ToLowerInvariant();

            if (expectedType == "image" && !allowedImageMimeTypes.Contains(mimeType))
                return Result<string>.Failure("Invalid MIME type. Only image files are allowed.", ErrorType.BadRequest);

            if (expectedType == "video" && !allowedVideoMimeTypes.Contains(mimeType))
                return Result<string>.Failure("Invalid MIME type. Only video files are allowed.", ErrorType.BadRequest);

            return Result<string>.Success("MIME type is valid.");
        }

        public Result<bool> ValidateFileSize(IFormFile file, long maxSizeInBytes)
        {
            if (file == null)
                return Result<bool>.Failure("File is null.", ErrorType.BadRequest);

            if (file.Length > maxSizeInBytes)
                return Result<bool>.Failure($"File size exceeds {maxSizeInBytes / 1024 / 1024} MB limit.", ErrorType.BadRequest);

            return Result<bool>.Success(true);
        }
    }
}
