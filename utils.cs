namespace J_RPG;

public class Utils
{
    public static int PromptClassChoice()
    {
        int result;
        bool isPromptValid;
        string[] existingCharacterClass = { "Warrior", "Mage", "Paladin", "Thief\n" };

        do
        {
            Console.WriteLine("Enter a number corresponding to a class: ");
            for (int i = 1; i < existingCharacterClass.Length+1; i++)
            {
                Console.Write($"{i} - {existingCharacterClass[i-1]}\n");
            }

            Console.Write("Choose: ");
            isPromptValid = int.TryParse(Console.ReadLine(), out result) && result >= 1 && result < existingCharacterClass.Length+1;

            if (!isPromptValid)
            {
                Console.WriteLine("Invalid entry, please try again");
            }
        } while (!isPromptValid);

        return result; 
    }

    public static int PromptChoice(string[] options)
    {
        int result;
        bool isPromptValid;

        do
        {
            Console.WriteLine("\nEnter a number corresponding to the desired action: ");
            for (int i = 1; i == options.Length; i++)
            {
                Console.Write($"{i} - {options[i-1]}\n");
            }

            Console.Write("Choose: ");
            isPromptValid = int.TryParse(Console.ReadLine(), out result) && result >= 1 && result < options.Length + 1;

            if (!isPromptValid)
            {
                Console.WriteLine("Invalid entry, please try again");
            }
        } while (!isPromptValid);

        return result;
    }
}