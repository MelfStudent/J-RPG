namespace J_RPG.Services;

using System;
using System.Collections.Generic;
using System.Linq;

using Models;
using Enums;

public static class Utils
{
    public static HashSet<string> UsedNames { get; } = new();
    private static int _teamSize { get; set; } = 3;
    
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
    
    private static Character CreatePlayer(string chosenName, int chosenClass)
    {
        switch (chosenClass)
        {
            case 1:
                var warriorConfig = ClassConfigLoader.GetConfig("Warrior");
                return new Warrior(
                    chosenName,
                    warriorConfig.MaxHitPoints,
                    warriorConfig.PhysicalAttackPower,
                    warriorConfig.MagicAttackPower,
                    Enum.Parse<TypeOfArmor>(warriorConfig.Armor!),
                    warriorConfig.DodgeChance,
                    warriorConfig.ParadeChance,
                    warriorConfig.ChanceSpellResistance,
                    warriorConfig.Speed
                );
            
            case 2:
                var mageConfig = ClassConfigLoader.GetConfig("Mage");
                return new Mage(
                    chosenName,
                    mageConfig.MaxHitPoints,
                    mageConfig.PhysicalAttackPower,
                    mageConfig.MagicAttackPower,
                    Enum.Parse<TypeOfArmor>(mageConfig.Armor!),
                    mageConfig.DodgeChance,
                    mageConfig.ParadeChance,
                    mageConfig.ChanceSpellResistance,
                    mageConfig.Speed,
                    mageConfig.HasMana,
                    mageConfig.ManaPoints
                );
            
            case 3:
                var paladinConfig = ClassConfigLoader.GetConfig("Paladin");
                return new Paladin(
                    chosenName,
                    paladinConfig.MaxHitPoints,
                    paladinConfig.PhysicalAttackPower,
                    paladinConfig.MagicAttackPower,
                    Enum.Parse<TypeOfArmor>(paladinConfig.Armor!),
                    paladinConfig.DodgeChance,
                    paladinConfig.ParadeChance,
                    paladinConfig.ChanceSpellResistance,
                    paladinConfig.Speed,
                    paladinConfig.HasMana,
                    paladinConfig.ManaPoints
                );
            
            case 4:
                var thiefConfig = ClassConfigLoader.GetConfig("Thief");
                return new Thief(
                    chosenName,
                    thiefConfig.MaxHitPoints,
                    thiefConfig.PhysicalAttackPower,
                    thiefConfig.MagicAttackPower,
                    Enum.Parse<TypeOfArmor>(thiefConfig.Armor!),
                    thiefConfig.DodgeChance,
                    thiefConfig.ParadeChance,
                    thiefConfig.ChanceSpellResistance,
                    thiefConfig.Speed
                );
            
            case 5:
                var priestConfig = ClassConfigLoader.GetConfig("Priest");
                return new Priest(
                    chosenName,
                    priestConfig.MaxHitPoints,
                    priestConfig.PhysicalAttackPower,
                    priestConfig.MagicAttackPower,
                    Enum.Parse<TypeOfArmor>(priestConfig.Armor!),
                    priestConfig.DodgeChance,
                    priestConfig.ParadeChance,
                    priestConfig.ChanceSpellResistance,
                    priestConfig.Speed,
                    priestConfig.HasMana,
                    priestConfig.ManaPoints
                );
            
            default: throw new ArgumentException("Invalid class choice");
        }
    }
    
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
                skill.UseSkill(player, skillUsage.Target);   
                if (player.UsesMana && player.CurrentMana < skill.ManaCost) 
                {
                    Console.WriteLine($"{player.Name} failed to cast {skill.Name} due to insufficient mana and passes their turn!");
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

    private static List<Character> ExecutionSpeedCalculation(List<Character> characters)
    {
        var random = new Random();
        var shuffledCharacters  = characters.OrderBy(_ => random.Next()).ToList();
        var sortedCharacters = shuffledCharacters.OrderByDescending(character  => character.Speed).ToList();

        return sortedCharacters;
    }

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
}
