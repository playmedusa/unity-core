using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SpriteSwapButtonAnimation : ButtonAnimation
{
	public Sprite releasedImage;
	public Sprite pressedImage;
	public float innerTextPressedOffset;

	public float innerTextVerticalPivot;
	public FXSO buttonClickFX;

	Text buttonText;
	Image buttonImage;

	void Start()
	{
		animationTime = 0.1f;
		buttonImage = GetComponent<Image>();
		buttonText = GetComponentInChildren<Text>();
		if (buttonText != null)
			innerTextVerticalPivot = buttonText.transform.localPosition.y;
	}

	void SetReleasedImage()
	{
		buttonImage.sprite = releasedImage;
		if (buttonText != null)
		{
			Vector3 p = buttonText.transform.localPosition;
			p.y = innerTextVerticalPivot;
			buttonText.transform.localPosition = p;
		}
	}

	void SetPressedImage()
	{
		buttonImage.sprite = pressedImage;
		if (buttonText != null)
		{
			Vector3 p = buttonText.transform.localPosition;
			p.y = innerTextVerticalPivot - innerTextPressedOffset;
			buttonText.transform.localPosition = p;
		}
	}

	override public IEnumerator Idle()
	{
		SetReleasedImage();
		while (currentState == state.Idle)
		{
			yield return 0;
		}
	}

	override public IEnumerator Select()
	{
		SetReleasedImage();
		while (currentState == state.Select)
			yield return 0;
	}

	override public IEnumerator Press()
	{
		buttonClickFX.RaiseFX(transform.position, Quaternion.identity);
		SetPressedImage();
		while (currentState == state.Press)
			yield return 0;
	}

	override public IEnumerator Release()
	{
		SetReleasedImage();
		if (currentState == state.Release)
			ChangeState(state.Idle);
		yield return 0;
	}

	override public IEnumerator Click()
	{
		SetPressedImage();
		clickTrigered = false;
		TrigerClick();
		if (EventSystem.current.currentSelectedGameObject == gameObject)
			ChangeState(state.Select);
		else
			ChangeState(state.Idle);
		yield return 0;
	}

}