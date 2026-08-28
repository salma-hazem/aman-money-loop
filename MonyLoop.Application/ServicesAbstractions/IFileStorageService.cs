using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.ServicesAbstractions
{
    public interface IFileStorageService
    {
        Task<string> SaveAsync(IFormFile file, string subFolder, CancellationToken ct = default);

        Stream? OpenRead(string filePath);
        void Delete(string filePath);
    }
}
