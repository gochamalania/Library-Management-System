using Application.Interfaces;
using UI.Helpers;

namespace UI.Menus;

public class AdminMenu
{
    private readonly ILibraryService _libraryService;
    private readonly IAuthService _authService;

    public AdminMenu(ILibraryService libraryService, IAuthService authService)
    {
        _libraryService = libraryService;
        _authService = authService;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            
            // yellow title
            ConsoleHelper.WriteAdminHeader($"Admin menu | {_authService.CurrentUser.Username}");

            

            Console.WriteLine("1. View all book lists");
            Console.WriteLine("2. Add new book");
            Console.WriteLine("3. Delete book");
            Console.WriteLine("4. Show all active borrows");
            Console.WriteLine("5. Check client info");
            Console.WriteLine("6. Logout");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nSelect option(1-6): ");
            Console.ResetColor();

            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowAllBooks();
                    break;
                case "2":
                    AddBook();
                    break;
                case "3":
                    RemoveBook();
                    break;
                case "4":
                    ShowActiveBorrows();
                    break;
                case "5":
                    CheckClientInfo();
                    break;
                case "6":
                    _authService.Logout();
                    return;
                default:
                    Console.WriteLine("\nInvalid choice! Press any key...");
                    Console.ReadKey();
                    break;
            }
        }
    }

    private void ShowAllBooks()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("All books");

        var books = _libraryService.GetAllBooks();
        TablePrinter.PrintBooks(books);
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void AddBook()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("Add new book");
        
        Console.Write("Title: ");
        string title = Console.ReadLine() ?? "";

        Console.Write("Author: ");
        string author = Console.ReadLine() ?? "";
        
        Console.Write("ISBN: ");
        string isbn = Console.ReadLine() ?? "";
        
        Console.Write("Number of copies: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            ConsoleHelper.WriteError("\nIncorrect quantity");
            Console.ReadKey();
            return;
        }

        try
        {
            _libraryService.AddBook(title, author, isbn, quantity);
            ConsoleHelper.WriteSuccess("\nThe book was successfully added to the database ");
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"\nError: {ex.Message}");
        }
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void RemoveBook()
    {
        Console.Clear();
        ShowAllBooks();
        ConsoleHelper.WriteAdminHeader("Delete book");
        
        Console.Write("\nEnter book ID, which you want to delete: ");
        string bookId = Console.ReadLine();

        try
        {
            _libraryService.RemoveBook(bookId);
            ConsoleHelper.WriteSuccess("\nThe book was successfully deleted!");
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"\nError: {ex.Message}");
        }
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void ShowActiveBorrows()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("All unreturned books");
        
        var borrows = _libraryService.GetAllActiveBorrows();

        if (borrows.Count == 0)
        {
            ConsoleHelper.WriteInfo("There are no active borrows.");
        }
        else
        {
            string line = new string('-', 80);
            Console.WriteLine(line);
            Console.WriteLine($"| {"Borrow ID", -10} | {"User ID", -12} | {"Book ID", -10} | {"Due Date", -12} |");
            Console.WriteLine(line);
            
            foreach (var r in borrows)
            {
                Console.WriteLine($"| {r.Id, -10} | {r.UserId, -12} | {r.BookId, -10} | {r.DueDate:yyyy-MM-dd} |");
            }

            Console.WriteLine(line);
        }
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    
    //Checking the books and fines of a specific client
    private void CheckClientInfo()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("Check Client Info");
        
        //showAllclient
        var clients = _libraryService.GetAllClients();

        if (clients.Count == 0)
        {
            ConsoleHelper.WriteInfo("No clients found in the system.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            return;  
        }
        
        Console.WriteLine("--- Registered Clients List ---");
        string clientTableLine = new string('-', 45);
        Console.WriteLine(clientTableLine);
        Console.WriteLine($"| {"User ID", -12} | {"Username", -25} |");
        Console.WriteLine(clientTableLine);
        
        foreach (var client in clients)
        {
            Console.WriteLine($"| {client.Id, -12} | {client.Username, -25} |");
        }
        Console.WriteLine(clientTableLine);
        
        //Requesting ID from the list
        Console.Write("\nEnter Client User ID to inspect: ");
        string userId = Console.ReadLine() ?? "";

        if (string.IsNullOrWhiteSpace(userId))
        {
            ConsoleHelper.WriteError("User ID cannot be empty!");
            Console.ReadKey();
            return;
        }
        
        // Check if such a client exists
        var selectedClient = clients.FirstOrDefault(c => c.Id == userId);
        if (selectedClient == null)
        {
            ConsoleHelper.WriteError("Client with this ID was not found!");
            Console.ReadKey();
            return;
        }

        var borrows = _libraryService.GetUsersBorrows(userId);
        decimal totalFine = _libraryService.GetTotalUserFine(userId);
        
        Console.WriteLine($"\n--- Borrow History for User: {selectedClient.Username} (ID: {userId}) ---");
        if (borrows.Count == 0)
        {
            Console.WriteLine("No borrowing records found for this user.");
        }
        else
        {
            string line = new string('-', 75);
            Console.WriteLine(line);
            Console.WriteLine($"| {"Borrow ID", -10} | {"Book ID", -10} | {"Borrow Date", -12} | {"Due Date", -12} | {"Status", -10} |");
            Console.WriteLine(line);

            foreach (var r in borrows)
            {
                string status = r.IsReturned ? "Returned" : "Borrowed";
                Console.WriteLine($"| {r.Id, -10} | {r.BookId, -10} | {r.BorrowDate:yyyy-MM-dd} | {r.DueDate:yyyy-MM-dd} | {status, -10} |");
            }
            Console.WriteLine(line);
        }

        Console.WriteLine($"\n--- Fine Status ---");
        if (totalFine > 0)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($" Total calculated fine: ${totalFine:F2}");
            Console.ResetColor();
        }
        else
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("✅ User has no active fines.");
            Console.ResetColor();
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
        
    }

}