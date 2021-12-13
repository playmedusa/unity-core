using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.InputSystem.Users;

public class PlayerDeviceSelection : MonoBehaviour
{
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
    private MenuPlayerActions m;

    public void SetState(state nextState)
    {
        currentState = nextState;
    }
    
    void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public GameObject CreatePlayerInput(GameObject playerInputPrefab, int playerIndex)
    {
        GameObject obj = Instantiate(playerInputPrefab);
        return obj;
    }

    public static void SetupPlayerInput(GameObject playerInputObj, int playerIndex)
    {
        ActionMapper actionMapper = playerInputObj.GetComponentInChildren<ActionMapper>();

        var user = InputUser.all[playerIndex];
        actionMapper.user = user;
        actionMapper.user.AssociateActionsWithUser(actionMapper.actionCollection);
    }


}

