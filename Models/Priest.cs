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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occurred during the defense phase: {ex.Message}");
            Console.ResetColor();
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
                
                if (skill.CurrentCooldown != 0)
                {
                    Console.WriteLine($"{skill.Name} skill is recharging, cannot be used. Please choose another action.");
                    continue;
                }
                
                // Prompt for target selection based on the skill's target type.
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
        
        // If a valid skill was chosen, add it to the current skill usage.
        if (skill != null)
        {
            Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target!));
        }
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
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occurred while generating the character summary: {ex.Message}");
            Console.ResetColor();
            return string.Empty;
        }
    }
}
