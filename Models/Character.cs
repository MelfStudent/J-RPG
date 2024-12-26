namespace J_RPG.Models;

using Services;
using Enums;

/// <summary>
/// Abstract class representing a character in the game.
/// Manages health points, attack power, defense, skills, and mana.
/// </summary>
public abstract class Character
{
    /// <summary>
    /// Name of the character.
    /// </summary>
    public string Name { get; protected set; }

    /// <summary>
    /// Current hit points of the character.
    /// </summary>
    public int CurrentHitPoints { get; private set; }

    /// <summary>
    /// Maximum hit points of the character.
    /// </summary>
    public int MaxHitPoints { get; private set; }

    /// <summary>
    /// Physical attack power of the character.
    /// </summary>
    public int PhysicalAttackPower { get; set; }

    /// <summary>
    /// Magic attack power of the character.
    /// </summary>
    public int MagicAttackPower { get; set; }

    /// <summary>
    /// Type of armor equipped by the character.
    /// </summary>
    protected TypeOfArmor Armor { get; private set; }

    /// <summary>
    /// Chance to dodge an attack.
    /// </summary>
    public int DodgeChance { get; set; }

    /// <summary>
    /// Chance to parry an attack.
    /// </summary>
    protected int ParadeChance { get; private set; }

    /// <summary>
    /// Chance to resist a magical attack.
    /// </summary>
    public int ChanceSpellResistance { get; set; }

    /// <summary>
    /// Speed of the character, used to determine action order.
    /// </summary>
    public int Speed { get; private set; }

    /// <summary>
    /// Indicates whether the character is dead.
    /// </summary>
    public bool IsDead { get; private set; }

    /// <summary>
    /// List of skills the character has.
    /// </summary>
    protected List<Skill> Skills { get; set; } = new List<Skill>();

    /// <summary>
    /// Indicates if the character uses mana.
    /// </summary>
    public bool UsesMana { get; private set; }

    /// <summary>
    /// Current mana of the character.
    /// </summary>
    public int CurrentMana { get; set; }

    /// <summary>
    /// Maximum mana of the character.
    /// </summary>
    public int MaxMana { get; private set; }

    /// <summary>
    /// Random instance used for luck tests.
    /// </summary>
    private Random _rand { get; set; } = new Random();

    /// <summary>
    /// Constructor to initialize a character.
    /// </summary>
    /// <param name="name">The character's name.</param>
    /// <param name="maxHitPoints">The character's maximum hit points.</param>
    /// <param name="physicalAttackPower">The character's physical attack power.</param>
    /// <param name="magicAttackPower">The character's magic attack power.</param>
    /// <param name="armor">The type of armor equipped by the character.</param>
    /// <param name="dodgeChance">Chance to dodge an attack.</param>
    /// <param name="paradeChance">Chance to parry an attack.</param>
    /// <param name="chanceSpellResistance">Chance to resist a magical attack.</param>
    /// <param name="speed">The character's speed.</param>
    /// <param name="usesMana">Indicates whether the character uses mana.</param>
    /// <param name="maxMana">The maximum mana for the character, if applicable.</param>
    protected Character(string name, int maxHitPoints, int physicalAttackPower,
                        int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed, bool usesMana = false, int maxMana = 0)
    {
        Name = name;
        CurrentHitPoints = maxHitPoints;
        MaxHitPoints = maxHitPoints;
        PhysicalAttackPower = physicalAttackPower;
        MagicAttackPower = magicAttackPower;
        Armor = armor;
        DodgeChance = dodgeChance;
        ParadeChance = paradeChance;
        ChanceSpellResistance = chanceSpellResistance;
        Speed = speed;
        IsDead = false;
        UsesMana = usesMana;
        if (UsesMana)
        {
            CurrentMana = maxMana;
            MaxMana = maxMana;
            Skills.Add(new Skill(
                "Drink",
                "Regenerates half mana",
                1,
                TargetType.Self,
                0,
                ActionType.Buff,
                MaxMana / 2
            ));
        }
    }

    /// <summary>
    /// Executes an attack from one character to another.
    /// </summary>
    /// <param name="attack">The attack to execute.</param>
    /// <exception cref="ArgumentNullException">Thrown if the attack or any character is null.</exception>
    /// <exception cref="InvalidOperationException">Thrown if either the attacking or target character is not specified.</exception>
    public static void Tackle(Attack attack)
    {
        if (attack == null)
            throw new ArgumentNullException(nameof(attack), "Attack cannot be null.");
        if (attack.AttackingCharacter == null || attack.TargetCharacter == null)
            throw new InvalidOperationException("Both attacking and target characters must be specified.");
        
        try
        {
            Console.WriteLine("\n========== ATTACK PHASE ==========");
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Attack Name: {attack.Name}");
            Console.WriteLine($"[{attack.AttackingCharacter.Name.ToUpper()}] attacks [{attack.TargetCharacter.Name.ToUpper()}]");
            Console.WriteLine($"Attack Type: {attack.TypeOfDamage}");
            Console.WriteLine($"Damage: {attack.Damage}");
            Console.ResetColor();
            attack.TargetCharacter.Defend(attack.AttackingCharacter ,attack.TypeOfDamage, attack.Damage);
            Console.WriteLine("===================================\n");
        }
        catch (Exception ex)
        {
            Utils.LogError($"An error occurred during the attack: {ex.Message}");
        }
    }

