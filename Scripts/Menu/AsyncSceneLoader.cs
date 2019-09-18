using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AsyncSceneLoader : Singleton<AsyncSceneLoader>
{

	override protected void Init()
	{
		DontDestroyOnLoad(gameObject);
	}

	IEnumerator LoadScene(string sceneName, UnityAction callback = null)
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

	IEnumerator UnloadScene(string sceneName, UnityAction callback = null)
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

	public void DoLoadScene(string sceneName, UnityAction callback = null)
	{
		StartCoroutine(LoadScene(sceneName, callback));
	}

	public void DoUnloadScene(string sceneName, UnityAction callback = null)
	{
		StartCoroutine(UnloadScene(sceneName, callback));
	}

	public void ShowVeil(UnityAction callback = null)
	{
		StartCoroutine(LoadScene("LoadingScreen", () =>
		{
			TransitionVeil.instance.ShowVeil(callback);
		}));
	}

	public void HideVeil(UnityAction callback = null)
	{
		TransitionVeil.instance.HideVeil(() =>
		{
			StartCoroutine(UnloadScene("LoadingScreen", callback));
		});
	}

}
