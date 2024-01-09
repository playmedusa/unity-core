
#if ENABLE_INPUT_SYSTEM
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.XInput;

public class LobbyManager : MonoBehaviour
{
    public int initialMaxPlayers = 1;
    
    public static Action onNewPlayer;
    public static Action onDeviceUpdated;
    
    public static readonly List<PlayerManager> players = new();
    public static int count => players?.Count ?? 0;
    
    public static int maxPlayers => _maxPlayers;
    private static int _maxPlayers = 2;

    [SerializeField]
    private PlayerInputManager playerInputManager;


    void Awake()
    {
        SetMaxPlayers(initialMaxPlayers);
    }
    
    private void OnValidate()
    {
        if (playerInputManager == null)
            playerInputManager = GetComponent<PlayerInputManager>();
    }

    private void OnEnable()
    {
        InputUser.onUnpairedDeviceUsed += OnUnpairedDeviceUsed;
        InputSystem.onDeviceChange += OnDeviceChange;

    }

    private void OnDisable()
    {
        InputUser.onUnpairedDeviceUsed -= OnUnpairedDeviceUsed;
        InputSystem.onDeviceChange -= OnDeviceChange;
    }
    
    void OnUnpairedDeviceUsed(InputControl usedControl, InputEventPtr arg2)
    {
        // Only react to button presses on unpaired devices.
        //if (!(usedControl is ButtonControl))
        //    return;
        Debug.Log("Unpaired device used " + usedControl);

        //if (usedControl is XInputController) return;
        
        if (count < _maxPlayers)
        {
            playerInputManager.JoinPlayer(-1, -1, null, usedControl.device);
        }
    }
    
    void OnDeviceChange(InputDevice device, InputDeviceChange change)
    {
        Debug.Log("Device changed " + change);
    }
    

    public static void SetMaxPlayers(int newMax)
    {
        _maxPlayers = newMax;

        for (int i = players.Count-1; i >= 0; i--)
        {
            if (i == 0)
            {
                players[i].allowP0AutoSwitch = newMax == 1;
                players[i]._playerInput.neverAutoSwitchControlSchemes = newMax != 1;
            }
            if (i >= newMax)
            {
              // players[i]._playerInput.user.UnpairDevicesAndRemoveUser();
               Destroy(players[i].gameObject);
            }
        }
    }
    
    
    

    public static bool IsPlayerReady(int playerIndex)
    {
        return (bool)GetPlayer(playerIndex)?.isDeviceReady;
    }

    public static void AddPlayerManager(PlayerManager playerManager)
    {
        players.Add(playerManager);
        playerManager.SetPlayerIndex(players.Count - 1);
        onNewPlayer?.Invoke();
    }
    
    public static void RemovePlayerManager(PlayerManager playerManager)
    {
        if (players.Contains(playerManager))
            players.Remove(playerManager);
    }

    public static void RemoveAllDevices ()
    {
        for (int i = 0; i < InputUser.all.Count; i++)
        {
            InputUser.all[i].UnpairDevices();
        }
        InputSystem.FlushDisconnectedDevices();
    }

    public static PlayerManager GetPlayer(int index)
    {
        try
        {
            return players[index];
        }
        catch
        {
            return null;
        }
    }
    
    public static async Task<PlayerManager> GetPlayerAsync(int index)
    {
        await TaskExtensions.WaitUntil(() => count > index);
        return GetPlayer(index);
    }

    void Update()
    {
        if (Keyboard.current.anyKey.wasPressedThisFrame && InputUser.FindUserPairedToDevice(Keyboard.current) == null)
        {
            if (count < _maxPlayers)
            {
                playerInputManager.JoinPlayer(-1, -1, null, Keyboard.current);
            }
        }

        if (Gamepad.current?.buttonSouth.wasPressedThisFrame == true && InputUser.FindUserPairedToDevice(Gamepad.current) == null)
        {
            playerInputManager.JoinPlayer(-1, -1, null, Gamepad.current);
        }
    }

}
#endif
