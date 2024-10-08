using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.Extensions.Logging;

namespace PlanningPoker.UseCases;

public class CurrentUserContext(ProtectedLocalStorage protectedLocalStorage, ILogger<CurrentUserContext> logger)
    : ICurrentUserContext
{
    private const string identifier = "PlanningPokerParticipantName";
    private const string scope = "PlanningPokerApp";
    private UserContextData? cachedUser;

    public async Task SetAsync(UserContextData user)
    {
        await protectedLocalStorage.SetAsync(scope, identifier, user);
        cachedUser = user;
    }

    public async Task<UserContextData?> GetAsync()
    {
        if (cachedUser is not null)
        {
            return cachedUser;
        }

        try
        {
            var value = await protectedLocalStorage.GetAsync<UserContextData>(scope, identifier);
            return value.Value;
        }
        catch (Exception e) when (e is CryptographicException or JsonException)
        {
            logger.LogWarning("Outdated/undecryptable key found in local storage");
        }

        return null;
    }

    public sealed record UserContextData(string Id, string Name, string? AvatarUrl);
}
