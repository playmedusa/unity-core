using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using InputMapper;

public class MenuCursor : InputHandler
{
	static MenuCursor _instance;
	public static MenuCursor instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<MenuCursor>();
			if (_instance == null)
				Debug.LogError("Menu cursor not found");
			return _instance;
		}
	}

	public AudioClip highlightClip;
	public AudioClip selectClip;
	public GameObject selectedGameObject
	{
		get;
		private set;
	}

	RectTransform rt;

	void Awake()
	{
		rt = GetComponent<RectTransform>();
		_instance = this;
		SelectGameObject(null);
	}

	void Start()
	{
		SetInteractive(true);
	}

	void Update()
	{
		if (selectedGameObject != EventSystem.current.currentSelectedGameObject)
		{
			//if (EventSystem.current.currentSelectedGameObject != null)
			SelectGameObject(EventSystem.current.currentSelectedGameObject);
			if (EventSystem.current.currentSelectedGameObject == null) return;
			//else return;
		}

		/*if (Input.GetMouseButtonDown(0))
			HandleActuator(Actuator.Use);*/

		if (selectedGameObject != null)
			transform.position = selectedGameObject.transform.position;
	}

	override public void HandleActuator(Actuator actuator)
	{
		switch (actuator)
		{
			case Actuator.Use:
				ClickCurrentButton();
				break;
		}
	}

	void SelectGameObject(GameObject target)
	{
		if (selectedGameObject != null)
		{
			Animator[] animators = selectedGameObject.GetComponentsInChildren<Animator>();
			ButtonAnimation buttonAnimation = selectedGameObject.GetComponent<ButtonAnimation>();
			for (int i = 0; i < animators.Length; i++)
			{
				animators[i].SetTrigger("exit");
			}
			if (buttonAnimation != null)
			{
				buttonAnimation.ChangeState(ButtonAnimation.state.Idle);
			}
			/*else
			{
				RectTransform rt = selectedGameObject.GetComponent<RectTransform>();
				rt.GetComponent<MonoBehaviour>().StopAllCoroutines();
				rt.DoTween01(t =>
				{
					float scale = PennerAnimation.BackEaseIn(t, 0, 1, 1);
					rt.localScale = Vector3.one + Vector3.one * 0.25f * (1 - scale);
				}, 0.25f);
			}
			*/
		}

		selectedGameObject = target;
		if (target != null)
		{
			if (highlightClip != null)
				AudioInstance.PlayClipAtPoint(highlightClip, Vector3.zero);
			transform.position = target.transform.position;
			//ResizeToTarget(target);
			AnimatedButton animatedButton = selectedGameObject.GetComponent<AnimatedButton>();
			if (animatedButton != null)
			{
				animatedButton.Select();
			}
			else if (target.GetComponent<IPointerClickHandler>() != null)
			{
				Animator[] animators = target.GetComponentsInChildren<Animator>();
				for (int i = 0; i < animators.Length; i++)
				{
					animators[i].SetTrigger("enter");
				}
				/*
				RectTransform rt = selectedGameObject.GetComponent<RectTransform>();
				rt.DoTween01(t =>
				{
					float scale = PennerAnimation.BackEaseOut(t, 0, 1, 1);
					rt.localScale = Vector3.one + Vector3.one * 0.25f * scale;
				}, 0.75f);
				*/
			}
		}
		else
		{
			transform.position = new Vector2(-5000, -5000);
		}
	}

	void ResizeToTarget(GameObject target)
	{
		if (StateViewManager.instance.currentView != null && StateViewManager.instance.currentView.showCursor)
		{
			RectTransform targetRt = target.GetComponent<RectTransform>();
			if (targetRt == null)
				return;

			rt.sizeDelta = new Vector2(targetRt.sizeDelta.x, rt.sizeDelta.y);
		}
		else
		{
			rt.sizeDelta = new Vector2(0, rt.sizeDelta.y);
		}
	}

	public void PlaySelectSound()
	{
		if (selectClip != null)
			AudioInstance.PlayClipAtPoint(selectClip, Vector3.zero);
	}

	void ClickCurrentButton()
	{
		if (selectedGameObject == null) return;
		AnimatedButton ab = selectedGameObject.GetComponent<AnimatedButton>();
		if (ab == null)
		{
			/*
			RectTransform rt = selectedGameObject.GetComponent<RectTransform>();
			rt.DoTween01(t =>
			{
				float scale = PennerAnimation.QuadEaseOut(t, 0, 1, 1);
				rt.localScale = Vector3.one * 1.25f - Vector3.one * 0.25f * scale;
			}, 0.1f, () =>
			{
				rt.DoTween01(t =>
				{
					float scale = PennerAnimation.QuadEaseIn(t, 0, 1, 1);
					rt.localScale = Vector3.one * 1.25f - Vector3.one * 0.25f * (1 - scale);
				}, 0.1f, () =>
				{
				});
			});
			*/
		}
		else
		{
			if (!ab.isClickable)
				return;
		}
		SimulateUIClick(selectedGameObject);
		PlaySelectSound();
	}

}
