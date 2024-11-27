namespace J_RPG;

public class Warrior : Character
{
    public Warrior(string name) : base(100, 50, 0, TypeOfArmor.Plates, 5, 25, 10)
    {
        Name = name;
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
}