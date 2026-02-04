namespace WellMind.Services;

public interface ITipFeedbackStore
{
    Task<int> GetHelpfulCountAsync(string tipId, CancellationToken ct = default);
    Task MarkHelpfulAsync(string tipId, CancellationToken ct = default);
}
