namespace J_RPG.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using Models;
using Enums;

/// <summary>
/// A static utility class responsible for various game-related helper functions, such as user input prompts, team management,
/// character creation, game flow control, and skill execution.
/// </summary>
public static class Utils
{
    /// <summary>
    /// A set to track the names of all used characters.
    /// </summary>
    public static HashSet<string> UsedNames { get; } = new();

    /// <summary>
    /// The size of each team in the game.
    /// </summary>
    private static int _teamSize { get; set; } = 3;

    /// <summary>
    /// Prompts the user to choose an option from a list of strings and returns the chosen index (1-based).
    /// </summary>
    /// <param name="options">The list of available options for the user to choose from.</param>
    /// <param name="titled">The title or message to display above the options list.</param>
    /// <returns>The index of the selected option.</returns>
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
    
    /// <summary>
    /// Prompts the user to input a valid character name, ensuring it is unique.
    /// </summary>
    /// <param name="titled">The title or message to display when asking for the name.</param>
    /// <returns>The valid character name entered by the user.</returns>
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
    
    /// <summary>
    /// Prompts the user to create a team by selecting character names and classes.
    /// </summary>
    /// <param name="titled">The title or message to display when asking for team creation details.</param>
    /// <returns>A list of created characters for the team.</returns>
    public static List<Character> PromptTeam(string titled)
    {
        var result = new List<Character>();
        List<string> existingCharacterClass = new() { "Warrior", "Mage", "Paladin", "Thief", "Priest\n" };

        Console.WriteLine(titled);
        for (var i = 1; i <= _teamSize; i++)
        {
            var choiceCharacterName = PromptName($"\nEnter the character name n°{i} :");
            Console.Write($"\nChoose a class for the player {i}: \n");
            var choiceCharacterClass = PromptChoice(existingCharacterClass, "Enter a number corresponding to a class: ");
            result.Add(CreatePlayer(choiceCharacterName, choiceCharacterClass));
        }
        
        return result;
    }
    
    /// <summary>
    /// Prompts the user to choose a target from a team, ensuring the actor does not target themselves or dead characters.
    /// </summary>
    /// <param name="titled">The title or message to display when asking for target selection.</param>
    /// <param name="team">The team from which the target is to be chosen.</param>
    /// <param name="actor">The character performing the action (cannot target themselves).</param>
    /// <returns>The selected target character.</returns>
    public static Character PromptTarget(string titled, Team team, Character actor)
    {
        int result;
        bool isPromptValid;

        do
        {
            Console.WriteLine(titled);
            for (var i = 1; i <= team.Members.Count; i++)
            {
                if (!team.Members[i - 1].IsDead && team.Members[i - 1] != actor)
                {
                    Console.Write($"{i} - {team.Members[i - 1].Name}\n");
                }
            }

            Console.Write("Choose: ");
            isPromptValid = int.TryParse(Console.ReadLine(), out result) && result >= 1 && result <= team.Members.Count && !team.Members[result - 1].IsDead && team.Members[result - 1] != actor;

            if (!isPromptValid)
            {
                Console.WriteLine("Invalid entry or the selected character is not alive. Please try again.");
            }
        } while (!isPromptValid);

        return team.Members[result-1]; 
    }
    
