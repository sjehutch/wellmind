namespace WellMind.Services;

public sealed class ExternalLinkService : IExternalLinkService
{
    public async Task OpenAsync(string url, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(url))
        {
            return;
        }

        if (Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            await Launcher.OpenAsync(uri);
        }
    }
}
