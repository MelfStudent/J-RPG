namespace J_RPG.Services;

using System.Text.Json;
using System.IO;

public static class ClassConfigLoader
{
    private static Dictionary<string, ClassConfig> _classConfigs { get;}

    static ClassConfigLoader()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(basePath, "Resources", "classes.json");
        var json = File.ReadAllText(filePath);
        _classConfigs = JsonSerializer.Deserialize<Dictionary<string, ClassConfig>>(json);
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
    public string Armor { get; set; }
    public int DodgeChance { get; set; }
    public int ParadeChance { get; set; }
    public int ChanceSpellResistance { get; set; }
    public int Speed { get; set; }
    public bool HasMana { get; set; }
    public int ManaPoints { get; set; }
}
