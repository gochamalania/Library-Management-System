using Application.Interfaces;

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

            Console.WriteLine("====================================");
            Console.WriteLine($"Admin Menu: {_authService.CurrentUser.Username}");
            Console.WriteLine("====================================");

            Console.WriteLine("1. View all book lists");
            Console.WriteLine("2. Add new book");
            Console.WriteLine("3. Delete book");
            Console.WriteLine("4. Show all active borrows");
            Console.WriteLine("5. Logout");

            Console.WriteLine("\nSelect option(1-5): ");

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
        Console.WriteLine("--- Library books ---");
        var books = _libraryService.GetAllBooks();

        foreach (var b in books)
        {
            Console.WriteLine(
                $"ID: {b.Id} | Title: {b.Title} | Author: {b.Author} | ISBN: {b.ISBN} | Quantity: {b.AvailableCopies}/{b.TotalCopies}");
        }

        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void AddBook()
    {
        Console.Clear();
        Console.WriteLine("--- Add new book ---");
        
        Console.Write("Title: ");
        string title = Console.ReadLine();

        Console.Write("Author: ");
        string author = Console.ReadLine();
        
        Console.Write("ISBN: ");
        string isbn = Console.ReadLine();
        
        Console.Write("Number of copies: ");
        if (!int.TryParse(Console.ReadLine(), out int quantity))
        {
            Console.WriteLine("\nIncorrect quantity");
            Console.ReadKey();
            return;
        }

        try
        {
            _libraryService.AddBook(title, author, isbn, quantity);
            Console.WriteLine("\nThe book was successfully added to the database ");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void RemoveBook()
    {
        Console.Clear();
        ShowAllBooks();
        Console.Write("\nEnter book ID, which you want to delete: ");
        string bookId = Console.ReadLine();

        try
        {
            _libraryService.ReturnBook(bookId);
            Console.WriteLine("\nThe book was successfully deleted!");
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
        Console.WriteLine("--- All unreturned books ---");
        var borrows = _libraryService.GetAllActiveBorrows();

        if (borrows.Count == 0)
        {
            Console.WriteLine("There are no active borrows.");
        }
        else
        {
            foreach (var r in borrows)
            {
                Console.WriteLine($"Borrow ID: {r.Id} | User ID: {r.UserId} | Book ID: {r.BookId} | Deadline: {r.DueDate: yyyy-MM-dd}");
            }
        }
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

}