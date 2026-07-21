namespace Core.Models;

public class Book
{
    public string Id { get; private set; }
    public string Title { get; private set; }
    public string Author { get; private set; }
    public string ISBN { get; private set; }
    public int TotalCopies { get; private set; } //How many pieces are there in total?
    public int AvailableCopies { get; private set; } //How many pieces are free?

    public Book(string id, string title, string author, string isbn, int totalCopies, int availableCopies)
    {
        Id = id;
        Title = title;
        Author = author;
        ISBN = isbn;
        TotalCopies = totalCopies;
        AvailableCopies = availableCopies;
    }
    
    // When a book is withdrawn, the available quantity decreases
    public bool BorrowOneCopy()
    {
        if (AvailableCopies > 0)
        {
            AvailableCopies--;
            return true;
        }
        return false;
    }
    
    // When returning a book, the available quantity increases
    public void ReturnOneCopy()
    {
        if (AvailableCopies < TotalCopies)
        {
            AvailableCopies++;
        }
    }
    
}