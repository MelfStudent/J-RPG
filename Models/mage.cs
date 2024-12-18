namespace J_RPG.Models;

using Services;

public class Mage : Character
{
    public int AttackReductionNumber { get; set; }
    private bool IsSpellReturned = false;
    
    public Mage(string name, int manaPoints) : base(name, 60, 0, 75, TypeOfArmor.Fabric, 5, 5, 25, 75, true, 100)
    {
        AttackReductionNumber = 0;
        
        Skills.Add(new Skill(
            "Frost bolt",
            1,
            TargetType.Enemy,
            15,
            ActionType.Damage,
            75,
            TypeDamage.Magic
        ));
        
        Skills.Add(new Skill(
            "Frost Barrier",
            2,
            TargetType.Self,
            25,
            ActionType.Buff,
            0
        ));
        
        Skills.Add(new Skill(
            "Blizzard",
            2,
            TargetType.AllEnemies,
            25,
            ActionType.Damage,
            75 / 2,
            TypeDamage.Magic
        ));
        
        Skills.Add(new Skill(
            "Spell Return",
            0,
            TargetType.Self,
            25,
            ActionType.Buff,
            0
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
        Console.WriteLine("Choose an action:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("1. Frost bolt (a magical attack that deals 100% of magical attack power to the target)");
        Console.WriteLine("2. Frost barrier (reduces damage from the next two attacks received)");
        Console.ResetColor();
        
        var skillNames = Skills.Select(s => s.Name).ToList();
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
                target = Utils.PromptTarget("\nChoose a target:");
            }
            break;
        }
        
        Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target));
    }
    
    
}