namespace J_RPG;

public class Character
{
    public string Name { get; set; }
    public int CurrentHitPoints { get; set; }
    public int MaxHitPoints { get; set; }
    public int PhysicalAttackPower  { get; set; }
    public int MagicAttackPower  { get; set; }
    public enum TypeOfArmor { Fabric, Leather, Mesh, Plates }
    public int DodgeChance { get; set; }
    public int ParadeChance { get; set; }
    public int ChanceSpellResistance { get; set; }
    
    public Character(string name, int currentHitPoints, int maxHitPoints, int physicalAttackPower, int magicAttackPower, int dodgeChance, int paradeChance, int chanceSpellResistance)
    {
        Name = name;
        CurrentHitPoints = currentHitPoints;
        MaxHitPoints = maxHitPoints;
        PhysicalAttackPower = physicalAttackPower;
        MagicAttackPower = magicAttackPower;
        DodgeChance = dodgeChance;
        ParadeChance = paradeChance;
        ChanceSpellResistance = chanceSpellResistance;
    }

    public void Tackle(Character target)
    {
        
    }

    public void Defend(string typeOfAttttack, int attackPower)
    {
        int damage = 0;
        if (typeOfAttttack == "PhysicalAttack")
        {
            if (LuckTest(DodgeChance))
            {
                Console.WriteLine($"The {Name} character dodged the attack !");
            }
            else if (LuckTest(ParadeChance))
            {
                damage = attackPower / 2;
                damage = GetArmorResistance(TypeOfArmor.Leather, typeOfAttttack, damage);
            }
        } else if (typeOfAttttack == "MagicAttack")
        {
            if (LuckTest(ChanceSpellResistance))
            {
                Console.WriteLine($"The {Name} character resisted the attack !");
            }
            else
            {
                damage = GetArmorResistance(TypeOfArmor.Leather, typeOfAttttack, damage);
            }
        }
        
        CurrentHitPoints -= damage;
    }

    public void Heal()
    {
        
    }

    private bool LuckTest(int percentage)
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

    private int GetArmorResistance(TypeOfArmor armure, string typeOfAttttack ,int damageReceived)
    {
        int newDamage;
        if (typeOfAttttack == "PhysicalAttack")
        {
            switch (armure)
            {
                case TypeOfArmor.Leather:
                    newDamage = (damageReceived / 100) * 85;
                    break;
                case TypeOfArmor.Mesh:
                    newDamage = (damageReceived / 100) * 70;
                    break;
                case TypeOfArmor.Plates:
                    newDamage = (damageReceived / 100) * 55;
                    break;
                default:
                    newDamage = damageReceived;
                    break;
            }
        } else if (typeOfAttttack == "MagicAttack")
        {
            switch (armure)
            {
                case TypeOfArmor.Fabric:
                    newDamage = (damageReceived / 100) * 70;
                    break;
                case TypeOfArmor.Leather:
                    newDamage = (damageReceived / 100) * 80;
                    break;
                case TypeOfArmor.Mesh:
                    newDamage = (damageReceived / 100) * 90;
                    break;
                case TypeOfArmor.Plates:
                    newDamage = damageReceived;
                    break;
                default:
                    newDamage = damageReceived;
                    break;
            }
        }
        else
        {
            newDamage = damageReceived;
        }

        return newDamage;
    }
}