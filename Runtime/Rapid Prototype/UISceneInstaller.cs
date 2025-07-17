using PrimeTween;
using SabanMete.Core.Utils;
using UnityEngine;
using Zenject;

namespace SabanMete.Core.UI
{
    public class UISceneInstaller : MonoInstaller
    {
        [Inject] private SignalBus signalBus;
        public override void InstallBindings()
        {
            Debug.Log("ui scene installer");
        }

        public override void Start()
        {
            base.Start();
            signalBus.Fire(new ShowLoadingScreenSignal());
            Tween.Custom(
                0f,
                100f,
                duration: 5f,
                value => {
                    signalBus.Fire(new LoadingProgressSignal(value));
                },Ease.InOutSine
            ).OnComplete(() => {
                signalBus.Fire(new HideLoadingScreenSignal());
            });
        }
    }
}