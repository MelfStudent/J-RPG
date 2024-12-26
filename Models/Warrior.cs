namespace J_RPG.Models;

using Services;
using Enums;

/// <summary>
/// Represents a Warrior character in the game. The Warrior is a tanky character with high physical attack power
/// and the ability to buff allies and deal damage to multiple enemies.
/// It inherits from the <see cref="Character"/> class and overrides some methods for specialized behavior.
/// </summary>
public class Warrior : Character
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Warrior"/> class with specified attributes.
    /// The Warrior starts with unique skills such as "Heroic Strike", "Battle Cry", and "Whirlwind".
    /// </summary>
    /// <param name="name">The name of the Warrior.</param>
    /// <param name="maxHitPoints">The maximum hit points of the Warrior.</param>
    /// <param name="physicalAttackPower">The physical attack power of the Warrior.</param>
    /// <param name="magicAttackPower">The magic attack power of the Warrior.</param>
    /// <param name="armor">The armor type of the Warrior.</param>
    /// <param name="dodgeChance">The dodge chance of the Warrior.</param>
    /// <param name="paradeChance">The parade chance of the Warrior.</param>
    /// <param name="chanceSpellResistance">The spell resistance chance of the Warrior.</param>
    /// <param name="speed">The speed of the Warrior.</param>
    public Warrior(string name, int maxHitPoints, int physicalAttackPower, int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed) : base(name, maxHitPoints, physicalAttackPower, magicAttackPower, armor, dodgeChance, paradeChance, chanceSpellResistance, speed)
    {
        Skills.Add(new Skill(
            "Heroic Strike",
            "Physical attack that deals 100% of physical attack power to a target",
            1,
            TargetType.Enemy,
            0,
            ActionType.Damage,
            physicalAttackPower,
            TypeDamage.Physical
        ));

        Skills.Add(new Skill(
            "Battle cry",
            "Increases the physical attack power of all characters on the team by 25",
            2,
            TargetType.AllAllies,
            0,
            ActionType.Buff,
            25,
            TypeDamage.Null,
            AffectedStat.PhysicalAttack
        ));

        Skills.Add(new Skill(
            "Whirlwind",
            "Physical attack that deals 33% of physical attack power to the entire enemy team",
            2,
            TargetType.AllEnemies,
            0,
            ActionType.Damage,
            (int)(physicalAttackPower * 0.33),
            TypeDamage.Physical
        ));
    }

    /// <summary>
    /// Handles the defense phase for the Warrior. If the Warrior parries or performs a luck-based counterattack,
    /// it executes a counterattack with enhanced or halved damage based on the situation.
    /// </summary>
    /// <param name="attacker">The character attacking the Warrior.</param>
    /// <param name="typeOfAttack">The type of attack being made (physical, magical, etc.).</param>
    /// <param name="attackPower">The power of the attack being made.</param>
    /// <returns>A <see cref="DefenseResult"/> containing the outcome of the defense.</returns>
    protected override DefenseResult Defend(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        var result = new DefenseResult();
        
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");
        
        try
        {
            // Base defense handling (from the Character class).
            var defenseResult = base.Defend(attacker, typeOfAttack, attackPower);

            // If the attack is physical and the Warrior successfully parries or gets lucky, a counterattack occurs.
            if (typeOfAttack == TypeDamage.Physical)
            {
                if (defenseResult.IsParried || PerformLuckTest(25))
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[{Name.ToUpper()}] successfully counterattacked!");
                    Console.ResetColor();
                    
                    // Counterattack damage calculation based on whether it was a parry or luck-based.
                    var counterAttackPower = defenseResult.IsParried
                        ? (int)(defenseResult.DamageTaken * 1.50) // 50% more damage if parried.
                        : defenseResult.DamageTaken / 2; // Halve the damage if luck-based.
                
                    var counterAttack = new Attack("Counterattack", this, attacker, counterAttackPower, TypeDamage.Physical );
                    Tackle(counterAttack); // Perform the counterattack using the Tackle method (from Character).
                }
            }
        }
        catch (Exception ex)
        {
            Utils.LogError($"An error occurred during the defense phase: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Prompts the user to choose an action for the Warrior during the player's turn.
    /// Displays available skills, handles cooldowns, and allows the selection of a target.
    /// </summary>
    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: WARRIOR)");
        Console.WriteLine(ToString());
        
        var skillDetails = Skills.Select(s => 
        $"{s.Name} - {s.Description}\n" +
        $"  Cooldown: {s.CurrentCooldown}/{s.Cooldown}\n" +
        $"  Mana Cost: {s.ManaCost}\n" +
        $"  Damage: {s.EffectPower}\n" +
        $"  Type: {s.TypeOfDamage}\n" +
        $"  Target: {s.Target}\n"
        ).ToList();
        skillDetails.Add("Skip the turn");

        Skill? skill = null;
        Character? target = null;

        while (true)
        {
            try
            {
                // Prompt the user to select a skill or skip the turn.
                var skillChoice = Utils.PromptChoice(skillDetails, "Enter a number corresponding to the desired action:");

                if (skillChoice == skillDetails.Count)
                {
                    Console.WriteLine("You decided to skip the turn.");
                    break;
                }
                
                skill = Skills[skillChoice - 1]; 
                
                // If the selected skill is on cooldown, ask the player to choose another action.
                if (skill.CurrentCooldown != 0)
                {
                    Console.WriteLine($"{skill.Name} skill is recharging, cannot be used. Please choose another action.");
                    continue;
                }
                
                // If the skill targets an enemy, prompt for target selection.
                if (skill.Target == TargetType.Enemy)
                {
                    target = Utils.PromptTarget("\nChoose a target:", Menu.TeamThatDefends!, this);
                }
                break;
            }
            catch (Exception ex)
            {
                Utils.LogError($"An error occurred during action selection: {ex.Message}");
            }
        }
        
        // Add the selected skill usage to the current turn.
        if (skill != null)
        {
            Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target!));
        }
    }
    
    /// <summary>
    /// Returns a string representation of the Warrior, displaying its current stats.
    /// </summary>
    /// <returns>A string representing the Warrior's current stats, including health, attack power, armor, and more.</returns>
    public override string ToString()
    {
        try
        {
            return $"HP: {CurrentHitPoints}/{MaxHitPoints} | " +
                   $"Physical Attack: {PhysicalAttackPower} | " +
                   $"Magic Attack: {MagicAttackPower} | " +
                   $"Armor: {Armor} | " +
                   $"Dodge: {DodgeChance}% | " +
                   $"Parade: {ParadeChance}% | " +
                   $"Spell Resistance: {ChanceSpellResistance}% | " +
                   $"Speed: {Speed}\n";
        }
        catch (Exception ex)
        {
            Utils.LogError($"An error occurred while generating the character summary: {ex.Message}");
            return string.Empty;
        }
    }
}
