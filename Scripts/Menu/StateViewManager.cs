﻿using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;
using InputMapper;

public class StateViewManager : FSM<StateViewManager.state>
{

	public enum state
	{
		idle,
		showView
	}

	private static StateViewManager _instance;
	public static StateViewManager instance
	{
		get
		{
			if (_instance == null)
				_instance = FindObjectOfType<StateViewManager>();
			return _instance;
		}
	}
	public StateView initialyOpen;
	public StateView staticView;

	public StateView currentView
	{
		get;
		private set;
	}

	StateView nextView;
	bool lockView = false;

	public bool isUsingMouse
	{
		get;
		private set;
	}

	InputDeviceComponent idc;

	void Start()
	{
		idc = FindObjectOfType<InputDeviceComponent>();


		if (initialyOpen == null)
			initialyOpen = GetComponentInChildren<StateView>();
		ShowStateView(initialyOpen);
	}

	void Update()
	{
		if (idc != null)
			SetUsingMouse(idc.currentDevice.inputDevice == InputDevice.Keyboard);

		if (isUsingMouse)
		{
			Cursor.visible = currentView.showMouse;
			Cursor.lockState =
				currentView.showMouse ?
					CursorLockMode.None :
					CursorLockMode.Locked;
		}
		else
		{
			Cursor.visible = false;
		}
	}

	public void ShowStateView(StateView nextView, bool lockView)
	{
		if (this.lockView) return;
		this.lockView = lockView;
		this.nextView = nextView;
		ChangeState(state.showView);
	}

	public void ShowStateView(StateView nextView)
	{
		ShowStateView(nextView, false);
	}

	public void UnlockAndShowStateView(StateView nextView)
	{
		lockView = false;
		ShowStateView(nextView);
	}

	public void SetUsingMouse(bool status)
	{
		isUsingMouse = status;
	}

	IEnumerator showView()
	{
		while (currentState == state.showView)
		{
			bool done = false;
			if (currentView != null)
			{
				if (currentView.ui != null)
				{
					var newPreviouslySelected = (EventSystem.current != null) ? EventSystem.current.currentSelectedGameObject : null;
					if (currentView.rememberPreviouslySelected)
						currentView.m_PreviouslySelected = newPreviouslySelected;
					SetSelected(null);
				}
				currentView.Hide(() =>
				{
					done = true;
				});
				yield return new WaitUntil(() => done);
			}

			currentView = nextView;
			done = false;

			HandleStaticViewVisibility();

			currentView.Show(() =>
			{
				done = true;
			});

			yield return new WaitUntil(() => done);

			SelectBestCandidate();

			if (currentView == nextView)
			{
				ChangeState(state.idle);
			}
		}
	}

	void HandleStaticViewVisibility()
	{
		if (staticView != null)
		{
			if (currentView.showStatic && !staticView.isOpen)
				staticView.Show();
			else if (!currentView.showStatic && staticView.isOpen)
				staticView.Hide();
		}
	}

	IEnumerator idle()
	{
		while (currentState == state.idle)
		{
			if (currentView == null) yield break;

			if (EventSystem.current.currentSelectedGameObject == null)
			{
				SelectBestCandidate();
			}
			if (Input.GetAxis("Mouse Y") != 0)
				SetUsingMouse(true);
			if (idc.raw(Actuator.ForwardAxis) != 0)
				SetUsingMouse(false);
			yield return 0;
		}
	}

	void SelectBestCandidate()
	{
		if (currentView.ui != null)
		{
			if (currentView.m_PreviouslySelected != null)
			{
				SetSelected(currentView.m_PreviouslySelected);
			}
			else if (!isUsingMouse)
			{
				if (currentView.skipAutoselect) return;

				var newPreviouslySelected = FindFirstEnabledSelectable(currentView.ui.gameObject);
				SetSelected(newPreviouslySelected);
			}
		}
	}

	private void SetSelected(GameObject go)
	{
		//if (EventSystem.current == null)
		//return;
		//Select the GameObject.
		EventSystem.current.SetSelectedGameObject(go);

		//If we are using the keyboard right now, that's all we need to do.
		var standaloneInputModule = EventSystem.current.currentInputModule as StandaloneInputModule;
		if (standaloneInputModule != null)
			return;

		//Since we are using a pointer device, we don't want anything selected. 
		//But if the user switches to the keyboard, we want to start the navigation from the provided game object.
		//So here we set the current Selected to null, so the provided gameObject becomes the Last Selected in the EventSystem.
		EventSystem.current.SetSelectedGameObject(null);
	}

	static GameObject FindFirstEnabledSelectable(GameObject gameObject)
	{
		GameObject go = null;
		var selectables = gameObject.GetComponentsInChildren<Selectable>(true);
		foreach (var selectable in selectables)
		{
			if (selectable.IsActive() && selectable.IsInteractable())
			{
				go = selectable.gameObject;
				break;
			}
		}
		return go;
	}

}