    /// <summary>
    /// Defends the character against an attack.
    /// </summary>
    /// <param name="attacker">The character attacking.</param>
    /// <param name="typeOfAttack">The type of the attack (physical or magical).</param>
    /// <param name="attackPower">The power of the attack.</param>
    /// <returns>A <see cref="DefenseResult"/> object representing the defense result.</returns>
    /// <exception cref="ArgumentNullException">Thrown if the attacker is null.</exception>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the attack power is negative.</exception>
    protected virtual DefenseResult Defend(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        ValidateDefenseInputs(attacker, attackPower);

        var result = new DefenseResult();
        try
        {
            if (HandleEvasionAndParry(typeOfAttack, attackPower, ref result))
                return result;

            var damage = CalculateDamageAfterResistance(attacker, typeOfAttack, attackPower, ref result);

            ApplyDamageAndCheckDeath(damage, result);
        }
        catch (Exception ex)
        {
            Utils.LogError($"Error during defense: {ex.Message}");
        }

        return result;
    }

    private void ValidateDefenseInputs(Character attacker, int attackPower)
    {
        if (attacker == null)
            throw new ArgumentNullException(nameof(attacker), "Attacker cannot be null.");
        if (attackPower < 0)
            throw new ArgumentOutOfRangeException(nameof(attackPower), "Attack power must be non-negative.");
    }

    private bool HandleEvasionAndParry(TypeDamage typeOfAttack, int attackPower, ref DefenseResult result)
    {
        if (typeOfAttack == TypeDamage.Physical)
        {
            if (PerformLuckTest(DodgeChance))
            {
                result.IsDodged = true;
                Console.WriteLine($"{Name} dodged the attack!");
                return true;
            }
            if (PerformLuckTest(ParadeChance))
            {
                result.IsParried = true;
                result.DamageTaken = attackPower / 2;
                Console.WriteLine($"{Name} parried the attack and reduced damage to {result.DamageTaken}!");
                return true;
            }
        }
        else if (typeOfAttack == TypeDamage.Magic && PerformLuckTest(ChanceSpellResistance))
        {
            Console.WriteLine($"{Name} resisted the magic attack!");
            return true;
        }
        return false;
    }

    private int CalculateDamageAfterResistance(Character attacker, TypeDamage typeOfAttack, int attackPower, ref DefenseResult result)
    {
        var damage = attackPower;

        // Reduce speed for magic attacks
        if (typeOfAttack == TypeDamage.Magic)
            Speed = (int)(Speed * 0.85);

        damage = GetArmorResistance(Armor, typeOfAttack, damage);
        result.DamageTaken = damage;

        // Special case for Paladin to restore health
        if (attacker is Paladin paladin)
            paladin.RestoreHealth(damage / 2);

        return damage;
    }

    private void ApplyDamageAndCheckDeath(int damage, DefenseResult result)
    {
        CurrentHitPoints -= damage;

        if (CurrentHitPoints <= 0)
        {
            CurrentHitPoints = 0;
            IsDead = true;
            Console.WriteLine($"{Name} has died.");
            return;
        }

        Console.WriteLine($"The {Name} character received {damage} damage. Remaining HP: {CurrentHitPoints}");
    }

    /// <summary>
    /// Restores health points to the character.
    /// </summary>
    /// <param name="extraLife">The amount of health to restore.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if extra life is negative.</exception>
    public void RestoreHealth(int extraLife)
    {
        if (extraLife < 0)
            throw new ArgumentOutOfRangeException(nameof(extraLife), "Extra life must be non-negative.");

        if (CurrentHitPoints + extraLife <= MaxHitPoints)
        {
            CurrentHitPoints += extraLife;
            Console.WriteLine(
                $"{Name} regenerated {extraLife} hp. It now has {CurrentHitPoints} hp");
            return;
        }
        CurrentHitPoints = MaxHitPoints;
        Console.WriteLine($"{Name} has regenerated life. It now has {CurrentHitPoints} hp");
    }

    /// <summary>
    /// Performs a luck test to determine the success based on a given probability.
    /// </summary>
    /// <param name="successProbabilityPercentage">The probability of success in percentage.</param>
    /// <returns>True if the luck test is successful, otherwise false.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the probability is not between 0 and 100.</exception>

