# Il2CppDumper GUI - Complete Documentation

## Overview

Il2CppDumper GUI is a comprehensive reverse engineering tool for Unity IL2CPP applications. It provides both a graphical user interface and extensive programmatic APIs for dumping IL2CPP metadata, generating dummy DLLs, and creating analysis scripts for various reverse engineering tools.

## 📁 Documentation Structure

This repository contains comprehensive documentation for all public APIs, functions, and components:

### 📋 Main Documentation Files

1. **[API_Documentation.md](./API_Documentation.md)** - Complete API reference
   - Core Components (MainForm, Il2Cpp, Metadata)
   - Executable Format Handlers (PE, ELF, Mach-O, WebAssembly, NSO)
   - Metadata Handling Classes
   - Utility Classes and Extensions
   - GUI Components
   - Python Scripts for Analysis Tools
   - Configuration Options
   - Data Structures and Enums

2. **[Usage_Examples.md](./Usage_Examples.md)** - Practical usage examples
   - Getting Started Guide
   - Basic and Advanced Usage Examples
   - Platform-Specific Examples
   - Integration Examples (IDA Pro, Ghidra, Hopper)
   - Troubleshooting Guide
   - Performance Optimization Tips

3. **[README_Complete.md](./README_Complete.md)** - This overview document

## 🚀 Quick Start

### Prerequisites
- Windows 10/11 (64-bit recommended)
- .NET 8.0 Runtime
- Target files: IL2CPP binary and global-metadata.dat

### Basic Usage

#### GUI Method
1. Download and run `Il2CppDumper GUI.exe`
2. Drag and drop APK/IPA files onto the "Start" button
3. Or manually select:
   - Executable file (libil2cpp.so, Game.exe, etc.)
   - global-metadata.dat file
   - Output directory
4. Click "Start" to begin dumping

#### Programmatic Method
```csharp
var mainForm = new MainForm();
await mainForm.APKDump("game.apk", "C:\\output\\");
```

## 🏗️ Architecture Overview

### Core Components

```
Il2CppDumper GUI
├── MainForm (GUI Controller)
├── Il2Cpp (Base Binary Handler)
│   ├── PE (Windows)
│   ├── Elf/Elf64 (Android)
│   ├── Macho/Macho64 (iOS)
│   ├── WebAssembly (Web)
│   └── NSO (Nintendo Switch)
├── Metadata (Metadata Parser)
├── Il2CppExecutor (Type Resolution)
├── SectionHelper (Address Finding)
└── Utils (Helper Classes)
```

### Supported Platforms

| Platform | Binary Format | Extensions | Notes |
|----------|---------------|------------|-------|
| Windows | PE | .exe, .dll | 32/64-bit support |
| Android | ELF | .so | ARM64, ARMv7, x86, x86_64 |
| iOS | Mach-O | (no extension) | ARM64, ARMv7 |
| Web | WebAssembly | .wasm | Browser-based games |
| Nintendo Switch | NSO | .nso | Compressed format |

## 📚 API Reference Summary

### Main Classes

#### MainForm
- **Purpose**: Main GUI controller and orchestration
- **Key Methods**: `Init()`, `Dump()`, `APKDump()`, `IPADump()`
- **Usage**: Primary entry point for all dumping operations

#### Il2Cpp (Abstract Base)
- **Purpose**: Base class for all binary format handlers
- **Key Methods**: `Search()`, `Init()`, `MapVATR()`, `GetMethodPointer()`
- **Usage**: Handles IL2CPP binary parsing and address mapping

#### Metadata
- **Purpose**: Parses and provides access to IL2CPP metadata
- **Key Methods**: `GetStringFromIndex()`, `GetStringLiteralFromIndex()`
- **Usage**: Extracts type, method, and field information

#### Il2CppExecutor
- **Purpose**: Executes IL2CPP operations and type resolution
- **Key Methods**: `GetTypeName()`, `GetTypeDefName()`, `GetMethodSpecName()`
- **Usage**: Resolves complex type names and generic types

### Utility Classes

#### SectionHelper
- **Purpose**: Locates IL2CPP registration structures
- **Key Methods**: `FindCodeRegistration()`, `FindMetadataRegistration()`
- **Usage**: Automatic address detection for IL2CPP structures

#### BinaryReaderExtensions
- **Purpose**: Extended binary reading capabilities
- **Key Methods**: `ReadULeb128()`, `ReadCompressedUInt32()`
- **Usage**: Handles IL2CPP-specific data formats

## 🔧 Configuration

### Config Class Properties
```csharp
public class Config
{
    public bool DumpMethod { get; set; } = true;
    public bool DumpField { get; set; } = true;
    public bool DumpProperty { get; set; } = true;
    public bool DumpAttribute { get; set; } = true;
    public bool DumpFieldOffset { get; set; } = true;
    public bool DumpMethodOffset { get; set; } = true;
    public bool DumpTypeDefIndex { get; set; } = true;
    public bool GenerateDummyDll { get; set; } = true;
    public bool GenerateStruct { get; set; } = true;
    public bool DummyDllAddToken { get; set; } = true;
    public bool RequireAnyKey { get; set; } = true;
    public bool ForceIl2CppVersion { get; set; } = false;
    public double ForceVersion { get; set; } = 24.3;
    public bool ForceDump { get; set; } = false;
    public bool NoRedirectedPointer { get; set; } = false;
}
```

