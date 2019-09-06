using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
		showMessage,
		showChoose
	}
	FSMObject<states> fsm;
	bool initialized = false;

	Action callback;

	public RectTransform canvasRT;
	public CanvasGroup background;

	[Header("Loading popup")]
	public RectTransform loadingPopup;
	public TextMeshProUGUI loadingTitle;
	public Transform loadingWheel;

	[Header("Message popup")]
	public RectTransform messagePopup;
	public TextMeshProUGUI messageTitle;
	public TextMeshProUGUI messageBody;

	[Header("Choose popup")]
	public RectTransform choosePopup;
	public TextMeshProUGUI chooseTitle;
	public TextMeshProUGUI chooseBody;
	Action yesCallback = null;
	Action noCallback = null;

	public float screenOffset
	{
		get
		{
			return canvasRT.sizeDelta.y;
		}
	}

	[Header("General settings")]
	public float rotateSpeed = 50;


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
		choosePopup.anchoredPosition = Vector3.down * screenOffset;

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

	public void ShowChoose(string title, string message, Action yesCallback = null, Action noCallback = null)
	{
		this.yesCallback = yesCallback;
		this.noCallback = noCallback;
		chooseTitle.text = I18n.T(title);
		chooseBody.text = I18n.T(message);
		fsm.ChangeState(states.showChoose);
	}

	public void ClosePopup()
	{
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

	IEnumerator showView(RectTransform holder, states state)
	{
		loadingPopup.anchoredPosition = Vector3.down * screenOffset;
		messagePopup.anchoredPosition = Vector3.down * screenOffset;
		choosePopup.anchoredPosition = Vector3.down * screenOffset;

		yield return this.DoTween01(t =>
		{
			//loadingPopup.localScale = Vector3.one * PennerAnimation.QuadEaseInOut(t, 0, 1, 1);
			holder.localPosition = new Vector3(
				0,
				PennerAnimation.BackEaseOut(t, -screenOffset, screenOffset, 1),
				0
			);
		}, 0.75f);

		background.interactable = true;
		background.blocksRaycasts = true;

		while (fsm.currentState == state)
			yield return 0;

		this.DoTween01(t =>
		{
			//loadingPopup.localScale = Vector3.one * PennerAnimation.QuadEaseInOut(t, 1, -1, 1);
			holder.localPosition = new Vector3(
				0,
				PennerAnimation.BackEaseIn(t, 0, screenOffset, 1),
				0
			);
		}, 0.5f);
	}

	IEnumerator loading()
	{
		yield return StartCoroutine(showView(loadingPopup, states.loading));
	}

	IEnumerator showMessage()
	{
		yield return StartCoroutine(showView(messagePopup, states.showMessage));
		callback?.Invoke();
	}

	IEnumerator showChoose()
	{
		yield return StartCoroutine(showView(choosePopup, states.showChoose));
	}

	public void YesButton()
	{
		yesCallback?.Invoke();
		ClosePopup();
	}

	public void NoButton()
	{
		noCallback?.Invoke();
		ClosePopup();
	}

}
