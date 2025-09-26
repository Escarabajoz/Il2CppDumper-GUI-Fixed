using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Il2CppDumper.Utils
{
    public static class SecureConfigLoader
    {
        private const long MaxConfigFileSize = 1024 * 1024; // 1MB max for config files
        
        public static T LoadConfig<T>(string configPath) where T : new()
        {
            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentException("Config path cannot be null or empty", nameof(configPath));

            if (!File.Exists(configPath))
            {
                MainForm.Log($"Config file not found at {configPath}, using defaults", System.Windows.Media.Brushes.Yellow);
                return new T();
            }

            try
            {
                // Check file size to prevent memory exhaustion
                var fileInfo = new FileInfo(configPath);
                if (fileInfo.Length > MaxConfigFileSize)
                {
                    throw new InvalidOperationException($"Config file is too large ({fileInfo.Length} bytes). Maximum allowed: {MaxConfigFileSize} bytes.");
                }

                // Read file content
                string jsonContent = File.ReadAllText(configPath);
                
                // Validate JSON structure before deserializing
                if (string.IsNullOrWhiteSpace(jsonContent))
                {
                    MainForm.Log("Config file is empty, using defaults", System.Windows.Media.Brushes.Yellow);
                    return new T();
                }

                // Configure JsonSerializer with secure settings
                var options = new JsonSerializerOptions
                {
                    // Prevent reference loops
                    ReferenceHandler = ReferenceHandler.IgnoreCycles,
                    // Limit max depth to prevent stack overflow
                    MaxDepth = 32,
                    // Allow trailing commas for flexibility
                    AllowTrailingCommas = true,
                    // Case insensitive for robustness
                    PropertyNameCaseInsensitive = true,
                    // Ignore unknown properties to prevent injection
                    UnknownTypeHandling = JsonUnknownTypeHandling.JsonElement
                };

                // Deserialize safely
                T config = JsonSerializer.Deserialize<T>(jsonContent, options);
                
                // Validate the deserialized config
                if (config == null)
                {
                    MainForm.Log("Failed to deserialize config, using defaults", System.Windows.Media.Brushes.Yellow);
                    return new T();
                }

                // Additional validation for Config type
                if (config is Config configObj)
                {
                    ValidateConfig(configObj);
                }

                MainForm.Log("Configuration loaded successfully");
                return config;
            }
            catch (JsonException ex)
            {
                MainForm.Log($"Invalid JSON in config file: {ex.Message}", System.Windows.Media.Brushes.Orange);
                MainForm.Log("Using default configuration", System.Windows.Media.Brushes.Yellow);
                return new T();
            }
            catch (Exception ex)
            {
                MainForm.Log($"Error loading config: {ex.Message}", System.Windows.Media.Brushes.Orange);
                MainForm.Log("Using default configuration", System.Windows.Media.Brushes.Yellow);
                return new T();
            }
        }

        private static void ValidateConfig(Config config)
        {
            // Validate version ranges
            if (config.ForceVersion < 16.0 || config.ForceVersion > 50.0)
            {
                MainForm.Log($"Invalid ForceVersion value: {config.ForceVersion}. Using default.", System.Windows.Media.Brushes.Yellow);
                config.ForceVersion = 24.3;
            }

            // Log configuration values for audit (without sensitive data)
            MainForm.Log($"Config loaded - GenerateDummyDll: {config.GenerateDummyDll}, GenerateStruct: {config.GenerateStruct}");
        }

        public static bool SaveConfig<T>(T config, string configPath)
        {
            if (config == null)
                throw new ArgumentNullException(nameof(config));
                
            if (string.IsNullOrWhiteSpace(configPath))
                throw new ArgumentException("Config path cannot be null or empty", nameof(configPath));

            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = true,
                    ReferenceHandler = ReferenceHandler.IgnoreCycles
                };

                string jsonContent = JsonSerializer.Serialize(config, options);
                
                // Ensure directory exists
                string directory = Path.GetDirectoryName(configPath);
                if (!string.IsNullOrEmpty(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                File.WriteAllText(configPath, jsonContent);
                return true;
            }
            catch (Exception ex)
            {
                MainForm.Log($"Error saving config: {ex.Message}", System.Windows.Media.Brushes.Orange);
                return false;
            }
        }
    }
}