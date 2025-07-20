using System;
using Cysharp.Threading.Tasks;
using PrimeTween;
using SabanMete.Core.Utils;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace SabanMete.Core.UI
{
    public interface ILoadingScreenService
    {
        void Show(string message = "",float duration=1);
        void Hide();
    }
    public struct LoadingProgressSignal
    {
        public float Progress;

        public LoadingProgressSignal(float progress)
        {
            Progress = progress;
        }
    }
    public class LoadingScreenService : MonoBehaviour, ILoadingScreenService
    {
        [SerializeField] private CanvasGroup rootCanvas;
        [SerializeField] private TextMeshProUGUI messageText;
        [SerializeField] private Slider progressBar;
        
        [Inject]private SignalBus signalBus;

        private void Awake()
        {
            signalBus.Subscribe<GameSceneReadySignal>(OnGameSceneReady);
            signalBus.Subscribe<ShowLoadingScreenSignal>(OnShowLoadingRequested);
            signalBus.Subscribe<HideLoadingScreenSignal>(OnHideLoadingRequested);
            signalBus.Subscribe<LoadingProgressSignal>(OnLoadingProgressSignal);
        }

        private void OnLoadingProgressSignal(LoadingProgressSignal signal)
        {
            progressBar.value = signal.Progress;
        }

        private void OnShowLoadingRequested()
        {
             Show("Loading...",0f);
        }

        private  void OnHideLoadingRequested()
        {
             Hide();
        }

        private void OnGameSceneReady()
        {
            signalBus.Fire<HideLoadingScreenSignal>();
        }
        public void Show(string message = "Loading...",float duration = 1.0f)
        {
            messageText.text = message;
            if (duration > 0)
            {
                Tween.Custom(
                    0f,
                    1f,
                    duration: duration,
                    value => {
                        rootCanvas.alpha = value;
                    },Ease.InOutSine
                ).OnComplete(() => {
                    rootCanvas.alpha = 1;
                    rootCanvas.interactable = true;
                    rootCanvas.blocksRaycasts = true;
                }); 
            }
            else
            {
                rootCanvas.alpha = 1;
                rootCanvas.interactable = true;
                rootCanvas.blocksRaycasts = true;
            }
        }

        public void  Hide()
        {
            if (rootCanvas == null) return;
            Tween.Custom(
                1f,
                0f,
                duration: .5f,
                value => {
                    rootCanvas.alpha = value;
                },Ease.InOutSine
            ).OnComplete(() => {
                rootCanvas.alpha = 0;
                rootCanvas.interactable = false;
                rootCanvas.blocksRaycasts = false;
            });
        }
        private void OnDestroy()
        {
            signalBus?.Unsubscribe<GameSceneReadySignal>(OnGameSceneReady);
            signalBus?.Unsubscribe<ShowLoadingScreenSignal>(OnShowLoadingRequested);
            signalBus?.Unsubscribe<HideLoadingScreenSignal>(OnHideLoadingRequested);
            signalBus?.Unsubscribe<LoadingProgressSignal>(OnLoadingProgressSignal);

        }
    }
}