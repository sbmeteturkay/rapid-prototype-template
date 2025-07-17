using Cysharp.Threading.Tasks;
using SabanMete.Core.UI;
using SabanMete.Core.Utils;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SabanMete.Core.GameStates
{
    public class GameStateManager : IGameStateManager, IInitializable
    {
        private readonly ISceneLoader sceneLoader;
        public GameState Current { get; private set; }


        public GameStateManager(ISceneLoader sceneLoader)
        {
            this.sceneLoader = sceneLoader;
        }
        public void Initialize()
        {
            sceneLoader.LoadScenesAsync(new[] { "MainScene", "UIScene"});
            Current = GameState.MainMenu;
        }
        public void SetState(GameState newState)
        {
            if (Current == newState)
                return;

            Current = newState;
            _ = HandleStateAsync(newState);
        }

        private async UniTaskVoid HandleStateAsync(GameState state)
        {
            switch (state)
            {
                case GameState.Boot:
                    // Boot sahnesi zaten açık
                    break;

                case GameState.MainMenu:
                    await sceneLoader.UnloadSceneAsync("GameScene");
                    await sceneLoader.LoadScenesWithProgress(new[] { "MainScene", "UIScene" });
                    break;

                case GameState.Gameplay:
                    await sceneLoader.UnloadSceneAsync("MainScene");
                    await sceneLoader.LoadScenesWithProgress(new[] { "GameScene" });
                    break;

                case GameState.GameOver:
                    // GameScene açık kalır, UI ile kontrol edilir
                    Debug.Log("Game Over. No scene transition.");
                    break;
            }
        }


    }
}
