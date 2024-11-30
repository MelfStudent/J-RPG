namespace J_RPG.Services;

using Models;

public static class Utils
{
    public static int PromptChoice(List<string> options, string titled)
    {
        int result;
        bool isPromptValid;

        do
        {
            Console.WriteLine(titled);
            for (var i = 1; i < options.Count+1; i++)
            {
                Console.Write($"{i} - {options[i-1]}\n");
            }

            Console.Write("Choose: ");
            isPromptValid = int.TryParse(Console.ReadLine(), out result) && result >= 1 && result < options.Count+1;

            if (!isPromptValid)
            {
                Console.WriteLine("Invalid entry, please try again");
            }
        } while (!isPromptValid);

        return result; 
    }
    
    public static string PromptName(string titled)
    {
        string? result;
        bool isPromptValid;

        do
        {
            Console.WriteLine(titled);
            Console.Write("Choose: ");
            result = Console.ReadLine()?.Trim();
            isPromptValid = !string.IsNullOrEmpty(result);

            if (!isPromptValid)
            {
                Console.WriteLine("Invalid entry, please try again");
            }
        } while (!isPromptValid);

        return result ?? ""; 
    }
    
    public static Character CreatePlayer(string chosenName, int chosenClass)
    {
        switch (chosenClass)
        {
            case 1: return new Warrior(chosenName);
            case 2: return new Mage(chosenName, 100);
            case 3: return new Paladin(chosenName, 60);
            case 4: return new Thief(chosenName);
            default: throw new ArgumentException("Invalid class choice");
        }
    }
    
    public static void SwitchPlayers()
    {
        (Menu.CharacterWhoDefends, Menu.CharacterWhoAttacks) = (Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends);
    }
    
    public static void StartGame()
    {
        while (Menu.Player1.IsDead == false && Menu.Player2.IsDead == false)
        {
            Menu.CharacterWhoAttacks.ChoiceAction();
            Utils.SwitchPlayers();
        }

        Menu.EndGame();
    }
}