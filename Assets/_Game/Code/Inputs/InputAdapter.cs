using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputAdapter : MonoBehaviour
{
    public bool isInputActive { get; private set;  }
    private InputActionMap _playerActionsMap;
    private InputAction moveInput;
    private InputAction jumpInput;
    private InputAction jumpInputUp;
    private InputAction jumpInputDown;
    private InputAction dashInput;
    private InputAction test;


    public event Action<InputAction.CallbackContext> OnDash;
    public event Action<InputAction.CallbackContext> OnJumpUp;
    public event Action<InputAction.CallbackContext> OnJumpDown;

    private void HandleOnDash(InputAction.CallbackContext context)
    {
        if(OnDash != null) OnDash.Invoke(context);
    }

    private void HandleOnJumpUp(InputAction.CallbackContext context)
    {
        if (OnJumpUp != null) OnJumpUp.Invoke(context);
    }

    private void HandleOnJumpDown(InputAction.CallbackContext context)
    {
        if (OnJumpDown != null) OnJumpDown.Invoke(context);
    }


    private void Awake()
    {
        PlayerInput player = GetComponent<PlayerInput>();

        _playerActionsMap = player.actions.FindActionMap("Player");
        moveInput = _playerActionsMap.FindAction("Move");
        jumpInput = _playerActionsMap.FindAction("Jump");
        jumpInputUp = _playerActionsMap.FindAction("JumpUp");
        jumpInputDown = _playerActionsMap.FindAction("JumpDown");
        dashInput = _playerActionsMap.FindAction("Dash");
        test = _playerActionsMap.FindAction("Test");
        test.performed += Testing;
        Debug.Log("Awake");
    }

    private void Testing(InputAction.CallbackContext context)
    {
        Debug.Log("Test Input");
        Debug.Log(context);
        Debug.Log(context.control);
        Debug.Log(context.duration);
        Debug.Log(context.phase);
        Debug.Log(context.canceled);
        Debug.Log(context.started);
        Debug.Log(context.startTime);
    }

    public void ToggleInputs(bool toggle)
    {
        isInputActive = toggle;
        //Debug.Log($"{(toggle ? "A" : "Dea")}ctivating the inputs");

        if(toggle)
        {
            jumpInputUp.performed += HandleOnJumpUp;
            jumpInputDown.performed += HandleOnJumpDown;
            dashInput.performed += HandleOnDash;
        }
        else
        {
            jumpInputUp.performed -= HandleOnJumpUp;
            jumpInputDown.performed -= HandleOnJumpDown;
            dashInput.performed -= HandleOnDash;
        }
    }

    public Vector2 GetMovement()
    {
        if(!isInputActive)
        {
            return Vector2.zero;
        }
        return moveInput.ReadValue<Vector2>().normalized;
    }

    public bool GetJumpButton()
    {
        if (!isInputActive)
        {
            return false;
        }
        return jumpInput.IsPressed();
    }

    private void OnEnable()
    {
        _playerActionsMap.Enable();
    }

    private void OnDisable()
    {
        _playerActionsMap.Disable();
    }

}
