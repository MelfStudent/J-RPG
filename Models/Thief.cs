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
    /// Prompts the user to choose an action for the Thief during their turn.
    /// Displays available skills, handles cooldowns, and allows the selection of a target.
    /// </summary>
    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: THIEF)");
        Console.WriteLine(ToString());

        var skillDetails = PrepareSkillDetails();
        Skill? selectedSkill = PromptSkillChoice(skillDetails);

        if (selectedSkill == null)
        {
            Console.WriteLine("You decided to skip the turn.");
            return;
        }

        Character? target = SelectTargetForSkill(selectedSkill);

        if (selectedSkill != null && target != null)
        {
            RegisterSkillUsage(selectedSkill, target);
        }
    }

    /// <summary>
    /// Prepares a list of skill details for display during the action selection phase.
    /// </summary>
    /// <returns>A list of strings representing each skill's details.</returns>
    private List<string> PrepareSkillDetails()
    {
        var skillDetails = Skills.Select(s => 
            $"{s.Name} - {s.Description}\n" +
            $"  Cooldown: {s.CurrentCooldown}/{s.Cooldown}\n" +
            $"  Mana Cost: {s.ManaCost}\n" +
            $"  Damage: {s.EffectPower}\n" +
            $"  Type: {s.TypeOfDamage}\n" +
            $"  Target: {s.Target}\n"
        ).ToList();

        skillDetails.Add("Skip the turn");
        return skillDetails;
    }

    /// <summary>
    /// Prompts the user to select a skill or skip the turn.
    /// </summary>
    /// <param name="skillDetails">A list of strings representing skill options.</param>
    /// <returns>The selected skill, or null if the user skips the turn.</returns>
    private Skill? PromptSkillChoice(List<string> skillDetails)
    {
        while (true)
        {
            try
            {
                var skillChoice = Utils.PromptChoice(skillDetails, "Enter a number corresponding to the desired action:");

                if (skillChoice == skillDetails.Count)
                {
                    return null; // Skip the turn
                }

                var skill = Skills[skillChoice - 1];
                if (skill.CurrentCooldown != 0)
                {
                    Console.WriteLine($"{skill.Name} skill is recharging, cannot be used. Please choose another action.");
                    continue;
                }

                return skill;
            }
            catch (Exception ex)
            {
                Utils.LogError($"An error occurred during skill selection: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// Selects a target for the given skill based on its target type.
    /// </summary>
    /// <param name="skill">The skill for which a target needs to be selected.</param>
    /// <returns>The selected target, or null if no valid target is chosen.</returns>
    private Character? SelectTargetForSkill(Skill skill)
    {
        try
        {
            if (skill.Target == TargetType.Enemy)
            {
                return Utils.PromptTarget("\nChoose a target:", Menu.TeamThatDefends!, this);
            }
            if (skill.Target == TargetType.AllAllies || skill.Target == TargetType.Self)
            {
                // For self-target or ally-target skills, return null (no explicit target needed).
                return null;
            }
        }
        catch (Exception ex)
        {
            Utils.LogError($"An error occurred during target selection: {ex.Message}");
        }

        return null;
    }

    /// <summary>
    /// Registers the usage of a skill by adding it to the current skill usage list.
    /// </summary>
    /// <param name="skill">The skill being used.</param>
    /// <param name="target">The target of the skill.</param>
    private void RegisterSkillUsage(Skill skill, Character? target)
    {
        Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target!));
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
