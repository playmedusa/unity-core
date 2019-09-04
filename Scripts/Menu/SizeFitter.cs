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
	Canvas canvas;

	public bool preserveAspect;
	public float aspect;

	public float maxWidth;
	public float maxHeight;
	public float minXPadding;
	public float minYPadding;

	void OnValidate()
	{
		if (rectTransform == null)
			rectTransform = GetComponent<RectTransform>();
		if (canvas == null)
			canvas = GetComponentInParent<Canvas>();

		if (preserveAspect && aspect <= 0)
		{
			aspect = rectTransform.sizeDelta.x / rectTransform.sizeDelta.y;
			maxWidth = rectTransform.sizeDelta.x;
			maxHeight = rectTransform.sizeDelta.y;
		}
	}

	void OnEnable()
	{
		StateViewManager.OnOrientationChange += OnOrientationChange;
	}

	void OnDisable()
	{
		StateViewManager.OnOrientationChange -= OnOrientationChange;
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.Space))
			OnOrientationChange(DeviceOrientation.Portrait);
	}

	void OnOrientationChange(DeviceOrientation newOrientation)
	{
		float match = 0;
		float xScaleFactor = ((float)Screen.width / 1920f) * (1f - match);
		float yScaleFactor = ((float)Screen.height / 1080f) * match;

		float refAspect = 16f / 9f;
		float screenAspect = (float)Screen.width / (float)Screen.height;
		float scaleFactor = (aspect * screenAspect) / refAspect;

		float minWidth = 0;
		float minHeight = 0;

		if (screenAspect / refAspect < 1) //Narrower
		{
			float unscaledScreenWidth = Screen.width / canvas.scaleFactor;
			minWidth = unscaledScreenWidth < maxWidth + minXPadding ? unscaledScreenWidth - minXPadding : maxWidth;
			if (minWidth < 0) minWidth = 0;
		}
		else //Wider
		{
			float unscaledScreenHeight = Screen.height / canvas.scaleFactor;
			minHeight = unscaledScreenHeight < maxHeight + minYPadding ? unscaledScreenHeight - minXPadding : maxWidth;
			if (minHeight < 0) minHeight = 0;
		}

		float width = Mathf.Clamp(maxHeight * scaleFactor, minWidth, maxWidth);
		float height = Mathf.Clamp(maxWidth / scaleFactor, minHeight, maxHeight);

		if (width <= maxWidth)
		{
			rectTransform.sizeDelta = new Vector2(
				width,
				preserveAspect ? maxHeight * (width / maxWidth) : height
			);
		}
		else
		{
			rectTransform.sizeDelta = new Vector2(
				preserveAspect ? maxWidth * (height / maxHeight) : width,
				height
			);
		}

	}

}
