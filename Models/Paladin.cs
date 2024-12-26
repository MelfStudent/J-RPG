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

        var skillDetails = Skills.Select(s => FormatSkillDetails(s)).ToList();
        skillDetails.Add("Skip the turn");

        var skill = PromptSkillChoice(skillDetails);
        if (skill == null) return;

        Character? target = skill.Target switch
        {
            TargetType.Enemy => PromptTargetSelection(Menu.TeamThatDefends!),
            TargetType.Ally => PromptTargetSelection(Menu.TeamThatAttacks!),
            _ => null
        };

        Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target!));
    }

    /// <summary>
    /// Formats the details of a skill for display.
    /// </summary>
    /// <param name="skill">The skill to format.</param>
    /// <returns>A formatted string with the skill details.</returns>
    private string FormatSkillDetails(Skill skill)
    {
        return $"{skill.Name} - {skill.Description}\n" +
               $"  Cooldown: {skill.CurrentCooldown}/{skill.Cooldown}\n" +
               $"  Mana Cost: {skill.ManaCost}\n" +
               $"  Damage: {skill.EffectPower}\n" +
               $"  Type: {skill.TypeOfDamage}\n" +
               $"  Target: {skill.Target}\n";
    }

    /// <summary>
    /// Prompts the player to choose a skill or skip the turn.
    /// </summary>
    /// <param name="skillDetails">The list of skill details to display.</param>
    /// <returns>The chosen skill, or null if the turn is skipped.</returns>
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
    /// Prompts the player to select a target from the given team.
    /// </summary>
    /// <param name="team">The team to select a target from.</param>
    /// <returns>The chosen target.</returns>
    private Character? PromptTargetSelection(Team team)
    {
        return Utils.PromptTarget("\nChoose a target:", team, this);
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
