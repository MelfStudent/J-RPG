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
            Utils.LogError($"An error occurred during the defense phase: {ex.Message}");
        }
        
        return result;
    }

    /// <summary>
    /// Allows the Priest to choose an action during their turn, including using skills or skipping the turn.
    /// </summary>
    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: PRIEST)");
        Console.WriteLine(ToString());

        var skillDetails = Skills.Select(s => FormatSkillDetails(s)).ToList();
        skillDetails.Add("Skip the turn");

        var skill = PromptSkillChoice(skillDetails);
        if (skill == null) return;

        Character? target = skill.Target == TargetType.Enemy ? PromptTargetSelection() : null;
        Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target!));
    }

   /// <summary>
    /// Formats the details of a skill for display in the action selection menu.
    /// </summary>
    /// <param name="skill">The skill to format.</param>
    /// <returns>A string containing formatted details of the skill, including cooldown, mana cost, damage, type, and target.</returns>
    private static string FormatSkillDetails(Skill skill)
    {
        return $"{skill.Name} - {skill.Description}\n" +
               $"  Cooldown: {skill.CurrentCooldown}/{skill.Cooldown}\n" +
               $"  Mana Cost: {skill.ManaCost}\n" +
               $"  Damage: {skill.EffectPower}\n" +
               $"  Type: {skill.TypeOfDamage}\n" +
               $"  Target: {skill.Target}\n";
    }

    /// <summary>
    /// Prompts the player to choose a skill from the available options.
    /// </summary>
    /// <param name="skillDetails">A list of formatted skill descriptions to display.</param>
    /// <returns>The chosen <see cref="Skill"/> object, or null if the player decides to skip the turn.</returns>
    /// <exception cref="Exception">Handles errors during the skill selection process.</exception>
    private Skill? PromptSkillChoice(List<string> skillDetails)
    {
        while (true)
        {
            try
            {
                var choice = Utils.PromptChoice(skillDetails, "Enter a number corresponding to the desired action:");
                if (choice == skillDetails.Count)
                {
                    Console.WriteLine("You decided to skip the turn.");
                    return null;
                }

                var skill = Skills[choice - 1];
                if (skill.CurrentCooldown != 0)
                {
                    Console.WriteLine($"{skill.Name} skill is recharging, cannot be used. Please choose another action.");
                    continue;
                }

                return skill;
            }
            catch (Exception ex)
            {
                Utils.LogError($"An error occurred during action selection: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Prompts the player to select a target for their action.
    /// </summary>
    /// <returns>The selected <see cref="Character"/> as the target, or null if no target is selected.</returns>
    /// <exception cref="Exception">Handles errors during target selection.</exception>
    private Character? PromptTargetSelection()
    {
        return Utils.PromptTarget("\nChoose a target:", Menu.TeamThatDefends!, this);
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
            Utils.LogError($"An error occurred while generating the character summary: {ex.Message}");
            return string.Empty;
        }
    }
}
