using Core.Models;

namespace Core.Interfaces;

public interface ILoggerService
{
    void LogInfo(string message);
    void LogWarning(string message);
    void LogError(string message);
    
    // If the admin wants to see the logs
    List<LogEntry> GetAllLogs(); 
}