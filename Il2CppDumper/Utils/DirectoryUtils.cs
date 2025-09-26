using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Il2CppDumper.Utils
{
    [Obsolete("Use SecureDirectoryOperations instead for better security")]
    internal class DirectoryUtils
    {
        internal static void Delete(string path)
        {
            // Redirect to secure implementation
            SecureDirectoryOperations.SafeDeleteDirectory(path, true);
        }

        internal static bool SafeDelete(string path)
        {
            return SecureDirectoryOperations.SafeDeleteDirectory(path, true);
        }

        internal static bool SafeCreate(string path)
        {
            return SecureDirectoryOperations.SafeCreateDirectory(path);
        }
    }
}
