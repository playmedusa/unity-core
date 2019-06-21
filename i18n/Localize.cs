using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mgl;
using TMPro;

public class Localize : MonoBehaviour
{

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

	}

	public string label;
	public bool upperCase;

	public Vector3[] positionFix;
	public Vector2[] sizeFix;

	[SerializeField]
	TextWrapper textLabel;

	[SerializeField]
	RectTransform rt;

	void OnValidate()
	{
		textLabel = new TextWrapper(gameObject);
		rt = GetComponent<RectTransform>();
	}

	void Awake()
	{
		if (textLabel != null)
		{
			LocalizeTextLabel();
		}
	}

	void LocalizeTextLabel()
	{
		if (label.Length > 0)
		{
			textLabel.text = upperCase ? I18n.T(label) : I18n.t(label);
		}
		else if (textLabel.text.Length > 0)
		{
			textLabel.text = upperCase ? I18n.T(textLabel.text) : I18n.t(textLabel.text);
		}

		if (positionFix.Length > 0)
		{
			if (rt != null)
			{
				rt.anchoredPosition = positionFix[I18n.localeId];
				if (sizeFix.Length > 0)
				{
					rt.sizeDelta = sizeFix[I18n.localeId];
				}
			}
			else
			{
				transform.localPosition = positionFix[I18n.localeId];
			}
		}
	}
}

