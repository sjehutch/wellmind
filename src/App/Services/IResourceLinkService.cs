using WellMind.Models;

namespace WellMind.Services;

public interface IResourceLinkService
{
    Task<IReadOnlyList<ResourceLink>> GetLinksAsync(CancellationToken cancellationToken = default);
}
