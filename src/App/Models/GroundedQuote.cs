namespace WellMind.Models;

public sealed class GroundedQuote
{
    public string Id { get; init; } = "";
    public string Text { get; init; } = "";
    public string Author { get; init; } = "";
    public IReadOnlyList<string> Tags { get; init; } = Array.Empty<string>();
    public string Tone { get; init; } = "";
}
