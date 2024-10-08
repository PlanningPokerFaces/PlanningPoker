using PlanningPoker.UseCases.Data;

namespace PlanningPoker.Website.Components.Layout;

public partial class PokerRoomLayout
{
    private PokerGameData? currentPokerGame;

    public async Task SetCurrentPokerGameAsync(PokerGameData pokerGame)
    {
        currentPokerGame = pokerGame;
        await InvokeAsync(StateHasChanged);
    }
}
