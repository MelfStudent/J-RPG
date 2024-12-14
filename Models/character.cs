﻿namespace J_RPG.Models;

using Services;

public abstract class Character
{
    public string Name { get; protected set; }
    protected int CurrentHitPoints { get; private set; }
    protected int MaxHitPoints { get; private set; }
    public int PhysicalAttackPower  { get; set; }
    public int MagicAttackPower  { get; set; }
    private TypeOfArmor Armor { get; set; }
    public int DodgeChance { get; set; }
    private int ParadeChance { get; set; }
    protected int ChanceSpellResistance { get; set; }
    public int Speed { get; protected set; }
    public bool IsDead { get; private set; }
    protected List<Skill> Skills { get; set; } = new List<Skill>();
    
    private Random Rand { get; set; } = new Random();
    
    protected Character(string name, int maxHitPoints, int physicalAttackPower,
                        int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance, int speed)
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
    }

    public static void Tackle(Attack attack)
    {
        Console.WriteLine("\n========== ATTACK PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"[{attack.AttackingCharacter.Name.ToUpper()}] attacks [{attack.TargetCharacter.Name.ToUpper()}]");
        Console.WriteLine($"Attack Type: {attack.TypeOfDamage}");
        Console.WriteLine($"Damage: {attack.Damage}");
        Console.ResetColor();
        attack.TargetCharacter.Defend(attack.TypeOfDamage, attack.Damage);
        Console.WriteLine("===================================\n");
    }

    protected virtual DefenseResult Defend(TypeDamage typeOfAttack, int attackPower)
    {
        var result = new DefenseResult();
        
        var damage = attackPower;
        
        if (typeOfAttack == TypeDamage.Physical)
        {
            if (LuckTest(DodgeChance))
            {
                result.IsDodged = true;
                Console.WriteLine($"{Name} dodged the attack!");
                return result;
            }
            if (LuckTest(ParadeChance))
            {
                result.IsParried = true;
                damage = attackPower / 2;
                Console.WriteLine($"{Name} parried the attack and reduced damage to {damage}!");
            }
        } else if (typeOfAttack == TypeDamage.Magic)
        {
            if (LuckTest(ChanceSpellResistance))
            {
                result.IsResisted = true;
                Console.WriteLine($"{Name} resisted the magic attack!");
                return result;
            }
        }
        
        damage = GetArmorResistance(Armor, typeOfAttack, damage);
        result.DamageTaken = damage;
        
        if ((CurrentHitPoints -= damage) <= 0)
        {
            CurrentHitPoints = 0;
            IsDead = true;
            Console.WriteLine($"{Name} has died.");
            return result;
        }
        
        if (Menu.TeamThatAttacks.GetType().Name == "Paladin")
        {
            //Menu.CharacterWhoAttacks.Heal((int)(damage * 0.50));
        }
        Console.WriteLine($"The {Name} character received {damage} damage. Remaining HP: {CurrentHitPoints}");
        
        return result;
    }

    public void Heal(int extraLife)
    {
       /* if (CurrentHitPoints + extraLife <= MaxHitPoints)
        {
            CurrentHitPoints += extraLife;
            Console.WriteLine(
                $"{Menu.CharacterWhoAttacks.Name} regenerated {extraLife} hp. It now has {Menu.CharacterWhoAttacks.CurrentHitPoints} hp");
            return;
        }
        CurrentHitPoints = MaxHitPoints;
        Console.WriteLine($"{Menu.CharacterWhoAttacks.Name} has regenerated life. It now has {Menu.CharacterWhoAttacks.CurrentHitPoints} hp");
    */
    }

    protected bool LuckTest(int percentage)
    {
        var toFind = Rand.Next(1, 100);
        var test1 = new int[100];
        for (var i = 1; i < test1.Length; i++)
        {
            test1[i-1] = i;
        }

        test1 = Shuffle(test1);
            
        for (var j = 0; j < percentage; j++)
        {
            if (test1[j] == toFind)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private int[] Shuffle(int[] values)
    {
        for (var i = values.Length - 1; i > 0; i--) {
            var k = Rand.Next(i + 1);
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
            if (GetType().GetProperty("ManaPoints") is not null)
            {
                var manaPoints = (int)GetType().GetProperty("ManaPoints")?.GetValue(this)!;
                result += $"Mana Points: {manaPoints}\n";
            }
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
}
