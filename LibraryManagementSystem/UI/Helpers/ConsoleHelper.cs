namespace UI.Helpers;

public class ConsoleHelper
{
    // Print Client header (Cyan)
    public static void WriteClientHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Cyan;
        Console.WriteLine("====================================");
        Console.WriteLine($" {title}");
        Console.WriteLine("====================================");
        Console.ResetColor();
    }
    
    // Print Admin header (Yellow)
    public static void WriteAdminHeader(string title)
    {
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine("====================================");
        Console.WriteLine($" {title}");
        Console.WriteLine("====================================");
        Console.ResetColor();       
    }
    
    // Success message (Green)
    public static void WriteSuccess(string message)
    {
        Console.ForegroundColor = ConsoleColor.Green;
        Console.WriteLine($"\n {message}");
        Console.ResetColor();
    }
    
    // Error message (Red)
    public static void WriteError(string message)
    {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine($"\n Error: {message}");
        Console.ResetColor();
    }
    
    // Warning info (blue)
    public static void WriteInfo(string message)
    {
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine(message);
        Console.ResetColor();       
    }
}