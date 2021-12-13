
using System;
using System.Linq;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class ActionMapper: MonoBehaviour
{
    public InputUser user { get; set; }
    public IInputActionCollection actionCollection { get; private set; }

    private Type currentSchema;

    private void Awake()
    {
        // InputUser.onChange += InputUserOnChange;
    }

    private void OnDestroy()
    {
        // InputUser.onChange -= InputUserOnChange;
        if (user.valid)
            DisableCurrentSchema(actionCollection);
    }
    
    private void InputUserOnChange(InputUser inputUser, InputUserChange inputUserChange, InputDevice inputDevice)
    {
        if (inputUser != user)
            return;
        
        switch (inputUserChange)
        {
            case InputUserChange.Removed:
                Destroy(gameObject);
                break;
            case InputUserChange.DeviceUnpaired:
                DisableCurrentSchema(actionCollection);
                break;
            case InputUserChange.DevicePaired:
                EnableCurrentSchema(actionCollection);
                break;
        }
    }
    
    public void SetCallbacks<T>(Component handler, IInputActionCollection actionCollection)
    {
        this.actionCollection = actionCollection;
        
        if (currentSchema != null)
        {
            var properties= actionCollection.GetType().GetProperties();
            var wrapperInfo = properties.First(p => p.PropertyType == currentSchema);
            var oldWrapper = (T) wrapperInfo.GetValue(actionCollection);
            var disable= oldWrapper.GetType().GetMethod("Disable");
            disable.Invoke(oldWrapper, null);
        }
        currentSchema = typeof(T);

        var wrapper = GetWrapper<T>(actionCollection);
        var setCallbacks= wrapper.GetType().GetMethod("SetCallbacks");
        var enable= wrapper.GetType().GetMethod("Enable");
        setCallbacks.Invoke(wrapper, new object[] {handler});
        enable.Invoke(wrapper, null);
    }

    public void DisableCurrentSchema(IInputActionCollection actionCollection)
    {
        var properties= actionCollection.GetType().GetProperties();
        var wrapperInfo = properties.First(p => p.PropertyType == currentSchema);
        var oldWrapper = wrapperInfo.GetValue(actionCollection);
        var disable= oldWrapper.GetType().GetMethod("Disable");
        disable.Invoke(oldWrapper, null);
    }
    
    public void EnableCurrentSchema(IInputActionCollection actionCollection)
    {
        var properties= actionCollection.GetType().GetProperties();
        var wrapperInfo = properties.First(p => p.PropertyType == currentSchema);
        var oldWrapper = wrapperInfo.GetValue(actionCollection);
        var enable= oldWrapper.GetType().GetMethod("Enable");
        enable.Invoke(oldWrapper, null);
    }

    T GetWrapper<T>(IInputActionCollection actionCollection)
    {
        var properties= actionCollection.GetType().GetProperties();
        var wrapperInfo = properties.First(p => p.PropertyType == typeof(T));
        var wrapper = (T) wrapperInfo.GetValue(actionCollection);
        return wrapper;
    }
}