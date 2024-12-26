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
    /// Defends the Mage against an attack. Handles the use of magical defense abilities like Spell Return and Frost Barrier.
    /// </summary>
    /// <param name="attacker">The character attacking the Mage.</param>
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
            // Handling the "Spell Return" defense mechanism.
            if (_isSpellBeingReturned && typeOfAttack == TypeDamage.Magic)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
                Console.WriteLine($"{Name} returns the magic attack to {attacker.Name} !");
                Console.ResetColor();

                var damageAttack = new Attack("Spell Return", this, attacker, attackPower, typeOfAttack);
                Tackle(damageAttack);

                _isSpellBeingReturned = false;
                return result;
            } 
            
            // Handling damage reduction from Frost Barrier.
            if (RemainingDamageReductions > 0)
            {
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
            }
            
            // Default defense handling (from the Character class).
            base.Defend(attacker, typeOfAttack, attackPower);
        }
        catch (Exception ex)
        {
            Utils.LogError($"An error occurred during the defense phase: {ex.Message}");
        }

        return result;
    }

    /// <summary>
    /// Allows the Mage to choose an action during their turn, including using skills or skipping the turn.
    /// </summary>
    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: MAGE)");
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
                
                // Prompt the user to select a target if the skill targets an enemy.
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
        
        // If a valid skill was chosen, add it to the current skill usage.
        if (skill != null)
        {
            Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target!));
        }
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
