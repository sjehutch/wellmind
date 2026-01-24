using WellMind.Models;

namespace WellMind.Services;

public sealed class InMemoryResourceLinkService : IResourceLinkService
{
    public Task<IReadOnlyList<ResourceLink>> GetLinksAsync(CancellationToken cancellationToken = default)
    {
        IReadOnlyList<ResourceLink> links =
        [
            new ResourceLink
            {
                Title = "WHO: stress basics",
                Url = "https://www.who.int/news-room/questions-and-answers/item/stress"
            },
            new ResourceLink
            {
                Title = "NIH: sleep hygiene",
                Url = "https://www.nhlbi.nih.gov/health/sleep-deprivation"
            },
            new ResourceLink
            {
                Title = "Headspace: trial",
                Url = "https://www.headspace.com/" 
            }
        ];

        return Task.FromResult(links);
    }
}
