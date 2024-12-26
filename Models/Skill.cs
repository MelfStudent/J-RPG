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
    /// Uses the skill, applying its effect to the target(s) if applicable.
    /// Checks for cooldown, mana cost, and target validity before applying the effect.
    /// </summary>
    /// <param name="user">The character using the skill.</param>
    /// <param name="target">The target of the skill, if required.</param>
    public void UseSkill(Character user, Character target = null!)
    {
        if (IsSkillOnCooldown()) return;
        if (!HasSufficientMana(user)) return;
        ConsumeManaIfApplicable(user);

        try
        {
            ExecuteSkillBasedOnTarget(user, target);
            ApplyCooldown();
        }
        catch (Exception ex)
        {
            Utils.LogError($"An error occurred while using skill {Name}: {ex.Message}");
        }
    }

    /// <summary>
    /// Checks if the skill is on cooldown.
    /// </summary>
    /// <returns>True if the skill is on cooldown, otherwise false.</returns>
    private bool IsSkillOnCooldown()
    {
        if (CurrentCooldown != 0)
        {
            Console.WriteLine($"{Name} is not ready (recharging)!");
            return true;
        }
        return false;
    }

    /// <summary>
    /// Checks if the user has sufficient mana to use the skill.
    /// </summary>
    /// <param name="user">The character attempting to use the skill.</param>
    /// <returns>True if the user has enough mana, otherwise false.</returns>
    private bool HasSufficientMana(Character user)
    {
        if (user.UsesMana && user.CurrentMana < ManaCost)
        {
            Console.WriteLine($"{user.Name} doesn't have enough mana to cast {Name}!");
            return false;
        }
        return true;
    }

    /// <summary>
    /// Consumes mana from the user if the skill has a mana cost.
    /// </summary>
    /// <param name="user">The character using the skill.</param>
    private void ConsumeManaIfApplicable(Character user)
    {
        if (user.UsesMana)
        {
            user.UseMana(ManaCost);
        }
    }

    /// <summary>
    /// Executes the skill's effect based on its target type.
    /// </summary>
    /// <param name="user">The character using the skill.</param>
    /// <param name="target">The target of the skill.</param>
    private void ExecuteSkillBasedOnTarget(Character user, Character target)
    {
        switch (Target)
        {
            case TargetType.Self:
                ExecuteEffect(user, user);
                break;

            case TargetType.Enemy:
            case TargetType.Ally:
                EnsureTargetIsNotNull(target);
                ExecuteEffect(user, target);
                break;

            case TargetType.AllEnemies:
                ApplyEffectToAll(Menu.TeamThatDefends!.Members, user);
                break;

            case TargetType.AllAllies:
                ApplyEffectToAll(Menu.TeamThatAttacks!.Members, user);
                break;

            default:
                Console.WriteLine($"Skill {Name} has an unhandled target type.");
                break;
        }
    }

    /// <summary>
    /// Ensures that the target is not null when required by the skill.
    /// </summary>
    /// <param name="target">The target to check.</param>
    /// <exception cref="ArgumentNullException">Thrown if the target is null.</exception>
    private void EnsureTargetIsNotNull(Character target)
    {
        if (target == null)
        {
            throw new ArgumentNullException(nameof(target), $"Target cannot be null for skill {Name}.");
        }
    }

    /// <summary>
    /// Applies the skill's effect to all characters in the target group.
    /// </summary>
    /// <param name="targets">The group of characters to target.</param>
    /// <param name="user">The character using the skill.</param>
    private void ApplyEffectToAll(IEnumerable<Character> targets, Character user)
    {
        foreach (var target in targets)
        {
            ExecuteEffect(user, target);
        }
    }

    /// <summary>
    /// Applies the skill's cooldown.
    /// </summary>
    private void ApplyCooldown()
    {
        CurrentCooldown = Cooldown;
    }

    /// <summary>
    /// Executes the skill's primary effect (e.g., damage, healing, buff, debuff).
    /// </summary>
    /// <param name="user">The character using the skill.</param>
    /// <param name="target">The target of the skill.</param>
    private void ExecuteEffect(Character user, Character target)
    {
        switch (_skillAction)
        {
            case ActionType.Damage:
                ApplyDamage(user, target);
                break;

            case ActionType.Heal:
                ApplyHealing(target);
                break;

            case ActionType.Buff:
                ApplyBuff(user, target);
                break;

            case ActionType.Debuff:
                ApplyDebuff(user, target);
                break;
        }
    }

    /// <summary>
    /// Applies damage to the target based on the skill's power and type.
    /// </summary>
    /// <param name="user">The character using the skill.</param>
    /// <param name="target">The target receiving the damage.</param>
    private void ApplyDamage(Character user, Character target)
    {
        var damageAttack = new Attack(Name, user, target, EffectPower, TypeOfDamage);
        if (Name == "Low blow" && target.CurrentHitPoints < target.MaxHitPoints)
        {
            damageAttack.Damage = (int)(damageAttack.Damage * 1.50);
        }
        Character.Tackle(damageAttack);
    }

    /// <summary>
    /// Heals the target by the skill's effect power.
    /// </summary>
    /// <param name="target">The character to heal.</param>
    private void ApplyHealing(Character target)
    {
        target.RestoreHealth(EffectPower);
        Console.WriteLine($"{target.Name} recovers {EffectPower} HP thanks to {Name}!");
    }

    /// <summary>
    /// Applies a buff to the target, modifying stats or triggering special effects.
    /// </summary>
    /// <param name="user">The character applying the buff.</param>
    /// <param name="target">The character receiving the buff.</param>
    private void ApplyBuff(Character user, Character target)
    {
        switch (Name)
        {
            case "Drink":
                RecoverMana(user);
                break;

            case "Frost Barrier":
                ActivateFrostBarrier(user);
                break;

            case "Escape":
                ActivateEscape(user);
                break;

            default:
                ApplyGenericBuff(target);
                break;
        }
    }

    /// <summary>
    /// Recovers mana for the user based on the skill's effect power.
    /// Ensures that the recovered mana does not exceed the user's maximum mana.
    /// </summary>
    /// <param name="user">The character recovering mana.</param>
    private void RecoverMana(Character user)
    {
        var manaRecovered = Math.Min(EffectPower, user.MaxMana - user.CurrentMana);
        user.CurrentMana += manaRecovered;
        Console.WriteLine($"{user.Name} drinks a potion and recovers {manaRecovered} mana points. Current Mana: {user.CurrentMana}/{user.MaxMana}");
    }

    /// <summary>
    /// Activates the Frost Barrier buff for the user, granting damage reduction for a limited number of attacks.
    /// Only applicable to characters of the Mage class.
    /// </summary>
    /// <param name="user">The Mage activating Frost Barrier.</param>
    private static void ActivateFrostBarrier(Character user)
    {
        if (user is Mage mage)
        {
            Console.WriteLine($"{user.Name} activates Frost Barrier!");
            mage.RemainingDamageReductions = 2;
        }
    }

    /// <summary>
    /// Activates the Escape buff for the user, increasing dodge and spell resistance chances.
    /// Only applicable to characters of the Thief class.
    /// </summary>
    /// <param name="user">The Thief activating Escape.</param>
    private static void ActivateEscape(Character user)
    {
        if (user is Thief thief)
        {
            Console.WriteLine($"{user.Name} uses Escape!");
            thief.DodgeChance = Math.Min(thief.DodgeChance + 20, 50);
            thief.ChanceSpellResistance = Math.Min(thief.ChanceSpellResistance + 20, 50);
        }
    }

    /// <summary>
    /// Applies a generic buff to the target, modifying stats such as physical or magic attack power.
    /// The type of stat affected is determined by the skill's configuration.
    /// </summary>
    /// <param name="target">The character receiving the buff.</param>
    private void ApplyGenericBuff(Character target)
    {
        switch (_targetStat)
        {
            case AffectedStat.PhysicalAttack:
                target.PhysicalAttackPower += EffectPower;
                Console.WriteLine($"{target.Name}'s physical attack increases by {EffectPower} due to {Name}!");
                break;

            case AffectedStat.MagicAttack:
                target.MagicAttackPower += EffectPower;
                Console.WriteLine($"{target.Name}'s magic attack increases by {EffectPower} due to {Name}!");
                break;

            default:
                Console.WriteLine($"{target.Name} receives a generic buff with {Name}.");
                break;
        }
    }

    /// <summary>
    /// Applies a debuff to the target, reducing stats or triggering negative effects.
    /// </summary>
    /// <param name="user">The character applying the debuff.</param>
    /// <param name="target">The character receiving the debuff.</param>
    private void ApplyDebuff(Character user, Character target)
    {
        if (Name == "Mana Burn")
        {
            Console.WriteLine($"{user.Name} uses Mana Burn on {target.Name}!");
            target.CurrentMana = Math.Max(target.CurrentMana / 2, target.CurrentMana - 40);
            Console.WriteLine($"{target.Name}'s mana is reduced to {target.CurrentMana}!");
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
