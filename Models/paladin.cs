namespace J_RPG.Models;

using Services;

public class Paladin : Character
{
    public Paladin(string name) : base(name, 95, 40, 40, TypeOfArmor.Mesh, 5, 10, 20, 75, true, 60)
    { 
        Skills.Add(new Skill(
            "Crusader Strike",
            "Physical attack that deals 100% of physical attack power to the target",
            1,
            TargetType.Enemy,
            5,
            ActionType.Damage,
            40,
            TypeDamage.Physical
        ));
        
        Skills.Add(new Skill(
            "Judgement",
            "Magic attack that deals 100% of magic attack power to the target",
            1,
            TargetType.Enemy,
            10,
            ActionType.Damage,
            40,
            TypeDamage.Magic
        ));
        
        Skills.Add(new Skill(
            "Bright flash",
            "Heals the target for 125% of Magic Attack Power",
            1,
            TargetType.Ally,
            25,
            ActionType.Heal,
            (int)(40 * 1.25),
            TypeDamage.Magic
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
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: PALADIN)");
        Console.WriteLine(
            $"HP: {CurrentHitPoints}/{MaxHitPoints} | Physical Attack: {PhysicalAttackPower} | Magic Attack: {MagicAttackPower}");

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
            
            if (skill.Target == TargetType.Ally)
            {
                target = Utils.PromptTarget("\nChoose a target:", Menu.TeamThatAttacks, this);
            }

            break;
        }

        Menu.SkillsTourCurrent.Add(new SkillUsage(this, skill, target));
    }
}