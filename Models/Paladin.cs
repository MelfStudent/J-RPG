namespace J_RPG.Models;

using Services;
using Enums;

/// <summary>
/// Represents a Paladin character in the game.
/// The Paladin is a tanky hybrid class that combines physical attacks with healing abilities.
/// </summary>
public class Paladin : Character
{
    /// <summary>
    /// Constructs a new Paladin character with the given parameters.
    /// </summary>
    /// <param name="name">The name of the Paladin character.</param>
    /// <param name="maxHitPoints">The maximum hit points of the Paladin.</param>
    /// <param name="physicalAttackPower">The physical attack power of the Paladin.</param>
    /// <param name="magicAttackPower">The magic attack power of the Paladin.</param>
    /// <param name="armor">The armor type the Paladin wears.</param>
    /// <param name="dodgeChance">The chance to dodge an attack.</param>
    /// <param name="paradeChance">The chance to parry an attack.</param>
    /// <param name="chanceSpellResistance">The chance the Paladin has to resist magic spells.</param>
    /// <param name="speed">The speed of the Paladin.</param>
    /// <param name="usesMana">Indicates whether the Paladin uses mana.</param>
    /// <param name="maxMana">The maximum mana the Paladin can have.</param>
    public Paladin(string name, int maxHitPoints, int physicalAttackPower, int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed, bool usesMana, int maxMana) : base(name, maxHitPoints, physicalAttackPower, magicAttackPower, armor, dodgeChance, paradeChance, chanceSpellResistance, speed, usesMana, maxMana)
    { 
        Skills.Add(new Skill(
            "Crusader Strike",
            "Physical attack that deals 100% of physical attack power to the target",
            1,
            TargetType.Enemy,
            5,
            ActionType.Damage,
            physicalAttackPower,
            TypeDamage.Physical
        ));
        
        Skills.Add(new Skill(
            "Judgement",
            "Magic attack that deals 100% of magic attack power to the target",
            1,
            TargetType.Enemy,
            10,
            ActionType.Damage,
            magicAttackPower,
            TypeDamage.Magic
        ));
        
        Skills.Add(new Skill(
            "Bright flash",
            "Heals the target for 125% of Magic Attack Power",
            1,
            TargetType.Ally,
            25,
            ActionType.Heal,
            (int)(magicAttackPower * 1.25),
            TypeDamage.Magic
        ));
    }
    
    /// <summary>
    /// Defends the Paladin against an attack. Since the Paladin is a defensive class, this method handles basic defense without special mechanics.
    /// </summary>
    /// <param name="attacker">The character attacking the Paladin.</param>
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
    /// Allows the Paladin to choose an action during their turn, including using skills or skipping the turn.
    /// </summary>
    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: PALADIN)");
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
                
                if (skill.Target == TargetType.Ally)
                {
                    target = Utils.PromptTarget("\nChoose a target:", Menu.TeamThatAttacks!, this);
                }
                break;
            }
            catch (Exception ex)
            {
                Utils.LogError($"An error occurred during action selection: {ex.Message}");
            }
        }

        // If a valid skill was chosen, add it to the current skill usage.
        if (skill != null)
        {
            Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target!));
        }
    }
    
    /// <summary>
    /// Returns a string representation of the Paladin character, including their stats.
    /// </summary>
    /// <returns>A string containing the Paladin's health, attack power, armor, and other stats.</returns>
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
