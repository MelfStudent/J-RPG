namespace J_RPG.Services;

using Models;

public static class Utils
{
    private static HashSet<string> UsedNames = new();
    
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
    
    private static string PromptName(string titled)
    {
        string? result;
        bool isPromptValid;

        do
        {
            Console.WriteLine(titled);
            Console.Write("Choose: ");
            result = Console.ReadLine()?.Trim();
            if (string.IsNullOrEmpty(result))
            {
                Console.WriteLine("Invalid name. Please try again.");
                isPromptValid = false;
            }
            else if (UsedNames.Contains(result))
            {
                Console.WriteLine($"The name '{result}' is already used.");
                isPromptValid = false;
            }
            else
            {
                isPromptValid = true;
            }
        } while (!isPromptValid);

        UsedNames.Add(result!);
        return result!; 
    }
    
    public static List<Character> PromptTeam(string titled)
    {
        var result = new List<Character>();
        List<string> existingCharacterClass = new() { "Warrior", "Mage", "Paladin", "Thief\n" };

        Console.WriteLine(titled);
        for (var i = 1; i < 4; i++)
        {
            var choiceCharacterName = PromptName($"\nEnter the character name n°{i} :");
            Console.Write($"\nChoose a class for the player {i}: \n");
            var choiceCharacterClass = PromptChoice(existingCharacterClass, "Enter a number corresponding to a class: ");
            result.Add(CreatePlayer(choiceCharacterName, choiceCharacterClass));
        }
        
        return result;
    }
    
    private static Character CreatePlayer(string chosenName, int chosenClass)
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
    
    private static void SwitchPlayers()
    {
        (Menu.CharacterWhoDefends, Menu.CharacterWhoAttacks) = (Menu.CharacterWhoAttacks, Menu.CharacterWhoDefends);
    }
    
    public static void StartGame()
    {
        while (true)
        {
            Menu.CharacterWhoAttacks.ChoiceAction();
            Utils.SwitchPlayers();
        }

        Menu.EndGame();
    }
}