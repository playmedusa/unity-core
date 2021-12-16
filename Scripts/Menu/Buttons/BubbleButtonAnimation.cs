using System.Collections;
using UnityEngine;

public class BubbleButtonAnimation : ButtonAnimation
{

	Vector3 pivotScale;
	float scaleFactor = 0.75f;

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
		float elapsedTime = 0;
		float animationTime = 0.5f;
		while (currentState == state.Select && elapsedTime < animationTime)
		{
			float scale = PennerAnimation.BackEaseOut(elapsedTime / animationTime, 0, 1, 1);
			transform.localScale = pivotScale + Vector3.one * 0.25f * scale;
			elapsedTime += Time.unscaledDeltaTime;
			yield return 0;
		}
		transform.localScale = pivotScale + Vector3.one * 0.25f;

		while (currentState == state.Select)
		{
			yield return 0;
		}

		yield return gameObject.DoUnscaledTween01(t =>
		{
			float scale = PennerAnimation.QuadEaseIn(t, 1, -1, 1);
			transform.localScale = pivotScale + Vector3.one * 0.25f * scale;
		}, 0.1f);
	}

	override public IEnumerator Press()
	{
		float elapsedTime = 0;
		float initScale = 1.0f;
		float targetScale = 0.9f;
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
		float initScale = 0.9f;
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
		float initScale = 0.9f;
		float targetScale = 1.0f;
		float tweenTime = animationTime * 8.0f;
		clickTrigered = false;
		while (/*currentState == state.Click &&*/ elapsedTime < tweenTime)
		{
			float s = PennerAnimation.ElasticEaseOut(elapsedTime, initScale, targetScale - initScale, tweenTime);
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