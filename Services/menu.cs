﻿namespace J_RPG.Services;

using Models;

public static class Menu
{
    public static Character? Player1 { get; private set; }
    public static Character? Player2 { get; private set; }
    public static Character? CharacterWhoAttacks { get; set; }
    public static Character? CharacterWhoDefends { get; set; }
    
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
        Console.WriteLine("\n========== WELCOME TO JRPG! ==========");
        List<string> existingOptions = new() { "Start a new game", "Quit the game\n"};
        var enter = Utils.PromptChoice(existingOptions, "Choose an option to begin:");
        Console.WriteLine("=======================================");
        
        switch (enter)
        {
            case 1:
                PrintClassChoiceMenu();
                break;
            case 2:
                Console.WriteLine("Game stopped! Thank you and see you soon.");
                break;
        }
    }
    
    private static void PrintClassChoiceMenu()
    {
        Console.WriteLine("\n========== CHARACTER CREATION ==========");
        var choiceCharacterName1 = Utils.PromptName("\nEnter the name of the first character: ");
        
        List<string> existingCharacterClass = new() { "Warrior", "Mage", "Paladin", "Thief\n" };
        
        Console.Write("Choose a class for the player 1: ");
        var choiceCharacterClass1 = Utils.PromptChoice(existingCharacterClass, "Enter a number corresponding to a class: ");
        Player1 = Utils.CreatePlayer(choiceCharacterName1, choiceCharacterClass1);
        
        var choiceCharacterName2 = Utils.PromptName("\nEnter the name of the second character: ");
        
        Console.Write("Choose a class for the player 2: ");
        var choiceCharacterClass2 = Utils.PromptChoice(existingCharacterClass, "Enter a number corresponding to a class: ");
        Player2 = Utils.CreatePlayer(choiceCharacterName2, choiceCharacterClass2);
        
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine("\n========== CHARACTERS READY ==========");
        Console.WriteLine($"Player 1: {Player1.Name} ({Player1.GetType().Name})");
        DisplayCharacterStats(Player1);

        Console.WriteLine($"\nPlayer 2: {Player2.Name} ({Player2.GetType().Name})");
        DisplayCharacterStats(Player2);
        Console.ResetColor();
        
        CharacterWhoAttacks = Player1;
        CharacterWhoDefends = Player2;
        
        Console.WriteLine("\n========================================");
        Console.WriteLine("Press any key to start the battle...");
        Console.ReadKey();
        Utils.StartGame();
    }
    
    private static void DisplayCharacterStats(Character character)
    {
        Console.WriteLine("----------------------------------------");
        Console.WriteLine($"Name: {character.Name}");
        Console.WriteLine($"Class: {character.GetType().Name}");
        Console.WriteLine($"HP: {character.CurrentHitPoints}/{character.MaxHitPoints}");
        Console.WriteLine($"Physical Attack: {character.PhysicalAttackPower}");
        Console.WriteLine($"Magical Attack: {character.MagicAttackPower}");
        Console.WriteLine($"Dodge Chance: {character.DodgeChance}%");
        Console.WriteLine($"Parade Chance: {character.ParadeChance}%");
        Console.WriteLine($"Spell Resistance Chance: {character.ChanceSpellResistance}%");
        Console.WriteLine($"Armor Type: {character.Armor} (Resistance: {GetArmorPercentage(character.Armor)})");
        Console.WriteLine("----------------------------------------");
    }
    
    private static string GetArmorPercentage(Character.TypeOfArmor armor)
    {
        return armor switch
        {
            Character.TypeOfArmor.Fabric => "30% vs Magic",
            Character.TypeOfArmor.Leather => "15% vs Physical, 20% vs Magic",
            Character.TypeOfArmor.Mesh => "30% vs Physical, 10% vs Magic",
            Character.TypeOfArmor.Plates => "45% vs Physical",
            _ => "No resistance"
        };
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
        for (var i = 10; i > 0; i--)
        {
            Console.Write($"\r{i} seconds remaining");
            Thread.Sleep(1000);
        }
        Console.WriteLine();
        PrintNavigationMenu();
    }
}