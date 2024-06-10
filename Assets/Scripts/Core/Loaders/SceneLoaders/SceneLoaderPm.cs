using System;
using Core.Shared.Foundation;
using Core.Shared.Loaders;
using UniRx;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Loaders.SceneLoaders
{
	public class SceneLoaderPm : BaseDisposable, ISceneLoaderMaster
	{
    private bool _isLoading;
    private IDisposable _unloadingLevel;
    private IDisposable _loadingLevel;

    public SceneLoaderPm()
    {
      //log.Mute = true;
      _isLoading = false;
    }

    protected override void OnDispose()
    {
      reset();
      base.OnDispose();
    }

    private void reset()
    {
      _isLoading = false;
      _unloadingLevel?.Dispose();
      _loadingLevel?.Dispose();
    }
    
    void ISceneLoaderMaster.ChangeScene(string sceneName, Action onUnload, Action onComplete)
    {
      if (_isLoading)
      {
        Log.Err($"Can't start load {sceneName}. Level loader is busy");
        onComplete?.Invoke();
        return;
      }
      _isLoading = true;
      Log.Info($"Trying to load scene {sceneName}");
      Scene oldScene = SceneManager.GetActiveScene();
      onNewSceneLoadedToMemory(true);
      //_ctx.resourceLoader.LoadSceneBundle(sceneName, OnNewSceneLoadedToMemory);

      void onNewSceneLoadedToMemory(bool result)
      {
        Log.Info($"Scene {sceneName} bundle loaded to memory");
        loadSceneAsync(sceneName, onNewSceneLoaded);
      }

      void onNewSceneLoaded()
      {
        Log.Info($"Scene {sceneName} loaded");
        _isLoading = false;
        _loadingLevel?.Dispose();
        onUnload?.Invoke();
        tryUnloadScene(oldScene, onComplete);
      }
    }

    void ISceneLoader.LoadScene(string sceneName, Action<Scene> onComplete)
    {
      if (_isLoading)
      {
        Log.Err($"Can't start load {sceneName}. Level loader is busy");
        onComplete?.Invoke(SceneManager.GetActiveScene());
        return;
      }
      _isLoading = true;
      Log.Info($"Trying to load scene {sceneName}");
      int sceneCount = SceneManager.sceneCount;
      loadSceneAsync(sceneName, onNewSceneLoaded, LocalPhysicsMode.Physics2D | LocalPhysicsMode.Physics3D);
      
      void onNewSceneLoaded()
      {
        Log.Info($"Scene {sceneName} loaded");
        _isLoading = false;
        _loadingLevel?.Dispose(); 
        Scene newScene = SceneManager.GetSceneAt(sceneCount);
        onComplete?.Invoke(newScene);
      }
    }

    void ISceneLoader.UnloadScene(Scene scene)
    {
      try
      {
        SceneManager.UnloadSceneAsync(scene);
      }
      catch (Exception e)
      {
        Log.Err($"Exception in unloading scene {e}.");
        throw;
      }
    }
    
    private void loadSceneAsync(string name, Action onComplete, LocalPhysicsMode physicsMode = LocalPhysicsMode.None)
    {
      // Resources.UnloadUnusedAssets();
      _loadingLevel?.Dispose();
      //AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync(name,new LoadSceneParameters(LoadSceneMode.Additive, LocalPhysicsMode.Physics2D | LocalPhysicsMode.Physics3D));
#if false
      AsyncOperation loadingSceneOp = EditorSceneManager.LoadSceneAsyncInPlayMode(name, new LoadSceneParameters(LoadSceneMode.Additive, physicsMode));
#else
      AsyncOperation loadingSceneOp = SceneManager.LoadSceneAsync(name, new LoadSceneParameters(LoadSceneMode.Additive, physicsMode));
#endif
      _loadingLevel = loadingSceneOp.AsAsyncOperationObservable().Take(1).Subscribe(_ => { onComplete?.Invoke(); });
    }

    private void tryUnloadScene(Scene sceneToUnload, Action onComplete)
    {
      _unloadingLevel?.Dispose();
      try
      {
        // SceneManager.CreateScene(EMPTY_SCENE_NAME);
        AsyncOperation unloadingSceneOp = SceneManager.UnloadSceneAsync(sceneToUnload);
        if (unloadingSceneOp == null)
        {
          onComplete?.Invoke();
          return;
        }
        _unloadingLevel = unloadingSceneOp.AsAsyncOperationObservable().Take(1)
          .Subscribe(_ => { onComplete?.Invoke(); });
      }
      catch (Exception e)
      {
        Log.Err($"Exception in unloading scene {e}.");
        onComplete?.Invoke();
      }
    }
	}
}