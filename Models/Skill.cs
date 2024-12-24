namespace J_RPG.Models;

using Services;
using Enums;

/// <summary>
/// Represents a skill that a character can use in combat.
/// Each skill has a name, description, cooldown, target type, effect power, and mana cost.
/// Skills can perform different actions such as damage, healing, buffs, and debuffs.
/// </summary>
public class Skill
{
   /// <summary>
        /// Gets or sets the name of the skill.
        /// </summary>
        /// <value>The name of the skill.</value>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the description of the skill.
        /// This provides details about the skill's effect and usage.
        /// </summary>
        /// <value>The description of the skill.</value>
        public string Description { get; set; }

        /// <summary>
        /// Gets the cooldown period of the skill in turns.
        /// </summary>
        /// <value>The number of turns the skill must wait before it can be used again.</value>
        public int Cooldown { get; private set; }

        /// <summary>
        /// Gets the current cooldown of the skill.
        /// This is the number of turns left before the skill can be used again.
        /// </summary>
        /// <value>The remaining number of turns before the skill is ready to use again.</value>
        public int CurrentCooldown { get; private set; }

        /// <summary>
        /// Gets the target type of the skill, which indicates who or what the skill affects.
        /// Possible values include self, enemy, ally, all enemies, all allies.
        /// </summary>
        /// <value>The target type (e.g., Self, Enemy, Ally, etc.).</value>
        public TargetType Target { get; private set; }

        /// <summary>
        /// Gets the mana cost required to use the skill.
        /// </summary>
        /// <value>The amount of mana required to use the skill.</value>
        public int ManaCost { get; private set; }

        /// <summary>
        /// Gets or sets the action type that determines the skill's effect (e.g., damage, healing, buff, debuff).
        /// </summary>
        /// <value>The type of action performed by the skill (e.g., Damage, Heal, Buff, etc.).</value>
        private ActionType _skillAction { get; set; }

        /// <summary>
        /// Gets the effect power of the skill.
        /// This determines the strength of the skill's effect, such as damage dealt, healing amount, or buff strength.
        /// </summary>
        /// <value>The power of the skill's effect (e.g., damage, healing amount, etc.).</value>
        public int EffectPower { get; private set; }

        /// <summary>
        /// Gets the type of damage dealt by the skill, if applicable.
        /// This defines the damage type (e.g., physical, magical, etc.).
        /// </summary>
        /// <value>The type of damage (e.g., physical, magical) or a null type if not applicable.</value>
        public TypeDamage TypeOfDamage { get; private set; }

        /// <summary>
        /// Gets or sets the stat that is affected by the skill (e.g., physical attack, magic attack).
        /// This is only relevant for buff or debuff skills.
        /// </summary>
        /// <value>The stat affected by the skill (e.g., PhysicalAttack, MagicAttack).</value>
        private AffectedStat _targetStat { get; set; }

    /// <summary>
    /// Initializes a new instance of the <see cref="Skill"/> class.
    /// </summary>
    /// <param name="name">The name of the skill.</param>
    /// <param name="description">A description of the skill.</param>
    /// <param name="cooldown">The number of turns the skill needs to recharge.</param>
    /// <param name="target">The target type (Self, Enemy, Ally, etc.).</param>
    /// <param name="manaCost">The mana cost for using the skill.</param>
    /// <param name="actionType">The type of action the skill performs (e.g., damage, heal, buff).</param>
    /// <param name="effectPower">The strength of the effect (e.g., damage dealt, healing amount, buff strength).</param>
    /// <param name="typeOfDamage">The type of damage (if applicable) for the skill.</param>
    /// <param name="targetStat">The stat that the skill affects (if applicable, e.g., physical attack).</param>
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

    /// <summary>
    /// Uses the skill on a target. The skill effect is based on its action type.
    /// </summary>
    /// <param name="user">The character using the skill.</param>
    /// <param name="target">The target character (if applicable).</param>
    public void UseSkill(Character user, Character target = null!)
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
            user.UseMana(ManaCost);
        }

        try
        {
            switch (Target)
            {
                case TargetType.Self:
                    ExecuteEffect(user, user);
                    break;

                case TargetType.Enemy:
                case TargetType.Ally:
                    if (target == null)
                    {
                        throw new ArgumentNullException(nameof(target), $"Target cannot be null for skill {Name}.");
                    }
                    ExecuteEffect(user, target);
                    break;

                case TargetType.AllEnemies:
                    foreach (var enemy in Menu.TeamThatDefends!.Members)
                    {
                        ExecuteEffect(user, enemy);
                    }
                    break;

                case TargetType.AllAllies:
                    foreach (var ally in Menu.TeamThatAttacks!.Members)
                    {
                        ExecuteEffect(user, ally);
                    }
                    break;

                default:
                    Console.WriteLine($"Skill {Name} has an unhandled target type.");
                    break;
            }

            CurrentCooldown = Cooldown;
        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"An error occurred while using skill {Name}: {ex.Message}");
            Console.ResetColor();
        }
    }

    /// <summary>
    /// Executes the effect of the skill on the target.
    /// The effect depends on the action type (e.g., damage, healing, buff, debuff).
    /// </summary>
    /// <param name="user">The character using the skill.</param>
    /// <param name="target">The target character of the skill's effect.</param>
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
                    target.RestoreHealth(EffectPower);
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
                        mage.RemainingDamageReductions = 2;
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

    /// <summary>
    /// Reduces the cooldown of the skill by 1 turn.
    /// </summary>
    public void ReduceCooldown()
    {
        if (CurrentCooldown > 0)
            CurrentCooldown--;
    }

    /// <summary>
    /// Returns a string representation of the skill.
    /// </summary>
    /// <returns>A string with the skill's name, mana cost, cooldown, effect, and power.</returns>
    public override string ToString()
    {
        return $"{Name} (Cost: {ManaCost} Mana, Cooldown: {Cooldown} turns, Effect: {_skillAction}, Power: {EffectPower})";
    }
}
