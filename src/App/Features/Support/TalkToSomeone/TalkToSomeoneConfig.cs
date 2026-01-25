using System.Text.Json.Serialization;

namespace WellMind.Features.Support.TalkToSomeone;

public sealed class TalkToSomeoneConfig
{
    public bool Enabled { get; init; }
    public string IntroTitle { get; init; } = "";
    public string IntroBody { get; init; } = "";
    public List<TalkToSomeoneOption> Options { get; init; } = new();

    // This flag is set by the loader so the UI can show a safe empty state.
    [JsonIgnore]
    public bool LoadFailed { get; init; }
}

public sealed class TalkToSomeoneOption
{
    public string Id { get; init; } = "";
    public string Title { get; init; } = "";
    public string Body { get; init; } = "";
    public bool Enabled { get; init; }
}
