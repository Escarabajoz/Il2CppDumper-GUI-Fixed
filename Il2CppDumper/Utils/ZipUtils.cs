using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;

namespace Il2CppDumper.Utils
{
    internal class ZipUtils
    {
        private const long MaxFileSize = 100 * 1024 * 1024; // 100MB limit per file
        
        public static void ExtractFile(ZipArchiveEntry entry, string destinationPath)
        {
            // Validate entry name to prevent path traversal
            if (string.IsNullOrEmpty(entry.Name) || entry.Name.Contains(".."))
            {
                throw new SecurityException($"Unsafe file name detected: {entry.Name}");
            }

            // Check file size to prevent zip bombs
            if (entry.Length > SecurityUtils.FileSizeLimits.ZipEntry)
            {
                throw new SecurityException($"File too large: {entry.Name} ({entry.Length} bytes)");
            }

            // Sanitize filename using security utils
            string sanitizedName = SecurityUtils.SanitizeFileName(Path.GetFileName(entry.Name));
            string fullPath = Path.GetFullPath(Path.Combine(destinationPath, sanitizedName));
            
            // Validate path is within destination directory
            if (!SecurityUtils.IsPathWithinDirectory(fullPath, destinationPath))
            {
                throw new SecurityException($"Path traversal attempt detected: {entry.Name}");
            }

            // Validate file extension (optional - add allowed extensions as needed)
            string extension = Path.GetExtension(sanitizedName).ToLowerInvariant();
            var allowedExtensions = new[] { ".dat", ".so", ".dll", ".exe", ".py", ".json", ".txt", ".cs" };
            if (!string.IsNullOrEmpty(extension) && !allowedExtensions.Contains(extension))
            {
                throw new SecurityException($"File type not allowed: {extension}");
            }

            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(fullPath));
                entry.ExtractToFile(fullPath, true);
            }
            catch (Exception ex)
            {
                throw new SecurityException($"Error extracting file {entry.Name}: {ex.Message}", ex);
            }
        }
    }
}