    /// <summary>
    /// Creates a player character based on the chosen name and class index.
    /// </summary>
    /// <param name="chosenName">The name of the character.</param>
    /// <param name="chosenClass">The class index chosen for the character (1-5).</param>
    /// <returns>The newly created character of the selected class.</returns>
    private static Character CreatePlayer(string chosenName, int chosenClass)
    {
        var classNames = new[] { "Warrior", "Mage", "Paladin", "Thief", "Priest" };
        var className = classNames[chosenClass - 1];
        var config = ConfigLoader.GetConfig(className);

        return chosenClass switch
        {
            1 => new Warrior(chosenName, config.MaxHitPoints, config.PhysicalAttackPower, config.MagicAttackPower,Enum.Parse<TypeOfArmor>(config.Armor!), config.DodgeChance, config.ParadeChance, config.ChanceSpellResistance, config.Speed),
            2 => new Mage(chosenName, config.MaxHitPoints, config.PhysicalAttackPower, config.MagicAttackPower,Enum.Parse<TypeOfArmor>(config.Armor!), config.DodgeChance, config.ParadeChance, config.ChanceSpellResistance, config.Speed, config.HasMana, config.ManaPoints),
            3 => new Paladin(chosenName, config.MaxHitPoints, config.PhysicalAttackPower, config.MagicAttackPower,Enum.Parse<TypeOfArmor>(config.Armor!), config.DodgeChance, config.ParadeChance, config.ChanceSpellResistance, config.Speed, config.HasMana, config.ManaPoints),
            4 => new Thief(chosenName, config.MaxHitPoints, config.PhysicalAttackPower, config.MagicAttackPower,Enum.Parse<TypeOfArmor>(config.Armor!), config.DodgeChance, config.ParadeChance, config.ChanceSpellResistance, config.Speed),
            5 => new Priest(chosenName, config.MaxHitPoints, config.PhysicalAttackPower, config.MagicAttackPower,Enum.Parse<TypeOfArmor>(config.Armor!), config.DodgeChance, config.ParadeChance, config.ChanceSpellResistance, config.Speed, config.HasMana, config.ManaPoints),
            _ => throw new ArgumentException("Invalid class choice"),
        };
    }
    
    /// <summary>
    /// Starts the game loop, where actions are chosen and executed in turn-based gameplay.
    /// </summary>
    public static void StartGame()
    {
        while (true)
        {
            ChoiceActions();
            ExecutionOfAttacks();
            Menu.SkillsTourCurrent = new List<SkillUsage>();

            if (Menu.Teams[0].NumberPeopleAlive() == 0)
            {
                Menu.EndGame("Player 2 wins!");
                break;
            }

            if (Menu.Teams[1].NumberPeopleAlive() == 0)
            {
                Menu.EndGame("Player 1 wins!");
                break;
            }
        }
    }

    /// <summary>
    /// Prompts each player in the attacking team to choose an action.
    /// </summary>
    private static void ChoiceActions()
    {
        foreach (var player in Menu.TeamThatAttacks!.Members)
        {
            if (!player.IsDead)
            {
                player.ChoiceAction();   
            }
        }

        Team.SwitchPlayers();
        foreach (var player in Menu.TeamThatAttacks.Members)
        {
            if (!player.IsDead)
            {
                player.ChoiceAction();   
            }
        }
    }

    /// <summary>
    /// Executes attacks for each player in the turn order, considering mana and skill usage.
    /// </summary>
    private static void ExecutionOfAttacks()
    {
        var combinedTeam = Menu.Player1!.Concat(Menu.Player2!).ToList();
        var attackOrder = ExecutionSpeedCalculation(combinedTeam);
        
        foreach (var player in attackOrder)
        {
            var skillUsage = Menu.SkillsTourCurrent.FirstOrDefault(su => su.User == player);
            var skill = skillUsage!.ChosenSkill;
            
            if (skill != null!)
            {
                if (player.UsesMana && player.CurrentMana < skill.ManaCost) 
                {
                    Console.WriteLine($"{player.Name} failed to cast {skill.Name} due to insufficient mana and passes their turn!");
                }
                else
                {
                    skill.UseSkill(player, skillUsage.Target); 
                }
            }
            else
            {
                Console.WriteLine("\n========== ATTACK PHASE ==========");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"{player.Name} has passed his turn");
                Console.ResetColor();
                Console.WriteLine("===================================\n");
            }
        }
        CooldownReductionAllTeams();
    }

    /// <summary>
    /// Calculates the order of characters' actions based on their speed.
    /// </summary>
    private static List<Character> ExecutionSpeedCalculation(List<Character> characters)
    {
        var random = new Random();
        var shuffledCharacters  = characters.OrderBy(_ => random.Next()).ToList();
        var sortedCharacters = shuffledCharacters.OrderByDescending(character  => character.Speed).ToList();

        return sortedCharacters;
    }

    /// <summary>
    /// Reduces the cooldowns for all characters in all teams.
    /// </summary>
    private static void CooldownReductionAllTeams()
    {
        foreach (var team in Menu.Teams)
        {
            foreach (var character in team.Members)
            {
                character.ReduceCooldowns();
            }
        }
    }

    public static void LogError(string text)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(text);
        Console.ResetColor();
    }
}
