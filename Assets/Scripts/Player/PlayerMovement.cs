using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public List<Mask> masks_;
    private Mask currentMask_;
    private int currentMaskId_ = 0;

    public Camera playerCamera;
    public float cameraLeanAccumulation = 0.15f;
    public float cameraLeanMax = 30f;
    public float cameraLeanRecoverySpeed = 5f;

    [Header("Dash")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 0.5f;

    [Header("Mouse Look")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 90f;

    public Vector3 _currentLean;
    public Vector3 _moveDirection;

    private Vector3 _cameraBasePosition;
    private float _stepCycle;

    private float _dashTimer;
    private float _dashCooldownTimer;
    private bool _isDashing;

    void Start()
    {
        if (playerCamera != null)
            _cameraBasePosition = playerCamera.transform.localPosition;

        currentMask_ = masks_[currentMaskId_];

        //Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        HandleMovement();
        HandleMouseLook();
        HandleDash();
        //HandleCameraLean();
        HandleSteps();
    }

    private void HandleMouseLook()
    {
        float mouseX = Rewired.ReInput.players.GetPlayer(0).GetAxis("xCamera");
        float mouseY = Rewired.ReInput.players.GetPlayer(0).GetAxis("yCamera");
        float xRotation = mouseX * mouseSensitivity;
        float yRotation = mouseY * mouseSensitivity;

        if (!playerCamera)
            return;
        float currentXRotation = playerCamera.transform.localEulerAngles.x;
        if (currentXRotation > 180f)
            currentXRotation -= 360f;

        float newXRotation = currentXRotation - yRotation;
        newXRotation = Mathf.Clamp(newXRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(newXRotation, 0f, 0f);
        transform.Rotate(new Vector3(0, xRotation, 0));
    }

    private void HandleMovement()
    {
        float horizontal = Rewired.ReInput.players.GetPlayer(0).GetAxis("xAxis");
        float vertical = Rewired.ReInput.players.GetPlayer(0).GetAxis("yAxis");
        Vector3 moveInput = (transform.forward * vertical + transform.right * horizontal).normalized;
        _moveDirection = moveInput;
        float currentSpeed = _isDashing ? dashSpeed : currentMask_.stats_.realSpeed_;
        transform.position += _moveDirection * currentSpeed * Time.deltaTime;
    }

    private void HandleDash()
    {
        bool dashInput = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("Dash");
        if (dashInput && !_isDashing && _dashCooldownTimer <= 0f && _moveDirection.sqrMagnitude > 0.01f)
        {
            _isDashing = true;
            _dashTimer = dashDuration;
        }

        if (_isDashing && _dashTimer <= 0f)
        {
            _isDashing = false;
            _dashCooldownTimer = dashCooldown;
        }

        if (_isDashing)
            _dashTimer -= Time.deltaTime;

        if (_dashCooldownTimer > 0f)
            _dashCooldownTimer -= Time.deltaTime;
    }

    private void HandleCameraLean()
    {
        return;
        float horizontal = Rewired.ReInput.players.GetPlayer(0).GetAxis("xAxis");
        float vertical = Rewired.ReInput.players.GetPlayer(0).GetAxis("yAxis");
        Vector3 targetLean = Vector3.zero;

        if (Mathf.Abs(horizontal) > 0.01f)
            targetLean.z -= horizontal * cameraLeanAccumulation;

        if (Mathf.Abs(vertical) > 0.01f)
            targetLean.x += vertical * cameraLeanAccumulation;

        _currentLean = Vector3.Lerp(_currentLean, targetLean, cameraLeanRecoverySpeed * Time.deltaTime);
        _currentLean.x = Mathf.Clamp(_currentLean.x, -cameraLeanMax, cameraLeanMax);
        _currentLean.z = Mathf.Clamp(_currentLean.z, -cameraLeanMax, cameraLeanMax);
    }

    private void HandleSteps()
    {
        if (_moveDirection.sqrMagnitude > 0.01f)
            _stepCycle += Time.deltaTime * 5f;
        else
            _stepCycle = Mathf.Lerp(_stepCycle, 0f, 5f * Time.deltaTime);

        float bobOffset = Mathf.Sin(_stepCycle * Mathf.PI) * 0.1f;
        if (playerCamera)
        {
            Vector3 newCameraPosition = _cameraBasePosition;
            newCameraPosition.y += bobOffset;
            playerCamera.transform.localPosition = newCameraPosition;
        }
    }

    private void ChangeMask()
    {
        currentMaskId_ += 1;
        if (currentMaskId_ == 3) currentMaskId_ = 0;
        currentMask_ = masks_[currentMaskId_];
    }
}