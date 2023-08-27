using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : Singleton<InputManager> //Singleton to ensure there is only one instance
{
    public GameInputActions gameInput{ get; private set; }

    private void Awake()
    {
        gameInput = new GameInputActions();
    }

    private void Start()
    {
        ToggleMouseCursor(false);
    }

    public void ToggleMouseCursor(bool val)
    {
        if(val)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }

    #region ToggleInputs
    public void ToggleInventoryInputs(bool val)
    {
        if (val)
            gameInput.Hotbar.Enable();
        else
            gameInput.Hotbar.Disable();

    }

    public void ToggleUIInputs(bool val)
    {
        if(val)
        {
            gameInput.UI.Enable();
        }            
        else
        {
            gameInput.UI.Disable();
        }
            
    }

    public void TogglePlayerInputs(bool val)
    {
        if(val)
            gameInput.Player.Enable();
        else
            gameInput.Player.Disable();
    }

    public void TogglePlacementInputs(bool val)
    {
        if(val)
            gameInput.Placement.Enable();
        else
            gameInput.Placement.Disable();
    }

    #endregion
}
