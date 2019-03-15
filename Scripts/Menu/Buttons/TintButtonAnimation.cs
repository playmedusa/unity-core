using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
public class TintButtonAnimation : ButtonAnimation
{

	void Start()
	{
		animationTime = owner.colors.fadeDuration;
	}

	override public IEnumerator Idle()
	{
		elapsedTime = 0;
		Color startColor = owner.targetGraphic.color;
		while (elapsedTime < animationTime)
		{
			owner.targetGraphic.color = Color.Lerp(
				startColor,
				owner.colors.normalColor,
				elapsedTime / animationTime
			);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		owner.targetGraphic.color = owner.colors.normalColor;
		while (currentState == state.Idle)
		{
			yield return 0;
		}
	}

	override public IEnumerator Select()
	{
		elapsedTime = 0;
		Color startColor = owner.targetGraphic.color;
		while (elapsedTime < animationTime)
		{
			owner.targetGraphic.color = Color.Lerp(
				startColor,
				owner.colors.highlightedColor,
				elapsedTime / animationTime
			);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		owner.targetGraphic.color = owner.colors.highlightedColor;
		while (currentState == state.Select)
		{
			yield return 0;
		}
	}

	override public IEnumerator Press()
	{
		elapsedTime = 0;
		Color startColor = owner.targetGraphic.color;
		while (elapsedTime < animationTime)
		{
			owner.targetGraphic.color = Color.Lerp(
				startColor,
				owner.colors.pressedColor,
				elapsedTime / animationTime
			);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		owner.targetGraphic.color = owner.colors.pressedColor;
		while (currentState == state.Press)
		{
			yield return 0;
		}
	}

	override public IEnumerator Release()
	{
		//if (currentState == state.Release)
		ChangeState(state.Idle);
		yield break;
	}

	override public IEnumerator Click()
	{
		elapsedTime = 0;
		clickTrigered = false;
		TrigerClick();
		Color startColor = owner.targetGraphic.color;
		while (elapsedTime < animationTime)
		{
			owner.targetGraphic.color = Color.Lerp(
					startColor,
					owner.colors.pressedColor,
					elapsedTime / animationTime
				);
			elapsedTime += Time.deltaTime;
			yield return 0;
		}
		owner.targetGraphic.color = owner.colors.pressedColor;
		if (EventSystem.current.currentSelectedGameObject == gameObject)
			ChangeState(state.Select);
		else
			ChangeState(state.Idle);
	}

}