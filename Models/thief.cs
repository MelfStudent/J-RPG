namespace J_RPG.Models;

using Services;

public class Thief : Character
{
    private int AttackReductionNumber { get; set; }
    
    public Thief(string name, int maxHitPoints, int physicalAttackPower, int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed) : base(name, maxHitPoints, physicalAttackPower, magicAttackPower, armor, dodgeChance, paradeChance, chanceSpellResistance, speed)
    {
        AttackReductionNumber = 0;
        
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
    
    protected override DefenseResult Defend(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        var result = new DefenseResult();
        
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");
        var defendResult = base.Defend(attacker, typeOfAttack, attackPower);

        if (defendResult.IsDodged)
        {
            var attack = new Attack("Stab in the back", this, attacker, 15, TypeDamage.Physical);
            Tackle(attack);   
        }
        
        return result;
    }

    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: THIEF)");
        Console.WriteLine($"HP: {CurrentHitPoints}/{MaxHitPoints} | Physical Attack: {PhysicalAttackPower} | Magic Attack: {MagicAttackPower}");
        
        var skillNames = Skills.Select(s => $"{s.Name} - {s.Description}").ToList();
        skillNames.Add("Skip the turn");

        Skill skill = null;
        Character target = null;

        while (true)
        {
            var skillChoice = Utils.PromptChoice(skillNames, "Enter a number corresponding to the desired action:");

            if (skillChoice == skillNames.Count)
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