namespace J_RPG.Services;

using System.Text.Json;
using System.IO;

using Models;
using Enums;

/// <summary>
/// A static class responsible for handling various game menu interactions, such as displaying the main menu, 
/// character creation, and editing character configurations. This class also manages teams and game flow.
/// </summary>
public static class Menu
{
    /// <summary>
    /// List of characters for Player 1.
    /// </summary>
    public static List<Character>? Player1 { get; private set; }

    /// <summary>
    /// List of characters for Player 2.
    /// </summary>
    public static List<Character>? Player2 { get; private set; }

    /// <summary>
    /// List of skills used during the current turn.
    /// </summary>
    public static List<SkillUsage> SkillsTourCurrent { get; set; } = new List<SkillUsage>();

    /// <summary>
    /// The team that is currently attacking.
    /// </summary>
    public static Team? TeamThatAttacks { get; set; }

    /// <summary>
    /// The team that is currently defending.
    /// </summary>
    public static Team? TeamThatDefends { get; set; }

    /// <summary>
    /// List of teams in the game.
    /// </summary>
    public static List<Team> Teams { get; set; } = new List<Team>();

    /// <summary>
    /// Prints the game's launch screen with an ASCII art.
    /// </summary>
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

    /// <summary>
    /// Displays the main navigation menu and prompts the user to choose an option.
    /// </summary>
    public static void PrintNavigationMenu()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("\n========== WELCOME TO JRPG! ==========");
            List<string> existingOptions = new() { "Start a new game", "Edit configuration", "Quit the game\n"};
            var enter = Utils.PromptChoice(existingOptions, "Choose an option to begin:");
            Console.WriteLine("=======================================");
        
