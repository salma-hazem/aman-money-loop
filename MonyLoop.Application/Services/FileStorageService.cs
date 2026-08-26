using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using MonyLoop.Application.ServicesAbstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MonyLoop.Application.Services
{
    public class FileStorageService : IFileStorageService
    {
        private readonly string _rootPath;

        public FileStorageService(IConfiguration config)
        {
            _rootPath = config["FileStorage:RootPath"] ?? Path.Combine(Directory.GetCurrentDirectory(), "AppFiles");
        }

        public async Task<string> SaveAsync(IFormFile file, string subFolder, CancellationToken ct = default)
        {
            var folder = Path.Combine(_rootPath, subFolder);
            Directory.CreateDirectory(folder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(folder, fileName);

            await using var stream = new FileStream(fullPath, FileMode.Create);
            await file.CopyToAsync(stream, ct);

            return Path.Combine(subFolder, fileName).Replace("\\", "/");
        }

        public void Delete(string filePath)
        {
            var fullPath = Path.Combine(_rootPath, filePath);
            if (File.Exists(fullPath)) File.Delete(fullPath);
        }
    }
