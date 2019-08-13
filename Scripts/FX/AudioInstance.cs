using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public partial class AudioInstance : Singleton<AudioInstance>
{
	public AudioSource aSource;
	protected Dictionary<string, float> playerClipAtPointStamp;

	static float volume = 1;
	static partial void UpdateVolume();

	override protected void Init()
	{
		playerClipAtPointStamp = new Dictionary<string, float>();
		aSource = gameObject.AddComponent<AudioSource>();
	}

	static public void PlayOneShot(AudioClip clip, Vector3 position)
	{
		instance.aSource.PlayOneShot(clip, volume);
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

		UpdateVolume();
		aSource.volume = volume;
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
