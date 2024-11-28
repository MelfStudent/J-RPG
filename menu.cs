namespace J_RPG;

public class Menu
{
    public static Character Player1 { get; set; }
    public static Character Player2 { get; set; }
    public static Character CharacterWhoAttacks { get; set; }
    public static Character CharacterWhoDefends { get; set; }
    
    public static void PrintGameLaunch()
    {
        Console.Clear();
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(@"
                          _______________
                         |###############|
                         |### JRPG ###   |
                         |###############|
                         |_______________|
                            /         \
              (O)          /           \         (O)
           __/||\__       |   ENEMY!    |      __/||\__
             ||             \           /          ||
            /  \             \_________/          /  \
          __|  |__                             __|  |__
         /        \                           /        \
        / H E R O \_________________________/  ATTACK! \
       |___________|                       |____________|
                  ");
        Console.WriteLine("Prepare for an epic battle!");
        Console.ResetColor();
    }

    public static void PrintNavigationMenu()
    {
        Console.WriteLine("1. Started a game");
        Console.WriteLine("2. Quit the game");
        Console.Write("Choice: ");
        
        string enter = Console.ReadLine();
        
        int enter2 = int.Parse(enter);
        switch (enter2)
        {
            case 1:
                PrintClassChoiceMenu();
                break;
            case 2:
                Console.WriteLine("Game stopped! Thank you and see you soon.");
                break;
        }
    }
    
    public static Character CreatePlayer(string chosenName, int chosenClass)
    {
        switch (chosenClass)
        {
            case 1: return new Warrior(chosenName);
            case 2: return new Mage(chosenName);
            case 3: return new Paladin(chosenName);
            case 4: return new Thief(chosenName);
            default: throw new ArgumentException("Invalid class choice");
        }
    }
    
    public static void PrintClassChoiceMenu()
    {
        Console.WriteLine("\nChoose the name of the first character: ");
        string choiceCharacterName1 = Console.ReadLine();
        
        Console.Write("Choose a class for the player 1: ");
        int choiceCharacterClass1 = Utils.PromptClassChoice();
        Player1 = CreatePlayer(choiceCharacterName1, choiceCharacterClass1);
        
        Console.WriteLine("\nChoose the name of the second character: ");
        string choiceCharacterName2 = Console.ReadLine();
        
        Console.Write("Choose a class for the player 2: ");
        int choiceCharacterClass2 = Utils.PromptClassChoice();
        Player2 = CreatePlayer(choiceCharacterName2, choiceCharacterClass2);
        
        Console.WriteLine($"\nYou have chosen class {Player1.GetType().Name} for player 1 and class {Player2.GetType().Name} for player 2");
        CharacterWhoAttacks = Player1;
        CharacterWhoDefends = Player2;
        StartGame();
    }

    public static void SwitchPlayers()
    {
        Character temp = CharacterWhoAttacks;
        CharacterWhoAttacks = CharacterWhoDefends;
        CharacterWhoDefends = temp;
    }

    public static void EndGame()
    {
        Console.WriteLine("\n========== GAME OVER ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@$"
        ****************************************************
        *                                                  *
        *      CONGRATULATIONS, {CharacterWhoAttacks.Name.ToUpper()} !         *
        *                                                  *
        *        YOU HAVE EMERGED VICTORIOUS!              *
        *                                                  *
        ****************************************************

                    O  
                   /|\ 
                   / \ 
            ---------------------
           /                     \
          /    VICTORY IS YOURS!  \
         /_________________________\
        ");
        Console.ResetColor();
        
        Console.WriteLine("Returning to main menu in...");
        for (int i = 10; i > 0; i--)
        {
            Console.Write($"\r{i} seconds remaining");
            Thread.Sleep(1000);
        }
        Console.WriteLine();
        PrintNavigationMenu();
    }
    
    public static void StartGame()
    {
        //Console.Clear();
        while (Player1.IsDead == false && Player2.IsDead == false)
        {
            CharacterWhoAttacks.ChoiceAction();
            SwitchPlayers();
        }

        EndGame();
    }
}