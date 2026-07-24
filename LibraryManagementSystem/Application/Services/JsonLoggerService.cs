using System.Text.Json;
using Core.Enums;
using Core.Interfaces;
using Core.Models;

namespace Application.Services;

public class JsonLoggerService : ILoggerService
{
    private readonly string _filePath;
    private readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };
    
    public JsonLoggerService(string filePath = "logs.json")
    {
        _filePath = filePath;

        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
        {
            Directory.CreateDirectory(directory);
        }

        if (!File.Exists(_filePath) || new FileInfo(_filePath).Length == 0)
        {
            File.WriteAllText(_filePath, "[]");
        }
    }
    
    public void LogInfo(string message) => Log(LogLevel.Info, message);
    public void LogWarning(string message) => Log(LogLevel.Warning, message);
    public void LogError(string message) => Log(LogLevel.Error, message);
    
    private void Log(LogLevel level, string message)
    {
        try
        {
            var logs = GetAllLogs();
            logs.Add(new LogEntry(level, message));

            var json = JsonSerializer.Serialize(logs, _jsonOptions);
            File.WriteAllText(_filePath, json);
        }
        catch
        {
            // A logger error should not crash the application
        }
    }
    
    public List<LogEntry> GetAllLogs()
    {
        if (!File.Exists(_filePath))
            return new List<LogEntry>();

        var json = File.ReadAllText(_filePath);
        if (string.IsNullOrWhiteSpace(json))
            return new List<LogEntry>();

        try
        {
            return JsonSerializer.Deserialize<List<LogEntry>>(json, _jsonOptions) ?? new List<LogEntry>();
        }
        catch
        {
            return new List<LogEntry>();
        }
    }
}