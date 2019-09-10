using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SizeFitter : MonoBehaviour
{
	[HideInInspector]
	[SerializeField]
	RectTransform rectTransform;
	[HideInInspector]
	[SerializeField]
	RectTransform canvasRectTransform;


	public bool preserveAspect;
	public float aspect;

	public float maxWidth;
	public float maxHeight;
	public float minXPadding;
	public float minYPadding;

	void OnValidate()
	{
		rectTransform = GetComponent<RectTransform>();
		canvasRectTransform = GetComponentInParent<Canvas>().GetComponent<RectTransform>();

		if (preserveAspect && aspect <= 0)
		{
			aspect = rectTransform.sizeDelta.x / rectTransform.sizeDelta.y;
			maxWidth = rectTransform.sizeDelta.x;
			maxHeight = rectTransform.sizeDelta.y;
		}
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			OnDimensionsChange();
	}

	void OnRectTransformDimensionsChange()
	{
		OnDimensionsChange();
	}

	void OnDimensionsChange()
	{
		if (canvasRectTransform == null) return;

		float minWidth = 0;
		float minHeight = 0;
		float unscaledScreenWidth = canvasRectTransform.sizeDelta.x;
		float unscaledScreenHeight = canvasRectTransform.sizeDelta.y;

		float refAspect = 16f / 9f;
		float screenAspect = unscaledScreenWidth / unscaledScreenHeight;
		float scaleFactor = screenAspect / refAspect;

		minWidth = unscaledScreenWidth < maxWidth + minXPadding ? unscaledScreenWidth - minXPadding : maxWidth;
		if (minWidth < 0) minWidth = 0;

		minHeight = unscaledScreenHeight < maxHeight + minYPadding ? unscaledScreenHeight - minYPadding : maxHeight;
		if (minHeight < 0) minHeight = 0;


		float width = Mathf.Clamp(unscaledScreenWidth * scaleFactor, minWidth, maxWidth);
		float height = Mathf.Clamp(unscaledScreenHeight * scaleFactor, minHeight, maxHeight);

		if (width <= maxWidth)
		{
			rectTransform.sizeDelta = new Vector2(
				width,
				preserveAspect ? width / aspect : height
			);
		}
		else
		{
			rectTransform.sizeDelta = new Vector2(
				preserveAspect ? height / aspect : width,
				height
			);
		}
	}

}
