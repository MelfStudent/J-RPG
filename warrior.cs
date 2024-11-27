namespace J_RPG;

public class Warrior : Character
{
    public Warrior(string name) : base(100, 50, 0, TypeOfArmor.Plates, 5, 25, 10)
    {
        Name = name;
    }
    
    public void HeroicStrike(Character target)
    { 
        Tackle(target, "Heroic Strike", PhysicalAttackPower, "PhysicalAttack");
    }

    public void BattleCry(Character target)
    {
        Tackle(target, "Battle Cry", PhysicalAttackPower, "PhysicalAttack");
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
                HeroicStrike(Menu.CharacterWhoDefends);
                break;
            case 2:
                BattleCry(Menu.CharacterWhoDefends);
                break;
            
        }
    }
}