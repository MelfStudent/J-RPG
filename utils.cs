namespace J_RPG;

public class Utils
{
    public static int PromptChoice(string[] options, string titled)
    {
        int result;
        bool isPromptValid;

        do
        {
            Console.WriteLine(titled);
            for (int i = 1; i < options.Length+1; i++)
            {
                Console.Write($"{i} - {options[i-1]}\n");
            }

            Console.Write("Choose: ");
            isPromptValid = int.TryParse(Console.ReadLine(), out result) && result >= 1 && result < options.Length+1;

            if (!isPromptValid)
            {
                Console.WriteLine("Invalid entry, please try again");
            }
        } while (!isPromptValid);

        return result; 
    }
    
    public static string PromptName(string titled)
    {
        string result;
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

        return result; 
    }
}