using System;
using SabanMete.Core.Utils;
using Zenject;

namespace SabanMete.Core.UI
{
    public class LoadingScreenController : IInitializable, IDisposable
    {
        private readonly SignalBus signalBus;
        private readonly ILoadingScreenService loadingScreenService;

        public LoadingScreenController(SignalBus signalBus,ILoadingScreenService loadingScreenService)
        {
            this.signalBus = signalBus;
            this.loadingScreenService = loadingScreenService;
        }

        public void Initialize()
        {
            signalBus.Subscribe<GameSceneReadySignal>(OnGameSceneReady);
        }

        public void Dispose()
        {
            signalBus.Unsubscribe<GameSceneReadySignal>(OnGameSceneReady);
        }

        private async void OnGameSceneReady()
        {
            await loadingScreenService.HideAsync();
        }
    }
}