            switch (enter)
            {
                case 1:
                    PrintClassChoiceMenu();
                    break;
                case 2:
                    EditConfig();
                    break;
                case 3:
                    Console.WriteLine("Game stopped! Thank you and see you soon.");
                    Environment.Exit(0);
                    break;
            }   
        }
    }
    
    /// <summary>
    /// Displays the character creation menu, allowing players to choose characters for their teams.
    /// </summary>
    private static void PrintClassChoiceMenu()
    {
        Console.WriteLine("\n========== CHARACTER CREATION ==========");
        Console.ForegroundColor = ConsoleColor.Blue;
        Player1 = Utils.PromptTeam("\n\n\nCharacter selection team n°1");
        var team1 = new Team("Player 1");
        foreach (var player in Player1)
        {
            team1.AddMember(player);
        }
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Player2 = Utils.PromptTeam("\n\n\nCharacter selection team n°2");
        var team2 = new Team("Player 2");
        foreach (var player in Player2)
        {
            team2.AddMember(player);
        }
        
        Teams.Add(team1);
        Teams.Add(team2);

        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine("\n========== READY CHARACTERS TEAM 1 ==========");
        DisplayCharacterStats(Player1, "Player 1");
        
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("\n========== READY CHARACTERS TEAM 2 ==========");
        DisplayCharacterStats(Player2, "Player 2");
        Console.ResetColor();
        
        TeamThatAttacks = Teams[0];;
        TeamThatDefends = Teams[1];;
        

        Console.WriteLine("\n========================================");
        Console.WriteLine("Press any key to start the battle...");
        Console.ReadKey();
        Utils.StartGame();
    }
    
    /// <summary>
    /// Displays the stats of the characters in a given team.
    /// </summary>
    private static void DisplayCharacterStats(List<Character>? characters, string playerName)
    {
        if (characters == null) return;
        
        foreach (var character in characters)
        {
            Console.WriteLine($"{playerName}: {character.Name} ({character.GetType().Name})");
            Console.WriteLine(character.ToString());
        }
    }
    
    /// <summary>
    /// Returns the armor resistance percentage based on the type of armor.
    /// </summary>
    public static string GetArmorPercentage(TypeOfArmor armor)
    {
        return armor switch
        {
            TypeOfArmor.Fabric => "30% vs Magic",
            TypeOfArmor.Leather => "15% vs Physical, 20% vs Magic",
            TypeOfArmor.Mesh => "30% vs Physical, 10% vs Magic",
            TypeOfArmor.Plates => "45% vs Physical",
            _ => "No resistance"
        };
    }
    
    /// <summary>
    /// Resets the game state (e.g., clearing players, teams, skills).
    /// </summary>
    private static void ResetGame()
    {
        Player1 = null;
        Player2 = null;
        SkillsTourCurrent.Clear();
        Teams.Clear();
        TeamThatAttacks = null;
        TeamThatDefends = null;
        Utils.UsedNames.Clear();
    }

    /// <summary>
    /// Ends the game and displays a game-over message. After a delay, the main menu is displayed again.
    /// </summary>
    public static void EndGame(string message)
    {
        Console.WriteLine("\n========== GAME OVER ==========");
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine(@$"
        ****************************************************
        *                                                  *
        *      {message}         *
        *                                                  *
        ****************************************************

                    O  
                   /|\ 
                   / \ 
            ---------------------
           /                     \
          /    {message}  \
         /_________________________\
        ");
        Console.ResetColor();
        
        Console.WriteLine("Returning to main menu in...");
        for (var i = 10; i > 0; i--)
        {
            Console.Write($"\r{i} seconds remaining");
            Thread.Sleep(1000);
        }
        Console.WriteLine();
        ResetGame();
        PrintNavigationMenu();
    }
    
    /// <summary>
    /// Allows the player to edit the configuration of characters by modifying attributes such as hit points, armor, etc.
    /// The changes are saved back to the `classes.json` file.
    /// </summary>
    private static void EditConfig()
    {
        var basePath = AppDomain.CurrentDomain.BaseDirectory;
        var filePath = Path.Combine(basePath, "Resources", "classes.json");

        if (!File.Exists(filePath))
        {
            Console.WriteLine($"Configuration file not found: {filePath}");
            return;
        }

        var json = File.ReadAllText(filePath);
        var characters = JsonSerializer.Deserialize<Dictionary<string, ClassConfig>>(json);

        if (characters == null || characters.Count == 0)
        {
            Console.WriteLine("No characters found in the configuration.");
            return;
        }

        while (true)
        {
            Console.WriteLine("\n========== EDIT CHARACTER CONFIG ==========");
            var characterNames = characters.Keys.ToList();
            for (var i = 0; i < characterNames.Count; i++)
            {
                Console.WriteLine($"{i + 1}. {characterNames[i]}");
            }
            Console.WriteLine($"{characterNames.Count + 1}. Return to main menu");

            Console.Write("Choose a character to edit: ");
            if (!int.TryParse(Console.ReadLine(), out var charChoice) || charChoice < 1 || charChoice > characterNames.Count + 1)
            {
                Console.WriteLine("Invalid choice. Please try again.");
                continue;
            }

            if (charChoice == characterNames.Count + 1)
            {
                Console.WriteLine("Returning to main menu...");
                break;
            }

            var selectedCharacter = characterNames[charChoice - 1];
            var characterConfig = characters[selectedCharacter];

            while (true)
            {
                Console.WriteLine($"\nEditing: {selectedCharacter}");
                var properties = typeof(ClassConfig).GetProperties();
                for (var i = 0; i < properties.Length; i++)
                {
                    var value = properties[i].GetValue(characterConfig);
                    Console.WriteLine($"{i + 1}. {properties[i].Name}: {value}");
                }
                Console.WriteLine($"{properties.Length + 1}. Back to character selection");

                Console.Write("Choose a property to edit: ");
                if (!int.TryParse(Console.ReadLine(), out var propChoice) || propChoice < 1 || propChoice > properties.Length + 1)
                {
                    Console.WriteLine("Invalid choice. Please try again.");
                    continue;
                }

                if (propChoice == properties.Length + 1)
                {
                    Console.WriteLine("Returning to character selection...");
                    break;
                }

                var selectedProperty = properties[propChoice - 1];

                Console.WriteLine(selectedProperty.PropertyType.Name);
                if (selectedProperty.Name == "Armor")
                {
                    Console.WriteLine("Choose an armor type (Fabric, Leather, Mesh, Plates): ");
                    while (true)
                    {
                        var input = Console.ReadLine();
                        if (Enum.TryParse(input, true, out TypeOfArmor armor) &&
                            Enum.IsDefined(typeof(TypeOfArmor), armor))
                        {
                            selectedProperty.SetValue(characterConfig, armor.ToString());
                            Console.WriteLine($"{selectedProperty.Name} updated to {armor}.");
                            break;
                        }
                        Console.WriteLine("Invalid armor type. Please choose between Fabric, Leather, Mesh, Plates:");
                    }
                }
                else if (selectedProperty.PropertyType == typeof(int) || selectedProperty.PropertyType == typeof(float))
                {
                    Console.Write($"Enter a new non-negative value for {selectedProperty.Name}: ");
                    while (true)
                    {
                        var input = Console.ReadLine();
                        if (float.TryParse(input, out var numericValue) && numericValue >= 0)
                        {
                            var convertedValue = Convert.ChangeType(numericValue, selectedProperty.PropertyType);
                            selectedProperty.SetValue(characterConfig, convertedValue);
                            Console.WriteLine($"{selectedProperty.Name} updated to {convertedValue}.");
                            break;
                        }
                        Console.WriteLine("Invalid value. Please enter a non-negative number:");
                    }
                }
                else
                {
                    Console.Write($"Enter new value for {selectedProperty.Name}: ");
                    var newValue = Console.ReadLine();
                    selectedProperty.SetValue(characterConfig, newValue);
                    Console.WriteLine($"{selectedProperty.Name} updated to {newValue}.");
                }

                characters[selectedCharacter] = characterConfig;
                File.WriteAllText(filePath, JsonSerializer.Serialize(characters, new JsonSerializerOptions { WriteIndented = true }));
                Console.WriteLine("Changes saved successfully.");
            }
        }
    }
}
