using Application.Interfaces;
using Core.Enums;

namespace UI.Menus;

public class LoginMenu
{
    private readonly IAuthService _authService;
    private readonly ILibraryService _libraryService;
    public LoginMenu(IAuthService authService, ILibraryService libraryService)
    {
        _authService = authService;
        _libraryService = libraryService;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            Console.WriteLine("====================================");
            Console.WriteLine(" Library Management System (LMS) ");
            Console.WriteLine("====================================");
            Console.WriteLine("1. Login");
            Console.WriteLine("2. Register");
            Console.WriteLine("3. Exit");
            Console.WriteLine("\nchoose option (1-3): ");
            
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    HandleLogin();
                    break;
                case "2":
                    HandleRegister();
                    break;
                case "3":
                    Console.WriteLine("\nThank you for using the system. Goodbye!");
                    return;
                default:
                    Console.WriteLine("\nIncorrect choice! Press any key...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void HandleLogin()
    {
        Console.Clear();
        Console.WriteLine("--- Authorization ---");
        Console.Write("Username: ");
        string username = Console.ReadLine();
        
        Console.Write("Password: ");
        string password = Console.ReadLine();

        if (_authService.Login(username, password))
        {
            Console.WriteLine($"\nWelcome, {_authService.CurrentUser.Username} ({_authService.CurrentUser.UserRole}).");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            
            
            // Enable the appropriate menu according to the role
            //(ClientMenu / AdminMenu)
            if (_authService.CurrentUser.UserRole == Role.Admin)
            {
                var adminMenu = new AdminMenu(_libraryService, _authService);
                adminMenu.Show();
            }
            else
            {
                var clientMenu = new ClientMenu(_libraryService, _authService);
                clientMenu.Show();
            }
            
        }
        else
        {
            Console.WriteLine("\nError: username or password is incorrect!");
            Console.WriteLine("Press any key to try again...");
            Console.ReadKey();
        }
    }

    private void HandleRegister()
    {
        Console.Clear();
        Console.WriteLine("--- Registration ---");
        Console.Write("User name: ");
        string username = Console.ReadLine();
        
        Console.Write("Password: ");
        string password = Console.ReadLine();
        
        Console.WriteLine("\nChoose Role: ");
        Console.WriteLine("1. Client");
        Console.WriteLine("2. Admin");
        Console.Write("Enter choice (1-2): ");
        string roleChoice = Console.ReadLine();
        
        Role role = roleChoice == "2" ? Role.Admin : Role.Client;

        try
        {
            if (_authService.Register(username, password, role))
            {
                Console.WriteLine("\nRegistration has been completed successfully! You can now log in.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
        
        Console.WriteLine("Press any key to continue...");
        Console.ReadKey();
    }
}