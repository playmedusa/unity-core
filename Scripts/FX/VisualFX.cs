using System.Collections;
using UnityEngine;

public class VisualFX : MonoBehaviour
{
	public enum Animation
	{
		None,
		ElasticGrowOut
	}

	public enum FXType
	{
		Particles,
		Shader,
		Sprite,
		Animation,
		Static
	}

	[Header("References")]
	public ParticleSystem ps;
	public Sprite sprite;
	public Renderer shaderRenderer;
	public new Animation animation;

	[Header("FX Settings")]
	public FXType fxType;
	public float displayTime = 0;
	public bool playOnAwake = false;

	[Header("Animations")]
	public Animation inAnimation;
	public float inTime = 1;
	public Animation outAnimation;
	public float outTime = 1;

	void Awake()
	{
		if (playOnAwake)
			Play();
	}

	public virtual void Play()
	{
		gameObject.SetActive(true);
		StartCoroutine(AnimateIn());

		switch (fxType)
		{
			case FXType.Particles:
				StartCoroutine(PlayParticlesFX());
				break;

			case FXType.Shader:
				PlayShaderFX();
				break;

			case FXType.Sprite:
				PlaySpriteFX();
				break;

			case FXType.Animation:
				PlayAnimation();
				break;
		}

		if (displayTime > 0)
			StartCoroutine(AnimateOut());
	}

	IEnumerator AnimateIn()
	{
		Vector3 scale = transform.localScale;
		switch (inAnimation)
		{
			case Animation.ElasticGrowOut:
				yield return gameObject.DoTween01(t =>
				{
					transform.localScale = scale * PennerAnimation.ElasticEaseOut(t, 0, 1, 1);
				}, inTime);
				break;
			default:
				break;
		}
	}

	IEnumerator AnimateOut()
	{
		yield return new WaitForSeconds(displayTime);
		Vector3 scale = transform.localScale;
		switch (outAnimation)
		{
			case Animation.ElasticGrowOut:
				yield return gameObject.DoTween01(t =>
				{
					transform.localScale = scale * PennerAnimation.ElasticEaseOut(t, 1, -1, 1);
				}, outTime);
				break;
			default:
				break;
		}

		gameObject.SetActive(false);
	}

	IEnumerator PlayParticlesFX()
	{
		if (ps != null)
		{
			ps.Play();
			yield return new WaitUntil(() => ps.isPlaying == false);
			gameObject.SetActive(false);
		}
	}

	void PlayShaderFX()
	{
		if (shaderRenderer != null)
		{

		}
	}

	void PlaySpriteFX()
	{
		if (sprite != null)
		{

		}
	}

	void PlayAnimation()
	{
		if (animation == Animation.None)
		{
			//animation.Play();
		}
	}

}
