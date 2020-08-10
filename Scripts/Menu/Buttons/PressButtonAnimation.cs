using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class PressButtonAnimation : ButtonAnimation
{
	public float scaleFactor = 0.1f;
	
	Vector3 pivotScale;

	public void Reset()
	{
		animationTime = 0.1f;
	}

	void Start()
	{
		pivotScale = transform.localScale;
	}

	override public IEnumerator Idle()
	{
		while (currentState == state.Idle)
		{
			yield return 0;
		}
	}

	override public IEnumerator Select()
	{
		while (currentState == state.Select)
		{
			yield return 0;
		}
	}

	override public IEnumerator Press()
	{
		float elapsedTime = 0;
		while (currentState == state.Press && elapsedTime < animationTime)
		{
			float s = PennerAnimation.CubicEaseInOut(elapsedTime, 1f, scaleFactor, animationTime);
			transform.localScale = pivotScale * s;
			elapsedTime += Time.unscaledDeltaTime;
			yield return 0;
		}
		transform.localScale = pivotScale * (1 + scaleFactor);
		while (currentState == state.Press)
			yield return 0;
	}

	override public IEnumerator Release()
	{
		float elapsedTime = 0;
		float initScale = 1 + scaleFactor;
		while (currentState == state.Release && elapsedTime < animationTime)
		{
			float s = PennerAnimation.CubicEaseInOut(elapsedTime, initScale, -scaleFactor, animationTime);
			transform.localScale = pivotScale * s;
			elapsedTime += Time.unscaledDeltaTime;
			yield return 0;
		}
		transform.localScale = pivotScale;
		if (currentState == state.Release)
			ChangeState(state.Idle);
	}

	override public IEnumerator Click()
	{
		float elapsedTime = 0;
		float initScale = 1 + scaleFactor;
		float tweenTime = animationTime * 3.0f;
		clickTrigered = false;
		while (/*currentState == state.Click &&*/ elapsedTime < tweenTime)
		{
			float s = PennerAnimation.CubicEaseOut(elapsedTime, initScale, -scaleFactor, tweenTime);
			transform.localScale = pivotScale * s;
			elapsedTime += Time.unscaledDeltaTime;
			//elastic out is a bit slow to finish, we force click here.
			if (elapsedTime > tweenTime * 0.35f)
			{
				TrigerClick();
			}
			yield return 0;
		}
		transform.localScale = pivotScale;

		ChangeState(state.Idle);
	}

}