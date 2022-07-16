using System;

public class PlayersObserveController
{
    public event Action<int> RegisterActivePlayerEvent;

    public int ActivePlayerId;

    public void Initialize()
    {
        ActivePlayerId = -1;
    }

    /// <summary>
    /// Regster player which turn is active.
    /// </summary>
    /// <param name="currentPlayerId"></param>
    public void RegisterActivePlayer(int currentPlayerId)
    {
        ActivePlayerId = currentPlayerId;

        RegisterActivePlayerEvent?.Invoke(ActivePlayerId);
    }

    /// <summary>
    /// Reset current player.
    /// </summary>
    public void UnregisterCurrentPlayer()
    { 
        ActivePlayerId = -1;
    }

    public int GetActivePlayer()
    {
        return ActivePlayerId;
    }
}