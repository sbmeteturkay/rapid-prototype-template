using System;
using System.Collections.Generic;
using System.Linq;
using Cysharp.Threading.Tasks;
using SabanMete.Core.UI;
using UnityEngine;
using UnityEngine.SceneManagement;
using Zenject;

namespace SabanMete.Core.Utils
{
    
        public class GameSceneReadySignal{}
        public class HideLoadingScreenSignal{}
        public class ShowLoadingScreenSignal{}
        public class SceneAlmostReadySignal {}
        public interface ISceneLoader
        {
            UniTask LoadScenesWithProgress(IEnumerable<string> sceneNames);
            void LoadScenesAsync(IEnumerable<string> sceneNames, LoadSceneMode mode = LoadSceneMode.Additive);
            UniTask UnloadSceneAsync(string sceneName);
        }

        public class SceneLoader : ISceneLoader
        {
            private SignalBus signalBus;

            [Inject]
            public SceneLoader(SignalBus signalBus)
            {
                this.signalBus = signalBus;
            }
            
            public async UniTask LoadScenesWithProgress(IEnumerable<string> sceneNames)
            {
                List<AsyncOperation> loadOps = new();

                foreach (var sceneName in sceneNames)
                {
                    var op = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
                    if (op == null)
                        throw new Exception($"Failed to load scene: {sceneName}");

                    op.allowSceneActivation = false;
                    loadOps.Add(op);
                }

                // Hepsi %90'a ulaşana kadar bekle
                while (true)
                {
                    float totalProgress = loadOps.Sum(op => op.progress);
                    float averageProgress = totalProgress / loadOps.Count;

                    signalBus.Fire(new LoadingProgressSignal(averageProgress));

                    bool allReady = loadOps.All(op => op.progress >= 0.9f);
                    if (allReady) break;

                    await UniTask.Yield();
                }

                signalBus.Fire(new SceneAlmostReadySignal());
                await UniTask.Delay(300); // (Opsiyonel: fade-out vs.)

                // Sahne geçişlerini tetikle
                foreach (var op in loadOps)
                    op.allowSceneActivation = true;

                // Tüm işlemler bitene kadar bekle
                while (true)
                {
                    if (loadOps.All(op => op.isDone))
                        break;

                    var totalProgress = loadOps.Sum(op => op.progress);
                    var averageProgress = totalProgress / loadOps.Count;

                    signalBus.Fire(new LoadingProgressSignal(averageProgress));
                    await UniTask.Yield();
                }

                signalBus.Fire(new GameSceneReadySignal());
            }

            public void LoadScenesAsync(IEnumerable<string> sceneNames, LoadSceneMode mode = LoadSceneMode.Additive)
            {
                foreach (var sceneName in sceneNames)
                {
                   SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
                }
            }
            

            public async UniTask UnloadSceneAsync(string sceneName)
            {
                if (!SceneManager.GetSceneByName(sceneName).isLoaded)
                    return;

                var unloadOp = SceneManager.UnloadSceneAsync(sceneName);
                await unloadOp.ToUniTask();
            }
        }
}
