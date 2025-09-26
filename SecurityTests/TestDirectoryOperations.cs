using System;
using System.IO;
using Il2CppDumper.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Il2CppDumper.SecurityTests
{
    [TestClass]
    public class DirectoryOperationsTests
    {
        private string testDirectory;
        private string safeTestDirectory;

        [TestInitialize]
        public void Setup()
        {
            testDirectory = Path.Combine(Path.GetTempPath(), "Il2CppDumper_DirectoryTests", Guid.NewGuid().ToString());
            safeTestDirectory = Path.Combine(testDirectory, "safe_to_delete");
            
            Directory.CreateDirectory(testDirectory);
            Directory.CreateDirectory(safeTestDirectory);
        }

        [TestCleanup]
        public void Cleanup()
        {
            if (Directory.Exists(testDirectory))
            {
                try
                {
                    Directory.Delete(testDirectory, true);
                }
                catch
                {
                    // Ignore cleanup errors
                }
            }
        }

        [TestMethod]
        public void TestSafeDirectoryCreation_ShouldSucceed()
        {
            string newDir = Path.Combine(testDirectory, "new_directory");
            
            bool result = SecureDirectoryOperations.SafeCreateDirectory(newDir);
            
            Assert.IsTrue(result, "Should successfully create directory");
            Assert.IsTrue(Directory.Exists(newDir), "Directory should exist after creation");
        }

        [TestMethod]
        public void TestSafeDirectoryDeletion_ValidPath_ShouldSucceed()
        {
            // Create a test file in the safe directory
            string testFile = Path.Combine(safeTestDirectory, "test.txt");
            File.WriteAllText(testFile, "test content");
            
            bool result = SecureDirectoryOperations.SafeDeleteDirectory(safeTestDirectory, true);
            
            Assert.IsTrue(result, "Should successfully delete directory");
            Assert.IsFalse(Directory.Exists(safeTestDirectory), "Directory should not exist after deletion");
        }

        [TestMethod]
        public void TestUnsafeDirectoryDeletion_SystemPath_ShouldFail()
        {
            // Try to delete a system directory (this should be blocked)
            string systemPath = @"C:\Windows"; // Windows system directory
            
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                systemPath = "/bin"; // Unix system directory
            }
            
            bool result = SecureDirectoryOperations.SafeDeleteDirectory(systemPath, true);
            
            Assert.IsFalse(result, "Should not delete system directory");
            Assert.IsTrue(Directory.Exists(systemPath), "System directory should still exist");
        }

        [TestMethod]
        public void TestInvalidDirectoryPath_ShouldFail()
        {
            string invalidPath = "invalid\0path"; // Contains null character
            
            bool createResult = SecureDirectoryOperations.SafeCreateDirectory(invalidPath);
            bool deleteResult = SecureDirectoryOperations.SafeDeleteDirectory(invalidPath, true);
            
            Assert.IsFalse(createResult, "Should not create directory with invalid path");
            Assert.IsFalse(deleteResult, "Should not delete directory with invalid path");
        }

        [TestMethod]
        public void TestEmptyPath_ShouldFail()
        {
            bool createResult = SecureDirectoryOperations.SafeCreateDirectory("");
            bool deleteResult = SecureDirectoryOperations.SafeDeleteDirectory("", true);
            
            Assert.IsFalse(createResult, "Should not create directory with empty path");
            Assert.IsFalse(deleteResult, "Should not delete directory with empty path");
        }

        [TestMethod]
        public void TestNonExistentDirectory_Deletion_ShouldReturnTrue()
        {
            string nonExistentPath = Path.Combine(testDirectory, "does_not_exist");
            
            bool result = SecureDirectoryOperations.SafeDeleteDirectory(nonExistentPath, true);
            
            // Should return true (consider it successful if directory doesn't exist)
            Assert.IsTrue(result, "Should return true for non-existent directory deletion");
        }

        [TestMethod]
        public void TestDirectorySize_Calculation()
        {
            // Create some test files
            string file1 = Path.Combine(safeTestDirectory, "file1.txt");
            string file2 = Path.Combine(safeTestDirectory, "file2.txt");
            
            File.WriteAllText(file1, "Hello"); // 5 bytes
            File.WriteAllText(file2, "World!"); // 6 bytes
            
            long size = SecureDirectoryOperations.GetDirectorySize(safeTestDirectory);
            
            Assert.AreEqual(11, size, "Directory size should be 11 bytes");
        }

        [TestMethod]
        public void TestDirectorySize_NonExistent_ShouldReturnZero()
        {
            string nonExistentPath = Path.Combine(testDirectory, "does_not_exist");
            
            long size = SecureDirectoryOperations.GetDirectorySize(nonExistentPath);
            
            Assert.AreEqual(0, size, "Non-existent directory size should be 0");
        }

        [TestMethod]
        public void TestPathTraversalInDirectoryOperations_ShouldFail()
        {
            string traversalPath = Path.Combine(testDirectory, "..", "..", "malicious");
            
            bool createResult = SecureDirectoryOperations.SafeCreateDirectory(traversalPath);
            bool deleteResult = SecureDirectoryOperations.SafeDeleteDirectory(traversalPath, true);
            
            // These operations should be blocked or fail safely
            Assert.IsFalse(createResult || deleteResult, "Path traversal attempts should be blocked");
        }
    }
}