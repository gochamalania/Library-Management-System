using Application.Interfaces;
using Core.Models;
using Repository.Interfaces;

namespace Application.Services;

public class LibraryService : ILibraryService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBorrowRepository _borrowRepository;
    private readonly IUserRepository _userRepository;

    public LibraryService(IBookRepository bookRepository, IBorrowRepository borrowRepository, IUserRepository userRepository)
    {
        _bookRepository = bookRepository;
        _borrowRepository = borrowRepository;
        _userRepository = userRepository;
    }

    public List<Book> GetAllBooks()
    {
        return _bookRepository.GetAllBooks();
    }

    public List<Book> SearchBooks(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) return GetAllBooks();
        
        query = query.Trim().ToLower();
        
        return _bookRepository.GetAllBooks()
            .Where(b => 
                            b.Title.ToLower().Contains(query) ||
                            b.Author.ToLower().Contains(query) ||
                            b.ISBN.ToLower().Contains(query))
            .ToList();
    }

    public void AddBook(string title, string author, string isbn, int quantity)
    {
        if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) || string.IsNullOrWhiteSpace(isbn))
            throw new ArgumentException("Title, author and ISBN cannot be empty");
        
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be greater than 0");

        var books = _bookRepository.GetAllBooks();
        string newId = (books.Count == 0 ? 1 : books.Max(b => int.Parse(b.Id)) + 1).ToString();
        
        var newBook = new Book(newId, title.Trim(), author.Trim(), isbn.Trim(), quantity, quantity);
        _bookRepository.AddBook(newBook);
    }

    public void RemoveBook(string bookId)
    {
        var book = _bookRepository.GetBookById(bookId);
        
        if(book == null)
            throw new Exception("Book not found");
        
        _bookRepository.DeleteBook(bookId);
    }

    public void BorrowBook(string userId, string bookId)
    {
        var book = _bookRepository.GetBookById(bookId);
        if(book == null)
            throw new Exception("Book not found");
        
        if(book.AvailableCopies <= 0)
            throw new Exception("There are no free copies of this book left!");
        
        // Check: Has the user already checked out this book and not returned it?
        var userBorrows = _borrowRepository.GetAllBorrows();
        bool alreadyBorrowed = userBorrows.Any(b => b.UserId == userId && b.BookId == bookId && !b.IsReturned);
        
        if(alreadyBorrowed)
            throw new Exception("You have already borrowed this book!");
        
        //1. Reducing the number of books
        book.BorrowOneCopy();
        _bookRepository.UpdateBook(book);
        
        // 2. Create a new record (return period - 14 days)
        string newBorrowId = (userBorrows.Count == 0 ? 1 : userBorrows.Max(b => int.Parse(b.Id)) + 1).ToString();
        var record = new BorrowRecord(newBorrowId, userId, bookId, DateTime.Now, DateTime.Now.AddDays(14));
        _borrowRepository.AddBorrow(record);
    }

    public void ReturnBook(string borrowId)
    {
        var record = _borrowRepository.GetById(borrowId);
        if (record == null)
            throw new Exception("Borrow record not found");
        
        if (record.IsReturned)
            throw new Exception("This book has already been returned");
        
        // 1. Update record
        record.MarkAsReturned();
        _borrowRepository.UpdateBorrow(record);
        
        // 2. Increase the number of books
        var book = _bookRepository.GetBookById(record.BookId);
        if (book != null)
        {
            book.ReturnOneCopy();
            _bookRepository.UpdateBook(book);
        }
        
        // 3. Calculate the fine (If the delay is more than 14 days, $1 per day)
        if (DateTime.Now > record.DueDate)
        {
            int lateDays = (DateTime.Now - record.DueDate).Days;
            if (lateDays > 0)
            {
                decimal fineAmount = lateDays * 1m;
                var user = _userRepository.GetAllUsers().FirstOrDefault(u => u.Id == record.UserId);
                if (user is ClientUser client)
                {
                    client.AddFine(fineAmount);
                    _userRepository.UpdateUser(client);
                }
            }
        }
    }

    public List<BorrowRecord> GetUsersBorrows(string userId)
    {
        return _borrowRepository.GetAllBorrows().Where(b => b.UserId == userId).ToList();
    }
    
    public List<BorrowRecord> GetAllActiveBorrows()
    {
        return _borrowRepository.GetAllBorrows().Where(b => !b.IsReturned).ToList();
    }
}