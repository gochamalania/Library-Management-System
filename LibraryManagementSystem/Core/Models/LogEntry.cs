using Core.Enums;

namespace Core.Models;

public class LogEntry
{
    public string Id { get; set; } = Guid.NewGuid().ToString();
    public DateTime Timestamp { get; set; } = DateTime.Now;
    public LogLevel Level { get; set; }
    public string Message { get; set; } = string.Empty;
    
    // Constructor for JSON deserialization
    public LogEntry() { }

    public LogEntry(LogLevel level, string message)
    {
        Id = Guid.NewGuid().ToString();
        Timestamp = DateTime.Now;
        Level = level;
        Message = message;
    }
}