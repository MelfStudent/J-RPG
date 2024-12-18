namespace J_RPG.Models;

using Services;

public class Skill
{
    public string Name { get; set; }
    private int Cooldown { get; set; }
    public int CurrentCooldown { get; set; }
    public TargetType Target { get; private set; }
    public int ManaCost { get; private set; }
    private ActionType SkillAction { get; set; }
    private int EffectPower { get; set; }
    private TypeDamage TypeOfDamage { get; set; }
    private AffectedStat TargetStat { get; set; }

    public Skill(string name, int cooldown, TargetType target, int manaCost, ActionType actionType, int effectPower, TypeDamage typeOfDamage = TypeDamage.Null, AffectedStat targetStat = AffectedStat.Null)
    {
        Name = name;
        Cooldown = cooldown;
        CurrentCooldown = 0;
        Target = target;
        ManaCost = manaCost;
        SkillAction = actionType;
        EffectPower = effectPower;
        TypeOfDamage = typeOfDamage;
        TargetStat = targetStat;
    }

    public void UseSkill(Character user, Character target = null)
    {
        if (CurrentCooldown != 0)
        {
            Console.WriteLine($"{Name} is not ready (recharging) !");
            return;
        }

        if (user.UsesMana && user.CurrentMana < ManaCost)
        {
            Console.WriteLine($"{user.Name} doesn't have enough mana to cast {Name}!");
            return;
        }

        if (user.UsesMana)
        {
            user.ConsumeMana(ManaCost);
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
                var damageAttack = new Attack(Name, user, target, EffectPower, TypeOfDamage);
                    Character.Tackle(damageAttack);   
                break;

            case ActionType.Heal:
                    target.Heal(EffectPower);
                    Console.WriteLine($"{target.Name} recover {EffectPower} PV thanks to {Name} !");
                break;

            case ActionType.Buff:
                if (Name == "Drink")
                {
                    var manaRecovered = Math.Min(EffectPower, user.MaxMana - user.CurrentMana);
                    user.CurrentMana += manaRecovered;
                    Console.WriteLine($"{user.Name} drinks a potion and recovers {manaRecovered} mana points. Current Mana: {user.CurrentMana}/{user.MaxMana}");
                }
                else
                {
                    switch (TargetStat)
                    {
                        case AffectedStat.PhysicalAttack:
                            Console.WriteLine($"{target.Name}'s physical attack increases by {EffectPower} due to {Name}!");
                            target.PhysicalAttackPower += EffectPower;
                            break;

                        case AffectedStat.MagicAttack:
                            Console.WriteLine($"{target.Name}'s magic attack increases by {EffectPower} due to {Name}!");
                            target.MagicAttackPower += EffectPower;
                            break;

                        default:
                            Console.WriteLine($"{target.Name} receives a generic buff with {Name}.");
                            break;
                    }   
                }
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
