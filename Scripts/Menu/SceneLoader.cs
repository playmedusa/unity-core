﻿using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
				var res = Resources.Load("SceneLoader", typeof(GameObject)) as GameObject;
				if (res == null)
					res = Resources.Load("SceneLoader_Default", typeof(GameObject)) as GameObject;
				res.name = "SceneLoader";
				GameObject go = Instantiate(res);
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
	public TextMeshProUGUI loadingText;
	public bool showLoadingTextOnFadeIn;
    
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
		if (loadingText != null && showLoadingTextOnFadeIn == false)
			loadingText.gameObject.SetActive(false);
		StartCoroutine(FadeIn(defaultFadeTime));
	}

	protected virtual IEnumerator FadeIn(float animationTime)
	{
		fadeCanvasGroup.alpha = 1.0f;
		yield return new WaitForSeconds(fadeInDelay);
		yield return this.DoTween01(t =>
		{
			fadeCanvasGroup.alpha = PennerAnimation.QuadEaseInOut(t, 1, -1, 1);
		}, animationTime);
	}

	protected virtual IEnumerator FadeOutAndLoad(string sceneName, float animationTime, float waitTime, Action callback = null)
	{
		fadeCanvasGroup.alpha = 0;
		if (loadingText != null)
			loadingText.gameObject.SetActive(true);
		yield return this.DoUnscaledTween01(t =>
		{
			if (music != null)
				music.volume = PennerAnimation.CubicEaseOut(t, 1, -1, 1);
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
