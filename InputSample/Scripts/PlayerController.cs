#if ENABLE_INPUT_SYSTEM
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.UI;

public class PlayerController : MonoBehaviour, SampleControls.IAvatarActions
{
    public InputSystemUIInputModule inputSystemUIInputModule;
    public GameObject ui;
    public int playerIndex = 0;
    
    async void Start()
    {
        var playerManager = await LobbyManager.GetPlayerAsync(playerIndex);
        if (inputSystemUIInputModule != null)
            playerManager.SetupInputSystemUI(inputSystemUIInputModule);
        playerManager.SetControls(new SampleControls());
        playerManager.BindControls<SampleControls.AvatarActions>(this);
        playerManager.SwitchActionMap<SampleControls.AvatarActions>();
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        transform.position += (Vector3) context.ReadValue<Vector2>() * Time.deltaTime * 5;
    }

    public void OnAttack(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        
        ui.SetActive(true);
        var playerManager = LobbyManager.GetPlayer(playerIndex);
        playerManager.SwitchActionMap<SampleControls.UIActions>();
    }

    public void HideUI()
    {
        var playerManager = LobbyManager.GetPlayer(playerIndex);
        playerManager.SwitchActionMap<SampleControls.AvatarActions>();
        ui.SetActive(false);
    }

}
#endif