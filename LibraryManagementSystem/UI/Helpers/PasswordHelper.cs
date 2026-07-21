namespace UI.Helpers;

public class PasswordHelper
{
    // Type the password with *s appearing on the screen
    public static string ReadPassword()
    {
        string password = "";
        ConsoleKeyInfo key;

        do
        {
            key = Console.ReadKey(true);
            // If it is a normal character (neither Enter nor Backspace)
            if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
            {
                password += key.KeyChar;
                Console.Write("*");
            }

            // If Backspace is pressed and the password is not empty
            else if (key.Key == ConsoleKey.Backspace && password.Length > 0)
            {
                password = password[0..^1];
                Console.Write("\b \b"); // Removes the last star from the screen
            }
        } while (key.Key != ConsoleKey.Enter);

        Console.WriteLine();
        return password;

    }
}