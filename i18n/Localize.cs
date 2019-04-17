using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mgl;

public class Localize : MonoBehaviour
{
	public string label;
	public bool upperCase;

	public Vector3[] positionFix;
	public Vector2[] sizeFix;

	[SerializeField]
	Text textLabel;

	[SerializeField]
	RectTransform rt;

	void OnValidate()
	{
		textLabel = GetComponent<Text>();
		rt = GetComponent<RectTransform>();
	}

	void Awake()
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