    protected bool PerformLuckTest(int successProbabilityPercentage)
    {
        if (successProbabilityPercentage < 0 || successProbabilityPercentage > 100)
            throw new ArgumentOutOfRangeException(nameof(successProbabilityPercentage), "Probability percentage must be between 0 and 100.");
        
        var targetNumber = _rand.Next(1, 100);
        var shuffledNumbers = new int[100];
        for (var i = 1; i < shuffledNumbers.Length; i++)
        {
            shuffledNumbers[i-1] = i;
        }

        shuffledNumbers = Shuffle(shuffledNumbers);
            
        for (var j = 0; j < successProbabilityPercentage; j++)
        {
            if (shuffledNumbers[j] == targetNumber)
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Shuffles an array of integers.
    /// </summary>
    /// <param name="values">Array of values to shuffle.</param>
    /// <returns>A shuffled array of integers.</returns>
    private int[] Shuffle(int[] values)
    {
        for (var i = values.Length - 1; i > 0; i--) {
            var k = _rand.Next(i + 1);
            (values[k], values[i]) = (values[i], values[k]);
        }

        return values;
    }

    /// <summary>
    /// Calculates the damage reduction based on the character's armor and the attack type.
    /// </summary>
    /// <param name="armor">The armor of the character.</param>
    /// <param name="typeOfAttack">The type of attack (physical or magical).</param>
    /// <param name="damageReceived">The damage received before armor reduction.</param>
    /// <returns>The damage after applying armor resistance.</returns>
    /// <exception cref="ArgumentOutOfRangeException">Thrown if the damage received is negative.</exception>
    private static int GetArmorResistance(TypeOfArmor armor, TypeDamage typeOfAttack ,int damageReceived)
    {
        if (damageReceived < 0)
            throw new ArgumentOutOfRangeException(nameof(damageReceived), "Damage received must be non-negative.");

        var reductionFactor = typeOfAttack switch
        {
            TypeDamage.Physical => armor switch
            {
                TypeOfArmor.Leather => 0.85,
                TypeOfArmor.Mesh => 0.70,
                TypeOfArmor.Plates => 0.55,
                _ => 1.0
            },
            TypeDamage.Magic => armor switch
            {
                TypeOfArmor.Fabric => 0.70,
                TypeOfArmor.Leather => .80,
                TypeOfArmor.Mesh => 0.90,
                _ => 1.0
            },
            _ => 1.0
        };
            
        return (int)(damageReceived * reductionFactor);
    }

    /// <summary>
    /// Abstract method for selecting an action for the character.
    /// </summary>
    /// <returns>The selected action.</returns>
    public abstract void ChoiceAction();

    /// <summary>
    /// Returns a string representation of the character, including key attributes such as name, health points, attack power, and armor.
    /// </summary>
    /// <returns>A formatted string with the character's details.</returns>
    public override string ToString()
    {
        try
        {
            var result =
                "----------------------------------------\n" +
                $"Name: {Name}\n" +
                $"Class: {GetType().Name}\n" +
                $"HP: {CurrentHitPoints}/{MaxHitPoints}\n" +
                $"Physical Attack: {PhysicalAttackPower}\n" +
                $"Magical Attack: {MagicAttackPower}\n" +
                $"Dodge Chance: {DodgeChance}%\n" +
                $"Parade Chance: {ParadeChance}%\n" +
                $"Spell Resistance Chance: {ChanceSpellResistance}%\n" +
                $"Speed : {Speed}\n" +
                $"Armor Type: {Armor} (Resistance: {Menu.GetArmorPercentage(Armor)})\n";
            result += "----------------------------------------";

            return result;
        }
        catch (Exception ex)
        {
            return $"Error in ToString: {ex.Message}";
        }
    }
    
    /// <summary>
    /// Reduces the cooldown of all skills the character possesses.
    /// This method iterates through each skill and attempts to reduce its cooldown.
    /// </summary>
    public void ReduceCooldowns()
    {
        foreach (var skill in Skills)
        {
            try
            {
                skill.ReduceCooldown();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reducing cooldown for skill {skill.Name}: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Uses a specified amount of mana for a skill. 
    /// Throws an exception if the character does not have enough mana.
    /// </summary>
    /// <param name="skillCost">The amount of mana required to use the skill.</param>
    /// <exception cref="ArgumentOutOfRangeException">Thrown when the skill cost is negative.</exception>
    /// <exception cref="InvalidOperationException">Thrown when there is not enough mana to use the skill.</exception>
    public void UseMana(int skillCost)
    {
        if (skillCost < 0)
            throw new ArgumentOutOfRangeException(nameof(skillCost), "Skill cost must be non-negative.");
        if (skillCost > CurrentMana)
            throw new InvalidOperationException("Not enough mana to use the skill.");

        CurrentMana -= skillCost;
        Console.WriteLine($"{Name} uses {skillCost} mana points. Remaining Mana: {CurrentMana}/{MaxMana}");
    }
}
