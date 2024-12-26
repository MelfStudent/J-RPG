namespace J_RPG.Services;

/// <summary>
/// Represents the configuration settings for a character class in the game.
/// This includes attributes such as health points, attack power, armor type, and other class-specific properties.
/// </summary>
public class Config
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
