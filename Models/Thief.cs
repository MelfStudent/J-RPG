namespace J_RPG.Models;

using Services;
using Enums;

/// <summary>
/// Represents a Thief character in the game. The Thief is a special type of character with unique skills such as "Low Blow" and "Escape".
/// It inherits from the <see cref="Character"/> class and overrides some methods for specialized behavior.
/// </summary>
public class Thief : Character
{
    /// <summary>
    /// Initializes a new instance of the <see cref="Thief"/> class with specified attributes.
    /// The Thief starts with unique skills such as "Low Blow" and "Escape".
    /// </summary>
    /// <param name="name">The name of the Thief.</param>
    /// <param name="maxHitPoints">The maximum hit points of the Thief.</param>
    /// <param name="physicalAttackPower">The physical attack power of the Thief.</param>
    /// <param name="magicAttackPower">The magic attack power of the Thief.</param>
    /// <param name="armor">The armor type of the Thief.</param>
    /// <param name="dodgeChance">The dodge chance of the Thief.</param>
    /// <param name="paradeChance">The parade chance of the Thief.</param>
    /// <param name="chanceSpellResistance">The spell resistance chance of the Thief.</param>
    /// <param name="speed">The speed of the Thief.</param>
    public Thief(string name, int maxHitPoints, int physicalAttackPower, int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed) : base(name, maxHitPoints, physicalAttackPower, magicAttackPower, armor, dodgeChance, paradeChance, chanceSpellResistance, speed)
    {
        Skills.Add(new Skill(
            "Low blow",
            "Physical attack that deals 100% of physical attack power to the target, or 150% if the target has less than half of his life points",
            1,
            TargetType.Enemy,
            0,
            ActionType.Damage,
            physicalAttackPower,
            TypeDamage.Physical
        ));
        
        Skills.Add(new Skill(
            "Escape",
            "Increases the thief's chance to dodge and resist spells by 20%",
            1,
            TargetType.Self,
            0,
            ActionType.Buff,
            20,
            TypeDamage.Physical
        ));
    }
    
    /// <summary>
    /// Handles the defense phase for the Thief. If the Thief dodges an attack, it performs a counterattack.
    /// </summary>
    /// <param name="attacker">The character attacking the Thief.</param>
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
            // Call the base class defense logic
            var defendResult = base.Defend(attacker, typeOfAttack, attackPower);

            // If the Thief dodges the attack, it counterattacks
            if (defendResult.IsDodged)
            {
                var attack = new Attack("Stab in the back", this, attacker, 15, TypeDamage.Physical);
                Tackle(attack);   // Perform the counterattack using the Tackle method (from Character).
            }
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occurred during the defense phase: {ex.Message}");
            Console.ResetColor();
        }
        
        return result;
    }

    /// <summary>
    /// Prompts the user to choose an action for the Thief during the player's turn.
    /// Displays available skills, handles cooldowns, and allows the selection of a target.
    /// </summary>
    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: THIEF)");
        Console.WriteLine(ToString());
        
        // Displays the Thief's skills and their descriptions
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
                // Prompt for the user's action choice
                var skillChoice = Utils.PromptChoice(skillDetails, "Enter a number corresponding to the desired action:");

                if (skillChoice == skillDetails.Count)
                {
                    Console.WriteLine("You decided to skip the turn.");
                    break;
                }
                
                skill = Skills[skillChoice - 1]; 
                
                // Check if the skill is ready to be used (i.e., not on cooldown)
                if (skill.CurrentCooldown != 0)
                {
                    Console.WriteLine($"{skill.Name} skill is recharging, cannot be used. Please choose another action.");
                    continue;
                }
                
                // Select the target for enemy-targeting skills
                if (skill.Target == TargetType.Enemy)
                {
                    target = Utils.PromptTarget("\nChoose a target:", Menu.TeamThatDefends!, this);
                }
                break;
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"An error occurred during action selection: {ex.Message}");
                Console.ResetColor();
            }
        }
        
        // Add the skill usage to the current skills tour
        if (skill != null)
        {
            Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target!));
        }
    }
    
    /// <summary>
    /// Returns a string representation of the Thief, displaying its current stats.
    /// </summary>
    /// <returns>A string representing the Thief's current stats, including health, attack power, armor, and more.</returns>
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occurred while generating the character summary: {ex.Message}");
            Console.ResetColor();
            return string.Empty;
        }
    }
}
