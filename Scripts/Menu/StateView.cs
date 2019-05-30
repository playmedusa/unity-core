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

	[Header("StateView config")]
	public RectTransform ui;

	public bool skipAutoselect;
	public bool showStatic;
	public bool rememberPreviouslySelected;
	public bool showMouse = true;
	public bool showCursor;
	public GameObject m_PreviouslySelected;
	public float animationTime = 1;
	public StateViewManager svm;

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
	}
	virtual protected void Awake()
	{
		if (ui == null)
		{
			ui = GetComponent<RectTransform>();
		}

		canvasGroup = ui.GetComponent<CanvasGroup>();
		if (canvasGroup != null)
		{
			canvasGroup.alpha = 0;
			canvasGroup.interactable = false;
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
		while (currentState == state.open)
		{
			currentState = state.execute;
			yield return StartCoroutine(Open());
		}
	}

	IEnumerator execute()
	{
		while (currentState == state.execute)
		{
			viewReady = true;
			yield return StartCoroutine(Execute());
		}
	}

	IEnumerator close()
	{
		while (currentState == state.close)
		{
			currentState = state.stop;
			yield return StartCoroutine(Close());
		}
	}

	IEnumerator stop()
	{
		StopCoroutine(fsm);
		fsm = null;
		yield break;
	}

	virtual protected IEnumerator Open()
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

	virtual protected IEnumerator Execute()
	{
		yield break;
	}

	virtual protected IEnumerator Close()
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
