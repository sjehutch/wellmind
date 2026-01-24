using Microsoft.Maui.Storage;

namespace WellMind.Services;

public sealed class FileLoggerService : ILoggerService
{
    private const long MaxLogBytes = 512 * 1024;
    private readonly object _sync = new();
    private readonly string _logPath;

    public FileLoggerService()
    {
        _logPath = Path.Combine(FileSystem.AppDataDirectory, "crash.log");
    }

    public string LogFilePath => _logPath;

    public void LogInfo(string message)
    {
        WriteLine($"INFO {message}");
    }

    public void LogException(Exception exception, string context)
    {
        WriteLine($"ERROR {context}{Environment.NewLine}{exception}");
    }

    private void WriteLine(string message)
    {
        lock (_sync)
        {
            RollIfNeeded();
            File.AppendAllText(_logPath, $"{DateTimeOffset.Now:O} {message}{Environment.NewLine}");
        }
    }

    private void RollIfNeeded()
    {
        if (!File.Exists(_logPath))
        {
            return;
        }

        var info = new FileInfo(_logPath);
        if (info.Length <= MaxLogBytes)
        {
            return;
        }

        var backupPath = Path.Combine(info.DirectoryName ?? FileSystem.AppDataDirectory, "crash.log.bak");
        File.Copy(_logPath, backupPath, true);
        File.WriteAllText(_logPath, string.Empty);
    }
}
