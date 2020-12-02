using UnityEngine;
using UnityEngine.Events;
using System;
using System.Collections;
using System.Collections.Generic;
using InputMapper;

public class InputDeviceSystem : MonoBehaviour
{

	public InputDeviceComponent idc;
	public string replayFile;
	public bool showMouseCursorDefault;
	public UnityAction OnStartReplay;
	bool replaying;
	public static float replayDeltaTime = 0;

	void Awake()
	{
		if (replayFile != "")
		{
			try
			{
				string path = Application.persistentDataPath + "/" + replayFile;
				string json = System.IO.File.ReadAllText(path);
				PersistedInput input = JsonUtility.FromJson<PersistedInput>(json);
				replaying = true;
				StartCoroutine(Replay(input));
			}
			catch
			{
				Debug.LogError("File not found: " + replayFile);
				replaying = false;
			}
		}

		Cursor.visible = showMouseCursorDefault;
		Cursor.lockState =
			showMouseCursorDefault ?
				CursorLockMode.None :
				CursorLockMode.Locked;
	}

	IEnumerator Replay(PersistedInput input)
	{
		int replayIndex = 0;
		int currentFrame = 0;

		OnStartReplay.Invoke();

		print("Starting replay in 2 seconds...");
		yield return new WaitForSeconds(2);
		print("Go!");

		while (replayIndex < input.persistedInputList.Count)
		{
			InputLog il = input.persistedInputList[replayIndex];
			while (il.frame <= currentFrame)
			{
				idc.actions[il.actuator] = il.value;
				replayDeltaTime = il.deltaTime;
				ProcessActuator(il.actuator, il.inputKey);
				replayIndex++;
				if (replayIndex == input.persistedInputList.Count) break;
				il = input.persistedInputList[replayIndex];
			}
			currentFrame++;
			yield return 0;
		}

		print("Replay Finished");
	}

	void Update()
	{
		SetCurrentDevice(idc);
		SetPause(idc);

		if (replaying) return;

		foreach (var bind in idc.currentDevice.binds)
		{
			float inputValue = ProcessInput(idc.currentDevice, bind.inputKey);
			float altValue = ProcessInput(idc.currentDevice, bind.altKey);
			//Debug.Log(bind.actuator + ": " + inputValue + ", " + altValue);
			idc.actions[bind.actuator] = inputValue == 0 ? altValue : inputValue;
			if (idc.recordInput)
				RecordInput(idc, bind, idc.actions[bind.actuator]);
			ProcessActuator(bind.actuator, bind.inputKey);
		}
	}

	void RecordInput(InputDeviceComponent idc, Bind bind, float value)
	{
		InputLog il = new InputLog();
		il.actuator = bind.actuator;
		il.inputKey = bind.inputKey;
		il.value = value;
		il.frame = Time.frameCount;
		il.deltaTime = Time.deltaTime;
		idc.persistedInput.persistedInputList.Add(il);
	}

	void SetCurrentDevice(InputDeviceComponent idc)
	{
		string[] temp = Input.GetJoystickNames();

		if (temp.Length > idc.playerIndex)
		{
			InputSO d = Array.Find(idc.supportedDevices,
				e => e.ControllerName == temp[idc.playerIndex]
			/*XInput Controller
			PS4 Controller*/
			);
			idc.currentDevice = d;
		}

		if (idc.currentDevice == null)
			idc.currentDevice = idc.supportedDevices[0];
	}

	void SetPause(InputDeviceComponent idc)
	{
		if (idc[Actuator.Start])
		{
			EventBus.Raise(new GameEvent_SetPause(true));
			if (idc.pauseTarget != null)
			{
				StateViewManager.instance.ShowStateView(idc.pauseTarget);
			}
		}
	}

	float ProcessInput(InputSO controls, InputKey input)
	{
		//Process input
		switch (input.type)
		{
			case InputType.Axis:
				ProcessAxis(controls, input);
				break;
			case InputType.KeyCode:
				ProcessKeyCode(controls, input);
				break;
			case InputType.Button:
				ProcessButton(controls, input);
				break;
		}

		//Process modifiers
		switch (input.modifier)
		{
			case InputModifier.Double:
				if (input.value != 0 && input.isConsecutiveDown)
				{
					return input.value;
				}
				else
					return 0;
			case InputModifier.Hold:
				return input.isHold ? 1 : 0;
		}
		return input.value;
	}

	void ProcessAxis(InputSO input, InputKey inputKey)
	{
		switch (input.inputDevice)
		{
			case InputDevice.Keyboard:
			case InputDevice.JoyPad:
				inputKey.value = Input.GetAxis(inputKey.axisOrButton);
				if (inputKey.invertValue)
					inputKey.value = -inputKey.value;
				break;
			default:
				Debug.LogError("Unhandled input device: " + input.inputDevice);
				break;
		}
	}

	void ProcessKeyCode(InputSO input, InputKey ak)
	{
		switch (input.inputDevice)
		{
			case InputDevice.Keyboard:
			case InputDevice.JoyPad:
				ak.value = Input.GetKey(ak.keyCode) ? 1 : 0;
				if (ak.invertValue)
					ak.value = -ak.value;
				break;
			default:
				Debug.LogError("Unhandled input device: " + input.inputDevice);
				break;
		}
	}

	void ProcessButton(InputSO input, InputKey ak)
	{
		switch (input.inputDevice)
		{
			case InputDevice.Keyboard:
				ak.value = Input.GetKeyDown(ak.axisOrButton) ? 1 : 0;
				if (ak.invertValue)
					ak.value = -ak.value;
				break;
			case InputDevice.JoyPad:
				KeyCode keycode = (KeyCode)(int.Parse(ak.axisOrButton));
				ak.value = Input.GetKey(keycode) ? 1 : 0;
				if (ak.invertValue)
					ak.value = -ak.value;
				break;
			default:
				Debug.LogError("Unhandled input device: " + input.inputDevice);
				break;
		}
	}

	void ProcessActuator(Actuator actuator, InputKey inputKey)
	{
		if (idc[actuator])
		{
			switch (inputKey.modifier)
			{
				case InputModifier.Single:
				case InputModifier.Double:
					if (!(actuator != idc.lastActuator || (actuator == idc.lastActuator && Time.unscaledTime - idc.lastActuatorStamp > 0.2f)))
						return;
					break;
			}
			idc.lastActuator = actuator;
			idc.lastActuatorStamp = Time.unscaledTime;
			idc.TriggerActuator(actuator);
		}
	}

}
