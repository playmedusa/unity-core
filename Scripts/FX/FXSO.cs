using UnityEngine;
using UnityEngine.Audio;
using System.Collections.Generic;
using System.Linq;

[CreateAssetMenu(menuName = "ScriptableObjects/FX")]
public class FXSO : ScriptableObject
{
	public GameObject[] visualFX;
	public int poolSize = 0;

	[System.NonSerialized]
	int lastSfxIndex = -1;

	public AudioClip sfx
	{
		get
		{
			if (sfxList.Length == 0) return null;

			if (sfxList.Length > 1)
			{
				var numberList = Enumerable.Range(0, sfxList.Length).ToList();
				numberList.Remove(lastSfxIndex);
				numberList.Shuffle();
				lastSfxIndex = numberList[0];
				return sfxList[lastSfxIndex];
			}

			return sfxList[0];
		}
	}
	public AudioMixerGroup outputMixer;

	public AudioClip[] sfxList;
	public float pitchRange = 0;

	public void RaiseFX(Vector3 position, Quaternion rotation, Transform parent = null, float scale = 1)
	{
		if (sfx != null)
		{
			AudioInstance.PlayClipAtPoint(sfx, position, outputMixer, pitchRange);
		}

		foreach (GameObject visualFX in visualFX)
		{
			GameObject instance = FXPool.getFXObject(visualFX, poolSize);
			instance.transform.position = position;
			instance.transform.rotation = rotation;
			instance.transform.SetParent(parent);
			instance.transform.localScale = Vector3.one * scale;
			instance.SetActive(true);
			VisualFX vFX = instance.GetComponent<VisualFX>();
			if (vFX != null)
			{
				vFX.Play();
			}
		}
	}
}
