﻿namespace J_RPG.Models;

using Services;

public class Mage : Character
{
    public int AttackReductionNumber { get; set; }
    private bool IsSpellReturned = false;
    
    public Mage(string name, int maxHitPoints, int physicalAttackPower, int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed, bool usesMana, int maxMana) : base(name, maxHitPoints, physicalAttackPower, magicAttackPower, armor, dodgeChance, paradeChance, chanceSpellResistance, speed, usesMana, maxMana)
    {
        AttackReductionNumber = 0;

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

        if (IsSpellReturned && typeOfAttack == TypeDamage.Magic)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine($"{Name} returns the magic attack to {attacker.Name} !");
            Console.ResetColor();

            var damageAttack = new Attack("Spell Return", this, attacker, attackPower, typeOfAttack);
            Tackle(damageAttack);

            IsSpellReturned = false;
            return result;
        } 
        if (AttackReductionNumber > 0)
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

            AttackReductionNumber--;
        }
        
        base.Defend(attacker, typeOfAttack, attackPower);

        return result;
    }

    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: MAGE)");
        Console.WriteLine($"HP: {CurrentHitPoints}/{MaxHitPoints} | Physical Attack: {PhysicalAttackPower} | Magic Attack: {MagicAttackPower}");
        
        var skillDetails = Skills.Select(s => 
            $"{s.Name} - {s.Description}\n" +
            $"  Cooldown: {s.CurrentCooldown}/{s.Cooldown}\n" +
            $"  Mana Cost: {s.ManaCost}\n" +
            $"  Damage: {s.EffectPower}\n" +
            $"  Type: {s.TypeOfDamage}\n" +
            $"  Target: {s.Target}"
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
    
    
}