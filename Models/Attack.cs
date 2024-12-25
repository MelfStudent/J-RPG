namespace J_RPG.Models;

using Enums;

public class Attack
{
    /// <summary>
    /// Gets or sets the name of the attack.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// Gets or sets the character performing the attack.
    /// </summary>
    public Character AttackingCharacter { get; set; }

    /// <summary>
    /// Gets or sets the character targeted by the attack.
    /// </summary>
    public Character TargetCharacter { get; set; }

    /// <summary>
    /// Gets or sets the amount of damage dealt by the attack.
    /// </summary>
    public int Damage { get; set; }

    /// <summary>
    /// Gets or sets the type of damage inflicted by the attack (e.g., Physical or Magic).
    /// </summary>
    public TypeDamage TypeOfDamage { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Attack"/> class.
    /// </summary>
    /// <param name="name">The name of the attack.</param>
    /// <param name="attackingCharacter">The character initiating the attack.</param>
    /// <param name="targetCharacter">The character targeted by the attack.</param>
    /// <param name="damage">The damage inflicted by the attack.</param>
    /// <param name="typeOfDamage">The type of damage inflicted (e.g., Physical, Magic).</param>
    public Attack(string name, Character attackingCharacter, Character targetCharacter, int damage, TypeDamage typeOfDamage)
    {
        Name = name;
        AttackingCharacter = attackingCharacter;
        TargetCharacter = targetCharacter;
        Damage = damage;
        TypeOfDamage = typeOfDamage;
    }
}
