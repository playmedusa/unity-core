using UnityEngine;
using System.Collections;

[RequireComponent(typeof(AnimatedButton))]
public abstract class ButtonAnimation : FSM<ButtonAnimation.state>, IAnimatedButton
{

	public enum state
	{
		Idle,
		Select,
		Press,
		Release,
		Click,
		Disabled
	}

	public float animationTime = 0.25f;
	protected RectTransform rectTransform;
	protected AnimatedButton owner;
	protected float elapsedTime;
	protected bool clickTrigered;

	abstract public IEnumerator Idle();
	abstract public IEnumerator Press();
	abstract public IEnumerator Select();
	abstract public IEnumerator Release();
	abstract public IEnumerator Click();
	virtual protected IEnumerator Disabled()
	{
		fsm = null;
		StopCoroutine(FSMLoop());
		yield break;
	}


	void Awake()
	{
		rectTransform = GetComponent<RectTransform>();
	}

	public void SetOwner(AnimatedButton owner)
	{
		this.owner = owner;
	}

	protected void TrigerClick()
	{
		if (!clickTrigered)
		{
			clickTrigered = true;
			owner.DoClick();
		}
	}

	override public void ChangeState(state newState)
	{
		currentState = newState;
		if (newState == state.Disabled)
		{
			return;
		}

		if (fsm == null)
		{
			fsm = StartCoroutine(FSMLoop());
		}
	}

}