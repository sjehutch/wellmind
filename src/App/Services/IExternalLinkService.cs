namespace WellMind.Services;

public interface IExternalLinkService
{
    Task OpenAsync(string url, CancellationToken cancellationToken = default);
}
