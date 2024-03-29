﻿using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AnimatedButton : Selectable, IPointerClickHandler, ISubmitHandler
{
	public bool ignoreStateView;
	public UnityEvent OnClick;
	ButtonAnimation buttonAnimation;
	StateView stateView;

	private bool isStateViewReady => (stateView != null && stateView.isReady) || stateView == null || ignoreStateView;

	public ButtonAnimation.state animationState => buttonAnimation.currentState;

	public bool isClickable =>
		animationState != ButtonAnimation.state.Disabled &&
		animationState != ButtonAnimation.state.Click && interactable;

	float deselectTime;

	new void Reset()
	{
		transition = Transition.None;
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

	override public void OnPointerDown(PointerEventData eventData)
	{
		base.OnPointerDown(eventData);
		if (!isClickable)
			return;

		if (isStateViewReady)
			buttonAnimation.ChangeState(ButtonAnimation.state.Press);
	}

	virtual public void OnPointerClick(PointerEventData eventData)
	{
		if (!isClickable)
			return;

		if (isStateViewReady)
		{
			buttonAnimation.ChangeState(ButtonAnimation.state.Click);
			if (StateViewManager.instance.isUsingMouse)
				MenuCursor.instance?.PlaySelectSound();
		}
	}

	public override void OnPointerExit(PointerEventData eventData)
	{
		if (buttonAnimation.currentState == ButtonAnimation.state.Click)
			return;

		base.OnPointerExit(eventData);
		if (isStateViewReady)
		{
			Deselect();
			if (stateView != null)
				stateView.m_PreviouslySelected = null;
		}
	}

	override public void OnDeselect(BaseEventData eventData)
	{
		if (!isClickable)
			return;
		base.OnDeselect(eventData);
		deselectTime = Time.time;
	}

	override public void OnPointerUp(PointerEventData eventData)
	{
		if (!isClickable)
			return;

		base.OnPointerUp(eventData);
		if (isStateViewReady)
		{
			if (Time.time - deselectTime < 0.1f || EventSystem.current.currentSelectedGameObject == gameObject)
			{
				buttonAnimation.ChangeState(ButtonAnimation.state.Click);
				MenuCursor.instance?.PlaySelectSound();
			}
			else
				buttonAnimation.ChangeState(ButtonAnimation.state.Release);
		}
	}

	public void DoClick()
	{
		if (interactable)
			OnClick.Invoke();
	}

	public void OnSubmit(BaseEventData eventData)
	{
		OnPointerClick(null);
	}
}
