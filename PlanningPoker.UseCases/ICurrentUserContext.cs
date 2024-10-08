namespace PlanningPoker.UseCases;

public interface ICurrentUserContext
{
    Task SetAsync(CurrentUserContext.UserContextData user);
    Task<CurrentUserContext.UserContextData?> GetAsync();
}