### Configuration File (config.json)
```json
{
  "DumpMethod": true,
  "DumpField": true,
  "DumpProperty": true,
  "DumpAttribute": true,
  "DumpFieldOffset": true,
  "DumpMethodOffset": true,
  "DumpTypeDefIndex": true,
  "GenerateDummyDll": true,
  "GenerateStruct": true,
  "DummyDllAddToken": true,
  "RequireAnyKey": true,
  "ForceIl2CppVersion": false,
  "ForceVersion": 24.3,
  "ForceDump": false,
  "NoRedirectedPointer": false
}
```

## 🐍 Python Scripts

### Analysis Tool Integration

The tool generates Python scripts for various reverse engineering tools:

#### IDA Pro Scripts
- `ida.py` - Main IDA Pro script
- `ida_py3.py` - Python 3 compatible
- `ida_with_struct.py` - With struct generation
- `ida_with_struct_py3.py` - Python 3 with structs

#### Ghidra Scripts
- `ghidra.py` - Main Ghidra script
- `ghidra_wasm.py` - WebAssembly support
- `ghidra_with_struct.py` - With struct generation

#### Other Tools
- `hopper-py3.py` - Hopper disassembler
- `il2cpp_header_to_binja.py` - Binary Ninja
- `il2cpp_header_to_ghidra.py` - Ghidra header import

### Usage Example
```python
# Load generated script.json in IDA Pro
import json

with open('script.json', 'r') as f:
    data = json.load(f)

# Process methods
if "ScriptMethod" in data:
    for scriptMethod in data["ScriptMethod"]:
        addr = get_addr(scriptMethod["Address"])
        name = scriptMethod["Name"]
        set_name(addr, name)
```

## 📖 Usage Examples

### Basic APK Dumping
```csharp
var mainForm = new MainForm();
await mainForm.APKDump("game.apk", "C:\\output\\");
```

### Manual File Selection
```csharp
var mainForm = new MainForm();
if (mainForm.Init("libil2cpp.so", "global-metadata.dat", out var metadata, out var il2Cpp))
{
    mainForm.Dump(metadata, il2Cpp, "C:\\output\\");
}
```

### Custom Configuration
```csharp
var config = new Config
{
    GenerateDummyDll = true,
    GenerateStruct = true,
    ForceVersion = 27.0
};

var executor = new Il2CppExecutor(metadata, il2Cpp);
var decompiler = new Il2CppDecompiler(executor);
decompiler.Decompile(config, outputPath);
```

### Type Resolution
```csharp
var executor = new Il2CppExecutor(metadata, il2Cpp);
string typeName = executor.GetTypeName(il2CppType, true, false);
// Returns: "System.Collections.Generic.List<int>"
```

## 🔍 Advanced Features

### Auto-Detection
- Automatic file format detection
- Automatic architecture detection
- Automatic registration address finding
- Automatic metadata parsing

### Security Features
- Input validation and sanitization
- File size limits
- Path traversal protection
- Secure JSON deserialization
- HTTPS-only update checks

### Performance Optimizations
- Memory-efficient processing
- Async file operations
- String and type caching
- Streaming for large files
- Parallel processing support

## 🛠️ Troubleshooting

### Common Issues

1. **"Can't use auto mode to process file"**
   - Use manual registration addresses
   - Check if file is properly dumped
   - Verify file format compatibility

2. **"Metadata file supplied is not valid"**
   - Verify global-metadata.dat integrity
   - Check file version compatibility
   - Ensure file is not corrupted

3. **"Il2cpp file not supported"**
   - Check file format (PE, ELF, Mach-O, etc.)
   - Verify file is not encrypted or obfuscated
   - Try different IL2CPP version

4. **Memory issues with large files**
   - Use 64-bit version of the tool
   - Increase system memory
   - Process files in smaller chunks

### Debug Tips
- Enable verbose logging
- Check file integrity
- Validate addresses manually
- Use async operations for large files

## 📊 Output Files

The tool generates several output files:

### Core Output
- `dump.cs` - Complete C# code dump
- `script.json` - Analysis tool script data
- `DummyDll/` - Generated dummy DLLs
- `Struct/` - Generated struct definitions

### Analysis Scripts
- `ida.py` - IDA Pro script
- `ghidra.py` - Ghidra script
- `hopper-py3.py` - Hopper script
- Additional tool-specific scripts

### Metadata Files
- `global-metadata.dat` - Original metadata (if extracted)
- `libil2cpp.so` - Original binary (if extracted)

## 🔗 Integration

### With Reverse Engineering Tools
1. **IDA Pro**: Use generated `ida.py` script
2. **Ghidra**: Use generated `ghidra.py` script
3. **Hopper**: Use generated `hopper-py3.py` script
4. **Binary Ninja**: Use generated `il2cpp_header_to_binja.py`

### With Custom Tools
- Parse `script.json` for method/string information
- Use generated dummy DLLs for type information
- Import struct definitions for better analysis

## 📝 License and Credits

- **Original Author**: [Perfare](https://github.com/Perfare)
- **GUI Version**: [AndnixSH](https://github.com/AndnixSH)
- **Custom Version**: Mr D - DS Gaming (VNC Team)
- **License**: See project repository for details

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## 📞 Support

- **GitHub Issues**: Report bugs and request features
- **Documentation**: Refer to this comprehensive guide
- **Community**: Join discussions in the project repository

---

## 📚 Complete Documentation Index

1. **[API_Documentation.md](./API_Documentation.md)** - Complete API reference with all classes, methods, and properties
2. **[Usage_Examples.md](./Usage_Examples.md)** - Practical examples and troubleshooting guide
3. **[README_Complete.md](./README_Complete.md)** - This overview document

For detailed information about any specific component, refer to the appropriate documentation file. The API documentation provides comprehensive coverage of all public interfaces, while the usage examples demonstrate practical implementation patterns.

**Happy Reverse Engineering! 🎯**