using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;

public class AnimatedButton : Selectable, IPointerDownHandler, IPointerUpHandler, IPointerClickHandler, IPointerEnterHandler
{
	public UnityEvent OnClick;
	ButtonAnimation buttonAnimation;
	StateView stateView;

	bool isStateViewReady
	{
		get
		{
			return (stateView != null && stateView.isReady) || stateView == null;
		}
	}

	public ButtonAnimation.state animationState
	{
		get
		{
			return buttonAnimation.currentState;
		}
	}

	public bool isClickable
	{
		get
		{
			return animationState != ButtonAnimation.state.Disabled &&
				animationState != ButtonAnimation.state.Click;
		}
	}

	protected override void Awake()
	{
		base.Awake();
		stateView = GetComponentInParent<StateView>();
		buttonAnimation = GetComponent<ButtonAnimation>();
		buttonAnimation?.SetOwner(this);
		buttonAnimation?.ChangeState(buttonAnimation.currentState);
	}

	override public void Select()
	{
		var newPreviouslySelected = (EventSystem.current != null) ? EventSystem.current.currentSelectedGameObject : null;
		if (newPreviouslySelected != gameObject)
			EventSystem.current.SetSelectedGameObject(gameObject);

		if (newPreviouslySelected == gameObject || MenuCursor.instance == null)
		{
			if (IsPressed())
				buttonAnimation.ChangeState(ButtonAnimation.state.Press);
			else
				buttonAnimation.ChangeState(ButtonAnimation.state.Select);
		}
		base.Select();
	}

	public void Deselect()
	{
		var newPreviouslySelected = (EventSystem.current != null) ? EventSystem.current.currentSelectedGameObject : null;
		if (newPreviouslySelected == gameObject)
		{
			EventSystem.current.SetSelectedGameObject(null);
			if (MenuCursor.instance == null)
				buttonAnimation.ChangeState(ButtonAnimation.state.Idle);
		}
	}

	public void SetEnabled(bool active)
	{
		if (active)
			buttonAnimation.ChangeState(ButtonAnimation.state.Idle);
		else
			buttonAnimation.ChangeState(ButtonAnimation.state.Disabled);
	}

	override public void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		if (buttonAnimation.currentState == ButtonAnimation.state.Disabled || buttonAnimation.currentState == ButtonAnimation.state.Click)
			return;

		if (isStateViewReady)
			buttonAnimation.ChangeState(ButtonAnimation.state.Press);
	}

	override public void OnPointerUp(PointerEventData eventData)
	{
		base.OnPointerUp(eventData);
		if (buttonAnimation.currentState == ButtonAnimation.state.Disabled || buttonAnimation.currentState == ButtonAnimation.state.Click)
			return;

		if (isStateViewReady)
		{
			buttonAnimation.ChangeState(ButtonAnimation.state.Release);
		}
	}

	virtual public void OnPointerClick(PointerEventData eventData)
	{
		if (buttonAnimation.currentState == ButtonAnimation.state.Disabled || buttonAnimation.currentState == ButtonAnimation.state.Click)
			return;

		if (isStateViewReady)
		{
			buttonAnimation.ChangeState(ButtonAnimation.state.Click);
			if (StateViewManager.instance.isUsingMouse)
				MenuCursor.instance?.PlaySelectSound();
		}
	}

	public override void OnPointerEnter(PointerEventData eventData)
	{
		base.OnPointerEnter(eventData);
		StateViewManager.instance.SetUsingMouse(true);
		if (isStateViewReady && interactable)
		{
			Select();
		}
		else
		{
			if (stateView != null)
				stateView.m_PreviouslySelected = gameObject;
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		base.OnPointerExit(eventData);
		if (isStateViewReady)
		{
			Deselect();
			if (stateView != null)
				stateView.m_PreviouslySelected = null;
		}
	}

	public void DoClick()
	{
		if (interactable)
			OnClick.Invoke();
	}

}
