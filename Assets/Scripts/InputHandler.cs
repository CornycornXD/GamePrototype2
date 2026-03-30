using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : MonoBehaviour
{
    private PlayerInputActions _input;

    public static event Action<int> OnClickPerformed;
    public static event Action<char> WASDEntered;

    private void Awake()
    {
        _input = new PlayerInputActions();
    }

    private void OnEnable()
    {
        _input.Enable();

        _input.Player.LeftClick.performed += HandleLeftClick;
        _input.Player.RightClick.performed += HandleRightClick;
        _input.Player.W_Entered.performed += HandleWEntered;
        _input.Player.A_Entered.performed += HandleAEntered;
        _input.Player.S_Entered.performed += HandleSEntered;
        _input.Player.D_Entered.performed += HandleDEntered;

    }

    private void OnDisable()
    {
        _input.Disable();

        _input.Player.LeftClick.performed -= HandleLeftClick;
        _input.Player.RightClick.performed -= HandleRightClick;
        _input.Player.W_Entered.performed -= HandleWEntered;
        _input.Player.A_Entered.performed -= HandleAEntered;
        _input.Player.S_Entered.performed -= HandleSEntered;
        _input.Player.D_Entered.performed -= HandleDEntered;
    }

    private void HandleLeftClick(InputAction.CallbackContext context)
    {
        OnClickPerformed?.Invoke(0);
    }

    private void HandleRightClick(InputAction.CallbackContext context) {
        OnClickPerformed?.Invoke(1);
    }
    private void HandleWEntered(InputAction.CallbackContext context) {
        WASDEntered?.Invoke('W');
    }

    private void HandleAEntered(InputAction.CallbackContext context)
    {
        WASDEntered?.Invoke('A');
    }

    private void HandleSEntered(InputAction.CallbackContext context)
    {
        WASDEntered?.Invoke('S');
    }

    private void HandleDEntered(InputAction.CallbackContext context)
    {
        WASDEntered?.Invoke('D');
    }
}
