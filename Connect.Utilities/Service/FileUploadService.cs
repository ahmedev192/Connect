using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Connect.Utilities.Service.IService;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace Connect.Utilities.Service
{

    public class FileUploadService : IFileUploadService
    {
        private readonly IWebHostEnvironment _env;

        public FileUploadService(IWebHostEnvironment env)
        {
            _env = env;
        }
        public async Task<string?> SaveImageAsync(IFormFile file, string folderName)
        {
            if (file == null || file.Length == 0) return null;

            try
            {
                string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);
                string relativePath = Path.Combine("images", folderName, fileName);
                string absolutePath = Path.Combine(_env.WebRootPath, relativePath);

                // Ensure the folder exists
                Directory.CreateDirectory(Path.GetDirectoryName(absolutePath)!);

                // Save the file to disk
                using (var stream = new FileStream(absolutePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return the public-facing path
                return "/" + relativePath.Replace("\\", "/");
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        public Task<bool> DeleteImageAsync(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl)) return Task.FromResult(false);

            var relativePath = imageUrl.TrimStart('/');
            var absolutePath = Path.Combine(_env.WebRootPath, relativePath.Replace("/", Path.DirectorySeparatorChar.ToString()));

            try
            {
                if (File.Exists(absolutePath))
                {
                    File.Delete(absolutePath);
                    return Task.FromResult(true);
                }
            }
            catch (Exception ex)
            {
            }

            return Task.FromResult(false);
        }

    }

}
