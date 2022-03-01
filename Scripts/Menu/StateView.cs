using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StateView : FSM<StateView.state>
{

	public enum state
	{
		_,
		open,
		execute,
		close,
		stop
	}

	[Header("Cached references")]
	public RectTransform ui;
	public StateViewManager svm;
	public SafeArea safeArea;

	[Header("StateView config")]
	public bool hideOnAwake = true;
	public bool showStatic;
	public bool rememberPreviouslySelected;
	public bool showMouse = true;
	public bool skipAutoselect;
	public bool ignoreSafeArea;
	public GameObject m_PreviouslySelected;
	public float animationTime = 1;

	IInitView iInitView;
	ISetupView iSetupView;
	IOpenView iOpenView;
	ICloseView iCloseView;
	IExecuteView iExecuteView;

	public bool isReady => isActiveAndEnabled && viewReady;
	public bool isOpen => currentState == state.open || currentState == state.execute;

	public UnityAction onViewOpened;
	public UnityAction onViewClosed;

	public CanvasGroup canvasGroup
	{
		get;
		protected set;
	}
	
	bool viewReady;

	void OnValidate()
	{
		if (svm == null)
			svm = GetComponentInParent<StateViewManager>();
		if (ui == null)
			ui = GetComponent<RectTransform>();
		if (safeArea == null && ignoreSafeArea)
		{
			safeArea = GetComponentInParent<SafeArea>();
			if (safeArea == null)
				Debug.LogError("Safe area not found");
		}
	}

	public override void ChangeState(state nextState)
	{
		if (!ui.gameObject.activeInHierarchy)
			ui.gameObject.SetActive(true);
		base.ChangeState(nextState);
	}

	void Awake()
	{
		iInitView = GetComponentInChildren<IInitView>();
		iSetupView = GetComponentInChildren<ISetupView>();
		iOpenView = GetComponentInChildren<IOpenView>();
		iCloseView = GetComponentInChildren<ICloseView>();
		iExecuteView = GetComponentInChildren<IExecuteView>();
		canvasGroup = ui.GetComponent<CanvasGroup>();

		if (hideOnAwake)
			HideCanvas();

		ui.anchoredPosition = Vector2.zero;
		iInitView?.InitView(this);
		if (rememberPreviouslySelected)
			EventSystem.current.SetSelectedGameObject(m_PreviouslySelected);
	}

	void HideCanvas()
	{
		if (canvasGroup == null)
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
			
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
	}

	void IgnoreSafeArea()
	{
		if (!ignoreSafeArea) return;
		IgnoreSafeArea(transform as RectTransform);
	}

	public void IgnoreSafeArea(RectTransform rt)
	{
		if (safeArea == null) return;
		var canvasScaler = rt.GetComponentInParent<CanvasScaler>();
		var safeRT = safeArea.transform as RectTransform;
		var rect = new Rect(
			-safeRT.localPosition,
			new Vector2(Screen.width, Screen.height)
		);
		rt.anchorMin = rt.anchorMax = Vector2.one * 0.5f;
		rt.anchoredPosition = rect.position;
		rt.sizeDelta = rect.size;
		if (canvasScaler != null)
		{
			var ratio = canvasScaler.referenceResolution / new Vector2(Screen.width, Screen.height);
			rt.localScale = Vector3.one * Mathf.Max(ratio.x, ratio.y);
		}
	}
	
	public void Show(UnityAction callback = null)
	{
		IgnoreSafeArea();
		viewReady = false;
		onViewOpened = callback;
		ChangeState(state.open);
	}

	public void Hide(UnityAction callback = null)
	{
		viewReady = false;
		onViewClosed = callback;
		ChangeState(state.close);
	}

	IEnumerator open()
	{
		transform.SetAsLastSibling();
		if (iSetupView != null)
			yield return iSetupView.SetupView().AsIEnumerator();
		if (canvasGroup != null)
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}
		while (currentState == state.open)
		{
			if (iOpenView != null)
				yield return StartCoroutine(iOpenView.Open());
			else
				yield return StartCoroutine(Open());

			if (currentState == state.open)
			{
				currentState = state.execute;
				onViewOpened?.Invoke();
			}
		}
	}

	IEnumerator execute()
	{
		viewReady = true;
		while (currentState == state.execute)
		{
			if (iExecuteView != null)
				yield return StartCoroutine(iExecuteView.Execute());
			else
			{
				StopFSM();
				yield break;
			}
		}
	}

	IEnumerator close()
	{
		if (canvasGroup != null)
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}

		while (currentState == state.close)
		{
			if (iCloseView != null)
				yield return StartCoroutine(iCloseView.Close());
			else
				yield return StartCoroutine(Close());

			if (currentState == state.close)
			{
				currentState = state.stop;
				onViewClosed?.Invoke();
			}
		}
	}

	IEnumerator stop()
	{
		StopCoroutine(fsm);
		fsm = null;
		yield break;
	}

	public virtual IEnumerator Open()
	{
		if (canvasGroup == null) yield break;
		
		yield return this.DoTween01(t =>
		{
			canvasGroup.alpha = Mathf.Lerp(0, 1, t);
		}, animationTime);
	}

	public virtual IEnumerator Close()
	{
		if (canvasGroup == null) yield break;
		
		float startAlpha = canvasGroup.alpha;
		yield return this.DoTween01(t =>
		{
			canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t);
		}, animationTime * 0.5f);
	}

}
