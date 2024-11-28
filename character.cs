namespace J_RPG;

public abstract class Character
{
    public string Name { get; set; }
    public int CurrentHitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int PhysicalAttackPower  { get; set; }
    public int MagicAttackPower  { get; set; }
    public TypeOfArmor Armor { get; set; }
    public int DodgeChance { get; set; }
    public int ParadeChance { get; set; }
    public int ChanceSpellResistance { get; set; }
    public bool IsDead { get; set; }
    
    public enum TypeOfArmor { Fabric, Leather, Mesh, Plates }
    
    public Character(int maxHitPoints, int physicalAttackPower,
                        int magicAttackPower, TypeOfArmor armor, int dodgeChance, int paradeChance, int chanceSpellResistance)
    {
        CurrentHitPoints = maxHitPoints;
        MaxHitPoints = maxHitPoints;
        PhysicalAttackPower = physicalAttackPower;
        MagicAttackPower = magicAttackPower;
        Armor = armor;
        DodgeChance = dodgeChance;
        ParadeChance = paradeChance;
        ChanceSpellResistance = chanceSpellResistance;
        IsDead = false;
    }

    public void Tackle(Attack attack)
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

    public virtual void Defend(Attack.TypeDamage typeOfAttack, int attackPower)
    {
        int damage = attackPower;
        if (typeOfAttack == Attack.TypeDamage.Physical)
        {
            if (LuckTest(DodgeChance))
            {
                Console.WriteLine($"The {Name} character dodged the attack !");
                return;
            }
            else if (LuckTest(ParadeChance))
            {
                Console.WriteLine($"The {Name} character parried the attack!");
                damage = attackPower / 2;
            }
        } else if (typeOfAttack == Attack.TypeDamage.Magic)
        {
            if (LuckTest(ChanceSpellResistance))
            {
                Console.WriteLine($"The {Name} character resisted the attack !");
                return;
            }
        }
        
        damage = GetArmorResistance(Armor, typeOfAttack, damage);
        
        if ((CurrentHitPoints -= damage) <= 0)
        {
            CurrentHitPoints = 0;
            IsDead = true;
            Console.WriteLine($"{Name} has died.");
            return;
        }
        
        if (Menu.CharacterWhoAttacks.GetType().Name == "Paladin")
        {
            Menu.CharacterWhoAttacks.Heal((int)(damage * 0.50));
        }
        Console.WriteLine($"The {Name} character received {damage} damage. Remaining HP: {CurrentHitPoints}");
    }

    public void Heal(int extraLife)
    {
        if (CurrentHitPoints + extraLife <= MaxHitPoints)
        {
            CurrentHitPoints += extraLife;
            Console.WriteLine(
                $"{Menu.CharacterWhoAttacks.Name} regenerated {extraLife} hp. It now has {Menu.CharacterWhoAttacks.CurrentHitPoints} hp");
            return;
        }
        CurrentHitPoints = MaxHitPoints;
        Console.WriteLine($"{Menu.CharacterWhoAttacks.Name} has regenerated life. It now has {Menu.CharacterWhoAttacks.CurrentHitPoints} hp");
    }

    protected bool LuckTest(int percentage)
    {
        int toFind = rand.Next(1, 100);
        int[] test1 = new int[100];
        for (int i = 1; i < test1.Length; i++)
        {
            test1[i-1] = i;
        }

        test1 = Shuffle(test1);
            
        for (int j = 0; j < percentage; j++)
        {
            if (test1[j] == toFind)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private Random rand = new Random();
    
    private int[] Shuffle(int[] values)
    {
        for (int i = values.Length - 1; i > 0; i--) {
            int k = rand.Next(i + 1);
            int value = values[k];
            values[k] = values[i];
            values[i] = value;
        }

        return values;
    }

    private int GetArmorResistance(TypeOfArmor armure, Attack.TypeDamage typeOfAttttack ,int damageReceived)
    {
        double newDamage = 1.0;
        
        if (typeOfAttttack == Attack.TypeDamage.Physical)
        {
            switch (armure)
            {
                case TypeOfArmor.Leather:
                    newDamage = 0.85;
                    break;
                case TypeOfArmor.Mesh:
                    newDamage = 0.70;
                    break;
                case TypeOfArmor.Plates:
                    newDamage = 0.55;
                    break;
            }
        } else if (typeOfAttttack == Attack.TypeDamage.Magic)
        {
            switch (armure)
            {
                case TypeOfArmor.Fabric:
                    newDamage = 0.70;
                    break;
                case TypeOfArmor.Leather:
                    newDamage = 0.80;
                    break;
                case TypeOfArmor.Mesh:
                    newDamage = 0.90;
                    break;
            }
        }
        return (int)(damageReceived * newDamage);
    }

    public abstract void ChoiceAction();
}