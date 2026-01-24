namespace WellMind.Models;

public sealed class CheckIn
{
    public string DateLocal { get; init; } = "";
    public string TimestampLocal { get; init; } = "";
    public int Energy { get; init; }
    public int Stress { get; init; }
    public int Focus { get; init; }
    public int SleepQuality { get; init; }
    public string? Note { get; init; }
}
