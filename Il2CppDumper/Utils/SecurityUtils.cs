using System;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;

namespace Il2CppDumper.Utils
{
    /// <summary>
    /// Utility class for security-related operations
    /// </summary>
    internal static class SecurityUtils
    {
        private static readonly Regex HexPattern = new Regex(@"^[0-9A-Fa-f]+$", RegexOptions.Compiled);
        private static readonly Regex SafePathPattern = new Regex(@"^[a-zA-Z0-9\-_\.\s\\\/\:]+$", RegexOptions.Compiled);
        
        /// <summary>
        /// Validates if a string contains only hexadecimal characters
        /// </summary>
        public static bool IsValidHexString(string input)
        {
            if (string.IsNullOrEmpty(input))
                return false;
            
            return HexPattern.IsMatch(input);
        }
        
        /// <summary>
        /// Validates if a path is safe and doesn't contain malicious characters
        /// </summary>
        public static bool IsValidPath(string path)
        {
            if (string.IsNullOrEmpty(path))
                return false;
                
            // Check for path traversal attempts
            if (path.Contains("..") || path.Contains("~"))
                return false;
                
            // Check for command injection attempts
            string[] dangerousChars = { "&", "|", ";", "$", "`", "<", ">", "(", ")", "{", "}", "[", "]", "^" };
            foreach (string dangerousChar in dangerousChars)
            {
                if (path.Contains(dangerousChar))
                    return false;
            }
            
            return SafePathPattern.IsMatch(path);
        }
        
        /// <summary>
        /// Sanitizes a filename by removing invalid characters
        /// </summary>
        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return "unknown";
                
            string sanitized = string.Join("_", fileName.Split(Path.GetInvalidFileNameChars()));
            
            // Remove any remaining dangerous patterns
            sanitized = Regex.Replace(sanitized, @"\.{2,}", "_");
            sanitized = Regex.Replace(sanitized, @"[<>:""\|?*]", "_");
            
            return sanitized.Length > 100 ? sanitized.Substring(0, 100) : sanitized;
        }
        
        /// <summary>
        /// Validates file size against specified limits
        /// </summary>
        public static bool IsFileSizeValid(string filePath, long maxSizeBytes)
        {
            try
            {
                var fileInfo = new FileInfo(filePath);
                return fileInfo.Length <= maxSizeBytes;
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Validates URL format and protocol
        /// </summary>
        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrEmpty(url))
                return false;
                
            return Uri.TryCreate(url, UriKind.Absolute, out Uri validUri) &&
                   (validUri.Scheme == Uri.UriSchemeHttp || validUri.Scheme == Uri.UriSchemeHttps);
        }
        
        /// <summary>
        /// Calculates SHA256 hash of a file for integrity verification
        /// </summary>
        public static string CalculateFileHash(string filePath)
        {
            try
            {
                using (var sha256 = SHA256.Create())
                using (var stream = File.OpenRead(filePath))
                {
                    byte[] hash = sha256.ComputeHash(stream);
                    return Convert.ToHexString(hash);
                }
            }
            catch
            {
                return null;
            }
        }
        
        /// <summary>
        /// Validates that a path is within a specified directory (prevents path traversal)
        /// </summary>
        public static bool IsPathWithinDirectory(string path, string directory)
        {
            try
            {
                string fullPath = Path.GetFullPath(path);
                string fullDirectory = Path.GetFullPath(directory);
                
                return fullPath.StartsWith(fullDirectory + Path.DirectorySeparatorChar) || 
                       fullPath.Equals(fullDirectory);
            }
            catch
            {
                return false;
            }
        }
        
        /// <summary>
        /// Constants for file size limits
        /// </summary>
        public static class FileSizeLimits
        {
            public const long Config = 1024 * 1024; // 1MB
            public const long Metadata = 500L * 1024 * 1024; // 500MB
            public const long Il2CppBinary = 2L * 1024 * 1024 * 1024; // 2GB
            public const long PEFile = 1L * 1024 * 1024 * 1024; // 1GB
            public const long ZipEntry = 100L * 1024 * 1024; // 100MB per file
        }
    }
}
