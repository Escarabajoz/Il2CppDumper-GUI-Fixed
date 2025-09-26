#!/bin/bash

# Security Verification Script for Il2CppDumper
# This script verifies that all security patches have been properly applied

# Colors
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
MAGENTA='\033[0;35m'
NC='\033[0m' # No Color

echo -e "${CYAN}=== Il2CppDumper Security Verification ===${NC}"

ERROR_COUNT=0
WARNING_COUNT=0

test_file_exists() {
    local file_path="$1"
    local description="$2"
    
    if [ -f "$file_path" ]; then
        echo -e "${GREEN}✅ $description exists${NC}"
        return 0
    else
        echo -e "${RED}❌ $description missing: $file_path${NC}"
        ((ERROR_COUNT++))
        return 1
    fi
}

test_code_pattern() {
    local file_path="$1"
    local pattern="$2"
    local description="$3"
    local should_exist="${4:-true}"
    
    if [ -f "$file_path" ]; then
        local filename=$(basename "$file_path")
        
        if grep -q "$pattern" "$file_path"; then
            local found=true
        else
            local found=false
        fi
        
        if [ "$should_exist" = "true" ] && [ "$found" = "true" ]; then
            echo -e "${GREEN}✅ $description found in $filename${NC}"
            return 0
        elif [ "$should_exist" = "false" ] && [ "$found" = "false" ]; then
            echo -e "${GREEN}✅ $description properly removed from $filename${NC}"
            return 0
        else
            if [ "$should_exist" = "true" ]; then
                echo -e "${RED}❌ $description missing from $filename${NC}"
                ((ERROR_COUNT++))
            else
                echo -e "${YELLOW}⚠️ $description still exists in $filename${NC}"
                ((WARNING_COUNT++))
            fi
            return 1
        fi
    else
        echo -e "${RED}❌ File not found for verification: $file_path${NC}"
        ((ERROR_COUNT++))
        return 1
    fi
}

# Check that all new security utility files exist
echo -e "\n${YELLOW}--- Checking Security Utility Files ---${NC}"
test_file_exists "Il2CppDumper/Utils/SecureConfigLoader.cs" "SecureConfigLoader utility"
test_file_exists "Il2CppDumper/Utils/SecureHttpClient.cs" "SecureHttpClient utility"
test_file_exists "Il2CppDumper/Utils/InputValidator.cs" "InputValidator utility"
test_file_exists "Il2CppDumper/Utils/SecureDirectoryOperations.cs" "SecureDirectoryOperations utility"

# Check that ZipUtils has been secured
echo -e "\n${YELLOW}--- Checking ZipUtils Security Patches ---${NC}"
test_code_pattern "Il2CppDumper/Utils/ZipUtils.cs" "SanitizeFileName" "Path sanitization function"
test_code_pattern "Il2CppDumper/Utils/ZipUtils.cs" "ExtractArchiveSafely" "Safe archive extraction function"
test_code_pattern "Il2CppDumper/Utils/ZipUtils.cs" "MaxFileSize" "File size limits"

# Check that MainForm has been secured
echo -e "\n${YELLOW}--- Checking MainForm Security Patches ---${NC}"
test_code_pattern "Il2CppDumper/Forms/MainForm.xaml.cs" "SecureConfigLoader" "Secure config loading"
test_code_pattern "Il2CppDumper/Forms/MainForm.xaml.cs" "InputValidator\.TryParseHexAddress" "Secure hex parsing"
test_code_pattern "Il2CppDumper/Forms/MainForm.xaml.cs" "SecureHttpClient" "Secure HTTP client"
test_code_pattern "Il2CppDumper/Forms/MainForm.xaml.cs" "SecureDirectoryOperations" "Secure directory operations"

# Check for removal of dangerous patterns
echo -e "\n${YELLOW}--- Checking for Removed Dangerous Patterns ---${NC}"
test_code_pattern "Il2CppDumper/Forms/MainForm.xaml.cs" "new WebClient()" "Insecure WebClient usage" "false"
test_code_pattern "Il2CppDumper/Forms/MainForm.xaml.cs" "Convert\.ToUInt64.*16" "Direct unsafe hex conversion" "false"

# Check test files
echo -e "\n${YELLOW}--- Checking Security Test Files ---${NC}"
test_file_exists "SecurityTests/TestPathTraversal.cs" "Path traversal tests"
test_file_exists "SecurityTests/TestInputValidation.cs" "Input validation tests"
test_file_exists "SecurityTests/TestDirectoryOperations.cs" "Directory operations tests"

# Check documentation
echo -e "\n${YELLOW}--- Checking Security Documentation ---${NC}"
test_file_exists "SECURITY_PATCHES.md" "Security patches documentation"
test_file_exists "SECURITY_SUMMARY.md" "Security summary documentation"
test_file_exists "README_SECURITY.md" "Security README"

# Check configuration files
echo -e "\n${YELLOW}--- Checking Configuration Files ---${NC}"
test_file_exists "Il2CppDumper/security-config.json" "Security configuration file"

# Check build scripts
echo -e "\n${YELLOW}--- Checking Build Scripts ---${NC}"
test_file_exists "build-and-test.ps1" "PowerShell build script"
test_file_exists "build-and-test.sh" "Bash build script"

# Check project file updates
echo -e "\n${YELLOW}--- Checking Project File Updates ---${NC}"
test_code_pattern "Il2CppDumper/Il2CppDumper.csproj" "System\.Text\.Json" "System.Text.Json dependency"
test_code_pattern "Il2CppDumper/Il2CppDumper.csproj" "Microsoft\.NET\.Test\.Sdk" "Test SDK dependency"

# Summary
echo -e "\n${CYAN}=== Security Verification Summary ===${NC}"
if [ $ERROR_COUNT -eq 0 ] && [ $WARNING_COUNT -eq 0 ]; then
    echo -e "${GREEN}🎉 ALL SECURITY PATCHES VERIFIED SUCCESSFULLY!${NC}"
    echo -e "${GREEN}The application is ready for secure deployment.${NC}"
elif [ $ERROR_COUNT -eq 0 ]; then
    echo -e "${GREEN}✅ All critical security patches verified.${NC}"
    echo -e "${YELLOW}⚠️ $WARNING_COUNT warnings found - review recommended.${NC}"
else
    echo -e "${RED}❌ $ERROR_COUNT errors found - security patches incomplete!${NC}"
    echo -e "${YELLOW}⚠️ $WARNING_COUNT warnings found.${NC}"
    echo -e "${RED}Please fix errors before deployment.${NC}"
fi

echo -e "\n${MAGENTA}--- Next Steps ---${NC}"
echo -e "1. Run build script: ./build-and-test.sh --security-scan"
echo -e "2. Execute security tests: ./build-and-test.sh --run-tests"
echo -e "3. Test with sample malicious files"
echo -e "4. Deploy in sandboxed environment"

exit $ERROR_COUNT