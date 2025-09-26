using System;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;

namespace Il2CppDumper.Utils
{
    public static class InputValidator
    {
        private static readonly Regex HexRegex = new Regex(@"^[0-9A-Fa-f]+$", RegexOptions.Compiled);
        private static readonly Regex SafePathRegex = new Regex(@"^[a-zA-Z0-9\s\-_\.\\/:\(\)]+$", RegexOptions.Compiled);

        public static bool TryParseHexAddress(string input, out ulong address)
        {
            address = 0;

            if (string.IsNullOrWhiteSpace(input))
                return false;

            // Remove common prefixes
            input = input.Trim();
            if (input.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                input = input.Substring(2);

            // Validate hex format
            if (!HexRegex.IsMatch(input))
                return false;

            // Limit length to prevent overflow
            if (input.Length > 16) // 64-bit max
                return false;

            try
            {
                address = Convert.ToUInt64(input, 16);
                
                // Basic sanity check for memory addresses
                if (address == 0)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsValidFilePath(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                // Check for invalid characters
                if (path.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
                    return false;

                // Check for path traversal attempts
                if (path.Contains("..") || path.Contains("./") || path.Contains(".\\"))
                    return false;

                // Additional safety check
                if (!SafePathRegex.IsMatch(path))
                    return false;

                // Try to get full path (this will throw if invalid)
                Path.GetFullPath(path);
                
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static bool IsValidDirectoryPath(string path)
        {
            if (!IsValidFilePath(path))
                return false;

            try
            {
                // Additional checks for directory paths
                var fullPath = Path.GetFullPath(path);
                
                // Ensure it's not a root directory
                if (Path.GetPathRoot(fullPath) == fullPath)
                    return false;

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static string SanitizeFileName(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
                return "unnamed_file";

            // Remove invalid characters
            char[] invalidChars = Path.GetInvalidFileNameChars();
            foreach (char c in invalidChars)
            {
                fileName = fileName.Replace(c, '_');
            }

            // Remove path traversal sequences
            fileName = fileName.Replace("..", "").Replace("./", "").Replace(".\\", "");

            // Trim and limit length
            fileName = fileName.Trim().Trim('.');
            if (fileName.Length > 200)
                fileName = fileName.Substring(0, 200);

            // Ensure it's not empty
            if (string.IsNullOrWhiteSpace(fileName))
                fileName = "unnamed_file";

            return fileName;
        }

        public static bool IsValidUrl(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
                return false;

            if (!Uri.TryCreate(url, UriKind.Absolute, out Uri uri))
                return false;

            // Only allow HTTP and HTTPS
            return uri.Scheme == "http" || uri.Scheme == "https";
        }

        public static bool IsValidVersion(string version)
        {
            if (string.IsNullOrWhiteSpace(version))
                return false;

            // Simple version format validation
            var parts = version.Split('.');
            if (parts.Length < 2 || parts.Length > 4)
                return false;

            foreach (var part in parts)
            {
                if (!int.TryParse(part, out int number) || number < 0 || number > 9999)
                    return false;
            }

            return true;
        }

        public static string SanitizeLogMessage(string message)
        {
            if (string.IsNullOrEmpty(message))
                return "";

            // Remove potential sensitive information patterns
            message = Regex.Replace(message, @"0x[0-9A-Fa-f]{8,16}", "[ADDRESS]", RegexOptions.IgnoreCase);
            message = Regex.Replace(message, @"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b", "[IP]");
            message = Regex.Replace(message, @"[A-Za-z0-9+/]{20,}={0,2}", "[DATA]"); // Base64-like patterns

            // Limit message length
            if (message.Length > 500)
                message = message.Substring(0, 497) + "...";

            return message;
        }
    }
}