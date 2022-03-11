using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class LobbyManager
{
    
    public static Action onNewPlayer;
    public static Action onDeviceUpdated;
    
    public static readonly List<PlayerManager> players = new();
    public static int count => players?.Count ?? 0;
    
    public static bool AllPlayersReady()
    {
        if (count == 0) return false;
        foreach (var playerManager in players)
        {
            if (!playerManager.isDeviceReady)
                return false;
        }
        return true;
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

}