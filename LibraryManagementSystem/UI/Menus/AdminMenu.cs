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
            ConsoleHelper.WriteAdminHeader("Admin menu | {_authService.CurrentUser.Username}");

            

            Console.WriteLine("1. View all book lists");
            Console.WriteLine("2. Add new book");
            Console.WriteLine("3. Delete book");
            Console.WriteLine("4. Show all active borrows");
            Console.WriteLine("5. Logout");
            
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("\nSelect option(1-5): ");
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
        string title = Console.ReadLine();

        Console.Write("Author: ");
        string author = Console.ReadLine();
        
        Console.Write("ISBN: ");
        string isbn = Console.ReadLine();
        
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
            _libraryService.ReturnBook(bookId);
            ConsoleHelper.WriteSuccess("\nThe book was successfully deleted!");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void ShowActiveBorrows()
    {
        Console.Clear();
        ConsoleHelper.WriteAdminHeader("--- All unreturned books ---");
        
        var borrows = _libraryService.GetAllActiveBorrows();

        if (borrows.Count == 0)
        {
            ConsoleHelper.WriteInfo("There are no active borrows.");
        }
        else
        {
            string line = new string('-', 85);
            Console.WriteLine(line);
            Console.WriteLine($"| {"User ID", -15} | {"Book ID", -10} | {"Borrow Date", -20} | {"Deadline", -20} |");
            Console.WriteLine(line);
            
            foreach (var r in borrows)
            {
                Console.WriteLine($"Borrow ID: {r.Id} | User ID: {r.UserId} | Book ID: {r.BookId} | Deadline: {r.DueDate: yyyy-MM-dd}");
            }
        }
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

}