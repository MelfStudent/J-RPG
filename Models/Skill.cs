namespace J_RPG.Models;

using Services;
using Enums;

public class Skill
{
    public string Name { get; set; }
    public string Description { get; set; }
    public int Cooldown { get; private set; }
    public int CurrentCooldown { get; private set; }
    public TargetType Target { get; private set; }
    public int ManaCost { get; private set; }
    private ActionType _skillAction { get; set; }
    public int EffectPower { get; private set; }
    public TypeDamage TypeOfDamage { get; private set; }
    private AffectedStat _targetStat { get; set; }

    public Skill(string name, string description, int cooldown, TargetType target, int manaCost, ActionType actionType, int effectPower, TypeDamage typeOfDamage = TypeDamage.Null, AffectedStat targetStat = AffectedStat.Null)
    {
        Name = name;
        Description = description;
        Cooldown = cooldown;
        CurrentCooldown = 0;
        Target = target;
        ManaCost = manaCost;
        _skillAction = actionType;
        EffectPower = effectPower;
        TypeOfDamage = typeOfDamage;
        _targetStat = targetStat;
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
        switch (_skillAction)
        {
            case ActionType.Damage:
                    var damageAttack = new Attack(Name, user, target, EffectPower, TypeOfDamage);
                    if (Name == "Low blow" && target.CurrentHitPoints < target.MaxHitPoints)
                    {
                        damageAttack.Damage = (int)(damageAttack.Damage * 1.50);
                    }
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
                } else if (Name == "Frost Barrier")
                {
                    if (user is Mage mage)
                    {
                        Console.WriteLine("\n========== ACTION PHASE ==========");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{user.Name} activates Frost Barrier!");
                        Console.WriteLine("The next two attacks will be reduced:");
                        Console.WriteLine("- Physical damage reduced by 60%");
                        Console.WriteLine("- Magical damage reduced by 50%");
                        Console.ResetColor();
                        mage.AttackReductionNumber = 2;
                        Console.WriteLine("===================================\n");
                    }
                } else if (Name == "Escape")
                {
                    if (user is Thief thief)
                    {
                        Console.WriteLine("\n========== ACTION PHASE ==========");
                        Console.ForegroundColor = ConsoleColor.Yellow;
                        Console.WriteLine($"{user.Name} uses Escape!");
                        var newDodgeChance = 50;
                        var newChanceSpellResistance = 50;
                        
                        if (thief.DodgeChance + 20 <= 50)
                        {
                            newDodgeChance += thief.DodgeChance + 20;
                        } else if (thief.ChanceSpellResistance + 20 <= 50)
                        {
                            newChanceSpellResistance += thief.ChanceSpellResistance + 20;
                        }
                        thief.DodgeChance = newDodgeChance;
                        thief.ChanceSpellResistance = newChanceSpellResistance;
                        
                        Console.WriteLine($"New Dodge Chance: {thief.DodgeChance}%");
                        Console.WriteLine($"New Resistance Chance: {thief.ChanceSpellResistance}%");
                        Console.ResetColor();
                        Console.WriteLine("===================================\n");
                    }
                }
                else
                {
                    switch (_targetStat)
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
                if (Name == "Mana Burn")
                {
                    Console.WriteLine("\n========== ACTION PHASE ==========");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"{user.Name} uses Mana Burn on {target.Name}!");
                    
                    if (target.CurrentMana / 2 >= 40)
                    {
                        var previousMana = target.CurrentMana;
                        target.CurrentMana /= 2;
                        Console.WriteLine($"{target.Name}'s mana is reduced by half from {previousMana} to {target.CurrentMana}!");
                    }
                    else
                    {
                        var previousMana = target.CurrentMana;
                        target.CurrentMana -= 40;
                        Console.WriteLine($"{target.Name}'s mana is reduced by 40 from {previousMana} to {target.CurrentMana}!");
                    }
                    Console.ResetColor();
                    Console.WriteLine("===================================\n");
                }
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
        return $"{Name} (Cost: {ManaCost} Mana, Cooldown: {Cooldown} turns, Effect: {_skillAction}, Power: {EffectPower})";
    }
}
