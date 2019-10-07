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
	public bool loopSFX;
	public float pitchRange = 0;

	public void Raise(Vector3 position, out AudioSource audioSource, out GameObject[] vfx)
	{
		if (sfx != null)
		{
			if (loopSFX)
				audioSource = AudioInstance.LoopClipAtPoint(sfx, position, outputMixer, pitchRange);
			else
			{
				AudioInstance.PlayClipAtPoint(sfx, position, outputMixer, pitchRange);
				audioSource = null;
			}
		}
		else
			audioSource = null;

		vfx = new GameObject[visualFX.Length];
		for (int i = 0; i < visualFX.Length; i++)
		{
			GameObject visualfx = visualFX[i];
			GameObject instance = FXPool.getFXObject(visualfx, poolSize);
			instance.transform.position = position;
			instance.transform.rotation = visualfx.transform.rotation;
			instance.transform.SetParent(null);
			instance.transform.localScale = Vector3.one;
			instance.SetActive(true);
			VisualFX vFX = instance.GetComponent<VisualFX>();
			if (vFX != null)
				vFX.Play();
			vfx[i] = instance;
		}
	}

	public float Raise(Vector3 position, Quaternion rotation, Vector3 scale, Transform parent = null)
	{
		float fxTime = 0;

		if (sfx != null)
		{
			AudioInstance.PlayClipAtPoint(sfx, position, outputMixer, pitchRange);
			fxTime = sfx.length;
		}

		foreach (GameObject visualFX in visualFX)
		{
			GameObject instance = FXPool.getFXObject(visualFX, poolSize);
			instance.transform.position = position;
			instance.transform.rotation = rotation;
			instance.transform.SetParent(parent);
			instance.transform.localScale = scale;
			instance.SetActive(true);
			VisualFX vFX = instance.GetComponent<VisualFX>();
			if (vFX != null)
			{
				if (fxTime < vFX.displayTime)
					fxTime = vFX.displayTime;

				vFX.Play();
			}
		}
		return fxTime;
	}

	public float Raise(Vector3 position, Quaternion rotation, Transform parent = null, float scale = 1)
	{
		return Raise(position, rotation, Vector3.one * scale, parent);
	}
}
