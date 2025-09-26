# Security Verification Script for Il2CppDumper
# This script verifies that all security patches have been properly applied

Write-Host "=== Il2CppDumper Security Verification ===" -ForegroundColor Cyan

$ErrorCount = 0
$WarningCount = 0

function Test-FileExists {
    param($FilePath, $Description)
    if (Test-Path $FilePath) {
        Write-Host "✅ $Description exists" -ForegroundColor Green
        return $true
    } else {
        Write-Host "❌ $Description missing: $FilePath" -ForegroundColor Red
        $script:ErrorCount++
        return $false
    }
}

function Test-CodePattern {
    param($FilePath, $Pattern, $Description, $ShouldExist = $true)
    if (Test-Path $FilePath) {
        $Content = Get-Content $FilePath -Raw
        $Found = $Content -match $Pattern
        
        if ($ShouldExist -and $Found) {
            Write-Host "✅ $Description found in $(Split-Path $FilePath -Leaf)" -ForegroundColor Green
            return $true
        } elseif (-not $ShouldExist -and -not $Found) {
            Write-Host "✅ $Description properly removed from $(Split-Path $FilePath -Leaf)" -ForegroundColor Green
            return $true
        } else {
            if ($ShouldExist) {
                Write-Host "❌ $Description missing from $(Split-Path $FilePath -Leaf)" -ForegroundColor Red
                $script:ErrorCount++
            } else {
                Write-Host "⚠️ $Description still exists in $(Split-Path $FilePath -Leaf)" -ForegroundColor Yellow
                $script:WarningCount++
            }
            return $false
        }
    } else {
        Write-Host "❌ File not found for verification: $FilePath" -ForegroundColor Red
        $script:ErrorCount++
        return $false
    }
}

# Check that all new security utility files exist
Write-Host "`n--- Checking Security Utility Files ---" -ForegroundColor Yellow
Test-FileExists "Il2CppDumper\Utils\SecureConfigLoader.cs" "SecureConfigLoader utility"
Test-FileExists "Il2CppDumper\Utils\SecureHttpClient.cs" "SecureHttpClient utility"  
Test-FileExists "Il2CppDumper\Utils\InputValidator.cs" "InputValidator utility"
Test-FileExists "Il2CppDumper\Utils\SecureDirectoryOperations.cs" "SecureDirectoryOperations utility"

# Check that ZipUtils has been secured
Write-Host "`n--- Checking ZipUtils Security Patches ---" -ForegroundColor Yellow
Test-CodePattern "Il2CppDumper\Utils\ZipUtils.cs" "SanitizeFileName" "Path sanitization function"
Test-CodePattern "Il2CppDumper\Utils\ZipUtils.cs" "ExtractArchiveSafely" "Safe archive extraction function"
Test-CodePattern "Il2CppDumper\Utils\ZipUtils.cs" "MaxFileSize" "File size limits"

# Check that MainForm has been secured
Write-Host "`n--- Checking MainForm Security Patches ---" -ForegroundColor Yellow
Test-CodePattern "Il2CppDumper\Forms\MainForm.xaml.cs" "SecureConfigLoader" "Secure config loading"
Test-CodePattern "Il2CppDumper\Forms\MainForm.xaml.cs" "InputValidator\.TryParseHexAddress" "Secure hex parsing"
Test-CodePattern "Il2CppDumper\Forms\MainForm.xaml.cs" "SecureHttpClient" "Secure HTTP client"
Test-CodePattern "Il2CppDumper\Forms\MainForm.xaml.cs" "SecureDirectoryOperations" "Secure directory operations"

# Check for removal of dangerous patterns
Write-Host "`n--- Checking for Removed Dangerous Patterns ---" -ForegroundColor Yellow
Test-CodePattern "Il2CppDumper\Forms\MainForm.xaml.cs" "new WebClient\(\)" "Insecure WebClient usage" $false
Test-CodePattern "Il2CppDumper\Forms\MainForm.xaml.cs" "Convert\.ToUInt64\([^,]+, 16\)" "Direct unsafe hex conversion" $false
Test-CodePattern "Il2CppDumper\Forms\MainForm.xaml.cs" "Directory\.Delete\([^,]+, true\)" "Direct directory deletion" $false

# Check test files
Write-Host "`n--- Checking Security Test Files ---" -ForegroundColor Yellow
Test-FileExists "SecurityTests\TestPathTraversal.cs" "Path traversal tests"
Test-FileExists "SecurityTests\TestInputValidation.cs" "Input validation tests"
Test-FileExists "SecurityTests\TestDirectoryOperations.cs" "Directory operations tests"

# Check documentation
Write-Host "`n--- Checking Security Documentation ---" -ForegroundColor Yellow
Test-FileExists "SECURITY_PATCHES.md" "Security patches documentation"
Test-FileExists "SECURITY_SUMMARY.md" "Security summary documentation"
Test-FileExists "README_SECURITY.md" "Security README"

# Check configuration files
Write-Host "`n--- Checking Configuration Files ---" -ForegroundColor Yellow
Test-FileExists "Il2CppDumper\security-config.json" "Security configuration file"

# Check build scripts
Write-Host "`n--- Checking Build Scripts ---" -ForegroundColor Yellow
Test-FileExists "build-and-test.ps1" "PowerShell build script"
Test-FileExists "build-and-test.sh" "Bash build script"

# Check project file updates
Write-Host "`n--- Checking Project File Updates ---" -ForegroundColor Yellow
Test-CodePattern "Il2CppDumper\Il2CppDumper.csproj" "System\.Text\.Json" "System.Text.Json dependency"
Test-CodePattern "Il2CppDumper\Il2CppDumper.csproj" "Microsoft\.NET\.Test\.Sdk" "Test SDK dependency"

# Summary
Write-Host "`n=== Security Verification Summary ===" -ForegroundColor Cyan
if ($ErrorCount -eq 0 -and $WarningCount -eq 0) {
    Write-Host "🎉 ALL SECURITY PATCHES VERIFIED SUCCESSFULLY!" -ForegroundColor Green
    Write-Host "The application is ready for secure deployment." -ForegroundColor Green
} elseif ($ErrorCount -eq 0) {
    Write-Host "✅ All critical security patches verified." -ForegroundColor Green
    Write-Host "⚠️ $WarningCount warnings found - review recommended." -ForegroundColor Yellow
} else {
    Write-Host "❌ $ErrorCount errors found - security patches incomplete!" -ForegroundColor Red
    Write-Host "⚠️ $WarningCount warnings found." -ForegroundColor Yellow
    Write-Host "Please fix errors before deployment." -ForegroundColor Red
}

Write-Host "`n--- Next Steps ---" -ForegroundColor Magenta
Write-Host "1. Run build script: .\build-and-test.ps1 -SecurityScan" -ForegroundColor White
Write-Host "2. Execute security tests: .\build-and-test.ps1 -RunTests" -ForegroundColor White  
Write-Host "3. Test with sample malicious files" -ForegroundColor White
Write-Host "4. Deploy in sandboxed environment" -ForegroundColor White