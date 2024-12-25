namespace J_RPG.Services;

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
            var basePath = AppDomain.CurrentDomain.BaseDirectory;
            var filePath = Path.Combine(basePath, "Resources", "classes.json");
            
            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException($"Configuration file not found at path: {filePath}");
            }
            
            var json = File.ReadAllText(filePath);
            _classConfigs = JsonSerializer.Deserialize<Dictionary<string, Config>>(json)
                            ?? throw new InvalidDataException("Failed to deserialize class configurations. The JSON may be malformed or empty.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error during ConfigLoader initialization: {ex.Message}");
            Console.ResetColor();
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
