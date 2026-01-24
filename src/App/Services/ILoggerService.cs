namespace WellMind.Services;

public interface ILoggerService
{
    string LogFilePath { get; }
    void LogInfo(string message);
    void LogException(Exception exception, string context);
}
