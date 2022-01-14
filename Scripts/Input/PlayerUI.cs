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
        playerManager = await PlayerManager.GetPlayerAsync(playerIndex);
        playerManager.SetupInputSystemUI(inputSystemUIInputModule);
        Init();
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
