using SabanMete.Core.GameStates;
using SabanMete.Core.UI;
using SabanMete.Core.Utils;
using UnityEngine;
using Zenject;

namespace SabanMete.Core.Installers
{
    public class BootSceneInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Debug.Log("BootSceneInstaller.InstallBindings");
            //signals
            //bindings
            Container.Bind<ISceneLoader>().To<SceneLoader>().AsSingle();
            Container.BindInterfacesAndSelfTo<GameStateManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<BootManager>().AsSingle();
        }
    }

}