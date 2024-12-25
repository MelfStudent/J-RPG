namespace J_RPG.Services;

using System.Text.Json;
using System.IO;

/// <summary>
/// A static class responsible for loading and accessing class configuration data from a JSON file.
/// It reads the configuration from a file and deserializes it into a dictionary of <see cref="ClassConfig"/> objects.
/// </summary>
public static class ClassConfigLoader
{
    /// <summary>
    /// A dictionary that holds class configurations, keyed by class name.
    /// </summary>
    private static Dictionary<string, ClassConfig> _classConfigs { get; }

    /// <summary>
    /// Static constructor to initialize the class configurations by loading data from a JSON file.
    /// </summary>
    static ClassConfigLoader()
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
            _classConfigs = JsonSerializer.Deserialize<Dictionary<string, ClassConfig>>(json)
                            ?? throw new InvalidDataException("Failed to deserialize class configurations. The JSON may be malformed or empty.");
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error during ClassConfigLoader initialization: {ex.Message}");
            Console.ResetColor();
            throw;
        }
    }

    /// <summary>
    /// Retrieves the configuration for a specified class.
    /// </summary>
    /// <param name="className">The name of the class whose configuration is to be retrieved.</param>
    /// <returns>A <see cref="ClassConfig"/> object representing the class configuration.</returns>
    /// <exception cref="Exception">Thrown if the class configuration cannot be found.</exception>
    public static ClassConfig GetConfig(string className)
    {
        if (_classConfigs.TryGetValue(className, out var config))
        {
            return config;
        }
        
        throw new Exception($"Class configuration for {className} not found.");
    }
}

/// <summary>
/// Represents the configuration settings for a character class in the game.
/// This includes attributes such as health points, attack power, armor type, and other class-specific properties.
/// </summary>
public class ClassConfig
{
    /// <summary>
    /// The maximum hit points for characters of this class.
    /// </summary>
    public int MaxHitPoints { get; set; }

    /// <summary>
    /// The physical attack power for characters of this class.
    /// </summary>
    public int PhysicalAttackPower { get; set; }

    /// <summary>
    /// The magical attack power for characters of this class.
    /// </summary>
    public int MagicAttackPower { get; set; }

    /// <summary>
    /// The type of armor that characters of this class can wear.
    /// </summary>
    public string? Armor { get; set; }

    /// <summary>
    /// The chance (in percentage) that characters of this class can dodge an attack.
    /// </summary>
    public int DodgeChance { get; set; }

    /// <summary>
    /// The chance (in percentage) that characters of this class can parade (deflect) an attack.
    /// </summary>
    public int ParadeChance { get; set; }

    /// <summary>
    /// The chance (in percentage) that characters of this class can resist a spell.
    /// </summary>
    public int ChanceSpellResistance { get; set; }

    /// <summary>
    /// The speed of characters of this class. Affects turn order and actions in combat.
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// Indicates whether characters of this class have mana points.
    /// </summary>
    public bool HasMana { get; set; }

    /// <summary>
    /// The total mana points for characters of this class, if applicable.
    /// </summary>
    public int ManaPoints { get; set; }
}
