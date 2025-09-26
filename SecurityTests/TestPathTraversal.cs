using System;
using System.IO;
using System.IO.Compression;
using Il2CppDumper.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Il2CppDumper.SecurityTests
{
    [TestClass]
    public class PathTraversalTests
    {
        private string testDirectory;

        [TestInitialize]
        public void Setup()
        {
            testDirectory = Path.Combine(Path.GetTempPath(), "Il2CppDumper_SecurityTests", Guid.NewGuid().ToString());
            Directory.CreateDirectory(testDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(testDirectory))
            {
                Directory.Delete(testDirectory, true);
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestPathTraversalAttack_ShouldThrow()
        {
            // Create a malicious ZIP entry
            var maliciousPath = "../../../../malicious.txt";
            
            // This should throw an InvalidOperationException due to path traversal detection
            var sanitized = InputValidator.SanitizeFileName(maliciousPath);
            
            // Even if sanitized, the ZipUtils should detect and prevent the traversal
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var entry = archive.CreateEntry(maliciousPath);
                    using (var entryStream = entry.Open())
                    {
                        var data = System.Text.Encoding.UTF8.GetBytes("malicious content");
                        entryStream.Write(data, 0, data.Length);
                    }
                }

                memoryStream.Position = 0;
                using (var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    var maliciousEntry = readArchive.Entries[0];
                    // This should throw due to path traversal detection
                    ZipUtils.ExtractFile(maliciousEntry, testDirectory);
                }
            }
        }

        [TestMethod]
        public void TestValidPath_ShouldSucceed()
        {
            var validPath = "valid_file.txt";
            
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var entry = archive.CreateEntry(validPath);
                    using (var entryStream = entry.Open())
                    {
                        var data = System.Text.Encoding.UTF8.GetBytes("valid content");
                        entryStream.Write(data, 0, data.Length);
                    }
                }

                memoryStream.Position = 0;
                using (var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    var validEntry = readArchive.Entries[0];
                    // This should succeed
                    ZipUtils.ExtractFile(validEntry, testDirectory);
                    
                    // Verify file was created in the correct location
                    var expectedPath = Path.Combine(testDirectory, validPath);
                    Assert.IsTrue(File.Exists(expectedPath));
                    
                    var content = File.ReadAllText(expectedPath);
                    Assert.AreEqual("valid content", content);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestZipBomb_ShouldThrow()
        {
            // Create a ZIP with too many files
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    // Create more files than the limit (10,000)
                    for (int i = 0; i < 10001; i++)
                    {
                        var entry = archive.CreateEntry($"file_{i}.txt");
                        using (var entryStream = entry.Open())
                        {
                            var data = System.Text.Encoding.UTF8.GetBytes($"content {i}");
                            entryStream.Write(data, 0, data.Length);
                        }
                    }
                }

                memoryStream.Position = 0;
                using (var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    // This should throw due to file count limit
                    ZipUtils.ExtractArchiveSafely(readArchive, testDirectory, out long totalSize);
                }
            }
        }

        [TestMethod]
        [ExpectedException(typeof(InvalidOperationException))]
        public void TestOversizedFile_ShouldThrow()
        {
            // Create a ZIP with a file larger than the limit
            using (var memoryStream = new MemoryStream())
            {
                using (var archive = new ZipArchive(memoryStream, ZipArchiveMode.Create, true))
                {
                    var entry = archive.CreateEntry("large_file.txt");
                    using (var entryStream = entry.Open())
                    {
                        // Create a file larger than 100MB limit
                        var largeData = new byte[101 * 1024 * 1024]; // 101MB
                        entryStream.Write(largeData, 0, largeData.Length);
                    }
                }

                memoryStream.Position = 0;
                using (var readArchive = new ZipArchive(memoryStream, ZipArchiveMode.Read))
                {
                    var oversizedEntry = readArchive.Entries[0];
                    // This should throw due to file size limit
                    ZipUtils.ExtractFile(oversizedEntry, testDirectory);
                }
            }
        }
    }
}