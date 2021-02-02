using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class PlayerDeviceSelection : MonoBehaviour
{
    public delegate void NewPlayer(InputUser inputUser);
    public static event NewPlayer OnNewPlayer;
    
    public delegate void DeviceUpdated(InputUser inputUser);
    public static event DeviceUpdated OnDeviceUpdated;
        
    static PlayerDeviceSelection _instance;
    public static PlayerDeviceSelection instance
    {
        get
        {
            if (_instance == null)
                _instance = FindObjectOfType<PlayerDeviceSelection>();
            return _instance;
        }
    }

    public enum state
    {
        listenMainPlayer,
        bindPlayers,
        lockDevices
    }

    private state currentState;

    public void SetState(state nextState)
    {
        currentState = nextState;
    }

    public int maxPlayers
    {
        get => _maxPlayers;
        set
        {
            _maxPlayers = value;
            InputUser.listenForUnpairedDeviceActivity = _maxPlayers;
        }
    }
    public int _maxPlayers = 1;
    
    public Func<InputControl, bool> pairDeviceCheck;

    void Awake()
    {
        InputUser.listenForUnpairedDeviceActivity = 1;
        DontDestroyOnLoad(gameObject);
    }

    void OnEnable()
    {
        InputUser.onUnpairedDeviceUsed += InputUserOnUnpairedDeviceUsed;
    }

    void OnDisable()
    {
        InputUser.onUnpairedDeviceUsed -= InputUserOnUnpairedDeviceUsed;
    }
    
    public void RemoveAllUsers()
    {
        for (int i = 0; i < InputUser.all.Count; i++)
        {
            InputUser.all[i].UnpairDevicesAndRemoveUser();
        }
    }

    public void UnpairAllDevices()
    {
        for (int i = 0; i < InputUser.all.Count; i++)
            InputUser.all[i].UnpairDevices();
    }

    private void InputUserOnUnpairedDeviceUsed(InputControl inputControl, InputEventPtr inputEventPtr)
    {
        if (pairDeviceCheck != null && !pairDeviceCheck.Invoke(inputControl))
            return;
        
        switch (currentState)
        {
            case state.listenMainPlayer:
                PairUserWithDevice(0, inputControl.device);
                break;
            case state.bindPlayers:
                for (var index = 0; index < InputUser.all.Count; index++)
                {
                    var inputUser = InputUser.all[index];
                    if (inputUser.pairedDevices.Count == 0)
                    {
                        PairUserWithDevice(index, inputControl.device);
                        return;
                    }
                }
                if (InputUser.all.Count < maxPlayers)
                    PairUserWithDevice(InputUser.all.Count, inputControl.device);
                break;
            case state.lockDevices:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    private void PairUserWithDevice(int userIndex, InputDevice device)
    {
        InputUser user;
        if (InputUser.all.Count > userIndex)
        {
            user = InputUser.all[userIndex];
            InputUser.PerformPairingWithDevice(device, user, InputUserPairingOptions.UnpairCurrentDevicesFromUser);
            OnDeviceUpdated?.Invoke(user);
        }
        else
        {
            user = InputUser.PerformPairingWithDevice(device);
            OnNewPlayer?.Invoke(user);
        }
    }
    
    public GameObject CreatePlayerInput(GameObject playerInputPrefab, int playerIndex)
    {
        GameObject obj = Instantiate(playerInputPrefab);
        SetupPlayerInput(obj, playerIndex);
        return obj;
    }

    public void SetupPlayerInput(GameObject playerInputObj, int playerIndex)
    {
        ActionMapper actionMapper = playerInputObj.GetComponentInChildren<ActionMapper>();
        
        var user = InputUser.all[playerIndex];
        actionMapper.user = user;
        actionMapper.user.AssociateActionsWithUser(actionMapper.actionCollection);
    }


}

