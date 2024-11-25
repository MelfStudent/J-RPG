namespace J_RPG;

public class Utils
{
    public static string PromptClassChoice()
    {
        int result;
        bool isPromptValid;
        string[] existingCharacterClass = { "Warrior", "Mage", "Thief", "Paladin\n" };

        do
        {
            Console.WriteLine("Enter a number corresponding to a class: ");
            for (int i = 0; i < existingCharacterClass.Length; i++)
            {
                Console.Write($"{i} - {existingCharacterClass[i]}\n");
            }

            isPromptValid = int.TryParse(Console.ReadLine(), out result) && result >= 0 && result < existingCharacterClass.Length;

            if (!isPromptValid)
            {
                Console.WriteLine("Invalid entry, please try again");
            }
        } while (!isPromptValid);

        return existingCharacterClass[result]; 
    }
}