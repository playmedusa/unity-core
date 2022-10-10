#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class UIController : PlayerUI, SampleControls.IUIActions
{
    private PlayerUI playerUI;

    protected override void Init()
    {
        playerManager.SetControls(new SampleControls());
        playerManager.BindControls<SampleControls.UIActions>(this);
        playerManager.SwitchActionMap<SampleControls.UIActions>();
    }

    public void OnTrackedDeviceOrientation(InputAction.CallbackContext context)
    {
        
    }

    public void OnTrackedDevicePosition(InputAction.CallbackContext context)
    {
        
    }

    public void OnRightClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnMiddleClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnScrollWheel(InputAction.CallbackContext context)
    {
        
    }

    public void OnClick(InputAction.CallbackContext context)
    {
        
    }

    public void OnPoint(InputAction.CallbackContext context)
    {
        
    }

    public void OnCancel(InputAction.CallbackContext context)
    {
        
    }

    public void OnNavigate(InputAction.CallbackContext context)
    {
        
    }

    public void OnSubmit(InputAction.CallbackContext context)
    {
        if (context.performed)
            SceneManager.LoadScene("Game");
    }
}
#endif