using Core.Models;

namespace Repository.Interfaces;

public interface IBookRepository
{
    List<Book> GetAllBooks();
    Book GetBookById(string id);
    void AddBook(Book book);
    void UpdateBook(Book book);
    void DeleteBook(string id);
}