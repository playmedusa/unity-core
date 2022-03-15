using System;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;
using UnityEngine.InputSystem.Users;

[RequireComponent(typeof(PlayerInput))]
//Based on: https://forum.unity.com/threads/playerinput-across-scene-loads.621586/
public class PlayerManager : MonoBehaviour
{
    public bool allowP0AutoSwitch = true;
    
    public PlayerInput _playerInput;
    public int playerIndex { get; private set; }
    public bool isDeviceReady => device != null && _isDeviceReady;
    public InputDevice device
    {
        get
        {
            if (_playerInput != null && _playerInput.devices.Count > 0) 
                return _playerInput.devices[0];
            return null;
        }
    }

    private bool _isDeviceReady = true;
    
    private IInputActionCollection _controls;
    private Type currentActionMap;
    
    public Action onDeviceUpdated;

    public void SetPlayerIndex(int index)
    {
        playerIndex = index;
    }

    void Awake ()
    {
        _playerInput = GetComponent<PlayerInput>();
        if (_playerInput == null)
        {
            Debug.LogError("Missing PlayerInput component!");
            return;
        }
        LobbyManager.AddPlayerManager(this);
        gameObject.name = $"Player {_playerInput.playerIndex}";
        DontDestroyOnLoad(gameObject);
        InputUser.onChange += InputUserOnChange;
    }

    private void InputUserOnChange(InputUser inputUser, InputUserChange inputUserChange, InputDevice inputDevice)
    {
        if (inputUser != _playerInput.user) return;
        Debug.Log(inputUserChange);
        switch (inputUserChange)
        {
            case InputUserChange.DevicePaired:
            case InputUserChange.DeviceRegained:
                SetDeviceStatus(true);
                break;
            case InputUserChange.DeviceUnpaired:
                ++InputUser.listenForUnpairedDeviceActivity;
                InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
                SetDeviceStatus(false);
                break;
            case InputUserChange.DeviceLost:
                SetDeviceStatus(false);
                break;
            case InputUserChange.Removed:
                Destroy(gameObject);
                break;
        }
    }

    private void OnUnpairedDeviceUsed(InputControl usedControl, InputEventPtr arg2)
    {
        // Only react to button presses on unpaired devices.
        //if (!(usedControl is ButtonControl))
        //    return;

        // Pair the device to a user.
        if (device == null && InputUser.FindUserPairedToDevice(usedControl.device) == null)
        {
            InputUser.PerformPairingWithDevice(usedControl.device, _playerInput.user);
            --InputUser.listenForUnpairedDeviceActivity;
            InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
        }
    }

    async void SetDeviceStatus(bool isReady)
    {
        _isDeviceReady = isReady;
        if (!isReady)
            await Task.Delay(500);
        onDeviceUpdated?.Invoke();
        LobbyManager.onDeviceUpdated?.Invoke();
    }

    private void OnDestroy()
    {
        InputUser.onChange -= InputUserOnChange;
    }

    public void SetupInputSystemUI(InputSystemUIInputModule uiInputModule)
    {
        _playerInput.uiInputModule = uiInputModule;
    }

    /// <summary><c>SetControls</c> expects <c>actionCollection</c> to be a new instance of your
    /// InputActionCollection. Ej: var p1Controls = new MyControls(); </summary>
    public void SetControls<T>(T actionCollection) where T : IInputActionCollection
    {
        _controls = actionCollection;
        var properties= actionCollection.GetType().GetProperty("asset");
        var asset = (InputActionAsset) properties.GetValue(actionCollection);

        if (_playerInput.devices.Count > 0) //based on https://forum.unity.com/threads/playerinput-loses-paired-devices-when-actions-are-changed-at-runtime.771140/
        {
            var devices = _playerInput.devices.ToArray();
            _playerInput.actions = asset;
            foreach (var device in devices)
            {
                InputUser.PerformPairingWithDevice(device, _playerInput.user);
            }
        }
        else
        {
            _playerInput.actions = asset;
        }

        if (_playerInput.uiInputModule != null)
        {
            _playerInput.uiInputModule.actionsAsset = _playerInput.actions;
            _playerInput.uiInputModule.actionsAsset.devices = _playerInput.devices;

            //This shouldn't be needed, BUT: https://forum.unity.com/threads/input-system-1-2-0-breaks-multiplayer-ui-navigation-weird-fix-inside.1210365/
            _playerInput.uiInputModule.enabled = false;
            _playerInput.uiInputModule.enabled = true;
        }

        _playerInput.neverAutoSwitchControlSchemes = _playerInput.playerIndex > 0 || !allowP0AutoSwitch;
    }

