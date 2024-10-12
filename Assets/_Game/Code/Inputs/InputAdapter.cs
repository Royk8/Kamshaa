using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputAdapter : MonoBehaviour
{
    public bool isInputActive { get; private set;  }
    public bool isUIActive { get; private set;  }
    private InputActionMap _playerActionsMap;
    private InputActionMap _UIActionsMap;
    private InputAction moveInput;
    private InputAction jumpInput;
    private InputAction jumpInputUp;
    private InputAction jumpInputDown;
    private InputAction dashInput;
    private InputAction shootInput;
    private InputAction uiNext;
    private InputAction uiClick;
    private InputAction uiSubmit;
    private InputAction test;


    public event Action<InputAction.CallbackContext> OnDash;
    public event Action<InputAction.CallbackContext> OnShoot;
    public event Action<InputAction.CallbackContext> OnJumpUp;
    public event Action<InputAction.CallbackContext> OnJumpDown;
    public event Action<InputAction.CallbackContext> OnNextMessage;

    private void HandleOnDash(InputAction.CallbackContext context)
    {
        if(OnDash != null) OnDash.Invoke(context);
    }    
    
    private void HandleOnShoot(InputAction.CallbackContext context)
    {
        if(OnShoot != null) OnShoot.Invoke(context);
    }

    private void HandleOnJumpUp(InputAction.CallbackContext context)
    {
        if (OnJumpUp != null) OnJumpUp.Invoke(context);
    }

    private void HandleOnJumpDown(InputAction.CallbackContext context)
    {
        if (OnJumpDown != null) OnJumpDown.Invoke(context);
    }

    private void HandleOnNextMessage(InputAction.CallbackContext context)
    {
        if (OnNextMessage != null) OnNextMessage.Invoke(context);
    }


    private void Awake()
    {
        PlayerInput player = GetComponent<PlayerInput>();

        _playerActionsMap = player.actions.FindActionMap("Player");
        _UIActionsMap = player.actions.FindActionMap("UI");
        moveInput = _playerActionsMap.FindAction("Move");
        jumpInput = _playerActionsMap.FindAction("Jump");
        jumpInputUp = _playerActionsMap.FindAction("JumpUp");
        jumpInputDown = _playerActionsMap.FindAction("JumpDown");
        dashInput = _playerActionsMap.FindAction("Dash");
        shootInput = _playerActionsMap.FindAction("Shoot");
        test = _playerActionsMap.FindAction("Test");

        uiClick = _UIActionsMap.FindAction("Click");
        uiNext = _UIActionsMap.FindAction("Next");
        uiSubmit = _UIActionsMap.FindAction("Submit");

        test.performed += Testing;
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

    [ContextMenu("ToggleFast")]
    public void ToggleFast()
    {
        ToggleToUI(!isUIActive);
    }

    public void ToggleToUI(bool toggle)
    {
        if(toggle)
        {
            isUIActive = true;
            _playerActionsMap.Disable();
            _UIActionsMap.Enable();
            ToggleUIInputs(true);
            ToggleInputs(false);
        }
        else
        {
            isUIActive = false;
            _UIActionsMap.Disable();
            _playerActionsMap.Enable();
            ToggleUIInputs(false);
            ToggleInputs(true);
        }
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
            shootInput.performed += HandleOnShoot;
        }
        else
        {
            jumpInputUp.performed -= HandleOnJumpUp;
            jumpInputDown.performed -= HandleOnJumpDown;
            dashInput.performed -= HandleOnDash;
            shootInput.performed -= HandleOnShoot;
        }
    }

    public void ToggleUIInputs(bool toggle)
    {
        if (toggle)
        {
            uiNext.performed += HandleOnNextMessage;
            uiClick.performed += HandleOnNextMessage;
            uiSubmit.performed += HandleOnNextMessage;
        }
        else
        {
            uiNext.performed -= HandleOnNextMessage;
            uiClick.performed -= HandleOnNextMessage;
            uiSubmit.performed -= HandleOnNextMessage;
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
