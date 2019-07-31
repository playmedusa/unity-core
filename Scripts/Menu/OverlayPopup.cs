using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mgl;
using TMPro;

public class OverlayPopup : MonoBehaviour
{

	private static OverlayPopup _instance;
	public static OverlayPopup instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<OverlayPopup>();
			if (_instance == null)
			{
				Debug.Log("OverlayPopup not found, instantiating");
				var res = Resources.Load("OverlayPopup", typeof(GameObject)) as GameObject;
				GameObject go = GameObject.Instantiate(res);
				_instance = go.GetComponent<OverlayPopup>();
				_instance.Init();
			}
			return _instance;
		}
	}

	enum states
	{
		closed,
		loading,
		showMessage
	}
	FSMObject<states> fsm;
	bool initialized = false;

	Action callback;

	public CanvasGroup background;

	[Header("Loading popup")]
	public RectTransform loadingPopup;
	public TextMeshProUGUI loadingTitle;
	public Transform loadingWheel;

	[Header("Message popup")]
	public RectTransform messagePopup;
	public TextMeshProUGUI messageTitle;
	public TextMeshProUGUI messageBody;

	public float screenOffset = 325;
	public float rotateSpeed = 250;


	void Awake()
	{
		if (instance != this)
			Destroy(gameObject);

		Init();
	}

	void Init()
	{
		if (initialized) return;

		fsm = new FSMObject<states>(this);
		loadingPopup.anchoredPosition = Vector3.down * screenOffset;
		messagePopup.anchoredPosition = Vector3.down * screenOffset;
		background.interactable = false;
		background.blocksRaycasts = false;
		background.alpha = 0;

		initialized = true;
	}

	public void ShowLoading()
	{
		callback = null;
		messageBody.text = "";
		loadingTitle.text = I18n.T("Please wait...");
		fsm.ChangeState(states.loading);
	}

	public void ShowMessage(string title, string message, Action callback = null)
	{
		this.callback = callback;
		messageTitle.text = I18n.T(title);
		messageBody.text = I18n.T(message);
		fsm.ChangeState(states.showMessage);
	}

	public void ShowError(string message, Action callback = null)
	{
		this.callback = callback;
		messageTitle.text = I18n.T("Oops!");
		messageBody.text = I18n.T(message);
		fsm.ChangeState(states.showMessage);
	}

	public void CloseMessage()
	{
		Debug.Log("Click");
		fsm.ChangeState(states.closed);
	}

	void Update()
	{
		background.alpha = Mathf.Lerp(
			background.alpha,
			(fsm.currentState == states.closed) ? 0 : 1,
			Time.deltaTime * 5
		);
		loadingWheel.Rotate(0, 0, Time.deltaTime * rotateSpeed);
	}

	IEnumerator closed()
	{
		background.interactable = false;
		background.blocksRaycasts = false;

		while (fsm.currentState == states.closed)
		{
			yield return 0;
		}

		background.interactable = true;
		background.blocksRaycasts = true;
	}

	IEnumerator loading()
	{
		yield return this.DoTween01(t =>
		{
			//loadingPopup.localScale = Vector3.one * PennerAnimation.QuadEaseInOut(t, 0, 1, 1);
			loadingPopup.localPosition = new Vector3(
				0,
				PennerAnimation.BackEaseOut(t, -screenOffset, screenOffset, 1),
				0
			);
		}, 0.75f);

		while (fsm.currentState == states.loading)
			yield return 0;

		this.DoTween01(t =>
		{
			//loadingPopup.localScale = Vector3.one * PennerAnimation.QuadEaseInOut(t, 1, -1, 1);
			loadingPopup.localPosition = new Vector3(
				0,
				PennerAnimation.BackEaseIn(t, 0, screenOffset, 1),
				0
			);
		}, 0.5f);
	}

	IEnumerator showMessage()
	{

		yield return this.DoTween01(t =>
		{
			//messagePopup.localScale = Vector3.one * PennerAnimation.QuadEaseInOut(t, 0, 1, 1);
			messagePopup.anchoredPosition = new Vector3(
				0,
				PennerAnimation.BackEaseOut(t, -screenOffset, screenOffset, 1),
				0
			);
		}, 0.75f);

		Debug.Log("Showing message");
		while (fsm.currentState == states.showMessage)
			yield return 0;
		Debug.Log("Message closed");
		this.DoTween01(t =>
		{
			//messagePopup.localScale = Vector3.one * PennerAnimation.QuadEaseInOut(t, 1, -1, 1);
			messagePopup.anchoredPosition = new Vector3(
				0,
				PennerAnimation.BackEaseIn(t, 0, screenOffset, 1),
				0
			);
		}, 0.5f);

		callback?.Invoke();
	}

}
