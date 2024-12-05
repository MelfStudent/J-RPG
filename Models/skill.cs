namespace J_RPG.Models;

using Services;

public class Skill
{
    public string Name { get; set; }
    private int Cooldown { get; set; }
    private int CurrentCooldown { get; set; }
    public TargetType Target { get; private set; }
    private int ManaCost { get; set; }
    private ActionType SkillAction { get; set; }
    private int EffectPower { get; set; }

    public enum TargetType { Self, Ally, Enemy, AllEnemies, AllAllies }
    public enum ActionType { Damage, Heal, Buff, Debuff }

    public Skill(string name, int cooldown, TargetType target, int manaCost, ActionType actionType, int effectPower)
    {
        Name = name;
        Cooldown = cooldown;
        CurrentCooldown = 0;
        Target = target;
        ManaCost = manaCost;
        SkillAction = actionType;
        EffectPower = effectPower;
    }

    public void UseSkill(Character user, Character target = null)
    {
        if (CurrentCooldown != 0)
        {
            Console.WriteLine($"{Name} is not ready (recharging) !");
            return;
        }

        if (Target == TargetType.Self)
        {
            ExecuteEffect(user, user);
        }
        else if (Target == TargetType.Enemy || Target == TargetType.Ally)
        {
            ExecuteEffect(user, target);
        } else if (Target == TargetType.AllEnemies)
        {
            foreach (var _target in Menu.TeamThatDefends.Members)
            {
                ExecuteEffect(user, _target);
            }
            
        } else if (Target == TargetType.AllAllies)
        {
            foreach (var _target in Menu.TeamThatAttacks.Members)
            {
                ExecuteEffect(user, _target);
            }
        }
        CurrentCooldown = Cooldown;
    }

    private void ExecuteEffect(Character user, Character target)
    {
        switch (SkillAction)
        {
            case ActionType.Damage:
                var damageAttack = new Attack(Name, user, target, EffectPower, Attack.TypeDamage.Physical);
                    Character.Tackle(damageAttack);   
                break;

            case ActionType.Heal:
                    target.Heal(EffectPower);
                    Console.WriteLine($"{target.Name} recover {EffectPower} PV thanks to {Name} !");
                break;

            case ActionType.Buff:
                    Console.WriteLine($"{target.Name} strengthens with {Name} !");
                    target.PhysicalAttackPower += EffectPower;
                break;

            case ActionType.Debuff:
                    Console.WriteLine($"{target.Name} undergoes a weakening with {Name} !");
                    target.DodgeChance = Math.Max(0, target.DodgeChance - EffectPower);   
                break;
        }
    }

    public void ReduceCooldown()
    {
        if (CurrentCooldown > 0)
            CurrentCooldown--;
    }

    public override string ToString()
    {
        return $"{Name} (Cost: {ManaCost} Mana, Cooldown: {Cooldown} turns, Effect: {SkillAction}, Power: {EffectPower})";
    }
}
