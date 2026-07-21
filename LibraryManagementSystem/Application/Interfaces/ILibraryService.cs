using Core.Models;

namespace Application.Interfaces;

public interface ILibraryService
{
    //Book Operations
    List<Book> GetAllBooks();
    List<Book> SearchBooks(string query);
    void AddBook(string title, string author, string isbn, int quantity);
    
    //borrow operations
    void BorrowBook(string userId, string bookId);
    void ReturnBook(string borrowId);
    void RemoveBook(string bookId);
    List<BorrowRecord> GetUsersBorrows(string userId);
    List<BorrowRecord> GetAllActiveBorrows();
}