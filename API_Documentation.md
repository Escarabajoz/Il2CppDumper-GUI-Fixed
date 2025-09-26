# Il2CppDumper GUI - Comprehensive API Documentation

## Table of Contents
1. [Overview](#overview)
2. [Core Components](#core-components)
3. [Executable Format Handlers](#executable-format-handlers)
4. [Metadata Handling](#metadata-handling)
5. [Utility Classes](#utility-classes)
6. [GUI Components](#gui-components)
7. [Python Scripts](#python-scripts)
8. [Configuration](#configuration)
9. [Usage Examples](#usage-examples)
10. [API Reference](#api-reference)

## Overview

Il2CppDumper GUI is a comprehensive tool for reverse engineering Unity IL2CPP applications. It provides both a graphical user interface and programmatic APIs for dumping IL2CPP metadata, generating dummy DLLs, and creating analysis scripts for various reverse engineering tools.

### Key Features
- **Multi-platform Support**: Windows, Android, iOS
- **Multiple Executable Formats**: PE, ELF, Mach-O, WebAssembly, NSO
- **Analysis Tool Integration**: IDA Pro, Ghidra, Hopper, Binary Ninja
- **Automated Dumping**: APK, IPA, and split APK support
- **Security Enhancements**: Input validation, secure file handling

## Core Components

### MainForm Class
**Namespace**: `Il2CppDumper`  
**Type**: `public partial class MainForm : Window`

The main GUI window that orchestrates the entire dumping process.

#### Key Properties
```csharp
public static MainForm main { get; private set; }
internal bool ActionButtonsEnabled { get; set; }
```

#### Key Methods

##### `Init(string il2cppPath, string metadataPath, out Metadata metadata, out Il2Cpp il2Cpp)`
Initializes the IL2CPP dumping process.

**Parameters:**
- `il2cppPath` (string): Path to the IL2CPP binary file
- `metadataPath` (string): Path to the global-metadata.dat file
- `metadata` (out Metadata): Output metadata object
- `il2Cpp` (out Il2Cpp): Output IL2CPP object

**Returns:** `bool` - True if initialization successful

**Example:**
```csharp
if (Init("libil2cpp.so", "global-metadata.dat", out var metadata, out var il2Cpp))
{
    // Proceed with dumping
    Dump(metadata, il2Cpp, outputPath);
}
```

##### `Dump(Metadata metadata, Il2Cpp il2Cpp, string outputDir)`
Performs the actual dumping process.

**Parameters:**
- `metadata` (Metadata): Initialized metadata object
- `il2Cpp` (Il2Cpp): Initialized IL2CPP object
- `outputDir` (string): Output directory path

**Example:**
```csharp
var executor = new Il2CppExecutor(metadata, il2Cpp);
var decompiler = new Il2CppDecompiler(executor);
decompiler.Decompile(config, outputDir);
```

##### `Dumper(string file, string metadataPath, string outputPath)`
High-level method that combines initialization and dumping.

**Parameters:**
- `file` (string): Path to IL2CPP binary
- `metadataPath` (string): Path to metadata file
- `outputPath` (string): Output directory

**Example:**
```csharp
Dumper("libil2cpp.so", "global-metadata.dat", "C:\\output\\");
```

#### Auto-Dump Methods

##### `IPADump(string file, string outputPath)`
Automatically extracts and dumps iOS IPA files.

**Parameters:**
- `file` (string): Path to IPA file
- `outputPath` (string): Output directory

**Example:**
```csharp
await IPADump("game.ipa", "C:\\output\\");
```

##### `APKDump(string file, string outputPath)`
Automatically extracts and dumps Android APK files.

**Parameters:**
- `file` (string): Path to APK file
- `outputPath` (string): Output directory

**Example:**
```csharp
await APKDump("game.apk", "C:\\output\\");
```

##### `SplitAPKDump(string file, string outputPath)`
Handles split APK files (APKS, XAPK).

**Parameters:**
- `file` (string): Path to split APK file
- `outputPath` (string): Output directory

**Example:**
```csharp
await SplitAPKDump("game.apks", "C:\\output\\");
```

#### Logging Methods

##### `Log(string text, SolidColorBrush color = null)`
Logs messages to the GUI with optional color formatting.

**Parameters:**
- `text` (string): Message to log
- `color` (SolidColorBrush, optional): Text color

**Example:**
```csharp
Log("Dumping started...", Brushes.Green);
Log("Error occurred", Brushes.Red);
```

### Il2Cpp Class
**Namespace**: `Il2CppDumper`  
**Type**: `public abstract class Il2Cpp : BinaryStream`

Base class for all IL2CPP binary format handlers.

#### Key Properties
```csharp
public ulong[] methodPointers { get; set; }
public Il2CppType[] types { get; set; }
public bool IsDumped { get; set; }
public double Version { get; set; }
```

#### Key Methods

##### `SetProperties(double version, long metadataUsagesCount)`
Sets version and metadata usage count properties.

**Parameters:**
- `version` (double): IL2CPP version
- `metadataUsagesCount` (long): Number of metadata usages

##### `Init(ulong codeRegistration, ulong metadataRegistration)`
Initializes IL2CPP with registration addresses.

**Parameters:**
- `codeRegistration` (ulong): Code registration address
- `metadataRegistration` (ulong): Metadata registration address

##### `MapVATR(ulong addr)`
Maps virtual address to raw address.

**Parameters:**
- `addr` (ulong): Virtual address

**Returns:** `ulong` - Raw address

##### `GetMethodPointer(string imageName, Il2CppMethodDefinition methodDef)`
Gets method pointer for a specific method.

**Parameters:**
- `imageName` (string): Image name
- `methodDef` (Il2CppMethodDefinition): Method definition

**Returns:** `ulong` - Method pointer address

## Executable Format Handlers

### PE Class
**Namespace**: `Il2CppDumper`  
**Type**: `public sealed class PE : Il2Cpp`

Handles Windows PE (Portable Executable) files.

#### Constructor
```csharp
public PE(Stream stream)
```

#### Key Methods

##### `LoadFromMemory(ulong addr)`
Loads PE from memory address (for dumped files).

**Parameters:**
- `addr` (ulong): Memory address

**Example:**
```csharp
var pe = new PE(stream);
pe.LoadFromMemory(0x180000000);
```

##### `CheckDump()`
Checks if the PE file is dumped.

**Returns:** `bool` - True if dumped

### Elf Class
**Namespace**: `Il2CppDumper`  
**Type**: `public sealed class Elf : ElfBase`

Handles 32-bit ELF files (Android).

#### Constructor
```csharp
public Elf(Stream stream)
```

#### Key Methods

##### `Reload()`
Reloads ELF sections for dumped files.

**Example:**
```csharp
if (elf.IsDumped)
{
    elf.Reload();
}
```

### Elf64 Class
**Namespace**: `Il2CppDumper`  
**Type**: `public sealed class Elf64 : ElfBase`

Handles 64-bit ELF files (Android).

#### Constructor
```csharp
public Elf64(Stream stream)
```

### Macho Class
**Namespace**: `Il2CppDumper`  
**Type**: `public sealed class Macho : Il2Cpp`

Handles 32-bit Mach-O files (iOS).

#### Constructor
```csharp
public Macho(Stream stream)
```

### Macho64 Class
**Namespace**: `Il2CppDumper`  
**Type**: `public sealed class Macho64 : Il2Cpp`

Handles 64-bit Mach-O files (iOS).

#### Constructor
```csharp
public Macho64(Stream stream)
```

### WebAssembly Class
**Namespace**: `Il2CppDumper`  
**Type**: `public sealed class WebAssembly : Il2Cpp`

Handles WebAssembly files.

#### Constructor
```csharp
public WebAssembly(Stream stream)
```

#### Key Methods

##### `CreateMemory()`
Creates WebAssembly memory object.

**Returns:** `WebAssemblyMemory` - Memory object

### NSO Class
**Namespace**: `Il2CppDumper`  
**Type**: `public sealed class NSO : Il2Cpp`

Handles Nintendo Switch NSO files.

#### Constructor
```csharp
public NSO(Stream stream)
```

#### Key Methods

##### `UnCompress()`
Decompresses NSO file.

**Returns:** `Il2Cpp` - Decompressed IL2CPP object

## Metadata Handling

### Metadata Class
**Namespace**: `Il2CppDumper`  
**Type**: `public sealed class Metadata : BinaryStream`

Handles IL2CPP metadata parsing and access.

#### Key Properties
```csharp
public Il2CppGlobalMetadataHeader header { get; set; }
public Il2CppImageDefinition[] imageDefs { get; set; }
public Il2CppTypeDefinition[] typeDefs { get; set; }
public Il2CppMethodDefinition[] methodDefs { get; set; }
public double Version { get; set; }
```

#### Key Methods

##### `GetStringFromIndex(uint index)`
Gets string from string index.

**Parameters:**
- `index` (uint): String index

**Returns:** `string` - String value

**Example:**
```csharp
string typeName = metadata.GetStringFromIndex(typeDef.nameIndex);
```

##### `GetStringLiteralFromIndex(uint index)`
Gets string literal from index.

**Parameters:**
- `index` (uint): String literal index

**Returns:** `string` - String literal value

##### `GetFieldDefaultValueFromIndex(int index, out Il2CppFieldDefaultValue value)`
Gets field default value.

**Parameters:**
- `index` (int): Field index
- `value` (out Il2CppFieldDefaultValue): Output default value

**Returns:** `bool` - True if found

##### `GetCustomAttributeIndex(Il2CppImageDefinition imageDef, int customAttributeIndex, uint token)`
Gets custom attribute index.

**Parameters:**
- `imageDef` (Il2CppImageDefinition): Image definition
- `customAttributeIndex` (int): Custom attribute index
- `token` (uint): Token

**Returns:** `int` - Attribute index

### Il2CppExecutor Class
**Namespace**: `Il2CppDumper`  
**Type**: `public class Il2CppExecutor`

Executes IL2CPP operations and provides type resolution.

#### Constructor
```csharp
public Il2CppExecutor(Metadata metadata, Il2Cpp il2Cpp)
```

#### Key Methods

##### `GetTypeName(Il2CppType il2CppType, bool addNamespace, bool is_nested)`
Gets full type name.

**Parameters:**
- `il2CppType` (Il2CppType): IL2CPP type
- `addNamespace` (bool): Include namespace
- `is_nested` (bool): Is nested type

**Returns:** `string` - Type name

**Example:**
```csharp
string fullName = executor.GetTypeName(il2CppType, true, false);
// Returns: "System.Collections.Generic.List<int>"
```

##### `GetTypeDefName(Il2CppTypeDefinition typeDef, bool addNamespace, bool genericParameter)`
Gets type definition name.

**Parameters:**
- `typeDef` (Il2CppTypeDefinition): Type definition
- `addNamespace` (bool): Include namespace
- `genericParameter` (bool): Include generic parameters

**Returns:** `string` - Type definition name

##### `GetMethodSpecName(Il2CppMethodSpec methodSpec, bool addNamespace = false)`
Gets method specification name.

**Parameters:**
- `methodSpec` (Il2CppMethodSpec): Method specification
- `addNamespace` (bool): Include namespace

**Returns:** `(string, string)` - Tuple of (type name, method name)

##### `GetTypeDefinitionFromIl2CppType(Il2CppType il2CppType)`
Gets type definition from IL2CPP type.

**Parameters:**
- `il2CppType` (Il2CppType): IL2CPP type

**Returns:** `Il2CppTypeDefinition` - Type definition

## Utility Classes

### SectionHelper Class
**Namespace**: `Il2CppDumper`  
**Type**: `public class SectionHelper`

Helps locate IL2CPP registration structures in binary files.

#### Constructor
```csharp
public SectionHelper(Il2Cpp il2Cpp, int methodCount, int typeDefinitionsCount, long metadataUsagesCount, int imageCount)
```

#### Key Methods

##### `FindCodeRegistration()`
Finds code registration structure.

**Returns:** `ulong` - Code registration address

##### `FindMetadataRegistration()`
Finds metadata registration structure.

**Returns:** `ulong` - Metadata registration address

##### `SetSection(SearchSectionType type, params SearchSection[] secs)`
Sets search sections.

**Parameters:**
- `type` (SearchSectionType): Section type
- `secs` (SearchSection[]): Sections

### BinaryReaderExtensions Class
**Namespace**: `Il2CppDumper`  
**Type**: `public static class BinaryReaderExtensions`

Extension methods for BinaryReader.

#### Key Methods

##### `ReadULeb128(this BinaryReader reader)`
Reads unsigned LEB128 value.

**Returns:** `uint` - LEB128 value

##### `ReadCompressedUInt32(this BinaryReader reader)`
Reads compressed unsigned 32-bit integer.

**Returns:** `uint` - Compressed integer

##### `ReadCompressedInt32(this BinaryReader reader)`
Reads compressed signed 32-bit integer.

**Returns:** `int` - Compressed integer

### ZipUtils Class
**Namespace**: `Il2CppDumper`  
**Type**: `public static class ZipUtils`

Utility methods for ZIP file operations.

#### Key Methods

##### `ExtractFile(ZipArchiveEntry entry, string destinationPath)`
Extracts file from ZIP archive.

**Parameters:**
- `entry` (ZipArchiveEntry): ZIP entry
- `destinationPath` (string): Destination path

### DirectoryUtils Class
**Namespace**: `Il2CppDumper`  
**Type**: `public static class DirectoryUtils`

Utility methods for directory operations.

#### Key Methods

##### `Delete(string path)`
Safely deletes directory.

**Parameters:**
- `path` (string): Directory path

## GUI Components

### MainForm.xaml
The main GUI layout file defining the user interface.

#### Key UI Elements
- **File Selection**: Binary file, metadata file, output directory
- **Architecture Selection**: Android architecture dropdown
- **Options**: Various dumping and generation options
- **Script Selection**: Analysis tool script checkboxes
- **Log Display**: Rich text box for operation logs

### InputOffsetForm Class
**Namespace**: `Il2CppDumper.Forms`  
**Type**: `public partial class InputOffsetForm : Window`

Dialog for manual offset input.

#### Key Properties
```csharp
public string ReturnedOffset { get; set; }
```

### DragDropUtils Class
**Namespace**: `Il2CppDumper.Forms`  
**Type**: `public static class DragDropUtils`

Handles drag and drop operations.

#### Key Methods

##### `CheckDragOver(this DragEventArgs e)`
Checks if drag operation is valid.

**Returns:** `bool` - True if valid

##### `CheckDragOver(this DragEventArgs e, string extension)`
Checks drag operation for specific extension.

**Parameters:**
- `extension` (string): File extension

**Returns:** `bool` - True if valid

##### `GetFilesDrop(this DragEventArgs e)`
Gets dropped files.

**Returns:** `string[]` - File paths

## Python Scripts

### IDA Pro Scripts

#### ida.py
Main IDA Pro script for importing IL2CPP dumps.

**Usage:**
1. Run Il2CppDumper to generate `script.json`
2. Load the target binary in IDA Pro
3. Run `ida.py` script
4. Select the generated `script.json` file

**Features:**
- Function creation and naming
- String literal labeling
- Metadata method labeling
- Address range processing

#### ida_py3.py
Python 3 compatible version of IDA script.

#### ida_with_struct.py
IDA script with struct generation support.

#### ida_with_struct_py3.py
Python 3 version with struct support.

### Ghidra Scripts

#### ghidra.py
Main Ghidra script for importing IL2CPP dumps.

**Usage:**
1. Run Il2CppDumper to generate `script.json`
2. Load the target binary in Ghidra
3. Run `ghidra.py` script
4. Select the generated `script.json` file

**Features:**
- Function creation and naming
- String literal labeling
- Metadata method labeling
- Progress monitoring

#### ghidra_wasm.py
Ghidra script for WebAssembly files.

#### ghidra_with_struct.py
Ghidra script with struct generation support.

### Hopper Scripts

#### hopper-py3.py
Hopper disassembler script for Python 3.

### Binary Ninja Scripts

#### il2cpp_header_to_binja.py
Binary Ninja header import script.

#### il2cpp_header_to_ghidra.py
Ghidra header import script.

## Configuration

### Config Class
**Namespace**: `Il2CppDumper`  
**Type**: `public class Config`

Configuration settings for the dumping process.

#### Properties
```csharp
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
```

#### Configuration File (config.json)
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

## Usage Examples

### Basic Dumping
```csharp
// Initialize the main form
var mainForm = new MainForm();

// Set up paths
string il2cppPath = "libil2cpp.so";
string metadataPath = "global-metadata.dat";
string outputPath = "C:\\output\\";

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

### APK Auto-Dumping
```csharp
var mainForm = new MainForm();

// Auto-dump APK file
await mainForm.APKDump("game.apk", "C:\\output\\");
```

### Custom Configuration
```csharp
var config = new Config
{
    DumpMethod = true,
    DumpField = true,
    GenerateDummyDll = true,
    GenerateStruct = true,
    ForceIl2CppVersion = true,
    ForceVersion = 27.0
};

// Use custom config in dumping process
var executor = new Il2CppExecutor(metadata, il2Cpp);
var decompiler = new Il2CppDecompiler(executor);
decompiler.Decompile(config, outputPath);
```

### Manual Registration Addresses
```csharp
// When auto-detection fails, use manual addresses
ulong codeRegistration = 0x12345678;
ulong metadataRegistration = 0x87654321;

il2Cpp.Init(codeRegistration, metadataRegistration);
```

### Type Resolution
```csharp
var executor = new Il2CppExecutor(metadata, il2Cpp);

// Get type name with namespace
string fullTypeName = executor.GetTypeName(il2CppType, true, false);

// Get method specification
var (typeName, methodName) = executor.GetMethodSpecName(methodSpec, true);
```

### Section Helper Usage
```csharp
var sectionHelper = new SectionHelper(il2Cpp, methodCount, typeDefCount, metadataUsagesCount, imageCount);

// Find registration structures
ulong codeReg = sectionHelper.FindCodeRegistration();
ulong metadataReg = sectionHelper.FindMetadataRegistration();

if (codeReg != 0 && metadataReg != 0)
{
    il2Cpp.Init(codeReg, metadataReg);
}
```

## API Reference

### Enums

#### Il2CppTypeEnum
```csharp
public enum Il2CppTypeEnum
{
    IL2CPP_TYPE_END = 0x00,
    IL2CPP_TYPE_VOID = 0x01,
    IL2CPP_TYPE_BOOLEAN = 0x02,
    IL2CPP_TYPE_CHAR = 0x03,
    IL2CPP_TYPE_I1 = 0x04,
    IL2CPP_TYPE_U1 = 0x05,
    IL2CPP_TYPE_I2 = 0x06,
    IL2CPP_TYPE_U2 = 0x07,
    IL2CPP_TYPE_I4 = 0x08,
    IL2CPP_TYPE_U4 = 0x09,
    IL2CPP_TYPE_I8 = 0x0a,
    IL2CPP_TYPE_U8 = 0x0b,
    IL2CPP_TYPE_R4 = 0x0c,
    IL2CPP_TYPE_R8 = 0x0d,
    IL2CPP_TYPE_STRING = 0x0e,
    IL2CPP_TYPE_PTR = 0x0f,
    IL2CPP_TYPE_BYREF = 0x10,
    IL2CPP_TYPE_VALUETYPE = 0x11,
    IL2CPP_TYPE_CLASS = 0x12,
    IL2CPP_TYPE_VAR = 0x13,
    IL2CPP_TYPE_ARRAY = 0x14,
    IL2CPP_TYPE_GENERICINST = 0x15,
    IL2CPP_TYPE_TYPEDBYREF = 0x16,
    IL2CPP_TYPE_I = 0x18,
    IL2CPP_TYPE_U = 0x19,
    IL2CPP_TYPE_FNPTR = 0x1b,
    IL2CPP_TYPE_OBJECT = 0x1c,
    IL2CPP_TYPE_SZARRAY = 0x1d,
    IL2CPP_TYPE_MVAR = 0x1e,
    IL2CPP_TYPE_CMOD_REQD = 0x1f,
    IL2CPP_TYPE_CMOD_OPT = 0x20,
    IL2CPP_TYPE_INTERNAL = 0x21,
    IL2CPP_TYPE_MODIFIER = 0x40,
    IL2CPP_TYPE_SENTINEL = 0x41,
    IL2CPP_TYPE_PINNED = 0x45,
    IL2CPP_TYPE_ENUM = 0x55,
    IL2CPP_TYPE_IL2CPP_TYPE_INDEX = 0x66
}
```

#### SearchSectionType
```csharp
public enum SearchSectionType
{
    Exec,
    Data,
    Bss
}
```

### Key Data Structures

#### Il2CppType
```csharp
public class Il2CppType
{
    public Il2CppTypeEnum type;
    public Il2CppTypeData data;
    public uint attrs;
    public uint token;
    public int byvalTypeIndex;
    public int byrefTypeIndex;
}
```

#### Il2CppTypeDefinition
```csharp
public class Il2CppTypeDefinition
{
    public uint nameIndex;
    public uint namespaceIndex;
    public int declaringTypeIndex;
    public int parentIndex;
    public int nestedTypesStart;
    public int nestedTypesCount;
    public int genericContainerIndex;
    public uint flags;
    public int fieldStart;
    public int methodStart;
    public int eventStart;
    public int propertyStart;
    public int nestedTypesStartIndex;
    public int interfacesStart;
    public int vtableStart;
    public int interfaceOffsetsStart;
    public int methodCount;
    public int propertyCount;
    public int fieldCount;
    public int eventCount;
    public int nestedTypeCount;
    public int vtableCount;
    public int interfacesCount;
    public int interfaceOffsetsCount;
    public int bitfield;
    public int token;
    public int rgctxStartIndex;
    public int rgctxCount;
    public int customAttributeStart;
    public int customAttributeCount;
}
```

#### Il2CppMethodDefinition
```csharp
public class Il2CppMethodDefinition
{
    public uint nameIndex;
    public uint declaringType;
    public uint returnType;
    public int parameterStart;
    public uint genericContainerIndex;
    public uint token;
    public uint flags;
    public int iflags;
    public int slot;
    public int parameterCount;
    public int genericParameterCount;
    public int methodIndex;
    public int invokerIndex;
    public int reversePInvokeWrapperIndex;
    public int rgctxStartIndex;
    public int rgctxCount;
    public int customAttributeStart;
    public int customAttributeCount;
}
```

### Error Handling

The API includes comprehensive error handling with logging:

```csharp
try
{
    // IL2CPP operations
    var result = il2Cpp.Search();
    if (!result)
    {
        MainForm.Log("Search failed, trying alternative method", Brushes.Yellow);
        result = il2Cpp.SymbolSearch();
    }
}
catch (Exception ex)
{
    MainForm.Log($"Error: {ex.Message}", Brushes.Red);
}
```

### Security Considerations

The API includes several security enhancements:

1. **Input Validation**: All file paths and user inputs are validated
2. **File Size Limits**: Config and metadata files have size limits
3. **Path Sanitization**: Prevents directory traversal attacks
4. **Secure HTTP**: HTTPS-only for update checks
5. **JSON Security**: Safe JSON deserialization with limits

### Performance Optimization

1. **Memory Management**: Efficient memory usage for large files
2. **Parallel Processing**: Async operations for file I/O
3. **Caching**: String and type caching for repeated access
4. **Streaming**: Stream-based processing for large files

---

This documentation provides comprehensive coverage of all public APIs, functions, and components in the Il2CppDumper GUI project. For additional examples and advanced usage patterns, refer to the source code and test cases.