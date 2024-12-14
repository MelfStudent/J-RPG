namespace J_RPG.Models;

using Services;

public class Thief : Character
{
    private int AttackReductionNumber { get; set; }
    
    public Thief(string name) : base(name, 80, 55, 0, TypeOfArmor.Leather, 15, 25, 25, 100)
    {
        AttackReductionNumber = 0;
    }
    
    protected override void Defend(TypeDamage typeOfAttack, int attackPower)
    {
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");
        base.Defend(typeOfAttack, attackPower);
    }
    
    private void LowBlow()
    {
        /*Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses LOW BLOW!");
        Console.ResetColor();

        var newPhysicalAttackPower = PhysicalAttackPower;

        if (Menu.CharacterWhoDefends.CurrentHitPoints < Menu.CharacterWhoDefends.MaxHitPoints / 2)
        {
            newPhysicalAttackPower = (int)(PhysicalAttackPower * 1.50);
        }
        
        var attack = new Attack("Low Blow", Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends, newPhysicalAttackPower, Attack.TypeDamage.Physical );
        Tackle(attack);*/
    }

    private void Escape()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses ESCAPE!");
        Console.ResetColor();

        var newDodgeChance = 50;
        var newChanceSpellResistance = 50;

        if (DodgeChance + 20 <= 50)
        {
            newDodgeChance += DodgeChance + 20;
        } else if (ChanceSpellResistance + 20 <= 50)
        {
            newChanceSpellResistance += ChanceSpellResistance + 20;
        }
        DodgeChance = newDodgeChance;
        ChanceSpellResistance = newChanceSpellResistance;
    }

    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: THIEF)");
        Console.WriteLine($"HP: {CurrentHitPoints}/{MaxHitPoints} | Physical Attack: {PhysicalAttackPower} | Magic Attack: {MagicAttackPower}");
        Console.WriteLine("Choose an action:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("1. Low Blow ()");
        Console.WriteLine("2. Escape ()");
        Console.ResetColor();
        
        List<string> options = new() { "Low Blow", "Escape" };
        var choice = Utils.PromptChoice(options, "\nEnter a number corresponding to the desired action: ");
        
        switch (choice)
        {
            case 1:
                LowBlow();
                break;
            case 2:
                Escape();
                break;
        }
    }
}