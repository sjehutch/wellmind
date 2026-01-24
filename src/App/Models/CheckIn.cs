namespace WellMind.Models;

public sealed class CheckIn
{
    public DateTime Date { get; init; } = DateTime.UtcNow.Date;
    public int Energy { get; init; }
    public int Stress { get; init; }
    public int Focus { get; init; }
    public int SleepQuality { get; init; }
    public string? Note { get; init; }
}
