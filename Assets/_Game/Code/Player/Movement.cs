using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour, IStuneable
{
    [Header("Reeferences")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public Transform playerModel;
    public Transform headPosition;
    private AnimationsControl animationsControl;
    public InputAdapter inputAdapter;
    public Transform floorDetectorPlane;

    [Header("Movement")]
    public float moveSpeed;
    public float timeToMaxSpeed;
    public float inputThresholdToStartMoving;
    private float acceleration;
    private Vector2 moveVelocity;
    private bool isStunned;
    private IEnumerator stunCoroutine;

    [Header("Jump")]
    public float jumpForce;
    public float maxJumpTime;
    public float jumpCooldown;
    public int extraJumps;
    private int extraJumpsLeft;
    private bool isJumping;
    private float jumpStarted;

    [Header("Dash")]
    public float dashDistance;
    public float dashTime;
    public float dashCooldown;
    private float dashStarted;

    [Header("Others")]
    [Range(0f, 10f)]
    public float gravityMultiplier;
    public float groundCheckRadius = 0.2f;
    private bool isGrounded;
    private float gravity = -9.8f;
    private Vector3 floorPlaneNormal;
    private Vector3 moveVelocity3;
    private IEnumerator jumpCoroutine;
    private Vector3 lookDirection;

    private void Start()
    {
        inputAdapter = GetComponent<InputAdapter>();
        acceleration = moveSpeed / timeToMaxSpeed;
        inputAdapter.OnJumpDown += StartJumpSimple;
        inputAdapter.OnJumpUp += StopJump;
        inputAdapter.OnDash += Dash;
        inputAdapter.OnShoot += Shoot;
        inputAdapter.ToggleInputs(true);
        animationsControl = GetComponent<AnimationsControl>();
    }


    private void Update()
    {
        CheckGround();
        UpdateLookDirection();
        RotateToLookDirection();
        Move();
        Jumping();
        HandleGravity();
        MoveExecuter();
        DetectSurface();
        FloatOnTheFloor();
    }
    [Header("Walking direction")]
    public Vector3 rightRotation = new Vector3(-40, -60, -45);
    public Vector3 leftRotation = new Vector3(-40, 60, 45);
    public Vector3 upRotation = new Vector3(-60, -180, 0);
    public Vector3 downRotation = new Vector3(0, 0, 0);
    public Vector3 upRightRotation = new Vector3(-40, -60, -45);
    public Vector3 uPLeftRotation = new Vector3(-40, 60, 45);
    public Vector3 downRightRotation = new Vector3(-60, -180, 0);
    public Vector3 downLeftRotation = new Vector3(0, 0, 0);

    public Vector3 GetDirection(Vector2 input)
    {
        // Get the angle in degrees (0 to 360)
        float angle = Mathf.Atan2(input.y, input.x) * Mathf.Rad2Deg;

        // Normalize the angle to be in the range of 0 to 360
        if (angle < 0)
            angle += 360;

        // Categorize the angle into 8 directions
        if (angle >= 337.5f || angle < 22.5f)
        {
            return rightRotation;
            //return "Right";
        }
        else if (angle >= 22.5f && angle < 67.5f)
        {
            return upRightRotation;
            //return "Up-Right";
        }
        else if (angle >= 67.5f && angle < 112.5f)
        {
            return upRotation;
            //return "Up";
        }
        else if (angle >= 112.5f && angle < 157.5f)
        {
            return uPLeftRotation;
            //return "Up-Left";
        }
        else if (angle >= 157.5f && angle < 202.5f)
        {
            return leftRotation;
            //return "Left";
        }
        else if (angle >= 202.5f && angle < 247.5f)
        {
            return downLeftRotation;
            //return "Down-Left";
        }
        else if (angle >= 247.5f && angle < 292.5f)
        {
            return downRotation;
            //return "Down";
        }
        else if (angle >= 292.5f && angle < 337.5f)
        {
            return downRightRotation;
            //return "Down-Right";
        }

        return downRotation;
    }

    private void RotateToLookDirection()
    {
        Vector2 moveInput = inputAdapter.GetMovement();

        Vector3 targetRotation = GetDirection(moveInput);

        // Smoothly rotate the character towards the target rotation if there's movement input
        if (moveInput.x != 0 || moveInput.y != 0)
        {
            Quaternion targetQuaternion = Quaternion.Euler(targetRotation);
            playerModel.rotation = Quaternion.Slerp(playerModel.rotation, targetQuaternion, 90f * Time.deltaTime);
        }
    }

    private void CheckGround()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundCheckRadius, groundLayer);
        animationsControl.CambiarEnPiso(isGrounded);
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

    private void StartJumpSimple(InputAction.CallbackContext context)
    {
        if (isGrounded || extraJumpsLeft > 0)
        {
            if (Time.time > (jumpStarted + jumpCooldown))
            {
                jumpStarted = Time.time;
                if (!isGrounded)
                {
                    extraJumpsLeft--;
                    animationsControl?.DobleSalto();
                    isJumping = false;
                    StopCoroutine(jumpCoroutine);
                }
                jumpCoroutine = ExecuteJump(inputAdapter.GetMovement());
                StartCoroutine(jumpCoroutine);
            }
        }
    }

    private IEnumerator ExecuteJump(Vector2 moveInput)
    {
        if (!isJumping)
        {
            isJumping = true;
            zVelocity = jumpForce;
            Vector3 moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
            while ((jumpStarted + maxJumpTime) > Time.time)
            {
                zVelocity = Math.Clamp(zVelocity + jumpForce * Time.deltaTime, float.MinValue, jumpForce);
                Vector3 moveForce = moveDirection * acceleration * moveSpeed * Time.deltaTime;
                transform.Translate((moveForce + Vector3.up * zVelocity) * Time.deltaTime);
                yield return null;
            }
            while (!isGrounded)
            {
                Vector3 moveForce = moveDirection * acceleration * moveSpeed * Time.deltaTime;
                transform.Translate(moveForce * Time.deltaTime);
                yield return null;
            }
        }
    }

    private void Jumping()
    {
        timeFlyingLeft = (jumpStarted + maxJumpTime) - Time.time;

        if (isJumping && (timeFlyingLeft > 0) && inputAdapter.GetJumpButton())
        {
            zVelocity = Math.Clamp(zVelocity + jumpForce * Time.deltaTime, float.MinValue, jumpForce);
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

    private Vector3 VerifyPlaneOfMovement(Vector2 direction)
    {
        Vector3 correctedDirection = new Vector3(direction.x, 0, direction.y);
        if (!IsPlaneParallelToXZ(correctedDirection))
        {
            correctedDirection = correctedDirection -
                Vector3.Dot(correctedDirection, floorDetectorPlane.up) * floorDetectorPlane.up;
        }
        return correctedDirection;
    }

    private void Move()
    {
        Vector2 moveInput = inputAdapter.GetMovement();

        if (moveInput != Vector2.zero && isGrounded)
        {
            moveVelocity3 = VerifyPlaneOfMovement(moveInput) *
                acceleration * moveSpeed * Time.deltaTime;
            if (moveVelocity3.magnitude > 20f)
            {
                //Debugs the moveVelocity, move input, acceleration, delta time
                Debug.Log("Move Velocity: " + moveVelocity3);
                Debug.Log("Move Input: " + moveInput);
                Debug.Log("Acceleration: " + acceleration);
                Debug.Log("Delta Time: " + Time.deltaTime);

            }
        }
        else
        {
            if (inputAdapter.isInputActive)
                moveVelocity3 = Vector3.zero;
        }
    }

    private void Dash(InputAction.CallbackContext context)
    {
        Vector2 moveInput = inputAdapter.GetMovement();
        if (isGrounded)
            StartCoroutine(DashExecuter());
    }

    public void MoveExecuter()
    {
        if(isStunned)
        {
            return;
        }
        Vector3 direction = new Vector3(moveVelocity3.x, zVelocity + moveVelocity3.y, moveVelocity3.z);
        transform.Translate(direction * Time.deltaTime);
    }

    public IEnumerator DashExecuter()
    {
        if (!isDashing && Time.time > (dashStarted + dashCooldown))
        {
            animationsControl.Dash();
            isDashing = true;
            dashStarted = Time.time;
            inputAdapter.ToggleInputs(false);
            Vector3 dashFinalPosition = transform.position + lookDirection * dashDistance;
            float startTime = Time.time;

            while (Time.time < (startTime + dashTime))
            {
                Vector3 thisFramePosition = Vector3.Lerp(transform.position, dashFinalPosition, Time.deltaTime / dashTime);
                Vector3 thisFrameDirection = thisFramePosition - transform.position;
                thisFrameDirection = VerifyPlaneOfMovement(new Vector2(thisFrameDirection.x, thisFrameDirection.z));
                transform.Translate(thisFrameDirection);
                yield return null;
            }
            isDashing = false;
            inputAdapter.ToggleInputs(true);
        }
    }

    public void Shoot(InputAction.CallbackContext context)
    {
        animationsControl.Disparar();
    }


    public void DetectSurface()
    {
        Ray ray = new Ray(groundCheck.position, Vector3.down);
        if (Physics.Raycast(ray, out RaycastHit hitInfo, 100, groundLayer))
        {
            floorDetectorPlane.position = hitInfo.point + Vector3.up * 0.01f;
            floorDetectorPlane.rotation = Quaternion.FromToRotation(Vector3.up, hitInfo.normal);
            floorDetectorPlane.localScale = new Vector3(1f, 0.01f, 1f);
        }
        else
        {
            floorDetectorPlane.localScale = new Vector3(0, 0, 0);
        }
    }

    bool IsPlaneParallelToXZ(Vector3 normal)
    {
        Vector3 xzPlaneNormal = Vector3.up;
        normal.Normalize();
        float dotProduct = Vector3.Dot(normal, xzPlaneNormal);
        return Mathf.Approximately(dotProduct, 1f) || Mathf.Approximately(dotProduct, -1f);
    }

    public void FloatOnTheFloor()
    {
        //Verify that the player is not in the middle of the ground, it uses the transform in the head to shoot a raycast down, if it touches the floor means that the player have to go up until it stops touch the floor with the ray, the ray should be 1.2f long
        Ray ray = new Ray(headPosition.position, Vector3.down);
        int counterToAvoidInfiniteLoop = 0;
        while (Physics.Raycast(ray, out RaycastHit hitInfo, 1.2f, groundLayer))
        {
            transform.position = hitInfo.point + Vector3.up * 0.01f;
            counterToAvoidInfiniteLoop++;
            if (counterToAvoidInfiniteLoop > 1000)
            {
                break;
            }
        }
    }

    public void UpdateLookDirection()
    {
        Vector2 inputDirection2D = inputAdapter.GetMovement();
        Vector3 newInputDirection = new Vector3(inputDirection2D.x, 0, inputDirection2D.y).normalized;
        if (newInputDirection != Vector3.zero)
        {
            lookDirection = newInputDirection;
        }
    }

    private Vector3 lastFramePosition = Vector3.zero;

    private void CheckTeleportationError()
    {
        //Check is the player is too far from the place it was in the last frame, if it is, if will debug the coordinates

        if (Vector3.Distance(transform.position, lastFramePosition) > 1f)
        {
            Debug.Log("++++++++++++++++++++++++++++++++++++++++++++++++++++++");
            //Debugs all important information to check the error
            Debug.Log("Last Frame Position: " + lastFramePosition);
            Debug.Log("Current Position: " + transform.position);
            Debug.Log("Time since start: " + Time.timeSinceLevelLoad);
            Debug.Log("User Input: " + inputAdapter.GetMovement());
            Debug.Log("Is Jump Pressed: " + inputAdapter.GetJumpButton());
            StopPlay();

        }
        lastFramePosition = transform.position;
    }

    void StopPlay()
    {
#if UNITY_EDITOR
        EditorApplication.isPaused = true; // Stop Play mode in the editor
#endif
    }

    public void GetStunned(float duration)
    {
        if (isStunned && stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
            isStunned = false;
        }
        stunCoroutine = Stun(duration);
        StartCoroutine(stunCoroutine);
    }

    public IEnumerator Stun(float duration)
    {
        if (!isStunned)
        {
            isStunned = true;
            inputAdapter.ToggleInputs(false);
            yield return new WaitForSeconds(duration);
            isStunned = false;
            inputAdapter.ToggleInputs(true);
        }
    }
}
