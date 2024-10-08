namespace PlanningPoker.Core.Entities;

/// <summary>
/// The possible states the <see cref="PokerGame">PokerGame</see> can be in
/// <remarks>
///In <see cref="FirstVoted">FirstVoted</see> and <see cref="AllVoted">AllVoted</see> it is still possible to switch the story and the vote.
///In <see cref="Revealed">Revealed</see> it is not possible to change the vote or the story.
///After <see cref="Revealed">Revealed</see> we jump back to <see cref="OpenForVote">OpenForVote</see>.
/// </remarks>
/// </summary>
public enum GameState
{
    /// <summary>
    /// is set, when starting a new game
    /// </summary>
    NoStorySelected,

    /// <summary>
    /// Is set, when story is selected but no one voted yet
    /// </summary>
    OpenForVote,

    /// <summary>
    /// Is set, when at least one player has chosen a card
    /// </summary>
    FirstVoted,

    /// <summary>
    /// Is set, when all players have chosen a card
    /// </summary>
    AllVoted,

    /// <summary>
    /// Is set, when someone pressed reveal and the calculated value is shown
    /// </summary>
    Revealed,
}
