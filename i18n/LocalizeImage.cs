using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Mgl;

public class LocalizeImage : MonoBehaviour
{
	[System.Serializable]
	public struct ImageLocalized
	{
		public string ln;
		public Sprite sprite;
	}
	[SerializeField]
	public ImageLocalized[] localizations;

	[SerializeField]
	Image image;

	void OnValidate()
	{
		image = GetComponent<Image>();
	}

	void Start()
	{
		Debug.Log("Localize");
		if (image != null)
		{
			LocalizeTheImage();
		}
	}

	void LocalizeTheImage()
	{
		for (int i = 0; i < localizations.Length; i++)
		{
			Debug.Log(localizations[i].ln + " " + I18n.GetLocale());
			if (localizations[i].ln == I18n.GetLocale())
			{
				Debug.Log(localizations[i].ln + "  " + localizations[i].sprite);
				image.sprite = localizations[i].sprite;
			}
		}
	}
}

