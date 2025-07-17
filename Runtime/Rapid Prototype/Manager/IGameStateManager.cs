namespace SabanMete.Core.GameStates
{
    public enum GameState
    {
        Boot,
        MainMenu,
        Gameplay,
        GameOver
    }

    public interface IGameStateManager
    {
        GameState Current { get; }
        void SetState(GameState newState);
    }
}