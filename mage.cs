namespace J_RPG;

public class Mage : Character
{
    private int AttackReductionNumber { get; set; }
    
    public Mage(string name) : base(60, 0, 75, TypeOfArmor.Fabric, 5, 5, 25)
    {
        Name = name;
        AttackReductionNumber = 0;
    }
    
    public override void Defend(Attack.TypeDamage typeOfAttack, int attackPower)
    {
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");

        if (AttackReductionNumber > 0)
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine($"{Name} is protected by FROST BARRIER!");
            Console.ResetColor();

            if (typeOfAttack == Attack.TypeDamage.Physical)
            {
                attackPower = (int)(attackPower * 0.40);
            } else if (typeOfAttack == Attack.TypeDamage.Magic)
            {
                attackPower = (int)(attackPower * 0.50);
            }

            AttackReductionNumber--;
        }
        
        base.Defend(typeOfAttack, attackPower);
    }
    
    public void Frostbolt()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses FROST BOLT!");
        Console.ResetColor();
        
        Attack attack = new Attack("Frostbolt", Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends, MagicAttackPower, Attack.TypeDamage.Magic );
        Tackle(attack);
    }

    public void FrostBarrier()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{Name.ToUpper()}] shouts a FROST BARRIER!");
        Console.WriteLine("The next two attacks will be reduced:");
        Console.WriteLine("- Physical damage reduced by 60%");
        Console.WriteLine("- Magical damage reduced by 50%");
        Console.ResetColor();
        AttackReductionNumber = 2;
        Console.WriteLine("===================================\n");
    }

    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: MAGE)");
        Console.WriteLine($"HP: {CurrentHitPoints}/{MaxHitPoints} | Physical Attack: {PhysicalAttackPower} | Magic Attack: {MagicAttackPower}");
        Console.WriteLine("Choose an action:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("1. Frostbolt (a magical attack that deals 100% of magical attack power to the target)");
        Console.WriteLine("2. Frost barrier (reduces damage from the next two attacks received)");
        Console.ResetColor();
        
        string[] options = { "Frostbolt", "Frost barrier" };
        int Choise = Utils.PromptChoice(options);
        
        switch (Choise)
        {
            case 1:
                Frostbolt();
                break;
            case 2:
                FrostBarrier();
                break;
        }
    }
}