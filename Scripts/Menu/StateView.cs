using System.Collections;
using System.Collections.Generic;
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
	public bool skipAutoselect;
	public bool showStatic;
	public bool rememberPreviouslySelected;
	public bool showMouse = true;
	public bool showCursor;
	public GameObject m_PreviouslySelected;

	public float animationTime = 1;
	[SerializeField]
	IOpenView iOpenView;
	[SerializeField]
	ICloseView iCloseView;
	[SerializeField]
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

	protected CanvasGroup canvasGroup;
	bool viewReady;

	void OnValidate()
	{
		if (svm == null)
			svm = GetComponentInParent<StateViewManager>();
		if (ui == null)
			ui = GetComponent<RectTransform>();
		if (iOpenView == null)
			iOpenView = GetComponentInChildren<IOpenView>();
		if (iCloseView == null)
			iCloseView = GetComponentInChildren<ICloseView>();
		if (iExecuteView == null)
			iExecuteView = GetComponentInChildren<IExecuteView>();
	}
	virtual protected void Awake()
	{
		canvasGroup = ui.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			canvasGroup.alpha = 0;
			canvasGroup.interactable = false;
			canvasGroup.blocksRaycasts = false;
		}
		ui.anchoredPosition = Vector2.zero;
	}

	public void Show(UnityAction callback = null)
	{
		viewReady = false;
		this.callback = callback;
		ChangeState(state.open);
	}

	public void Hide(UnityAction callback = null)
	{
		viewReady = false;
		this.callback = callback;
		ChangeState(state.close);
	}

	IEnumerator open()
	{
		transform.SetAsLastSibling();
		if (canvasGroup != null)
		{
			canvasGroup.interactable = true;
			canvasGroup.blocksRaycasts = true;
		}
		while (currentState == state.open)
		{
			currentState = state.execute;
			if (iOpenView != null)
				yield return StartCoroutine(iOpenView.Open());
			else
				yield return StartCoroutine(Open());
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
				yield return StartCoroutine(Execute());
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
			currentState = state.stop;
			if (iCloseView != null)
				yield return StartCoroutine(iCloseView.Close());
			else
				yield return StartCoroutine(Close());
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

	protected virtual IEnumerator Execute()
	{
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
