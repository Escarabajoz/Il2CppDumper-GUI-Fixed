# Il2CppDumper GUI - Quick Reference

## 🚀 Quick Start Commands

### Basic Dumping
```csharp
// APK Auto-dump
var mainForm = new MainForm();
await mainForm.APKDump("game.apk", "C:\\output\\");

// IPA Auto-dump
await mainForm.IPADump("game.ipa", "C:\\output\\");

// Manual dump
if (mainForm.Init("libil2cpp.so", "global-metadata.dat", out var metadata, out var il2Cpp))
{
    mainForm.Dump(metadata, il2Cpp, "C:\\output\\");
}
```

## 📋 Core Classes

| Class | Purpose | Key Methods |
|-------|---------|-------------|
| `MainForm` | GUI Controller | `Init()`, `Dump()`, `APKDump()`, `IPADump()` |
| `Il2Cpp` | Binary Handler | `Search()`, `Init()`, `MapVATR()` |
| `Metadata` | Metadata Parser | `GetStringFromIndex()`, `GetStringLiteralFromIndex()` |
| `Il2CppExecutor` | Type Resolution | `GetTypeName()`, `GetTypeDefName()` |
| `SectionHelper` | Address Finding | `FindCodeRegistration()`, `FindMetadataRegistration()` |

## 🏗️ Executable Formats

| Format | Class | Platform | Notes |
|--------|-------|----------|-------|
| PE | `PE` | Windows | 32/64-bit |
| ELF | `Elf`/`Elf64` | Android | ARM64, ARMv7, x86, x86_64 |
| Mach-O | `Macho`/`Macho64` | iOS | ARM64, ARMv7 |
| WebAssembly | `WebAssembly` | Web | Browser games |
| NSO | `NSO` | Nintendo Switch | Compressed |

## ⚙️ Configuration Options

```csharp
var config = new Config
{
    DumpMethod = true,           // Dump method information
    DumpField = true,            // Dump field information
    DumpProperty = true,         // Dump property information
    DumpAttribute = true,        // Dump custom attributes
    DumpFieldOffset = true,      // Dump field offsets
    DumpMethodOffset = true,     // Dump method offsets
    DumpTypeDefIndex = true,     // Dump type definition indices
    GenerateDummyDll = true,     // Generate dummy DLLs
    GenerateStruct = true,       // Generate struct definitions
    DummyDllAddToken = true,     // Add tokens to dummy DLLs
    RequireAnyKey = true,        // Require key press
    ForceIl2CppVersion = false,  // Force specific IL2CPP version
    ForceVersion = 24.3,         // Version to force
    ForceDump = false,           // Force dump mode
    NoRedirectedPointer = false  // Disable pointer redirection
};
```

## 🐍 Python Scripts

| Script | Tool | Purpose |
|--------|------|---------|
| `ida.py` | IDA Pro | Import IL2CPP data |
| `ida_py3.py` | IDA Pro | Python 3 compatible |
| `ida_with_struct.py` | IDA Pro | With struct generation |
| `ghidra.py` | Ghidra | Import IL2CPP data |
| `ghidra_wasm.py` | Ghidra | WebAssembly support |
| `ghidra_with_struct.py` | Ghidra | With struct generation |
| `hopper-py3.py` | Hopper | Python 3 compatible |
| `il2cpp_header_to_binja.py` | Binary Ninja | Header import |
| `il2cpp_header_to_ghidra.py` | Ghidra | Header import |

## 🔧 Common Operations

### Type Resolution
```csharp
var executor = new Il2CppExecutor(metadata, il2Cpp);
string typeName = executor.GetTypeName(il2CppType, true, false);
// Returns: "System.Collections.Generic.List<int>"
```

### Method Analysis
```csharp
var (typeName, methodName) = executor.GetMethodSpecName(methodSpec, true);
Console.WriteLine($"Method: {typeName}.{methodName}");
```

### Manual Addresses
```csharp
ulong codeRegistration = 0x12345678;
ulong metadataRegistration = 0x87654321;
il2Cpp.Init(codeRegistration, metadataRegistration);
```

### Architecture Selection
```csharp
// 0 = All, 1 = ARM64, 2 = ARMv7, 3 = x86, 4 = x86_64
Settings.Default.AndroArch = 1; // ARM64 only
```

## 🚨 Error Handling

### Common Errors and Solutions

| Error | Solution |
|-------|----------|
| "Can't use auto mode" | Use manual registration addresses |
| "Invalid metadata file" | Check file integrity and version |
| "File not supported" | Verify file format compatibility |
| "Memory issues" | Use 64-bit version, increase RAM |

### Debug Logging
```csharp
MainForm.Log("Debug message", Brushes.Cyan);
MainForm.Log("Error message", Brushes.Red);
MainForm.Log("Success message", Brushes.Green);
```

## 📁 Output Files

| File | Purpose |
|------|---------|
| `dump.cs` | Complete C# code dump |
| `script.json` | Analysis tool data |
| `DummyDll/` | Generated dummy DLLs |
| `Struct/` | Generated struct definitions |
| `ida.py` | IDA Pro script |
| `ghidra.py` | Ghidra script |

## 🔍 File Format Detection

```csharp
var magic = BitConverter.ToUInt32(File.ReadAllBytes(path, 0, 4), 0);
switch (magic)
{
    case 0x6D736100: // WebAssembly
    case 0x304F534E: // NSO
    case 0x905A4D:   // PE
    case 0x464c457f: // ELF
    case 0xCAFEBABE: // Mach-O
}
```

## 🎯 Best Practices

1. **Use 64-bit version** for large files
2. **Enable verbose logging** for debugging
3. **Validate file integrity** before processing
4. **Use async operations** for large files
5. **Check file permissions** and run as admin if needed
6. **Backup original files** before processing
7. **Use custom config** for specific requirements

## 📞 Quick Help

- **GUI Issues**: Check file paths and permissions
- **Auto-detection fails**: Use manual registration addresses
- **Memory errors**: Use 64-bit version, increase RAM
- **Format errors**: Verify file compatibility
- **Performance**: Use async operations, enable caching

---

**For detailed information, see:**
- [API_Documentation.md](./API_Documentation.md) - Complete API reference
- [Usage_Examples.md](./Usage_Examples.md) - Practical examples
- [README_Complete.md](./README_Complete.md) - Full overview