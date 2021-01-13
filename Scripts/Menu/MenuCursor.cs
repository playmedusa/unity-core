using UnityEngine;
using UnityEngine.EventSystems;

public class MenuCursor : MonoBehaviour
{
	static MenuCursor _instance;
	public static MenuCursor instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<MenuCursor>();
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
		transform.position = new Vector2(-5000, -5000);
	}

	void Update()
	{
		if (EventSystem.current.currentSelectedGameObject == null) return;

		if (selectedGameObject != EventSystem.current.currentSelectedGameObject)
		{
			SelectGameObject(EventSystem.current.currentSelectedGameObject);
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
		}
		
		if (highlightClip != null)
			AudioInstance.PlayClipAtPoint(highlightClip, Vector3.zero);
		
		selectedGameObject = target;
		if (target != null)
		{
			
			rt.SetParent(target.transform);
			rt.anchoredPosition = Vector2.zero;
			
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
			}
		}
		else
		{
			transform.position = new Vector2(-5000, -5000);
		}
	}

	public void PlaySelectSound()
	{
		if (selectClip != null)
			AudioInstance.PlayClipAtPoint(selectClip, Vector3.zero);
	}
}
