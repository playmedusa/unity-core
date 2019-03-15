using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioInstance : MonoBehaviour
{

	private static AudioInstance _instance;
	public static AudioInstance instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<AudioInstance>();
			if (_instance == null)
			{
				Debug.Log("AudioInstance not found, instantiating");
				GameObject go = new GameObject();
				go.name = "AudioInstance";
				_instance = go.AddComponent<AudioInstance>();
			}
			return _instance;
		}
	}

	public AudioSource aSource;

	protected Dictionary<string, float> playerClipAtPointStamp;

	void Awake()
	{
		playerClipAtPointStamp = new Dictionary<string, float>();
	}

	static public void PlayOneShot(AudioClip clip, Vector3 position)
	{
		instance.aSource.PlayOneShot(clip);
	}

	static public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos)
	{
		return PlayClipAtPoint(clip, pos, null, 0);
	}

	static public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos, AudioMixerGroup outputMixer, float pitchRange)
	{
		float lastStamp = -1;
		instance.playerClipAtPointStamp.TryGetValue(clip.name, out lastStamp);
		if (Time.time - lastStamp < 0.025f) return null;
		instance.playerClipAtPointStamp[clip.name] = Time.time;

		GameObject tempGO = new GameObject();
		tempGO.name = "TempAudio";
		tempGO.transform.position = pos;
		AudioSource aSource = tempGO.AddComponent<AudioSource>();
		aSource.clip = clip;
		aSource.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
		aSource.spatialBlend = 0;
		aSource.dopplerLevel = 0;
		aSource.rolloffMode = AudioRolloffMode.Logarithmic;
		aSource.outputAudioMixerGroup = outputMixer;
		aSource.Play();

		Destroy(tempGO, clip.length);
		return aSource;
	}

}
