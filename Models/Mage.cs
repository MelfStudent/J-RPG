namespace J_RPG.Models;

using Services;
using Enums;

/// <summary>
/// Represents a Mage character in the game.
/// The Mage is a ranged spellcaster who uses magic attacks and has abilities for both offense and defense.
/// </summary>
public class Mage : Character
{
    /// <summary>
    /// Number of remaining damage reductions from the Frost Barrier skill.
    /// </summary>
    public int RemainingDamageReductions { get; set; }

    /// <summary>
    /// Indicates whether the Mage is currently returning a spell with the Spell Return ability.
    /// </summary>
    private bool _isSpellBeingReturned;

    /// <summary>
    /// Constructs a new Mage character with the given parameters.
    /// </summary>
    /// <param name="name">The name of the Mage character.</param>
    /// <param name="maxHitPoints">The maximum hit points of the Mage.</param>
    /// <param name="physicalAttackPower">The physical attack power of the Mage.</param>
    /// <param name="magicAttackPower">The magic attack power of the Mage.</param>
    /// <param name="armor">The armor type the Mage wears.</param>
    /// <param name="dodgeChance">The chance to dodge an attack.</param>
    /// <param name="paradeChance">The chance to parry an attack.</param>
    /// <param name="chanceSpellResistance">The chance the Mage has to resist magic spells.</param>
    /// <param name="speed">The speed of the Mage.</param>
    /// <param name="usesMana">Indicates whether the Mage uses mana.</param>
    /// <param name="maxMana">The maximum mana the Mage can have.</param>
    public Mage(string name, int maxHitPoints, int physicalAttackPower, int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed, bool usesMana, int maxMana) : base(name, maxHitPoints, physicalAttackPower, magicAttackPower, armor, dodgeChance, paradeChance, chanceSpellResistance, speed, usesMana, maxMana)
    {
        RemainingDamageReductions = 0;

        Skills.Add(new Skill(
            "Frost bolt",
            "Magic attack that deals 100% of magic attack power to the target",
            1,
            TargetType.Enemy,
            15,
            ActionType.Damage,
            magicAttackPower,
            TypeDamage.Magic
        ));
        
        Skills.Add(new Skill(
            "Frost Barrier",
            "Reduces damage from the next two attacks received",
            2,
            TargetType.Self,
            25,
            ActionType.Buff,
            0
        ));
        
        Skills.Add(new Skill(
            "Blizzard",
            "Magic attack that deals 50% of magic attack power to the entire enemy team",
            2,
            TargetType.AllEnemies,
            25,
            ActionType.Damage,
            magicAttackPower / 2,
            TypeDamage.Magic
        ));
        
        Skills.Add(new Skill(
            "Spell Return",
            "Returns the next magical attack suffered to the attacker",
            1,
            TargetType.Self,
            25,
            ActionType.Buff,
            0
        ));
        
        Skills.Add(new Skill(
            "Mana Burn",
            "Halves the target's mana amount",
            3,
            TargetType.Enemy,
            20,
            ActionType.Debuff,
            40
        ));
    }
    
    /// <summary>
    /// Handles the Mage's defense against an attack.
    /// </summary>
    /// <param name="attacker">The attacking character.</param>
    /// <param name="typeOfAttack">The type of the attack (physical or magical).</param>
    /// <param name="attackPower">The power of the attack.</param>
    /// <returns>A <see cref="DefenseResult"/> object representing the outcome of the defense.</returns>
    /// <exception cref="Exception">Handles any unexpected errors during the defense process.</exception>
    protected override DefenseResult Defend(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        var result = new DefenseResult();
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");

        try
        {
            if (HandleSpellReturn(attacker, typeOfAttack, attackPower)) return result;
            attackPower = ApplyFrostBarrier(attackPower, typeOfAttack);
            base.Defend(attacker, typeOfAttack, attackPower);
        }
        catch (Exception ex)
        {
            Utils.LogError($"An error occurred during the defense phase: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Handles the Spell Return ability, reflecting the magic attack back to the attacker.
    /// </summary>
    /// <param name="attacker">The character who initiated the attack.</param>
    /// <param name="typeOfAttack">The type of attack (must be magical for this ability).</param>
    /// <param name="attackPower">The power of the attack.</param>
    /// <returns>True if the Spell Return ability was triggered and handled; otherwise, false.</returns>
    private bool HandleSpellReturn(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        if (!_isSpellBeingReturned || typeOfAttack != TypeDamage.Magic) return false;

        Console.ForegroundColor = ConsoleColor.Magenta;
        Console.WriteLine($"{Name} returns the magic attack to {attacker.Name}!");
        Console.ResetColor();

        var damageAttack = new Attack("Spell Return", this, attacker, attackPower, typeOfAttack);
        Tackle(damageAttack);

        _isSpellBeingReturned = false;
        return true;
    }

    /// <summary>
    /// Applies the Frost Barrier effect, reducing the damage from the next few attacks.
    /// </summary>
    /// <param name="attackPower">The original power of the attack.</param>
    /// <param name="typeOfAttack">The type of attack (physical or magical).</param>
    /// <returns>The reduced attack power after applying the Frost Barrier effect.</returns>
    private int ApplyFrostBarrier(int attackPower, TypeDamage typeOfAttack)
    {
        if (RemainingDamageReductions <= 0) return attackPower;

        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine($"{Name} is protected by FROST BARRIER!");
        Console.ResetColor();

        attackPower = typeOfAttack switch
        {
            TypeDamage.Physical => (int)(attackPower * 0.40),
            TypeDamage.Magic => (int)(attackPower * 0.50),
            _ => attackPower
        };

        RemainingDamageReductions--;
        return attackPower;
    }

    /// <summary>
    /// Allows the Mage to choose an action during their turn, including using skills or skipping the turn.
    /// </summary>
    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: MAGE)");
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
    /// Returns a string representation of the Mage character, including their stats.
    /// </summary>
    /// <returns>A string containing the Mage's health, attack power, armor, and other stats.</returns>
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
