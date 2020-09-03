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
	}

	void HideCanvas()
	{
		if (canvasGroup == null)
			canvasGroup = gameObject.AddComponent<CanvasGroup>();
			
		canvasGroup.alpha = 0;
		canvasGroup.interactable = false;
		canvasGroup.blocksRaycasts = false;
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
