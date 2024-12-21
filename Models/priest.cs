namespace J_RPG.Models;

using Services;

public class Priest : Character
{
    public Priest(string name) : base(name, 70, 0, 65, TypeOfArmor.Fabric, 10, 0, 20, 70, true, 100)
    {
        Skills.Add(new Skill(
            "Punishment",
            "Magic attack that deals 75% of magic attack power to the target",
            1,
            TargetType.Enemy,
            15,
            ActionType.Damage,
            (int)(65 * 0.75),
            TypeDamage.Magic
        ));
        
        Skills.Add(new Skill(
            "Circle of care",
            "Heals the entire team for 75% of magic attack power",
            2,
            TargetType.AllAllies,
            30,
            ActionType.Heal,
            (int)(65 * 0.75)
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