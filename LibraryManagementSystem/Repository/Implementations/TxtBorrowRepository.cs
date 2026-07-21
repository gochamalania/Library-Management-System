using System.Globalization;
using Core.Models;
using Repository.Interfaces;


namespace Repository.Implementations;

public class TxtBorrowRepository : IBorrowRepository
{
    private readonly string _filePath;
    private const string DateFormat = "yyyy-MM-dd HH:mm:ss";
    
    public TxtBorrowRepository(string filePath)
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

    public List<BorrowRecord> GetAllBorrows()
    {
        var records = new List<BorrowRecord>();
        if(!File.Exists(_filePath)) return records;

        var lines = File.ReadAllLines(_filePath);
        foreach (var line in lines)
        {
            if (string.IsNullOrWhiteSpace(line)) continue;

            var parts = line.Split('|');
            if (parts.Length < 7) continue;

            string id = parts[0].Trim();
            string userId = parts[1].Trim();
            string bookId = parts[2].Trim();

            DateTime.TryParseExact(parts[3].Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var borrowDate);
            DateTime.TryParseExact(parts[4].Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dueDate);

            DateTime? returnDate = null;
            if (!string.IsNullOrWhiteSpace(parts[5]) && DateTime.TryParseExact(parts[5].Trim(), DateFormat, CultureInfo.InvariantCulture, DateTimeStyles.None, out var parsedReturnDate))
            {
                returnDate = parsedReturnDate;
            }

            bool.TryParse(parts[6].Trim(), out var isReturned);

            records.Add(new BorrowRecord(id, userId, bookId, borrowDate, dueDate, returnDate, isReturned));
        }
        return records;
    }

    public BorrowRecord GetById(string id)
    {
        if (string.IsNullOrWhiteSpace(id)) return null;
        return GetAllBorrows().FirstOrDefault(b => b.Id.Equals(id.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public void AddBorrow(BorrowRecord record)
    {
        var records = GetAllBorrows();
        records.Add(record);
        SaveAll(records);
    }
    
    public void UpdateBorrow(BorrowRecord record)
    {
        var records = GetAllBorrows();
        var index = records.FindIndex(r => r.Id == record.Id);

        if (index != -1)
        {
            records[index] = record;
            SaveAll(records);
        }
    }

    private void SaveAll(List<BorrowRecord> records)
    {
        var lines = new List<string>();
        
        foreach (var r in records)
        {
            string returnDateStr = r.ReturnDate.HasValue ? r.ReturnDate.Value.ToString(DateFormat) : "";
            lines.Add($"{r.Id}|{r.UserId}|{r.BookId}|{r.BorrowDate.ToString(DateFormat)}|{r.DueDate.ToString(DateFormat)}|{returnDateStr}|{r.IsReturned}");
        }

        File.WriteAllLines(_filePath, lines);
    }
}