namespace WellMind.Models;

public sealed class Trend
{
    public string Label { get; init; } = string.Empty;
    public IReadOnlyList<int> Values { get; init; } = Array.Empty<int>();
    public string Summary { get; init; } = string.Empty;
    public int? LastValue { get; init; }
    public int? Min { get; init; }
    public int? Max { get; init; }
    public double? Average { get; init; }
}
