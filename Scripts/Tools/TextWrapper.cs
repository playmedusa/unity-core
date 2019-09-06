using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

[Serializable]
public class TextWrapper
{
	[SerializeField]
	Text uiText;
	[SerializeField]
	TextMeshPro tmpText;
	[SerializeField]
	TextMeshProUGUI tmpUiText;

	public TextWrapper(GameObject holder)
	{
		uiText = holder.GetComponent<Text>();
		tmpText = holder.GetComponent<TextMeshPro>();
		tmpUiText = holder.GetComponent<TextMeshProUGUI>();
	}

	public string text
	{
		get
		{
			if (uiText != null)
				return uiText.text;
			if (tmpText != null)
				return tmpText.text;
			if (tmpUiText != null)
				return tmpUiText.text;
			return "";
		}
		set
		{
			if (uiText != null)
				uiText.text = value;
			if (tmpText != null)
				tmpText.text = value;
			if (tmpUiText != null)
				tmpUiText.text = value;
		}
	}

	public TMP_FontAsset TMP_Font
	{
		get
		{
			if (tmpText != null)
				return tmpText.font;
			if (tmpUiText != null)
				return tmpUiText.font;
			return null;
		}
		set
		{
			if (tmpText != null)
				tmpText.font = value;
			if (tmpUiText != null)
				tmpUiText.font = value;
		}
	}

	public Color color
	{
		get
		{
			if (uiText != null)
				return uiText.color;
			if (tmpText != null)
				return tmpText.color;
			if (tmpUiText != null)
				return tmpUiText.color;
			return Color.clear;
		}
		set
		{
			if (uiText != null)
				uiText.color = value;
			if (tmpText != null)
				tmpText.color = value;
			if (tmpUiText != null)
				tmpUiText.color = value;
		}
	}

}