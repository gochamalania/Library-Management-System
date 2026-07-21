namespace Core.Models;

public class BorrowRecord
{
    public string Id { get; private set; }
    public string UserId { get; private set; }
    public string BookId { get; private set; }
    public DateTime BorrowDate { get; private set; }
    public DateTime DueDate { get; private set; } //When the book should be returned(e.g. 7 days)
    public DateTime? ReturnDate { get; private set; } //When the book was returned
    public bool IsReturned { get; private set; }
    
    public BorrowRecord(string id, string userId, string bookId, DateTime borrowDate, DateTime dueDate, DateTime? returnDate = null, bool isReturned = false)
    {
        Id = id;
        UserId = userId;
        BookId = bookId;
        BorrowDate = borrowDate;
        DueDate = dueDate;
        ReturnDate = returnDate;
        IsReturned = isReturned;
    }
    
    public void MarkAsReturned()
    {
        IsReturned = true;
        ReturnDate = DateTime.Now;
    }
}