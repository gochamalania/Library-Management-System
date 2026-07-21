using Core.Models;
using Repository.Interfaces;

namespace Repository.Implementations;

public class TxtBookRepository : IBookRepository
{
    private readonly string _filePath;
    
    public TxtBookRepository(string filePath)
    {
        _filePath = filePath;
        
        string directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath))
        {
            File.Create(_filePath).Close();
        }
    }

    public List<Book> GetAllBooks()
    {
        var books = new List<Book>();
        
        if(!File.Exists(_filePath))
            return books;

        var lines = File.ReadAllLines(_filePath);

        foreach (var line in lines)
        {
            if(string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('|');
            if(parts.Length < 6 ) continue;

            string id = parts[0].Trim();
            string title = parts[1].Trim();
            string author = parts[2].Trim();
            string isbn = parts[3].Trim();
            
            int.TryParse(parts[4].Trim(), out int totalCopies);
            int.TryParse(parts[5].Trim(), out int availableCopies);
            
            books.Add(new Book(id, title, author, isbn, totalCopies, availableCopies));
        }
        
        return books;
    }

    public Book GetBookById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        return GetAllBooks().FirstOrDefault(b => b.Id.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public void AddBook(Book book)
    {
        var books = GetAllBooks();
        if (books.Any(b => b.ISBN.Equals(book.ISBN.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            throw new Exception("Book with the same ISBN already exists!");
        }
        
        books.Add(book);
        SaveAll(books);
    }

    public void UpdateBook(Book book)
    {
        var books = GetAllBooks();
        var index = books.FindIndex(b => b.Id == book.Id);

        if (index != -1)
        {
            books[index] = book;
            SaveAll(books);
        }
    }

    public void DeleteBook(string id)
    {
        var books = GetAllBooks();
        var bookToRemove = books.FirstOrDefault(b => b.Id == id);

        if (bookToRemove != null)
        {
            books.Remove(bookToRemove);
            SaveAll(books);
        }
    }
    
    private void SaveAll(List<Book> books)
    {
        var lines = new List<string>();

        foreach (var book in books)
        {
            lines.Add($"{book.Id}|{book.Title.Trim()}|{book.Author.Trim()}|{book.ISBN.Trim()}|{book.TotalCopies}|{book.AvailableCopies}");
        }
        
        File.WriteAllLines(_filePath, lines);
    }
}