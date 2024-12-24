namespace J_RPG.Models;

using Services;
using Enums;

public class Priest : Character
{
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
    
    protected override DefenseResult Defend(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        var result = new DefenseResult();
        
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");
        
        base.Defend(attacker, typeOfAttack, attackPower);

        return result;
    }
    
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
        
        Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill!, target!));
    }
    
    public override string ToString()
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
}