﻿using UnityEngine;
using UnityEngine.SceneManagement;
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
	public UnityEvent onTimeOut;

	void Awake()
	{
		fadeCanvasGroup = GetComponent<CanvasGroup>();
		StartCoroutine(FadeIn());
		if (onTimeOut != null)
			Invoke("TimeOut", timeOut);
	}

	void TimeOut()
	{
		onTimeOut.Invoke();
	}

	public void Load(string sceneName)
	{
		StartCoroutine(FadeOutAndLoad(sceneName, 0.5f, 0));
	}

	public void Load(string sceneName, float animationTime = 0.5f, float waitTime = 0f)
	{
		StartCoroutine(FadeOutAndLoad(sceneName, animationTime, waitTime));
	}

	IEnumerator FadeIn(float animationTime = 0.5f)
	{
		fadeCanvasGroup.alpha = 1.0f;
		float elapsedTime = 0;
		while (elapsedTime < animationTime)
		{
			fadeCanvasGroup.alpha = PennerAnimation.SineEaseOut(elapsedTime / animationTime, 1, -1, 1);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		fadeCanvasGroup.alpha = 0;
	}

	IEnumerator FadeOutAndLoad(string sceneName, float animationTime, float waitTime)
	{
		fadeCanvasGroup.alpha = 0;
		float elapsedTime = 0;
		while (elapsedTime < animationTime)
		{
			if (music != null)
				music.volume = PennerAnimation.CubicEaseOut(elapsedTime / animationTime, 0, 1, 1);
			fadeCanvasGroup.alpha = PennerAnimation.SineEaseOut(elapsedTime / animationTime, 0, 1, 1);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		fadeCanvasGroup.alpha = 1.0f;
		if (music != null)
			music.volume = 0;
		if (waitTime > 0)
			yield return new WaitForSeconds(waitTime);
		SceneManager.LoadScene(sceneName);
	}

}
