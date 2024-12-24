namespace J_RPG.Services;

using System.Text.Json;
using System.IO;

public static class ClassConfigLoader
{
    private static Dictionary<string, ClassConfig> _classConfigs { get;}

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

    public static ClassConfig GetConfig(string className)
    {
        if (_classConfigs.TryGetValue(className, out var config))
        {
            return config;
        }
        
        throw new Exception($"Class configuration for {className} not found.");
    }
}

public class ClassConfig
{
    public int MaxHitPoints { get; set; }
    public int PhysicalAttackPower { get; set; }
    public int MagicAttackPower { get; set; }
    public string? Armor { get; set; }
    public int DodgeChance { get; set; }
    public int ParadeChance { get; set; }
    public int ChanceSpellResistance { get; set; }
    public int Speed { get; set; }
    public bool HasMana { get; set; }
    public int ManaPoints { get; set; }
}
