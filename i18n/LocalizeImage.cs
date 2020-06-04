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
		if (image != null)
		{
			LocalizeTheImage();
		}
	}

	void LocalizeTheImage()
	{
		for (int i = 0; i < localizations.Length; i++)
		{
			if (localizations[i].ln == I18n.GetLocale())
			{
				image.sprite = localizations[i].sprite;
			}
		}
	}
}

