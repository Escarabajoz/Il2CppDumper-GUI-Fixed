# Security Policy

## Overview

This document outlines the security measures implemented in Il2CppDumper GUI to protect users from common security vulnerabilities.

## Security Fixes Implemented

### Critical Vulnerabilities Fixed

#### 1. Command Injection Prevention
- **Issue**: Unsafe use of `Process.Start()` with user-controlled input
- **Fix**: Added input validation, path sanitization, and secure ProcessStartInfo configuration
- **Files**: `MainForm.xaml.cs`

#### 2. Path Traversal Protection
- **Issue**: ZIP extraction and file operations vulnerable to directory traversal attacks
- **Fix**: Implemented path validation, filename sanitization, and directory boundary checks
- **Files**: `ZipUtils.cs`, `MainForm.xaml.cs`, `SecurityUtils.cs`

#### 3. Secure HTTP Communication
- **Issue**: Insecure WebClient usage with no timeout or validation
- **Fix**: Replaced with HttpClient with proper timeout, SSL validation, and response validation
- **Files**: `MainForm.xaml.cs`

#### 4. Memory Safety
- **Issue**: Unsafe memory operations in PE loading
- **Fix**: Added bounds checking and overflow protection
- **Files**: `PELoader.cs`

### Medium Severity Fixes

#### 5. Input Validation
- **Issue**: Lack of proper input validation for hexadecimal addresses
- **Fix**: Added regex validation, exception handling, and user feedback
- **Files**: `MainForm.xaml.cs`, `InputOffsetForm.xaml.cs`

#### 6. File Size Validation
- **Issue**: No limits on file sizes leading to potential DoS
- **Fix**: Implemented file size limits for different file types
- **Files**: `MainForm.xaml.cs`, `PELoader.cs`, `SecurityUtils.cs`

#### 7. Secure JSON Deserialization
- **Issue**: Unsafe JSON deserialization settings
- **Fix**: Configured JsonSerializerSettings with security restrictions
- **Files**: `MainForm.xaml.cs`

## Security Utilities

A new `SecurityUtils` class has been created to centralize security-related operations:

- Path validation and sanitization
- Hexadecimal string validation
- URL validation
- File integrity checking (SHA256)
- File size limit constants

## File Size Limits

The following file size limits are enforced:

- Configuration files: 1MB
- Metadata files: 500MB
- Il2Cpp binary files: 2GB
- PE files: 1GB
- ZIP entry files: 100MB

## Best Practices Implemented

1. **Input Validation**: All user inputs are validated before processing
2. **Path Sanitization**: File paths are sanitized to prevent traversal attacks
3. **Error Handling**: Comprehensive error handling with user feedback
4. **Resource Limits**: File size limits prevent resource exhaustion
5. **Secure Defaults**: Safe default configurations for all security-sensitive operations

## Reporting Security Issues

If you discover a security vulnerability, please report it by:

1. Creating a private security advisory on GitHub
2. Emailing the maintainers directly
3. Providing detailed information about the vulnerability

Please do not create public issues for security vulnerabilities.

## Security Testing

Regular security testing should include:

1. Static code analysis
2. Dependency vulnerability scanning
3. Penetration testing
4. Code review for security issues

## Dependencies

All dependencies are regularly monitored for security updates:

- Newtonsoft.Json: 13.0.3
- Mono.Cecil: 0.11.6
- System.Text.Encoding.CodePages: 9.0.0
- Other dependencies as listed in the project file

## Updates

This security policy and the implemented fixes will be updated as new threats are identified and addressed.

Last updated: [Current Date]
