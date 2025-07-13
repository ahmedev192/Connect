using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace Connect.Utilities.Service.IService
{
    public interface IFileUploadService
    {
        Task<string?> SavePostImageAsync(IFormFile file, string folderName );
    }
}
