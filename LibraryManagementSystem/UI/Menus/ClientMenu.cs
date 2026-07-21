using Application.Interfaces;
using Core.Models;
using UI.Helpers;

namespace UI.Menus;

public class ClientMenu
{
    private readonly ILibraryService _libraryService;
    private readonly IAuthService _authService;

    public ClientMenu(ILibraryService libraryService, IAuthService authService)
    {
        _libraryService = libraryService;
        _authService = authService;
    }

    public void Show()
    {
        while (true)
        {
            Console.Clear();
            
            // blue title
            ConsoleHelper.WriteClientHeader($"Client menu | User: {_authService.CurrentUser.Username}");
            
            Console.Clear();
            Console.WriteLine($"====================================");
            Console.WriteLine($"Reader Menu | User: {_authService.CurrentUser.Username}");
            Console.WriteLine($"====================================");

            Console.WriteLine("1. View Books");
            Console.WriteLine("2. Search Books");
            Console.WriteLine("3. Borrow Book");
            Console.WriteLine("4. Return Book");
            Console.WriteLine("5. My borrowed books");
            Console.WriteLine("6. Check fine");
            Console.WriteLine("7. Logout");

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("\nChoice option (1-7): ");
            Console.ResetColor();
            
            string choice = Console.ReadLine();

            switch (choice)
            {
                case "1":
                    ShowAllBooks();
                    break;
                case "2":
                    SearchBooks();
                    break;
                case "3":
                    BorrowBook();
                    break;
                case "4":
                    ReturnBook();
                    break;
                case "5":
                    ShowMyBorrows();
                    break;
                case "6":
                    CheckFine();
                    break;
                case "7":
                    _authService.Logout();
                    return;
                default:
                    Console.WriteLine("\nIncorrect choice! Press any key...");
                    Console.ReadKey();
                    break;
            }
        }
    }
    private void ShowAllBooks()
    {
        Console.Clear();
        ConsoleHelper.WriteClientHeader("Books in the library");
        
        var books = _libraryService.GetAllBooks();
        TablePrinter.PrintBooks(books);

        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    private void SearchBooks()
    {
        Console.Clear();
        ConsoleHelper.WriteClientHeader("Search books");

        Console.Write("Search (title / author / ISBN)");
        string keyword = Console.ReadLine();
        
        var books = _libraryService.SearchBooks(keyword);
        TablePrinter.PrintBooks(books);
        
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey();
    }

    public void BorrowBook()
    {
        Console.Clear();
        ConsoleHelper.WriteClientHeader("Borrow book");
        
        Console.Write("Enter book ID: ");
        string bookId = Console.ReadLine();

        try
        {
            _libraryService.BorrowBook(_authService.CurrentUser.Id, bookId);
            Console.WriteLine("\nYou have successfully borrowed to the book! The return period is 14 days.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void ReturnBook()
    {
        Console.Clear();
        ShowMyBorrows();
        Console.Write("Enter borrowed book ID, which you want to return: ");
        string borrowId = Console.ReadLine();

        try
        {
            _libraryService.ReturnBook(borrowId);
            Console.WriteLine("\nBook returned successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\nError: {ex.Message}");
        }
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void ShowMyBorrows()
    {
        Console.Clear();
        ConsoleHelper.WriteClientHeader("My borrowed books");
        var borrows = _libraryService.GetUsersBorrows(_authService.CurrentUser.Id);

        if (borrows.Count == 0)
        {
            Console.WriteLine("You have not borrowed any books yet.");
        }
        else
        {
            string line = new string('-', 75);
            Console.WriteLine(line);
            Console.WriteLine($"| {"Book ID", -10} | {"Borrow date", -20} | {"Deadline", -20} | {"Status", -12} |");
            
            foreach (var r in borrows)
            {
                string status = r.IsReturned ? "Returned" : "Borrowed";
                Console.WriteLine($"Borrow ID: {r.Id} | Book ID: {r.BookId} | borrowed: {r.BorrowDate:yyyY-MM-dd} | due: {r.DueDate:yyy-MM-dd} | status: {status}");
            }
        }
        
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }

    private void CheckFine()
    {
        Console.Clear();
        ConsoleHelper.WriteClientHeader("Check fine");
        
        if (_authService.CurrentUser is ClientUser client)
        {
            Console.WriteLine($"Your current fines: {client.Fines:F2} $");
        }
        Console.WriteLine("\nPress any key...");
        Console.ReadKey();
    }
}