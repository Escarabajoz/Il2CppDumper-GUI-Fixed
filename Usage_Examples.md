# Il2CppDumper GUI - Usage Examples and Instructions

## Table of Contents
1. [Getting Started](#getting-started)
2. [Basic Usage](#basic-usage)
3. [Advanced Usage](#advanced-usage)
4. [Platform-Specific Examples](#platform-specific-examples)
5. [Integration Examples](#integration-examples)
6. [Troubleshooting](#troubleshooting)

## Getting Started

### Prerequisites
- Windows 10/11 (64-bit recommended)
- .NET 8.0 Runtime
- Target files: IL2CPP binary and global-metadata.dat

### Installation
1. Download the latest release from GitHub
2. Extract to a folder
3. Run `Il2CppDumper GUI.exe`

## Basic Usage

### Example 1: Dumping Android APK

```csharp
// Programmatic approach
var mainForm = new MainForm();

// Set up file paths
string apkPath = @"C:\Games\MyGame.apk";
string outputPath = @"C:\Output\MyGame_dumped\";

// Auto-dump APK (handles extraction automatically)
await mainForm.APKDump(apkPath, outputPath);
```

**GUI Steps:**
1. Drag and drop the APK file onto the "Start" button
2. The tool will automatically:
   - Extract the APK
   - Find libil2cpp.so and global-metadata.dat
   - Dump all architectures (ARM64, ARMv7, x86, x86_64)
   - Generate analysis scripts

### Example 2: Dumping iOS IPA

```csharp
var mainForm = new MainForm();

string ipaPath = @"C:\Games\MyGame.ipa";
string outputPath = @"C:\Output\MyGame_dumped\";

// Auto-dump IPA (handles extraction automatically)
await mainForm.IPADump(ipaPath, outputPath);
```

**GUI Steps:**
1. Drag and drop the IPA file onto the "Start" button
2. The tool will automatically:
   - Extract the IPA
   - Find the main executable and global-metadata.dat
   - Dump ARM64 architecture (default)
   - Generate analysis scripts

### Example 3: Manual File Selection

```csharp
var mainForm = new MainForm();

// Manual file selection
string il2cppPath = @"C:\Files\libil2cpp.so";
string metadataPath = @"C:\Files\global-metadata.dat";
string outputPath = @"C:\Output\Manual_dump\";

// Perform dumping
if (mainForm.Init(il2cppPath, metadataPath, out var metadata, out var il2Cpp))
{
    mainForm.Dump(metadata, il2Cpp, outputPath);
    mainForm.Log("Dumping completed successfully!", Brushes.Green);
}
else
{
    mainForm.Log("Failed to initialize IL2CPP", Brushes.Red);
}
```

**GUI Steps:**
1. Click "Select Executable File" and choose your IL2CPP binary
2. Click "Select global-metadata.dat" and choose the metadata file
3. Click "Select Output Directory" and choose output folder
4. Click "Start" to begin dumping

## Advanced Usage

### Example 4: Custom Configuration

```csharp
// Create custom configuration
var config = new Config
{
    DumpMethod = true,
    DumpField = true,
    DumpProperty = true,
    DumpAttribute = true,
    DumpFieldOffset = true,
    DumpMethodOffset = true,
    DumpTypeDefIndex = true,
    GenerateDummyDll = true,
    GenerateStruct = true,
    DummyDllAddToken = true,
    ForceIl2CppVersion = true,
    ForceVersion = 27.0,
    ForceDump = false,
    NoRedirectedPointer = false
};

// Save configuration to file
string configJson = JsonConvert.SerializeObject(config, Formatting.Indented);
File.WriteAllText("custom_config.json", configJson);

// Use in dumping process
var executor = new Il2CppExecutor(metadata, il2Cpp);
var decompiler = new Il2CppDecompiler(executor);
decompiler.Decompile(config, outputPath);
```

### Example 5: Manual Registration Addresses

```csharp
// When auto-detection fails, use manual addresses
var mainForm = new MainForm();

// Set manual addresses (obtained from reverse engineering)
ulong codeRegistration = 0x12345678;
ulong metadataRegistration = 0x87654321;

// Initialize with manual addresses
if (mainForm.Init("libil2cpp.so", "global-metadata.dat", out var metadata, out var il2Cpp))
{
    // Override with manual addresses
    il2Cpp.Init(codeRegistration, metadataRegistration);
    mainForm.Dump(metadata, il2Cpp, outputPath);
}
```

**GUI Steps:**
1. If auto-detection fails, the tool will prompt for manual addresses
2. Enter the CodeRegistration and MetadataRegistration addresses
3. The tool will use these addresses for initialization

### Example 6: Architecture-Specific Dumping

```csharp
// Dump specific Android architecture only
var mainForm = new MainForm();

// Set architecture preference (0 = All, 1 = ARM64, 2 = ARMv7, 3 = x86, 4 = x86_64)
Settings.Default.AndroArch = 1; // ARM64 only

await mainForm.APKDump("game.apk", outputPath);
```

**GUI Steps:**
1. Select desired architecture from the dropdown
2. Choose "All" for all architectures or specific one
3. Proceed with dumping

### Example 7: Split APK Handling

```csharp
// Handle split APK files (APKS, XAPK)
var mainForm = new MainForm();

string splitApkPath = @"C:\Games\MyGame.apks";
string outputPath = @"C:\Output\Split_dump\";

// Auto-dump split APK
await mainForm.SplitAPKDump(splitApkPath, outputPath);
```

**GUI Steps:**
1. Drag and drop APKS/XAPK file onto "Start" button
2. The tool will automatically:
   - Extract the split APK
   - Find metadata in the base APK
   - Extract binaries from architecture-specific APKs
   - Dump each architecture separately

## Platform-Specific Examples

### Example 8: Windows PE Dumping

```csharp
// Handle Windows PE files
var pe = new PE(File.OpenRead("Game.exe"));

// Check if dumped
if (pe.CheckDump())
{
    // Load from memory address
    pe.LoadFromMemory(0x180000000);
}

// Initialize and dump
if (pe.Search() || pe.PlusSearch(methodCount, typeDefCount, imageCount))
{
    // Dumping successful
}
```

### Example 9: Android ELF Dumping

```csharp
// Handle Android ELF files
var elf = new Elf64(File.OpenRead("libil2cpp.so"));

// Check if dumped
if (elf.IsDumped)
{
    // Reload sections for dumped files
    elf.Reload();
}

// Initialize and dump
if (elf.Search() || elf.PlusSearch(methodCount, typeDefCount, imageCount))
{
    // Dumping successful
}
```

### Example 10: iOS Mach-O Dumping

```csharp
// Handle iOS Mach-O files
var macho = new Macho64(File.OpenRead("Game"));

// Initialize and dump
if (macho.Search() || macho.PlusSearch(methodCount, typeDefCount, imageCount))
{
    // Dumping successful
}
```

### Example 11: WebAssembly Dumping

```csharp
// Handle WebAssembly files
var wasm = new WebAssembly(File.OpenRead("game.wasm"));
var memory = wasm.CreateMemory();

// Initialize and dump
if (memory.Search() || memory.PlusSearch(methodCount, typeDefCount, imageCount))
{
    // Dumping successful
}
```

## Integration Examples

### Example 12: IDA Pro Integration

```python
# ida.py usage
import json

# Load the generated script.json
with open('script.json', 'r') as f:
    data = json.load(f)

# Process methods
if "ScriptMethod" in data:
    scriptMethods = data["ScriptMethod"]
    for scriptMethod in scriptMethods:
        addr = get_addr(scriptMethod["Address"])
        name = scriptMethod["Name"]
        set_name(addr, name)

# Process strings
if "ScriptString" in data:
    scriptStrings = data["ScriptString"]
    for i, scriptString in enumerate(scriptStrings):
        addr = get_addr(scriptString["Address"])
        value = scriptString["Value"]
        name = f"StringLiteral_{i}"
        createLabel(addr, name, True, USER_DEFINED)
```

### Example 13: Ghidra Integration

```python
# ghidra.py usage
import json

# Load the generated script.json
f = askFile("script.json from Il2cppdumper", "Open")
data = json.loads(open(f.absolutePath, 'rb').read().decode('utf-8'))

# Process methods with progress monitoring
if "ScriptMethod" in data:
    scriptMethods = data["ScriptMethod"]
    monitor.initialize(len(scriptMethods))
    monitor.setMessage("Methods")
    for scriptMethod in scriptMethods:
        addr = get_addr(scriptMethod["Address"])
        name = scriptMethod["Name"]
        set_name(addr, name)
        monitor.incrementProgress(1)
```

### Example 14: Custom Analysis Script

```csharp
// Custom analysis using the dumped data
var executor = new Il2CppExecutor(metadata, il2Cpp);

// Analyze all types
foreach (var typeDef in metadata.typeDefs)
{
    string typeName = executor.GetTypeDefName(typeDef, true, true);
    Console.WriteLine($"Type: {typeName}");
    
    // Analyze methods
    for (int i = typeDef.methodStart; i < typeDef.methodStart + typeDef.methodCount; i++)
    {
        var methodDef = metadata.methodDefs[i];
        string methodName = metadata.GetStringFromIndex(methodDef.nameIndex);
        Console.WriteLine($"  Method: {methodName}");
    }
}
```

### Example 15: Type Resolution and Analysis

```csharp
// Advanced type resolution
var executor = new Il2CppExecutor(metadata, il2Cpp);

// Get type information
foreach (var il2CppType in il2Cpp.types)
{
    string typeName = executor.GetTypeName(il2CppType, true, false);
    
    // Handle different type kinds
    switch (il2CppType.type)
    {
        case Il2CppTypeEnum.IL2CPP_TYPE_CLASS:
            Console.WriteLine($"Class: {typeName}");
            break;
        case Il2CppTypeEnum.IL2CPP_TYPE_VALUETYPE:
            Console.WriteLine($"ValueType: {typeName}");
            break;
        case Il2CppTypeEnum.IL2CPP_TYPE_GENERICINST:
            Console.WriteLine($"Generic: {typeName}");
            break;
    }
}
```

### Example 16: Method Analysis

```csharp
// Analyze method specifications
var executor = new Il2CppExecutor(metadata, il2Cpp);

foreach (var methodSpec in il2Cpp.methodSpecs)
{
    var (typeName, methodName) = executor.GetMethodSpecName(methodSpec, true);
    var genericContext = executor.GetMethodSpecGenericContext(methodSpec);
    
    Console.WriteLine($"Method: {typeName}.{methodName}");
    Console.WriteLine($"  Generic Context: {genericContext.class_inst:X}, {genericContext.method_inst:X}");
}
```

### Example 17: Custom Attribute Analysis

```csharp
// Analyze custom attributes
var executor = new Il2CppExecutor(metadata, il2Cpp);

foreach (var imageDef in metadata.imageDefs)
{
    string imageName = metadata.GetStringFromIndex(imageDef.nameIndex);
    Console.WriteLine($"Image: {imageName}");
    
    // Get custom attributes for this image
    for (int i = imageDef.customAttributeStart; i < imageDef.customAttributeStart + imageDef.customAttributeCount; i++)
    {
        var attrIndex = metadata.GetCustomAttributeIndex(imageDef, i, (uint)i);
        if (attrIndex >= 0)
        {
            Console.WriteLine($"  Custom Attribute: {attrIndex}");
        }
    }
}
```

## Troubleshooting

### Common Issues and Solutions

#### Issue 1: "Can't use auto mode to process file"
**Solution:**
```csharp
// Use manual registration addresses
ulong codeRegistration = 0x12345678;  // Find these addresses manually
ulong metadataRegistration = 0x87654321;

il2Cpp.Init(codeRegistration, metadataRegistration);
```

#### Issue 2: "Metadata file supplied is not valid"
**Solution:**
```csharp
// Validate metadata file
try
{
    var metadata = new Metadata(new MemoryStream(File.ReadAllBytes(metadataPath)));
    Console.WriteLine($"Metadata version: {metadata.Version}");
}
catch (InvalidDataException ex)
{
    Console.WriteLine($"Invalid metadata: {ex.Message}");
}
```

#### Issue 3: "Il2cpp file not supported"
**Solution:**
```csharp
// Check file format
var magic = BitConverter.ToUInt32(File.ReadAllBytes(il2cppPath, 0, 4), 0);
switch (magic)
{
    case 0x6D736100: // WebAssembly
        var web = new WebAssembly(stream);
        break;
    case 0x304F534E: // NSO
        var nso = new NSO(stream);
        break;
    case 0x905A4D: // PE
        var pe = new PE(stream);
        break;
    case 0x464c457f: // ELF
        var elf = new Elf64(stream);
        break;
    case 0xCAFEBABE: // Mach-O
        var macho = new Macho64(stream);
        break;
    default:
        throw new NotSupportedException("Unsupported file format");
}
```

#### Issue 4: Memory issues with large files
**Solution:**
```csharp
// Use streaming approach for large files
using (var stream = new FileStream(largeFile, FileMode.Open, FileAccess.Read))
{
    var il2Cpp = new PE(stream);
    // Process in chunks
}
```

#### Issue 5: Permission issues
**Solution:**
```csharp
// Run as administrator or check file permissions
bool isAdmin = new WindowsPrincipal(WindowsIdentity.GetCurrent())
    .IsInRole(WindowsBuiltInRole.Administrator);

if (!isAdmin)
{
    Console.WriteLine("Consider running as administrator for better file access");
}
```

### Debugging Tips

1. **Enable Verbose Logging:**
```csharp
MainForm.Log("Debug: Starting initialization", Brushes.Cyan);
MainForm.Log($"Debug: File size: {fileInfo.Length}", Brushes.Cyan);
```

2. **Check File Integrity:**
```csharp
// Verify file sizes and formats
var il2cppInfo = new FileInfo(il2cppPath);
var metadataInfo = new FileInfo(metadataPath);

Console.WriteLine($"IL2CPP size: {il2cppInfo.Length}");
Console.WriteLine($"Metadata size: {metadataInfo.Length}");
```

3. **Validate Addresses:**
```csharp
// Check if addresses are within valid ranges
if (codeRegistration < il2Cpp.ImageBase || codeRegistration > il2Cpp.ImageBase + 0x10000000)
{
    Console.WriteLine("Warning: Code registration address seems invalid");
}
```

### Performance Optimization

1. **Use Async Operations:**
```csharp
await Task.Run(() => {
    // CPU-intensive operations
    var result = il2Cpp.Search();
});
```

2. **Memory Management:**
```csharp
// Dispose resources properly
using (var stream = new FileStream(path, FileMode.Open))
{
    // Process file
}
```

3. **Batch Processing:**
```csharp
// Process multiple files efficiently
var tasks = files.Select(file => ProcessFileAsync(file));
await Task.WhenAll(tasks);
```

---

This comprehensive guide provides practical examples for using the Il2CppDumper GUI API in various scenarios. For additional help, refer to the main API documentation or the project's GitHub issues page.