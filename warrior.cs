﻿namespace J_RPG;

public class Warrior : Character
{
    public Warrior(string name) : base(100, 50, 0, TypeOfArmor.Plates, 5, 25, 10)
    {
        Name = name;
    }

    public override void Defend(Attack.TypeDamage typeOfAttack, int attackPower)
    {
        Console.WriteLine("\n========== DEFENSE PHASE ==========");
        Console.WriteLine($"[{Name.ToUpper()}] is under attack!");
        
        int lifeBeforeDefense = CurrentHitPoints;
        base.Defend(typeOfAttack, attackPower);
        int lifeAfterDefense = CurrentHitPoints;
        
        if (typeOfAttack == Attack.TypeDamage.Physical)
        {
            if (LuckTest(25))
            {
                int damageReceived = lifeBeforeDefense - lifeAfterDefense;
                
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"[{Name.ToUpper()}] successfully counterattacked!");
                Console.ResetColor();
                
                Attack attack = new Attack("Heroic Strike", Menu.CharacterWhoDefends, Menu.CharacterWhoAttacks, damageReceived / 2, Attack.TypeDamage.Physical );
                CounterAttack(attack);
            }
        }
    }
    
    public void HeroicStrike()
    {
        Console.WriteLine("\n========== ACTION PHASE ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"[{Name.ToUpper()}] uses HEROIC STRIKE!");
        Console.ResetColor();
        
        Attack attack = new Attack("Heroic Strike", Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends, PhysicalAttackPower, Attack.TypeDamage.Physical );
        Tackle(attack);
    }

    public void BattleCry()
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
        Console.WriteLine("Choose an action:");
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("1. Heroic Strike (a physical attack that deals 100% of physical attack power to the target)");
        Console.WriteLine("2. Battle Cry (multiplies the warrior's attack power by 2)");
        Console.ResetColor();
        
        string[] options = { "Heroic Strike", "Battle Cry" };
        int Choise = Utils.PromptChoice(options);
        
        switch (Choise)
        {
            case 1:
                HeroicStrike();
                break;
            case 2:
                BattleCry();
                break;
        }
    }

    private void CounterAttack(Attack attack)
    {
        if (attack.AttackingCharacter.CurrentHitPoints - attack.Damage > 0)
        {
            attack.AttackingCharacter.CurrentHitPoints -= attack.Damage;
            Console.WriteLine($"{attack.TargetCharacter.Name} counterattacked by inflicting {attack.Damage} damage to candy with a physical attack");
            return;
        }
        Console.WriteLine($"{attack.AttackingCharacter.Name} died following a deadly counterattack from {attack.TargetCharacter.Name}");
        attack.AttackingCharacter.IsDead = true;
    }
}