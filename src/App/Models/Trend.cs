namespace WellMind.Models;

public sealed class Trend
{
    public string Label { get; init; } = string.Empty;
    public IReadOnlyList<int> Values { get; init; } = Array.Empty<int>();
}
