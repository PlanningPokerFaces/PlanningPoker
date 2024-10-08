using Microsoft.Extensions.Caching.Memory;
using PlanningPoker.UseCases.ChooseStory;

namespace PlanningPoker.Infrastructure.DataProvider.InMemory;

public class ViewSettingsCache(IMemoryCache cache) : IViewSettingsCache
{
    public ViewSettings? Get(string pokerGameId)
    {
        cache.TryGetValue(pokerGameId, out ViewSettings? viewSettings);
        return viewSettings;
    }

    public void Set(string pokerGameId, ViewSettings viewSettings)
    {
        cache.Set(pokerGameId, viewSettings);
    }
}
