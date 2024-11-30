﻿namespace J_RPG.Models;

using Services;

public class Warrior : Character
{
    public Warrior(string name) : base(name, 100, 50, 0, TypeOfArmor.Plates, 5, 25, 10) {}

    protected override void Defend(Attack.TypeDamage typeOfAttack, int attackPower)
    {
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");
        
        var lifeBeforeDefense = CurrentHitPoints;
        base.Defend(typeOfAttack, attackPower);
        var lifeAfterDefense = CurrentHitPoints;

        switch (typeOfAttack)
        {
            case Attack.TypeDamage.Physical:
                if (LuckTest(25))
                {
                    var damageReceived = lifeBeforeDefense - lifeAfterDefense;
                
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine($"[{Name.ToUpper()}] successfully counterattacked!");
                    Console.ResetColor();
                
                    var attack = new Attack("Heroic Strike", Menu.CharacterWhoDefends, Menu.CharacterWhoAttacks, damageReceived / 2, Attack.TypeDamage.Physical );
                    Tackle(attack);
                }
                break;
        }
    }
    
    private void HeroicStrike()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses HEROIC STRIKE!");
        Console.ResetColor();
        
        var attack = new Attack("Heroic Strike", Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends, PhysicalAttackPower, Attack.TypeDamage.Physical );
        Tackle(attack);
    }

    private void BattleCry()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine($"[{Name.ToUpper()}] shouts a BATTLE CRY!");
        Console.ResetColor();
        PhysicalAttackPower *= 2;
        Console.WriteLine($"{Name} now deals {PhysicalAttackPower} damage, because his damage has just been multiplied by two for the next hits.");
        Console.WriteLine("===================================\n");
    }

    public override void ChoiceAction()
    {
        Console.WriteLine("\n========== ACTION SELECTION ==========");
        Console.WriteLine($"Player: {Name.ToUpper()} (CLASS: WARRIOR)");
        Console.WriteLine($"HP: {CurrentHitPoints}/{MaxHitPoints} | Physical Attack: {PhysicalAttackPower} | Magic Attack: {MagicAttackPower}");
        Console.WriteLine("Choose an action:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("1. Heroic Strike (a physical attack that deals 100% of physical attack power to the target)");
        Console.WriteLine("2. Battle Cry (multiplies the warrior's attack power by 2)");
        Console.ResetColor();
        
        List<string> options = new() { "Heroic Strike", "Battle Cry" };
        var choice = Utils.PromptChoice(options, "\nEnter a number corresponding to the desired action: ");
        
        switch (choice)
        {
            case 1:
                HeroicStrike();
                break;
            case 2:
                BattleCry();
                break;
        }
    }
}