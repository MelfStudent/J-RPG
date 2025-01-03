﻿namespace J_RPG.Services;

using System.Text.Json;
using System.IO;

/// <summary>
/// A static class responsible for loading and accessing class configuration data from a JSON file.
/// It reads the configuration from a file and deserializes it into a dictionary of <see cref="Config"/> objects.
/// </summary>
public static class ConfigLoader
{
    /// <summary>
    /// A dictionary that holds class configurations, keyed by class name.
    /// </summary>
    private static Dictionary<string, Config> _classConfigs { get; }

    /// <summary>
    /// Static constructor to initialize the class configurations by loading data from a JSON file.
    /// </summary>
    static ConfigLoader()
    {
        try
        {
            // Define the base path of the application and the path to the configuration file
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "Resources", "classes.json");
            
            // Check if the configuration file exists
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Configuration file not found at path: {filePath}");
            }
            
            // Read the JSON data from the file
            var json = File.ReadAllText(filePath);
            
            // Deserialize the JSON into a dictionary of ClassConfig objects
            _classConfigs = JsonSerializer.Deserialize<Dictionary<string, Config>>(json)
                            ?? throw new InvalidDataException("Failed to deserialize class configurations. The JSON may be malformed or empty.");
        }
        catch (Exception ex)
        {
            // Handle any exceptions that occur during initialization
            Utils.LogError($"Error during ConfigLoader initialization: {ex.Message}");
            throw;
        }
    }

    /// <summary>
    /// Retrieves the configuration for a specified class.
    /// </summary>
    /// <param name="className">The name of the class whose configuration is to be retrieved.</param>
    /// <returns>A <see cref="Config"/> object representing the class configuration.</returns>
    /// <exception cref="Exception">Thrown if the class configuration cannot be found.</exception>
    public static Config GetConfig(string className)
    {
        if (_classConfigs.TryGetValue(className, out var config))
        {
            return config;
        }
        
        throw new Exception($"Class configuration for {className} not found.");
    }
}
