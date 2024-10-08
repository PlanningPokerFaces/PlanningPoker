namespace PlanningPoker.UseCases.ChooseStory;

public interface IViewSettingsCache
{
    ViewSettings? Get(string pokerGameId);
    void Set(string pokerGameId, ViewSettings viewSettings);
}
