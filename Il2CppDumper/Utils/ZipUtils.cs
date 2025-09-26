using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Il2CppDumper.Utils
{
    internal class ZipUtils
    {
        // Maximum file size for extraction (100MB)
        private const long MaxFileSize = 100 * 1024 * 1024;
        
        // Maximum total extraction size (500MB)
        private const long MaxTotalExtractionSize = 500 * 1024 * 1024;
        
        // Maximum number of files to extract
        private const int MaxFileCount = 10000;

        public static void ExtractFile(ZipArchiveEntry entry, string destinationPath)
        {
            // Validate entry
            if (entry == null)
                throw new ArgumentNullException(nameof(entry));
            
            if (string.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("Destination path cannot be null or empty", nameof(destinationPath));

            // Check file size
            if (entry.Length > MaxFileSize)
                throw new InvalidOperationException($"File {entry.Name} is too large ({entry.Length} bytes). Maximum allowed: {MaxFileSize} bytes.");

            // Sanitize the entry name to prevent path traversal
            string sanitizedName = SanitizeFileName(entry.Name);
            if (string.IsNullOrEmpty(sanitizedName))
                throw new InvalidOperationException($"Invalid or dangerous file name: {entry.Name}");

            string fullPath = Path.Combine(destinationPath, sanitizedName);
            
            // Ensure the full path is within the destination directory
            string normalizedDestination = Path.GetFullPath(destinationPath);
            string normalizedFullPath = Path.GetFullPath(fullPath);
            
            if (!normalizedFullPath.StartsWith(normalizedDestination + Path.DirectorySeparatorChar) && 
                !normalizedFullPath.Equals(normalizedDestination))
            {
                throw new InvalidOperationException($"Path traversal attempt detected: {entry.Name}");
            }

            // Create directory safely
            string directoryPath = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            // Extract file
            entry.ExtractToFile(fullPath, true);
        }

        public static void ExtractArchiveSafely(ZipArchive archive, string destinationPath, out long totalExtractedSize)
        {
            if (archive == null)
                throw new ArgumentNullException(nameof(archive));

            totalExtractedSize = 0;
            int fileCount = 0;

            foreach (var entry in archive.Entries)
            {
                fileCount++;
                if (fileCount > MaxFileCount)
                    throw new InvalidOperationException($"Archive contains too many files. Maximum allowed: {MaxFileCount}");

                totalExtractedSize += entry.Length;
                if (totalExtractedSize > MaxTotalExtractionSize)
                    throw new InvalidOperationException($"Archive extraction would exceed size limit. Maximum allowed: {MaxTotalExtractionSize} bytes.");

                if (!string.IsNullOrEmpty(entry.Name)) // Skip directories
                {
                    ExtractFile(entry, destinationPath);
                }
            }
        }

        private static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            // Remove path traversal sequences
            fileName = fileName.Replace("..", "").Replace("./", "").Replace(".\\", "");
            
            // Remove or replace invalid characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            // Remove leading/trailing whitespace and dots
            fileName = fileName.Trim().Trim('.');
            
            // Ensure it's not empty after sanitization
            if (string.IsNullOrWhiteSpace(fileName))
                return null;

            // Limit file name length
            if (fileName.Length > 255)
                fileName = fileName.Substring(0, 255);

            return fileName;
        }
    }
}
