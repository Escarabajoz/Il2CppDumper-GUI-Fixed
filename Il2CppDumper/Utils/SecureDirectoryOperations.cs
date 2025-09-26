using System;
using System.IO;
using System.Linq;

namespace Il2CppDumper.Utils
{
    public static class SecureDirectoryOperations
    {
        private static readonly string[] SafeDirectoryNames = 
        {
            "temp", "tmp", "il2cppdumper", "output", "dump", "extracted"
        };

        public static bool SafeDeleteDirectory(string path, bool recursive = false)
        {
            if (string.IsNullOrWhiteSpace(path))
            {
                MainForm.Log("Cannot delete directory: path is null or empty", System.Windows.Media.Brushes.Orange);
                return false;
            }

            try
            {
                // Validate path
                if (!InputValidator.IsValidDirectoryPath(path))
                {
                    MainForm.Log($"Invalid directory path: {path}", System.Windows.Media.Brushes.Orange);
                    return false;
                }

                // Get full path for validation
                string fullPath = Path.GetFullPath(path);
                
                // Security checks
                if (!IsSafeToDelete(fullPath))
                {
                    MainForm.Log($"Directory deletion blocked for security: {path}", System.Windows.Media.Brushes.Orange);
                    return false;
                }

                if (!Directory.Exists(fullPath))
                {
                    MainForm.Log($"Directory does not exist: {path}", System.Windows.Media.Brushes.Yellow);
                    return true; // Consider this success
                }

                // Additional check: ensure we're not trying to delete system directories
                if (IsSystemDirectory(fullPath))
                {
                    MainForm.Log($"Cannot delete system directory: {path}", System.Windows.Media.Brushes.Red);
                    return false;
                }

                // Perform deletion
                Directory.Delete(fullPath, recursive);
                MainForm.Log($"Successfully deleted directory: {path}");
                return true;
            }
            catch (UnauthorizedAccessException ex)
            {
                MainForm.Log($"Access denied deleting directory {path}: {ex.Message}", System.Windows.Media.Brushes.Orange);
                return false;
            }
            catch (DirectoryNotFoundException)
            {
                MainForm.Log($"Directory not found: {path}", System.Windows.Media.Brushes.Yellow);
                return true; // Consider this success
            }
            catch (IOException ex)
            {
                MainForm.Log($"IO error deleting directory {path}: {ex.Message}", System.Windows.Media.Brushes.Orange);
                return false;
            }
            catch (Exception ex)
            {
                MainForm.Log($"Error deleting directory {path}: {ex.Message}", System.Windows.Media.Brushes.Red);
                return false;
            }
        }

        public static bool SafeCreateDirectory(string path)
        {
            if (string.IsNullOrWhiteSpace(path))
                return false;

            try
            {
                if (!InputValidator.IsValidDirectoryPath(path))
                    return false;

                string fullPath = Path.GetFullPath(path);
                
                // Check if we're trying to create in a safe location
                if (!IsSafeToCreate(fullPath))
                {
                    MainForm.Log($"Directory creation blocked for security: {path}", System.Windows.Media.Brushes.Orange);
                    return false;
                }

                Directory.CreateDirectory(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                MainForm.Log($"Error creating directory {path}: {ex.Message}", System.Windows.Media.Brushes.Orange);
                return false;
            }
        }

        private static bool IsSafeToDelete(string fullPath)
        {
            // Convert to lowercase for comparison
            string lowerPath = fullPath.ToLowerInvariant();

            // Check if it's in a temp directory or application directory
            string tempPath = Path.GetTempPath().ToLowerInvariant();
            string appPath = AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant();

            // Allow deletion in temp directories or app directory
            if (lowerPath.StartsWith(tempPath) || lowerPath.StartsWith(appPath))
            {
                // Additional check: ensure the directory name suggests it's safe to delete
                string dirName = Path.GetFileName(lowerPath);
                return SafeDirectoryNames.Any(safe => dirName.Contains(safe));
            }

            // Allow deletion in user's documents or desktop (with caution)
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).ToLowerInvariant();
            if (lowerPath.StartsWith(userProfile))
            {
                // Only allow if it's clearly a dump/temp directory
                string dirName = Path.GetFileName(lowerPath);
                return SafeDirectoryNames.Any(safe => dirName.Contains(safe)) ||
                       dirName.Contains("dumped") || dirName.Contains("extracted");
            }

            return false;
        }

        private static bool IsSafeToCreate(string fullPath)
        {
            string lowerPath = fullPath.ToLowerInvariant();

            // Allow creation in temp directories
            string tempPath = Path.GetTempPath().ToLowerInvariant();
            if (lowerPath.StartsWith(tempPath))
                return true;

            // Allow creation in app directory
            string appPath = AppDomain.CurrentDomain.BaseDirectory.ToLowerInvariant();
            if (lowerPath.StartsWith(appPath))
                return true;

            // Allow creation in user directories
            string userProfile = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile).ToLowerInvariant();
            string documents = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments).ToLowerInvariant();
            string desktop = Environment.GetFolderPath(Environment.SpecialFolder.Desktop).ToLowerInvariant();

            return lowerPath.StartsWith(userProfile) || 
                   lowerPath.StartsWith(documents) || 
                   lowerPath.StartsWith(desktop);
        }

        private static bool IsSystemDirectory(string fullPath)
        {
            string lowerPath = fullPath.ToLowerInvariant();

            // Windows system directories
            string[] systemDirs = {
                @"c:\windows", @"c:\program files", @"c:\program files (x86)",
                @"c:\system volume information", @"c:\$recycle.bin",
                @"c:\users\all users", @"c:\users\default"
            };

            // Unix/Linux system directories
            string[] unixSystemDirs = {
                "/bin", "/sbin", "/usr", "/etc", "/var", "/sys", "/proc", "/dev", "/boot"
            };

            return systemDirs.Any(dir => lowerPath.StartsWith(dir)) ||
                   unixSystemDirs.Any(dir => lowerPath.StartsWith(dir));
        }

        public static long GetDirectorySize(string path)
        {
            if (!Directory.Exists(path))
                return 0;

            try
            {
                return Directory.GetFiles(path, "*", SearchOption.AllDirectories)
                               .Sum(file => new FileInfo(file).Length);
            }
            catch (Exception)
            {
                return -1; // Error calculating size
            }
        }
    }
}