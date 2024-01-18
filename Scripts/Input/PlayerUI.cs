#if ENABLE_INPUT_SYSTEM
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem.UI;

[RequireComponent(typeof(InputSystemUIInputModule))]
public class PlayerUI : MonoBehaviour
{
    public int playerIndex = 0;
    
    protected InputSystemUIInputModule inputSystemUIInputModule;
    protected PlayerManager playerManager;
    
    async void Awake()
    {
        inputSystemUIInputModule = GetComponent<InputSystemUIInputModule>();
        await SetUIPlayerOwner(0);
    }

    public async Task SetUIPlayerOwner(int index)
    {
        playerManager = await LobbyManager.GetPlayerAsync(index);
        playerManager.SetupInputSystemUI(inputSystemUIInputModule);
        
        //ONLY WORKS FOR 2 PLAYERS
        playerManager = await LobbyManager.GetPlayerAsync((index + 1) % 2);
        playerManager?.SetupInputSystemUI(null);
    }
    
    protected virtual void Init()
    {
        /*
         * Override this method to initialize the playerInput's actions.
         * This could be useful for main-menu handling, when the player is not
         * playing the game (not reading any actions somewhere else).
         * eg:
         *
         * playerManager.SetControls(new SampleControls());
         * playerManager.BindControls<SampleControls.UIActions>(this); //Only if needed
         * playerManager.SwitchActionMap<SampleControls.UIActions>();
         */
    }

    private void OnDestroy()
    {
        if (playerManager != null)
            playerManager.SetupInputSystemUI(null);
    }
}
#endif