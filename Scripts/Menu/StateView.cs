using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class StateView : FSM<StateView.state>
{

	public enum state
	{
		open,
		execute,
		close,
		stop
	}

	[Header("Cached references")]
	public RectTransform ui;
	public StateViewManager svm;

	[Header("StateView config")]
	public bool hideOnAwake = true;
	public bool disableCanvasOnHide = true;
	public bool showStatic;
	public bool rememberPreviouslySelected;
	public bool showMouse = true;
	public bool showCursor;
	public bool skipAutoselect;
	public GameObject m_PreviouslySelected;

	public float animationTime = 1;

	IInitView iInitView;
	ISetupView iSetupView;
	IOpenView iOpenView;
	ICloseView iCloseView;
	IExecuteView iExecuteView;

	public bool isReady
	{
		get
		{
			return isActiveAndEnabled && viewReady;
		}
	}

	public bool isOpen
	{
		get
		{
			return currentState == state.open || currentState == state.execute;
		}
	}

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
	}

	override public void ChangeState(state nextState)
	{
		if (!ui.gameObject.activeInHierarchy)
			ui.gameObject.SetActive(true);
		base.ChangeState(nextState);
	}

	virtual protected void Awake()
	{
		iInitView = GetComponentInChildren<IInitView>();
		iSetupView = GetComponentInChildren<ISetupView>();
		iOpenView = GetComponentInChildren<IOpenView>();
		iCloseView = GetComponentInChildren<ICloseView>();
		iExecuteView = GetComponentInChildren<IExecuteView>();
		canvasGroup = ui.GetComponent<CanvasGroup>();
		
		if (hideOnAwake) {
			if (canvasGroup == null)
				canvasGroup = gameObject.AddComponent<CanvasGroup>();
			
			canvasGroup.alpha = 0;
			DisableCanvasIfNeeded();
		}

		ui.anchoredPosition = Vector2.zero;
		iInitView?.InitView(this);
	}

	void DisableCanvasIfNeeded()
	{
		if (disableCanvasOnHide && canvasGroup != null)
		{
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}
	}

	public void Show(UnityAction callback = null)
	{
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
		if (iSetupView != null)
			yield return iSetupView.SetupView().AsIEnumerator();
		transform.SetAsLastSibling();
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
		while (currentState == state.execute)
		{
			viewReady = true;
			if (iExecuteView != null)
				yield return StartCoroutine(iExecuteView.Execute());
			else
				yield break;
		}
	}

	IEnumerator close()
	{
		DisableCanvasIfNeeded();
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
		if (canvasGroup != null)
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
			yield return this.DoTween01(t =>
			{
				canvasGroup.alpha = Mathf.Lerp(0, 1, t);
			}, animationTime);

		}
		yield break;
	}

	public virtual IEnumerator Close()
	{
		if (canvasGroup != null)
		{
			float startAlpha = canvasGroup.alpha;
			yield return this.DoTween01(t =>
			{
				canvasGroup.alpha = Mathf.Lerp(startAlpha, 0, t);
			}, animationTime * 0.5f);
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}
		yield break;
	}

}
