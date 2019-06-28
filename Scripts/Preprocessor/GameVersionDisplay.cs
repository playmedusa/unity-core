using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameVersionDisplay : MonoBehaviour
{

	[SerializeField]
	TextWrapper textLabel;

	void OnValidate()
	{
		textLabel = new TextWrapper(gameObject);
	}

	void Awake()
	{
		textLabel.text = string.Format(
			"v. {0} - {1}",
			GameVersionHelper.instance.loadedData.version,
			GameVersionHelper.instance.loadedData.bundle
		);
	}
}
