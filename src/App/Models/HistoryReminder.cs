using System.Text.Json.Serialization;

namespace WellMind.Models;

public sealed record HistoryReminder
{
    [JsonPropertyName("month")]
    public int Month { get; init; }

    [JsonPropertyName("day")]
    public int Day { get; init; }

    [JsonPropertyName("event")]
    public string Event { get; init; } = "";

    [JsonPropertyName("reflection")]
    public string Reflection { get; init; } = "";
}
