using SabanMete.Core.UI;
using SabanMete.Core.Utils;
using UnityEngine;
using Zenject;

namespace SabanMete.Core.GameStates
{
    public class ProjectInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("project installer");
            SignalBusInstaller.Install(Container);
            Container.DeclareSignal<GameSceneReadySignal>();
            Container.DeclareSignal<ShowLoadingScreenSignal>();
            Container.DeclareSignal<HideLoadingScreenSignal>();
            Container.DeclareSignal<LoadingProgressSignal>();
        }
    }


}