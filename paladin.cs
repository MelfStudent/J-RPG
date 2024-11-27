namespace J_RPG;

public class Paladin : Character
{
    private int AttackReductionNumber { get; set; }
    
    public Paladin(string name) : base(95, 40, 40, TypeOfArmor.Mesh, 5, 10, 20)
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
    
    public void CrusaderStrike()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses CRUSADER STRIKE!");
        Console.ResetColor();
        
        Attack attack = new Attack("Crusader Strike", Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends, PhysicalAttackPower, Attack.TypeDamage.Physical );
        Tackle(attack);
    }

    public void Judgement()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses JUDGEMENT!");
        Console.ResetColor();
        
        Attack attack = new Attack("Judgement", Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends, MagicAttackPower, Attack.TypeDamage.Physical );
        Tackle(attack);
    }
    
    public void BrightFlash()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses BRIGHT-FLASH!");
        Console.ResetColor();
        
        Heal((int)(MagicAttackPower * 1.25));
    }

    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: PALADIN)");
        Console.WriteLine("Choose an action:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("1. Crusader Strike (a physical attack that deals 100% of physical attack power to the target)");
        Console.WriteLine("2. Judgement (a physical attack that deals 100% of magical attack power to the target)");
        Console.WriteLine("3. Bright flash (heals for 125% of your magic attack power)");
        Console.ResetColor();
        
        string[] options = { "Crusader Strike", "Judgement", "Bright flash" };
        int Choise = Utils.PromptChoice(options);
        
        switch (Choise)
        {
            case 1:
                CrusaderStrike();
                break;
            case 2:
                Judgement();
                break;
            case 3:
                BrightFlash();
                break;
        }
    }
}