using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Connect.Application.Interfaces
{
    public interface IFileUploadService
    {
        Task<string?> SaveImageAsync(IFormFile file, string folderName );
        Task<bool> DeleteImageAsync(string imageUrl);
        string ResolveImageOrDefault(string? imageUrl,  string fallbackRelativePath );
    }
}
