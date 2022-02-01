using System;
using System.Collections;
using System.Threading.Tasks;
using Mgl;
using TMPro;
using UnityEngine;

public class OverlayPopup : Singleton<OverlayPopup>
{
	enum states
	{
		closed,
		loading,
		showMessage,
		showChoose,
		showInput
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
	public TextMeshProUGUI yesText;
	public TextMeshProUGUI noText;
	Action yesCallback = null;
	Action noCallback = null;

	[Header("Input popup")]
	public RectTransform inputPopup;
	public TextMeshProUGUI inputTitle;
	public TextMeshProUGUI inputBody;
	public TMP_InputField inputField;
	Action<string> okInputCallback = null;

	public float screenOffset
	{
		get
		{
			return canvasRT.sizeDelta.y;
		}
	}

	[Header("General settings")]
	public float rotateSpeed = 50;


	void Start()
	{
		if (instance != this)
			Destroy(gameObject);

		Init();
	}

	protected override void Init()
	{
		if (initialized) return;
		
		var res = Resources.Load("OverlayPopup", typeof(GameObject)) as GameObject;
		if (res == null)
			res = Resources.Load("OverlayPopup_Default", typeof(GameObject)) as GameObject;
		GameObject go = Instantiate(res);
		_instance = go.GetComponent<OverlayPopup>();

		_instance.fsm = new FSMObject<states>(_instance);
		_instance.loadingPopup.anchoredPosition = Vector3.down * _instance.screenOffset;
		_instance.messagePopup.anchoredPosition = Vector3.down * _instance.screenOffset;
		_instance.choosePopup.anchoredPosition = Vector3.down * _instance.screenOffset;

		_instance.background.interactable = false;
		_instance.background.blocksRaycasts = false;
		_instance.background.alpha = 0;

		_instance.initialized = true;
		Destroy(gameObject);
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

	public async Task ShowMessageAsync(string title, string message)
	{
		ShowMessage(title, message);
		await TaskExtensions.WaitUntil(() => fsm.currentState != states.showMessage);
	}

	public void ShowError(string message, Action callback = null)
	{
		this.callback = callback;
		messageTitle.text = I18n.T("Oops!");
		messageBody.text = I18n.T(message);
		fsm.ChangeState(states.showMessage);
	}
	
	public async Task ShowErrorAsync(string message)
	{
		ShowError(message);
		await TaskExtensions.WaitUntil(() => fsm.currentState != states.showMessage);
	}

	public void ShowChoose(string title, string message, Action yesCallback = null, Action noCallback = null)
	{
		this.yesCallback = yesCallback;
		this.noCallback = noCallback;
		chooseTitle.text = I18n.T(title);
		chooseBody.text = I18n.T(message);
		fsm.ChangeState(states.showChoose);
	}
	
	public void ShowChoose(string title, string message, string yesText, string noText, Action yesCallback = null, Action noCallback = null)
	{
		ShowChoose(title, message, yesCallback, noCallback);
		this.yesText.text = yesText;
		this.noText.text = noText;
	}
	
	public async Task<bool> ShowChooseAsync(string title, string message)
	{
		bool accepted = false;
		ShowChoose(title, message, () => { accepted = true;});
		await TaskExtensions.WaitUntil(() => fsm.currentState != states.showChoose);
		return accepted;
	}

	public void ShowInput(string title, string description, Action<string> okInputCallback, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
	{
		this.okInputCallback = okInputCallback;
		inputTitle.text = I18n.T(title);
		inputBody.text = I18n.T(description);
		inputField.text = "";
		inputField.contentType = contentType;
		fsm.ChangeState(states.showInput);
	}
	
	public async Task<string> ShowInputAsync(string title, string description, TMP_InputField.ContentType contentType = TMP_InputField.ContentType.Standard)
	{
		var response = "";
		okInputCallback = s =>
		{
			response = s;
		};
		inputTitle.text = I18n.T(title);
		inputBody.text = I18n.T(description);
		inputField.text = "";
		inputField.contentType = contentType;
		fsm.ChangeState(states.showInput);
		await TaskExtensions.WaitUntil(() => fsm.currentState != states.showInput);
		return response;
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
			Time.unscaledDeltaTime * 5
		);
		loadingWheel.Rotate(0, 0, Time.unscaledDeltaTime * rotateSpeed);
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
		Canvas.ForceUpdateCanvases();

		yield return this.DoUnscaledTween01(t =>
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

		this.DoUnscaledTween01(t =>
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

	IEnumerator showInput()
	{
		yield return StartCoroutine(showView(inputPopup, states.showInput));
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

	public void OkInputButton()
	{
		okInputCallback?.Invoke(inputField.text);
		ClosePopup();
	}

}
