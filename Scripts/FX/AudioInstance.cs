using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using Random = UnityEngine.Random;

public partial class AudioInstance : Singleton<AudioInstance>
{
	public AudioSource aSource;
	protected Dictionary<string, float> playerClipAtPointStamp;

	static float volume = 1;
	static partial void UpdateVolume();

	private void Awake()
	{
		Init();
	}

	override protected void Init()
	{
		if (instance != this)
		{
			Destroy(gameObject);
			return;
		}
		
		DontDestroyOnLoad(instance.gameObject);
		
		playerClipAtPointStamp = new Dictionary<string, float>();
		aSource = GetComponent<AudioSource>();
		if (aSource == null)
			aSource = gameObject.AddComponent<AudioSource>();
	}

	static public void PlayOneShot(AudioClip clip, Vector3 position)
	{
		instance.aSource.PlayOneShot(clip, volume);
	}

	static public AudioSource LoopClipAtPoint(AudioClip clip, Vector3 pos, AudioMixerGroup outputMixer, float pitchRange)
	{
		AudioSource aSource = PlayClipAtPoint(clip, pos, outputMixer, pitchRange, false);
		aSource.loop = true;
		return aSource;
	}

	static public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos)
	{
		return PlayClipAtPoint(clip, pos, null, 0);
	}

	static public AudioSource PlayClipAtPoint(AudioClip clip, Vector3 pos, AudioMixerGroup outputMixer, float pitchRange, bool purge = true)
	{
		float lastStamp = -1;
		instance.playerClipAtPointStamp.TryGetValue(clip.name, out lastStamp);
		if (Time.time - lastStamp < 0.025f) return null;
		instance.playerClipAtPointStamp[clip.name] = Time.time;

		GameObject tempGO = new GameObject();
		tempGO.name = "TempAudio";
		tempGO.transform.position = pos;
		AudioSource aSource = tempGO.AddComponent<AudioSource>();

		UpdateVolume();
		aSource.volume = volume;
		aSource.clip = clip;
		aSource.pitch = Random.Range(1f - pitchRange, 1f + pitchRange);
		aSource.spatialBlend = 0;
		aSource.dopplerLevel = 0;
		aSource.rolloffMode = AudioRolloffMode.Logarithmic;
		aSource.outputAudioMixerGroup = outputMixer;
		aSource.Play();

		if (purge)
			Destroy(tempGO, clip.length);
		return aSource;
	}
	
	public static void CrossFade(AudioClip clip, bool loop = true, float fadeTime = 1)
	{
		var newAudioSource = instance.gameObject.AddComponent<AudioSource>();
		newAudioSource.volume = 0;
		newAudioSource.clip = clip;
		newAudioSource.outputAudioMixerGroup = instance.aSource.outputAudioMixerGroup;
		newAudioSource.loop = loop;
		newAudioSource.Play();
		instance.DoTween01(t =>
		{
			instance.aSource.volume = 1 - t;
			newAudioSource.volume = t;
		}, fadeTime, () =>
		{
			var old = instance.aSource;
			instance.aSource = newAudioSource;
			Destroy(old);
		});
	}

	public static void FadeOutAndDestroy()
	{
		instance.DoTween01(t =>
		{
			instance.aSource.volume = 1 - t;
		}, 1, () => Destroy(instance.gameObject));
	}

}
