using UnityEngine;
using UnityEngine.SceneManagement;
using System;
using System.Collections;
using UnityEngine.Events;

public class SceneLoader : MonoBehaviour
{

	private static SceneLoader _instance;
	public static SceneLoader instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<SceneLoader>();
			if (_instance == null)
			{
				GameObject go = GameObject.Instantiate(Resources.Load("SceneLoader") as GameObject);
				go.name = "SceneLoader";
				_instance = go.GetComponent<SceneLoader>();
			}
			return _instance;
		}
	}

	CanvasGroup fadeCanvasGroup;
	public AudioSource music;
	public float timeOut;
	public float fadeInDelay;
	public float defaultFadeTime = 0.5f;
	public UnityEvent onTimeOut;

	void Awake()
	{
		fadeCanvasGroup = GetComponent<CanvasGroup>();
		FadeIn();
		if (onTimeOut != null)
			Invoke("TimeOut", timeOut);
	}

	void TimeOut()
	{
		onTimeOut.Invoke();
	}

	public void Load(string sceneName)
	{
		StartCoroutine(FadeOutAndLoad(sceneName, defaultFadeTime, 0));
	}

	public void Load(string sceneName, Action callback)
	{
		StartCoroutine(FadeOutAndLoad(sceneName, defaultFadeTime, 0, callback));
	}

	public void Load(string sceneName, float animationTime, float waitTime)
	{
		StartCoroutine(FadeOutAndLoad(sceneName, animationTime, waitTime));
	}

	public void FadeIn()
	{
		StartCoroutine(FadeIn(defaultFadeTime));
	}

	IEnumerator FadeIn(float animationTime)
	{
		fadeCanvasGroup.alpha = 1.0f;
		yield return new WaitForSeconds(fadeInDelay);
		yield return this.DoTween01(t =>
		{
			fadeCanvasGroup.alpha = PennerAnimation.QuadEaseInOut(t, 1, -1, 1);
		}, animationTime);
	}

	IEnumerator FadeOutAndLoad(string sceneName, float animationTime, float waitTime, Action callback = null)
	{
		fadeCanvasGroup.alpha = 0;
		yield return this.DoUnscaledTween01(t =>
		{
			if (music != null)
				music.volume = PennerAnimation.CubicEaseOut(t, 0, 1, 1);
			fadeCanvasGroup.alpha = PennerAnimation.QuadEaseInOut(t, 0, 1, 1);
		}, animationTime, () =>
		{
			callback?.Invoke();
		});

		fadeCanvasGroup.alpha = 1.0f;
		if (music != null)
			music.volume = 0;
		if (waitTime > 0)
			yield return new WaitForSeconds(waitTime);

		SceneManager.LoadScene(sceneName);
	}

}
