using System.Text.Json;
using Core.Interfaces;
using Core.Models;

namespace Repository.Implementations;

public class JsonBookRepository : IBookRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions 
    { 
        WriteIndented = true, //saves nicely formatted JSON
        PropertyNameCaseInsensitive = true
    };
    
    public JsonBookRepository(string filePath)
    {
        _filePath = filePath;
        
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // If the file does not exist or is empty, an empty JSON array "[]" is created
        if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    public List<Book> GetAllBooks()
    {
        if (!File.Exists(_filePath))
            return new List<Book>();

        var json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new List<Book>();

        try
        {
            return JsonSerializer.Deserialize<List<Book>>(json, _jsonOptions) ?? new List<Book>();
        }
        catch
        {
            return new List<Book>();
        }
    }

    public Book? GetBookById(string id)
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
        var json = JsonSerializer.Serialize(books, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }
}