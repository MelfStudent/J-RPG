namespace J_RPG.Models;

using Services;
using Enums;

public class Warrior : Character
{
    public Warrior(string name, int maxHitPoints, int physicalAttackPower, int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed) : base(name, maxHitPoints, physicalAttackPower, magicAttackPower, armor, dodgeChance, paradeChance, chanceSpellResistance, speed)
    {
        Skills.Add(new Skill(
            "Heroic Strike",
            "Physical attack that deals 100% of physical attack power to a target",
            1,
            TargetType.Enemy,
            0,
            ActionType.Damage,
            physicalAttackPower,
            TypeDamage.Physical
        ));

        Skills.Add(new Skill(
            "Battle cry",
            "Increases the physical attack power of all characters on the team by 25",
            2,
            TargetType.AllAllies,
            0,
            ActionType.Buff,
            25,
            TypeDamage.Null,
            AffectedStat.PhysicalAttack
        ));

        Skills.Add(new Skill(
            "Whirlwind",
            "Physical attack that deals 33% of physical attack power to the entire enemy team",
            2,
            TargetType.AllEnemies,
            0,
            ActionType.Damage,
            (int)(physicalAttackPower * 0.33),
            TypeDamage.Physical
        ));
    }

    protected override DefenseResult Defend(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        var result = new DefenseResult();
        
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");
        
        var defenseResult = base.Defend(attacker, typeOfAttack, attackPower);

        if (typeOfAttack == TypeDamage.Physical)
        {
            if (defenseResult.IsParried || LuckTest(25))
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{Name.ToUpper()}] successfully counterattacked!");
                Console.ResetColor();
                
                var counterAttackPower = defenseResult.IsParried
                    ? (int)(defenseResult.DamageTaken * 1.50)
                    : defenseResult.DamageTaken / 2;
            
                var counterAttack = new Attack("Counterattack", this, attacker, counterAttackPower, TypeDamage.Physical );
                Tackle(counterAttack);
            }
        }

        return result;
    }

    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: WARRIOR)");
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

        Skill skill = null;
        Character target = null;

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
                target = Utils.PromptTarget("\nChoose a target:", Menu.TeamThatDefends, this);
            }
            break;
        }
        
        Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target));
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
               $"Speed: {Speed}\n";
    }
}
