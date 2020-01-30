using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class PressButtonAnimation : ButtonAnimation
{

	Vector3 pivotScale;
	float scaleFactor = 1f;

	void Start()
	{
		animationTime = 0.1f;
		pivotScale = Vector3.one * scaleFactor;
		transform.localScale = pivotScale;
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
		float initScale = 1.0f;
		float targetScale = 1.1f;
		while (currentState == state.Press && elapsedTime < animationTime)
		{
			float s = PennerAnimation.CubicEaseInOut(elapsedTime, initScale, targetScale - initScale, animationTime);
			transform.localScale = pivotScale * s * scaleFactor;
			elapsedTime += Time.unscaledDeltaTime;
			yield return 0;
		}
		transform.localScale = pivotScale * targetScale * scaleFactor;
		while (currentState == state.Press)
			yield return 0;
	}

	override public IEnumerator Release()
	{
		float elapsedTime = 0;
		float initScale = 1.1f;
		float targetScale = 1.0f;
		while (currentState == state.Release && elapsedTime < animationTime)
		{
			float s = PennerAnimation.CubicEaseInOut(elapsedTime, initScale, targetScale - initScale, animationTime);
			transform.localScale = pivotScale * s;
			elapsedTime += Time.unscaledDeltaTime;
			yield return 0;
		}
		transform.localScale = pivotScale * targetScale;
		if (currentState == state.Release)
			ChangeState(state.Idle);
	}

	override public IEnumerator Click()
	{
		float elapsedTime = 0;
		float initScale = 1.1f;
		float targetScale = 1.0f;
		float tweenTime = animationTime * 8.0f;
		clickTrigered = false;
		while (/*currentState == state.Click &&*/ elapsedTime < tweenTime)
		{
			float s = PennerAnimation.CubicEaseOut(elapsedTime, initScale, targetScale - initScale, tweenTime);
			transform.localScale = pivotScale * s;
			elapsedTime += Time.unscaledDeltaTime;
			//elastic out is a bit slow to finish, we force click here.
			if (elapsedTime > tweenTime * 0.25f)
			{
				TrigerClick();
			}
			yield return 0;
		}
		transform.localScale = pivotScale * targetScale;

		ChangeState(state.Idle);
	}

}