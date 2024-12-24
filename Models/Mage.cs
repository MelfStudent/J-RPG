namespace J_RPG.Models;

using Services;
using Enums;

public class Mage : Character
{
    public int RemainingDamageReductions { get; set; }
    private bool _isSpellBeingReturned;
    
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
    
    protected override DefenseResult Defend(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        var result = new DefenseResult();
        
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");

        try
        {
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
        
        if (skill != null)
        {
            Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target!));
        }
    }
    
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
