namespace J_RPG;

public class Paladin : Character
{
    private int AttackReductionNumber { get; set; }
    
    public Paladin(string name) : base(name, 95, 40, 40, TypeOfArmor.Mesh, 5, 10, 20)
    {
        AttackReductionNumber = 0;
    }
    
    protected override void Defend(Attack.TypeDamage typeOfAttack, int attackPower)
    {
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");
        base.Defend(typeOfAttack, attackPower);
    }
    
    private void CrusaderStrike()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses CRUSADER STRIKE!");
        Console.ResetColor();
        
        Attack attack = new Attack("Crusader Strike", Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends, PhysicalAttackPower, Attack.TypeDamage.Physical );
        Tackle(attack);
    }

    private void Judgement()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses JUDGEMENT!");
        Console.ResetColor();
        
        Attack attack = new Attack("Judgement", Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends, MagicAttackPower, Attack.TypeDamage.Physical );
        Tackle(attack);
    }
    
    private void BrightFlash()
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
        Console.WriteLine($"HP: {CurrentHitPoints}/{MaxHitPoints} | Physical Attack: {PhysicalAttackPower} | Magic Attack: {MagicAttackPower}");
        Console.WriteLine("Choose an action:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("1. Crusader Strike (a physical attack that deals 100% of physical attack power to the target)");
        Console.WriteLine("2. Judgement (a physical attack that deals 100% of magical attack power to the target)");
        Console.WriteLine("3. Bright flash (heals for 125% of your magic attack power)");
        Console.ResetColor();
        
        List<string> options = new() { "Crusader Strike", "Judgement", "Bright flash" };
        var choice = Utils.PromptChoice(options, "\nEnter a number corresponding to the desired action: ");
        
        switch (choice)
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