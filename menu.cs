namespace J_RPG;

public class Menu
{
    public static string Player1 { get; set; }
    public static string Player2 { get; set; }
    
    public static void PrintGameLaunch()
    {
        Console.Clear();
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
    }

    public static void PrintNavigationMenu()
    {
        Console.WriteLine("1. Started a game");
        Console.WriteLine("2. Quit the game");
        Console.WriteLine("Choice: ");
        
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
    
    public static void PrintClassChoiceMenu()
    {
        Console.WriteLine("\nChoose a class for the player 1");
        Player1 = Utils.PromptClassChoice();
        
        Console.WriteLine("Choose a class for the player 2");
        Player2 = Utils.PromptClassChoice();
        
        Console.WriteLine($"You have chosen class {Player1} for player 1 and class {Player2} for player 2");

        StartGame();
    }

    public static void StartGame()
    {
        Console.Clear();
        
    }
}