    void DisableCurrentActionMap()
    {
        _playerInput.actions.Disable();
        if (_controls == null)
        {
            Debug.LogError("Call SetControls first to initialize");
            return;
        }
        
        if (currentActionMap != null)
        {
            var properties= _controls.GetType().GetProperties();
            var wrapperInfo = properties.First(p => p.PropertyType == currentActionMap);
            var oldWrapper = wrapperInfo.GetValue(_controls);
            var disable= oldWrapper.GetType().GetMethod("Disable");
            disable.Invoke(oldWrapper, null);
        }
    }
    
    public void BindControls<T>(Component handler)
    {
        if (_controls == null)
        {
            Debug.LogError("Call SetControls first to initialize");
            return;
        }
        var wrapper = GetWrapper<T>(_controls);
        var setCallbacks= wrapper.GetType().GetMethod("SetCallbacks");
        setCallbacks.Invoke(wrapper, new object[] {handler});
    }
    
    public void SwitchActionMap<T>()
    {
        if (_controls == null)
        {
            Debug.LogError("Call SetControls first to initialize");
            return;
        }
        
        Debug.Log($"Enabling {typeof(T).Name}");
        _playerInput.actions.Disable();
        var wrapper = GetWrapper<T>(_controls);
        var enable= wrapper.GetType().GetMethod("Enable");
        enable.Invoke(wrapper, null);
        currentActionMap = typeof(T);
    }
    
    T GetWrapper<T>(IInputActionCollection actionCollection)
    {
        var properties= actionCollection.GetType().GetProperties();
        var wrapperInfo = properties.First(p => p.PropertyType == typeof(T));
        var wrapper = (T) wrapperInfo.GetValue(actionCollection);
        return wrapper;
    }


    // Just for reference
    // Based on: https://forum.unity.com/threads/solved-can-the-new-input-system-be-used-without-the-player-input-component.856108/#post-5669128
    public void BindGamepad<T>(int playerIndex) where T : IInputActionCollection, new()
    {
        _controls?.Disable();
        _controls = new T();
        _controls.devices = new[] { Gamepad.all[playerIndex] };
        _controls.bindingMask = InputBinding.MaskByGroup("Gamepad");
        _controls.Enable();
    }

    public void BindKeyboardMouse(int playerIndex)
    {
        _controls?.Disable();
        _controls.devices = new InputDevice[] { Keyboard.current, Mouse.current };
        _controls.bindingMask = InputBinding.MaskByGroup("KeyboardMouse");
        _controls.Enable();
    }

    //Pair methods shouldn't be needed. Using the generated class should be enough with methods provided above.
    public void PairGamepad<T>(int playerIndex) where T : IInputActionCollection, new()
    {
        _controls?.Disable();
        _controls = new T();
        var iUser = InputUser.PerformPairingWithDevice(Gamepad.all[playerIndex]);
        iUser.AssociateActionsWithUser(_controls);
        iUser.ActivateControlScheme("Gamepad");
        _controls.Enable();
    }
    
    public void PairKeyboardMouse<T>(int playerIndex) where T : IInputActionCollection, new()
    {
        _controls?.Disable();
        _controls = new T();
        var iUser = InputUser.PerformPairingWithDevice(Keyboard.current);
        InputUser.PerformPairingWithDevice(Mouse.current, user: iUser);
        iUser.AssociateActionsWithUser(_controls);
        iUser.ActivateControlScheme("KeyboardMouse");
        _controls.Enable();
    }
    
}
#endif