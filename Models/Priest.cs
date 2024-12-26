namespace J_RPG.Models;

using Services;
using Enums;

/// <summary>
/// Represents a Priest character in the game.
/// The Priest is a magic-oriented class that specializes in healing and supporting allies.
/// </summary>
public class Priest : Character
{
    /// <summary>
    /// Constructs a new Priest character with the given parameters.
    /// </summary>
    /// <param name="name">The name of the Priest character.</param>
    /// <param name="maxHitPoints">The maximum hit points of the Priest.</param>
    /// <param name="physicalAttackPower">The physical attack power of the Priest.</param>
    /// <param name="magicAttackPower">The magic attack power of the Priest.</param>
    /// <param name="armor">The armor type the Priest wears.</param>
    /// <param name="dodgeChance">The chance to dodge an attack.</param>
    /// <param name="paradeChance">The chance to parry an attack.</param>
    /// <param name="chanceSpellResistance">The chance the Priest has to resist magic spells.</param>
    /// <param name="speed">The speed of the Priest.</param>
    /// <param name="usesMana">Indicates whether the Priest uses mana.</param>
    /// <param name="maxMana">The maximum mana the Priest can have.</param>
    public Priest(string name, int maxHitPoints, int physicalAttackPower, int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed, bool usesMana, int maxMana) : base(name, maxHitPoints, physicalAttackPower, magicAttackPower, armor, dodgeChance, paradeChance, chanceSpellResistance, speed, usesMana, maxMana)
    {
        Skills.Add(new Skill(
            "Punishment",
            "Magic attack that deals 75% of magic attack power to the target",
            1,
            TargetType.Enemy,
            15,
            ActionType.Damage,
            (int)(magicAttackPower * 0.75),
            TypeDamage.Magic
        ));
        
        Skills.Add(new Skill(
            "Circle of care",
            "Heals the entire team for 75% of magic attack power",
            2,
            TargetType.AllAllies,
            30,
            ActionType.Heal,
            (int)(magicAttackPower * 0.75)
        ));
    }
    
    /// <summary>
    /// Defends the Priest against an attack. Since the Priest is more fragile, this method handles basic defense without special mechanics.
    /// </summary>
    /// <param name="attacker">The character attacking the Priest.</param>
    /// <param name="typeOfAttack">The type of attack (Physical, Magic).</param>
    /// <param name="attackPower">The power of the attack.</param>
    /// <returns>A DefenseResult object indicating the outcome of the defense attempt.</returns>
    protected override DefenseResult Defend(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        var result = new DefenseResult();
        
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");
        
        try
        {
            // Base defense handling (from the Character class).
            base.Defend(attacker, typeOfAttack, attackPower);
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
    /// Returns a string representation of the Priest character, including their stats.
    /// </summary>
    /// <returns>A string containing the Priest's health, attack power, armor, and other stats.</returns>
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
                   $"Speed: {Speed} | " +
                   $"Mana: {CurrentMana}/{MaxMana}\n";
        }
        catch (Exception ex)
        {
            Utils.LogError($"An error occurred while generating the character summary: {ex.Message}");
            return string.Empty;
        }
    }
}
