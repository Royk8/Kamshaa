using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [Header("Reeferences")]
    public Transform groundCheck;
    public LayerMask groundLayer;

    [Header("Movement")]
    public float moveSpeed;
    public float maxSpeedTime;
    public float inputThreshold;
    private float acceleration;
    private Vector2 moveVelocity;

    [Header("Jump")]
    public float jumpForce;
    [Range(0f, 1f)]
    public float minJumpForceMultiplier;
    public float maxJumpTime;
    public float jumpCooldown;
    public int extraJumps;
    private int extraJumpsLeft;
    private bool isJumping;
    private float jumpStarted;

    [Header("Dash")]
    [Range(1f, 10f)]
    public float DashSpeedMultiplier;
    public float DashTime;

    [Header("Others")]
    [Range(0f, 10f)]
    public float gravityMultiplier;
    public float groundCheckRadius = 0.2f;
    public bool isGrounded;
    private float gravity = -9.8f;
    public InputAdapter inputAdapter;

    private void Start()
    {
        inputAdapter = GetComponent<InputAdapter>();
        acceleration = moveSpeed / maxSpeedTime;
        inputAdapter.OnJumpDown += StartJump;
        inputAdapter.OnJumpUp += StopJump;
        inputAdapter.OnDash += Dash;
        inputAdapter.ToggleInputs(true);
    }

    private void Update()
    {
        CheckGround();
        Move();
        Jumping();
        HandleGravity();
        MoveExecuter();
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
    }

    private void HandleGravity()
    {
        if (!isGrounded)
        {
            zVelocity += gravity * gravityMultiplier * Time.deltaTime;
        }
        else if (zVelocity < 0)
        {
            zVelocity = 0f;
        }
    }

    private float zVelocity = 0;
    float timeFlyingLeft;
    private bool isDashing;



    private void StartJump(InputAction.CallbackContext context)
    {
        if (isGrounded || extraJumpsLeft > 0)
        {
            if (Time.time > (jumpStarted + jumpCooldown))
            {
                isJumping = true;
                jumpStarted = Time.time;
                zVelocity = jumpForce * minJumpForceMultiplier;
                if (!isGrounded) extraJumpsLeft--;
            }
        }
    }

    private void Jumping()
    {
        timeFlyingLeft = (jumpStarted + maxJumpTime) - Time.time;

        if (isJumping && (timeFlyingLeft > 0) && inputAdapter.GetJumpButton())
        {
            zVelocity = Math.Clamp(zVelocity + jumpForce*Time.deltaTime, float.MinValue, jumpForce);
            //zVelocity = jumpForce;
        }

        if (!isJumping && isGrounded)
        {
            extraJumpsLeft = extraJumps;
        }
    }

    private void StopJump(InputAction.CallbackContext context)
    {
        isJumping = false;
        timeFlyingLeft = 0;
    }

    private void Move()
    {
        Vector2 moveInput = inputAdapter.GetMovement();

        if (moveInput != Vector2.zero)
        {
            Vector2 velocity = moveInput * acceleration * Time.deltaTime + moveVelocity;
            moveVelocity.x = Mathf.Clamp(velocity.x,
                -moveSpeed,
                moveSpeed);
            moveVelocity.y = Mathf.Clamp(velocity.y,
                -moveSpeed,
                moveSpeed);
            //Vector2.ClampMagnitude(moveVelocity, moveSpeed);

            if (moveInput.x < inputThreshold && moveInput.x > -inputThreshold)
            {
                moveVelocity.x = 0;
            }
            if (moveInput.y < inputThreshold && moveInput.y > -inputThreshold)
            {
                moveVelocity.y = 0;
            }
        }
        else
        {
            if(inputAdapter.isInputActive)
                moveVelocity = Vector2.zero;
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        Vector2 moveInput = inputAdapter.GetMovement();
        if (moveInput != Vector2.zero)
        {
            StartCoroutine(DashExecuter(moveInput));
        }
    }

    public void MoveExecuter()
    {
        transform.Translate(
            new Vector3(moveVelocity.x, zVelocity, moveVelocity.y) 
            * Time.deltaTime);
    }

    public IEnumerator DashExecuter(Vector2 direction)
    {
        if (!isDashing)
        {
            isDashing = true;
            inputAdapter.ToggleInputs(false);
            float startTime = Time.time;
            Vector2 dashVelocity = direction * moveSpeed * DashSpeedMultiplier * Time.deltaTime;

            while (Time.time < (startTime + DashTime))
            {
                moveVelocity = dashVelocity;
                transform.Translate(moveVelocity.x, 0, moveVelocity.y);
                yield return null;
            }
            isDashing = false;
            inputAdapter.ToggleInputs(true);
        }
    }

}
