using System.Text.Json;
using Core.Interfaces;
using Core.Models;

namespace Repository.Implementations;

public class JsonBorrowRepository : IBorrowRepository
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions 
    { 
        WriteIndented = true, // saves nicely formatted Json
        PropertyNameCaseInsensitive = true
    };
    
    public JsonBorrowRepository(string filePath)
    {
        _filePath = filePath;
        string? directory = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrWhiteSpace(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }
        
        // If the file does not exist or is empty, an empty JSON array "[]" is created
        if (!File.Exists(filePath) || new FileInfo(filePath).Length == 0)
        {
            File.WriteAllText(_filePath, "[]");
        }
    }
    
    public List<BorrowRecord> GetAllBorrows()
    {
        if (!File.Exists(_filePath))
            return new List<BorrowRecord>();

        var json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new List<BorrowRecord>();

        try
        {
            return JsonSerializer.Deserialize<List<BorrowRecord>>(json, _jsonOptions) ?? new List<BorrowRecord>();
        }
        catch
        {
            return new List<BorrowRecord>();
        }
    }

    public BorrowRecord? GetById(string id)
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
        var json = JsonSerializer.Serialize(records, _jsonOptions);
        File.WriteAllText(_filePath, json);
    }
}