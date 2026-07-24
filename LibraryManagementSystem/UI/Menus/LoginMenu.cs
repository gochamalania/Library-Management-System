using Application.Interfaces;
using Core.Enums;
using UI.Helpers;

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
            Console.WriteLine("\nChoose option (1-3): ");
            
            string choice = Console.ReadLine() ?? "";

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
        Console.Write("Username or Email: ");
        string identifier = Console.ReadLine() ?? "";
        
        Console.Write("Password: ");
        string password = Console.ReadLine() ?? "";

        if (_authService.Login(identifier, password))
        {
            Console.WriteLine($"\nWelcome, {_authService.CurrentUser!.Username} ({_authService.CurrentUser.UserRole}).");
            Console.WriteLine("Press any key to continue...");
            Console.ReadKey();
            
            switch (_authService.CurrentUser.UserRole)
            {
                case Role.SuperAdmin:
                    var superAdminMenu = new SuperAdminMenu(_libraryService, _authService);
                    superAdminMenu.Show();
                    break;

                case Role.Admin:
                    var adminMenu = new AdminMenu(_libraryService, _authService);
                    adminMenu.Show();
                    break;

                default: // Role.Client
                    var clientMenu = new ClientMenu(_libraryService, _authService);
                    clientMenu.Show();
                    break;
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
        Console.Write("Username: ");
        string username = Console.ReadLine() ?? "";
        
        Console.Write("Email: ");
        string email = Console.ReadLine() ?? "";
        
        Console.Write("Password: ");
        string password = PasswordHelper.ReadPassword();
        
        Console.WriteLine("\nChoose Role: ");
        Console.WriteLine("1. Client");
        Console.WriteLine("2. Admin");
        Console.Write("Enter choice (1-2): ");
        string roleChoice = Console.ReadLine() ?? "";
        
        Role role = roleChoice == "2" ? Role.Admin : Role.Client;

        try
        {
            if (_authService.Register(username, password, email, role))
            {
                Console.WriteLine("\nRegistration completed successfully! You can now log in.");
                Console.WriteLine("A 6 digit verification code has been generated (check logs.json).");
                
                Console.Write("\nEnter verification code: ");
                string code = Console.ReadLine() ?? "";

                if (_authService.VerifyEmail(email, code))
                {
                    Console.WriteLine("\nEmail verified successfully! You can now log in.");
                }
                else
                {
                    Console.WriteLine("\nInvalid code! Your account is created, but email remains unverified.");
                }
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