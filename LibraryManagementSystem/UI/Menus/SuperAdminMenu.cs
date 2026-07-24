using Application.Interfaces;
using UI.Helpers;
using Core.Enums;

namespace UI.Menus;

public class SuperAdminMenu
{
    private readonly ILibraryService _libraryService;
    private readonly IAuthService _authService;

    public SuperAdminMenu(ILibraryService libraryService, IAuthService authService)
    {
        _libraryService = libraryService;
        _authService = authService;
    }
    
    public void Show()
    {
        while (true)
        {
            Console.Clear();
            
            ConsoleHelper.WriteAdminHeader($" SUPER ADMIN MENU | {_authService.CurrentUser!.Username}");

            Console.WriteLine("---  Book Management ---");
            Console.WriteLine("1. View all books");
            Console.WriteLine("2. Add new book");
            Console.WriteLine("3. Delete book");
            Console.WriteLine("4. Show all active borrows");
            
            Console.WriteLine("\n--- User & Client Management ---");
            Console.WriteLine("5. Check client info & fines");
            Console.WriteLine("6. Register new Admin");
            Console.WriteLine("7. Delete User (Client or Admin)");
            Console.WriteLine("8. Clear User Fine");
            
            Console.WriteLine("\n--- System ---");
            Console.WriteLine("9. View System Statistics");
            Console.WriteLine("10. Logout");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write("\nSelect option (1-10): ");
            Console.ResetColor();

            string choice = Console.ReadLine() ?? "";

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
                    RegisterAdmin(); 
                    break;
                case "7": 
                    DeleteUser(); 
                    break;
                case "8": 
                    ClearFine(); 
                    break;
                case "9": 
                    ShowSystemStatistics(); 
                    break;
                case "10":
                    _authService.Logout();
                    return;
                default:
                    Console.WriteLine("\nInvalid choice! Press any key...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    //standart functions
    private void ShowAllBooks()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("All Books");
        var books = _libraryService.GetAllBooks();
        TablePrinter.PrintBooks(books);
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void AddBook()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("Add New Book");

        Console.Write("Title: ");
        string title = Console.ReadLine() ?? "";

        Console.Write("Author: ");
        string author = Console.ReadLine() ?? "";

        Console.Write("ISBN: ");
        string isbn = Console.ReadLine() ?? "";

        Console.Write("Number of copies: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            ConsoleHelper.WriteError("\nIncorrect quantity!");
            Console.ReadKey();
            return;
        }

        try
        {
            _libraryService.AddBook(title, author, isbn, quantity);
            ConsoleHelper.WriteSuccess("\nThe book was successfully added!");
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
        ConsoleHelper.WriteAdminHeader("Delete Book");

        Console.Write("\nEnter book ID to delete: ");
        string bookId = Console.ReadLine() ?? "";

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
        ConsoleHelper.WriteAdminHeader("All Active Borrows");

        var borrows = _libraryService.GetAllActiveBorrows();

        if (borrows.Count == 0)
        {
            ConsoleHelper.WriteInfo("There are no active borrows.");
        }
        else
        {
            string line = new string('-', 80);
            Console.WriteLine(line);
            Console.WriteLine($"| {"Borrow ID",-10} | {"User ID",-12} | {"Book ID",-10} | {"Due Date",-12} |");
            Console.WriteLine(line);

            foreach (var r in borrows)
            {
                Console.WriteLine($"| {r.Id,-10} | {r.UserId,-12} | {r.BookId,-10} | {r.DueDate:yyyy-MM-dd} |");
            }
            Console.WriteLine(line);
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void CheckClientInfo()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("Check Client Info");

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
        Console.WriteLine($"| {"User ID",-12} | {"Username",-25} |");
        Console.WriteLine(clientTableLine);

        foreach (var client in clients)
        {
            Console.WriteLine($"| {client.Id,-12} | {client.Username,-25} |");
        }
        Console.WriteLine(clientTableLine);

        Console.Write("\nEnter Client User ID to inspect: ");
        string userId = Console.ReadLine() ?? "";

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
            Console.WriteLine($"| {"Borrow ID",-10} | {"Book ID",-10} | {"Borrow Date",-12} | {"Due Date",-12} | {"Status",-10} |");
            Console.WriteLine(line);

            foreach (var r in borrows)
            {
                string status = r.IsReturned ? "Returned" : "Borrowed";
                Console.WriteLine($"| {r.Id,-10} | {r.BookId,-10} | {r.BorrowDate:yyyy-MM-dd} | {r.DueDate:yyyy-MM-dd} | {status,-10} |");
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
            Console.WriteLine(" User has no active fines.");
            Console.ResetColor();
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    
    //SuperAdmin exclusive functions
    private void RegisterAdmin()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("Register New Admin");

        Console.Write("Enter new Admin Username: ");
        string username = Console.ReadLine() ?? "";

        Console.Write("Enter Password: ");
        string password = Console.ReadLine() ?? "";

        try
        {
            _authService.RegisterAdmin(username, password);
            ConsoleHelper.WriteSuccess($"\nNew Admin '{username}' registered successfully!");
        }
        catch (Exception ex)
        {
            ConsoleHelper.WriteError($"\nError: {ex.Message}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void DeleteUser()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("Delete User (client or admin)");
        
        var users = _libraryService.GetAllUsers();
        
        // We do not allow SuperAdmin to delete themselves
        var deletableUsers = users.Where(u => u.Id != _authService.CurrentUser!.Id).ToList();

        if (deletableUsers.Count == 0)
        {
            ConsoleHelper.WriteInfo("No other users found in the system.");
            Console.WriteLine("\nPress any key...");
            Console.ReadKey();
            return;
        }

        Console.WriteLine("--- System Users ---");
        string line = new string('-', 55);
        Console.WriteLine(line);
        Console.WriteLine($"| {"User ID",-12} | {"Username",-20} | {"Role",-12} |");
        Console.WriteLine(line);

        foreach (var u in deletableUsers)
        {
            Console.WriteLine($"| {u.Id,-12} | {u.Username,-20} | {u.UserRole,-12} |");
        }
        Console.WriteLine(line);

        Console.Write("\nEnter User ID to DELETE: ");
        string targetId = Console.ReadLine() ?? "";

        var targetUser = deletableUsers.FirstOrDefault(u => u.Id == targetId);
        if (targetUser == null)
        {
            ConsoleHelper.WriteError("User not found!");
            Console.ReadKey();
            return;
        }

        Console.ForegroundColor = ConsoleColor.Red;
        Console.Write($"Are you sure you want to delete user '{targetUser.Username}'? (y/n): ");
        Console.ResetColor();

        string confirm = Console.ReadLine() ?? "";
        if (confirm.ToLower() == "y")
        {
            try
            {
                _libraryService.DeleteUser(targetId);
                ConsoleHelper.WriteSuccess($"\nUser '{targetUser.Username}' was successfully deleted.");
            }
            catch (Exception ex)
            {
                ConsoleHelper.WriteError($"\nError: {ex.Message}");
            }
        }
        else
        {
            ConsoleHelper.WriteInfo("\nDeletion cancelled.");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
    
    private void ClearFine()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("Clear / Waive User Fine");

        var clients = _libraryService.GetAllClients();
        
        Console.Write("Enter Client User ID to clear fine: ");
        string userId = Console.ReadLine() ?? "";

        var client = clients.FirstOrDefault(c => c.Id == userId);
        if (client == null)
        {
            ConsoleHelper.WriteError("Client not found!");
            Console.ReadKey();
            return;
        }

        decimal currentFine = _libraryService.GetTotalUserFine(userId);
        Console.WriteLine($"\nCurrent fine for '{client.Username}': ${currentFine:F2}");

        if (currentFine == 0)
        {
            ConsoleHelper.WriteInfo("User has no fines to clear.");
            Console.ReadKey();
            return;
        }

        Console.Write("Are you sure you want to clear this fine? (y/n): ");
        if (Console.ReadLine()?.ToLower() == "y")
        {
            _libraryService.ClearUserFine(userId);
            ConsoleHelper.WriteSuccess($"\nFine for '{client.Username}' has been cleared!");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void ShowSystemStatistics()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("System Statistics");

        var books = _libraryService.GetAllBooks();
        var users = _libraryService.GetAllUsers();
        var activeBorrows = _libraryService.GetAllActiveBorrows();

        int totalBooks = books.Sum(b => b.TotalCopies);
        int totalClients = users.Count(u => u.UserRole == Role.Client);
        int totalAdmins = users.Count(u => u.UserRole == Role.Admin || u.UserRole == Role.SuperAdmin);

        Console.WriteLine($" Total Book Titles: {books.Count}");
        Console.WriteLine($" Total Physical Copies: {totalBooks}");
        Console.WriteLine($" Active Borrows: {activeBorrows.Count}");
        Console.WriteLine($" Total Clients: {totalClients}");
        Console.WriteLine($" Total Admins: {totalAdmins}");

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
}