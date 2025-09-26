using System;
using System.IO;
using Il2CppDumper.Utils;

namespace Il2CppDumper
{
    public static class DummyAssemblyExporter
    {
        public static void Export(Il2CppExecutor il2CppExecutor, string outputDir, bool addToken)
        {
            if (!InputValidator.IsValidDirectoryPath(outputDir))
            {
                throw new ArgumentException("Invalid output directory path", nameof(outputDir));
            }

            Directory.SetCurrentDirectory(outputDir);
            string dummyDllPath = Path.Combine(outputDir, "DummyDll");
            
            if (Directory.Exists(dummyDllPath))
                SecureDirectoryOperations.SafeDeleteDirectory(dummyDllPath, true);
            SecureDirectoryOperations.SafeCreateDirectory(dummyDllPath);
            Directory.SetCurrentDirectory("DummyDll");
            var dummy = new DummyAssemblyGenerator(il2CppExecutor, addToken);
            foreach (var assembly in dummy.Assemblies)
            {
                using var stream = new MemoryStream();
                assembly.Write(stream);
                File.WriteAllBytes(assembly.MainModule.Name, stream.ToArray());
            }
        }
    }
}
