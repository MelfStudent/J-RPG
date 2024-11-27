namespace J_RPG;

public class Warrior : Character
{
    public Warrior(string name) : base(100, 50, 0, TypeOfArmor.Plates, 5, 25, 10)
    {
        Name = name;
    }

    public override void Defend(Attack.TypeDamage typeOfAttack, int attackPower)
    {
        int lifeBeforeDefense = CurrentHitPoints;
        base.Defend(typeOfAttack, attackPower);
        int lifeAfterDefense = CurrentHitPoints;
        if (typeOfAttack == Attack.TypeDamage.Physical)
        {
            if (LuckTest(25))
            {
                int damageReceived = lifeBeforeDefense - lifeAfterDefense;
                Attack attack = new Attack("Heroic Strike", Menu.CharacterWhoDefends, Menu.CharacterWhoAttacks, damageReceived / 2, Attack.TypeDamage.Physical );
                CounterAttack(attack);
            }
        }
    }
    
    public void HeroicStrike()
    {
        Attack attack = new Attack("Heroic Strike", Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends, PhysicalAttackPower, Attack.TypeDamage.Physical );
        Tackle(attack);
    }

    public void BattleCry()
    {
        PhysicalAttackPower *= 2;
        Console.WriteLine($"{Name} now deals {PhysicalAttackPower} damage, because his damage has just been multiplied by two for the next hits.");
    }

    public override void ChoiceAction()
    {
        Console.WriteLine($"\n\nPlayer: {Name} (WARRIOR)");
        Console.WriteLine("Choose an action");
        Console.WriteLine("1. Heroic Strike (a physical attack that deals 100% of physical attack power to the target)");
        Console.WriteLine("2. Battle Cry (multiplies the warrior's attack power by 2)");
        
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