using BackEnd.Application.Common.ResponseFormat;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Interfaces.Services
{
    public interface IFileService
    {
        Task<Result<string>> UploadFileAsync(IFormFile file, string targetFolder, string expectedType);
        Task<Result<List<string>>> UploadMultipleFilesAsync(IFormFileCollection files, string targetFolder, string expectedType);
        Task<string> CalculateFileHashAsync(IFormFile file);
        Result<bool> DeleteFile(string relativePath);
    }
}
