﻿namespace J_RPG.Models;

using Services;
using Enums;

public abstract class Character
{
    public string Name { get; protected set; }
    public int CurrentHitPoints { get; private set; }
    public int MaxHitPoints { get; private set; }
    public int PhysicalAttackPower  { get; set; }
    public int MagicAttackPower  { get; set; }
    protected TypeOfArmor Armor { get; private set; }
    public int DodgeChance { get; set; }
    protected int ParadeChance { get; private set; }
    public int ChanceSpellResistance { get; set; }
    public int Speed { get; private set; }
    public bool IsDead { get; private set; }
    protected List<Skill> Skills { get; set; } = new List<Skill>();
    public bool UsesMana { get; private set; }
    public int CurrentMana { get; set; }
    public int MaxMana { get; private set; }
    
    private Random _rand { get; set; } = new Random();
    
    protected Character(string name, int maxHitPoints, int physicalAttackPower,
                        int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed, bool usesMana = false, int maxMana = 0)
    {
        Name = name;
        CurrentHitPoints = maxHitPoints;
        MaxHitPoints = maxHitPoints;
        PhysicalAttackPower = physicalAttackPower;
        MagicAttackPower = magicAttackPower;
        Armor = armor;
        DodgeChance = dodgeChance;
        ParadeChance = paradeChance;
        ChanceSpellResistance = chanceSpellResistance;
        Speed = speed;
        IsDead = false;
        UsesMana = usesMana;
        if (UsesMana)
        {
            CurrentMana = maxMana;
            MaxMana = maxMana;
            Skills.Add(new Skill(
                "Drink",
                "Regenerates half mana",
                1,
                TargetType.Self,
                0,
                ActionType.Buff,
                MaxMana / 2
            ));
        }
    }

    public static void Tackle(Attack attack)
    {
        Console.WriteLine("\n========== ATTACK PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"Attack Name: {attack.Name}");
        Console.WriteLine($"[{attack.AttackingCharacter.Name.ToUpper()}] attacks [{attack.TargetCharacter.Name.ToUpper()}]");
        Console.WriteLine($"Attack Type: {attack.TypeOfDamage}");
        Console.WriteLine($"Damage: {attack.Damage}");
        Console.ResetColor();
        attack.TargetCharacter.Defend(attack.AttackingCharacter ,attack.TypeOfDamage, attack.Damage);
        Console.WriteLine("===================================\n");
    }

    protected virtual DefenseResult Defend(Character attacker, TypeDamage typeOfAttack, int attackPower)
    {
        var result = new DefenseResult();
        
        var damage = attackPower;
        
        if (typeOfAttack == TypeDamage.Physical)
        {
            if (PerformLuckTest(DodgeChance))
            {
                result.IsDodged = true;
                Console.WriteLine($"{Name} dodged the attack!");
                return result;
            }
            if (PerformLuckTest(ParadeChance))
            {
                result.IsParried = true;
                damage = attackPower / 2;
                Console.WriteLine($"{Name} parried the attack and reduced damage to {damage}!");
            }
        } else if (typeOfAttack == TypeDamage.Magic)
        {
            if (PerformLuckTest(ChanceSpellResistance))
            {
                result.IsResisted = true;
                Console.WriteLine($"{Name} resisted the magic attack!");
                return result;
            }
            Speed = (int)(Speed * 0.85);
        }
        
        damage = GetArmorResistance(Armor, typeOfAttack, damage);
        result.DamageTaken = damage;

        if (attacker is Paladin paladin)
        {
            paladin.RestoreHealth(damage / 2);
        }
        
        if ((CurrentHitPoints -= damage) <= 0)
        {
            CurrentHitPoints = 0;
            IsDead = true;
            Console.WriteLine($"{Name} has died.");
            return result;
        }
        
        Console.WriteLine($"The {Name} character received {damage} damage. Remaining HP: {CurrentHitPoints}");
        
        return result;
    }

    public void RestoreHealth(int extraLife)
    {
       if (CurrentHitPoints + extraLife <= MaxHitPoints)
       {
            CurrentHitPoints += extraLife;
            Console.WriteLine(
                $"{Name} regenerated {extraLife} hp. It now has {CurrentHitPoints} hp");
            return;
       }
       CurrentHitPoints = MaxHitPoints;
       Console.WriteLine($"{Name} has regenerated life. It now has {CurrentHitPoints} hp");
    }

    protected bool PerformLuckTest(int successProbabilityPercentage)
    {
        var targetNumber = _rand.Next(1, 100);
        var shuffledNumbers = new int[100];
        for (var i = 1; i < shuffledNumbers.Length; i++)
        {
            shuffledNumbers[i-1] = i;
        }

        shuffledNumbers = Shuffle(shuffledNumbers);
            
        for (var j = 0; j < successProbabilityPercentage; j++)
        {
            if (shuffledNumbers[j] == targetNumber)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private int[] Shuffle(int[] values)
    {
        for (var i = values.Length - 1; i > 0; i--) {
            var k = _rand.Next(i + 1);
            (values[k], values[i]) = (values[i], values[k]);
        }

        return values;
    }

    private static int GetArmorResistance(TypeOfArmor armor, TypeDamage typeOfAttack ,int damageReceived)
    {
        var reductionFactor = typeOfAttack switch
        {
            TypeDamage.Physical => armor switch
            {
                TypeOfArmor.Leather => 0.85,
                TypeOfArmor.Mesh => 0.70,
                TypeOfArmor.Plates => 0.55,
                _ => 1.0
            },
            TypeDamage.Magic => armor switch
            {
                TypeOfArmor.Fabric => 0.70,
                TypeOfArmor.Leather => .80,
                TypeOfArmor.Mesh => 0.90,
                _ => 1.0
            },
            _ => 1.0
        };
            
        return (int)(damageReceived * reductionFactor);
    }

    public abstract void ChoiceAction();

    public override string ToString()
    {
        var result = 
            "----------------------------------------\n" +
            $"Name: {Name}\n" +
            $"Class: {GetType().Name}\n" +
            $"HP: {CurrentHitPoints}/{MaxHitPoints}\n" +
            $"Physical Attack: {PhysicalAttackPower}\n" +
            $"Magical Attack: {MagicAttackPower}\n" +
            $"Dodge Chance: {DodgeChance}%\n" +
            $"Parade Chance: {ParadeChance}%\n" +
            $"Spell Resistance Chance: {ChanceSpellResistance}%\n" +
            $"Speed : {Speed}\n" +
            $"Armor Type: {Armor} (Resistance: {Menu.GetArmorPercentage(Armor)})\n";
        result += "----------------------------------------";
        
        return result;
    }
    
    public void ReduceCooldowns()
    {
        foreach (var skill in Skills)
        {
            skill.ReduceCooldown();
        }
    }
    
    public void UseMana(int skillCost)
    {
        if (skillCost <= CurrentMana)
        {
            CurrentMana -= skillCost;
            Console.WriteLine($"{Name} uses {skillCost} mana points. Remaining Mana: {CurrentMana}/{MaxMana}");
        }
    }
}