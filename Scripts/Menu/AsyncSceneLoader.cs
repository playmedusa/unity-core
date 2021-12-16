using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : Singleton<AsyncSceneLoader>
{

	override protected void Init()
	{
		DontDestroyOnLoad(gameObject);
	}

	static public void LoadScene(string sceneName, UnityAction callback = null)
	{
		instance.StartCoroutine(instance.DoLoadScene(sceneName, callback));
	}

	static public void UnloadScene(string sceneName, UnityAction callback = null)
	{
		instance.StartCoroutine(instance.DoUnloadScene(sceneName, callback));
	}

	IEnumerator DoLoadScene(string sceneName, UnityAction callback = null)
	{
		var asyncScene = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
		float _loadingProgress;

		// this value stops the scene from displaying when it's finished loading
		asyncScene.allowSceneActivation = false;

		while (!asyncScene.isDone)
		{
			// loading bar progress
			_loadingProgress = Mathf.Clamp01(asyncScene.progress / 0.9f) * 100;

			// scene has loaded as much as possible, the last 10% can't be multi-threaded
			if (asyncScene.progress >= 0.9f)
			{
				// we finally show the scene
				asyncScene.allowSceneActivation = true;
			}

			yield return null;
		}
		callback?.Invoke();
	}

	IEnumerator DoUnloadScene(string sceneName, UnityAction callback = null)
	{
		var asyncScene = SceneManager.UnloadSceneAsync(sceneName);
		float _unloadingProgress;

		// this value stops the scene from displaying when it's finished loading
		asyncScene.allowSceneActivation = false;

		while (!asyncScene.isDone)
		{
			// loading bar progress
			_unloadingProgress = Mathf.Clamp01(asyncScene.progress / 0.9f) * 100;
			// scene has loaded as much as possible, the last 10% can't be multi-threaded
			if (asyncScene.progress >= 0.9f)
			{
				// we finally show the scene
				asyncScene.allowSceneActivation = true;
			}

			yield return null;
		}
		callback?.Invoke();
	}

}
