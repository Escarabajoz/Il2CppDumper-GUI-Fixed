using System;
using Il2CppDumper.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Il2CppDumper.SecurityTests
{
    [TestClass]
    public class InputValidationTests
    {
        [TestMethod]
        public void TestValidHexAddress_ShouldSucceed()
        {
            var validAddresses = new[] { "1234ABCD", "0xDEADBEEF", "CAFEBABE", "0x12345678" };
            
            foreach (var address in validAddresses)
            {
                bool result = InputValidator.TryParseHexAddress(address, out ulong parsed);
                Assert.IsTrue(result, $"Failed to parse valid address: {address}");
                Assert.IsTrue(parsed > 0, $"Parsed address should not be zero: {address}");
            }
        }

        [TestMethod]
        public void TestInvalidHexAddress_ShouldFail()
        {
            var invalidAddresses = new[] { 
                "GHIJ", // Invalid hex characters
                "", // Empty string
                "   ", // Whitespace only
                "12345678901234567", // Too long (more than 16 hex chars)
                "0", // Zero address
                "0x0", // Zero address with prefix
                "ZZZZ", // Invalid characters
                "12G4" // Mixed valid/invalid
            };
            
            foreach (var address in invalidAddresses)
            {
                bool result = InputValidator.TryParseHexAddress(address, out ulong parsed);
                Assert.IsFalse(result, $"Should not parse invalid address: {address}");
            }
        }

        [TestMethod]
        public void TestValidFilePath_ShouldSucceed()
        {
            var validPaths = new[] {
                @"C:\Users\Test\file.txt",
                @"./relative/path.txt",
                @"simple_file.txt",
                @"/home/user/file.txt"
            };
            
            foreach (var path in validPaths)
            {
                bool result = InputValidator.IsValidFilePath(path);
                Assert.IsTrue(result, $"Should validate path: {path}");
            }
        }

        [TestMethod]
        public void TestInvalidFilePath_ShouldFail()
        {
            var invalidPaths = new[] {
                @"../../../etc/passwd", // Path traversal
                @"file\0name.txt", // Null character
                @"", // Empty string
                @"con.txt", // Windows reserved name
                @"file<name>.txt", // Invalid characters
                @"./malicious/../file.txt" // Path traversal
            };
            
            foreach (var path in invalidPaths)
            {
                bool result = InputValidator.IsValidFilePath(path);
                Assert.IsFalse(result, $"Should not validate path: {path}");
            }
        }

        [TestMethod]
        public void TestValidUrl_ShouldSucceed()
        {
            var validUrls = new[] {
                "https://example.com",
                "http://localhost:8080",
                "https://github.com/user/repo",
                "http://192.168.1.1/path"
            };
            
            foreach (var url in validUrls)
            {
                bool result = InputValidator.IsValidUrl(url);
                Assert.IsTrue(result, $"Should validate URL: {url}");
            }
        }

        [TestMethod]
        public void TestInvalidUrl_ShouldFail()
        {
            var invalidUrls = new[] {
                "ftp://example.com", // Wrong protocol
                "file:///etc/passwd", // File protocol
                "javascript:alert(1)", // JavaScript protocol
                "", // Empty string
                "not-a-url", // Not a URL
                "http://" // Incomplete URL
            };
            
            foreach (var url in invalidUrls)
            {
                bool result = InputValidator.IsValidUrl(url);
                Assert.IsFalse(result, $"Should not validate URL: {url}");
            }
        }

        [TestMethod]
        public void TestSanitizeLogMessage_ShouldRemoveSensitiveData()
        {
            var testCases = new[]
            {
                ("Address: 0xDEADBEEF", "Address: [ADDRESS]"),
                ("IP: 192.168.1.1", "IP: [IP]"),
                ("Base64: dGVzdGRhdGE=", "Base64: [DATA]"),
                ("Normal log message", "Normal log message"),
                ("Multiple addresses: 0x12345678 and 0xABCDEF00", "Multiple addresses: [ADDRESS] and [ADDRESS]")
            };
            
            foreach (var (input, expected) in testCases)
            {
                string result = InputValidator.SanitizeLogMessage(input);
                Assert.AreEqual(expected, result, $"Failed to sanitize: {input}");
            }
        }

        [TestMethod]
        public void TestSanitizeFileName_ShouldRemoveDangerousChars()
        {
            var testCases = new[]
            {
                ("normal_file.txt", "normal_file.txt"),
                ("file<with>bad:chars", "file_with_bad_chars"),
                ("../../../malicious", "malicious"),
                ("", "unnamed_file"),
                ("   ", "unnamed_file"),
                ("file.with.dots", "file.with.dots")
            };
            
            foreach (var (input, expected) in testCases)
            {
                string result = InputValidator.SanitizeFileName(input);
                Assert.AreEqual(expected, result, $"Failed to sanitize filename: {input}");
            }
        }

        [TestMethod]
        public void TestValidVersion_ShouldSucceed()
        {
            var validVersions = new[] {
                "1.0",
                "2.3.4",
                "10.20.30.40",
                "0.1"
            };
            
            foreach (var version in validVersions)
            {
                bool result = InputValidator.IsValidVersion(version);
                Assert.IsTrue(result, $"Should validate version: {version}");
            }
        }

        [TestMethod]
        public void TestInvalidVersion_ShouldFail()
        {
            var invalidVersions = new[] {
                "1", // Too few parts
                "1.2.3.4.5", // Too many parts
                "1.a", // Non-numeric
                "1.-1", // Negative number
                "", // Empty
                "1.10000" // Number too large
            };
            
            foreach (var version in invalidVersions)
            {
                bool result = InputValidator.IsValidVersion(version);
                Assert.IsFalse(result, $"Should not validate version: {version}");
            }
        }
    }
}