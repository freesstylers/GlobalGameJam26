using UnityEngine;
using System.Collections.Generic;

public class PlayerMovement : MonoBehaviour
{
    public Mask currentMask_;

    public Camera playerCamera;
    public GameObject cameraContainer;

    public bool playerCanInteract = true;
    [Header("MOUSE LOOK")]
    public float mouseSensitivity = 2f;
    public float maxLookAngle = 90f;

    [Header("STEPS")]
    public float stepHeightDelta = 0.25f;
    public float stepCadence = 5f;

    [Header("LEANING")]
    public float cameraLeanAccumulation = 0.15f;
    public float cameraLeanMax = 15f;
    public float cameraLeanRecoverySpeed = 5f;

    [Header("SPRINTING")]
    public float sprintSpeed = 25f;
    public float sprintFOVReduction = 5f;
    public float fovTransitionSpeed = 5f;

    [Header("DASH")]
    public float dashSpeed = 25f;
    public float dashDuration = 0.3f;
    public float dashCooldown = 0.5f;

    [Header("SHADER")]
    public Material screenMaterial;

    [Header("AUDIO")]
    private FMOD.Studio.EventInstance dashInstance_;
    private FMOD.Studio.EventInstance stepsInstance_;


    ////////////////////////////////////////////

    public Vector3 _moveDirection;
    
    private Vector3 _cameraBasePosition;
    private float _baseFOV;
    
    private float _stepCycle;

    public Vector3 _currentLean;

    private bool _isSprinting = false;
    private bool _isMoving = false;

    private float _dashTimer;
    private float _dashCooldownTimer;
    private bool _isDashing = false;

    private void Awake()
    {
        FlowManager.instance.currentPlayer = this;
    }

    void Start()
    {
        if (playerCamera != null)
        {
            _cameraBasePosition = playerCamera.transform.localPosition;
            _baseFOV = playerCamera.fieldOfView;
        }
        //FlowManager.instance.SuscribeMaskChange(OnMaskChange);
        //currentMask_ = FlowManager.instance.GetCurrentMask();
        Cursor.lockState = CursorLockMode.Locked; //pilla el foco

        dashInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/Dash");
        stepsInstance_ = FMODUnity.RuntimeManager.CreateInstance("event:/PlayerEvents/Steps");
    }

    void Update()
    {
        HandleSprint();
        HandleMovement();
        HandleMouseLook();
        HandleDash();
        HandleCameraLean();
        HandleCameraFOV();
        HandleSteps();
    }

    public void SetPlayerCanInteract(bool newValue)
    {
        playerCanInteract = newValue;
    }

    bool playerLook = true;

    public void SetPlayerLook(bool state)
    {
        playerLook = state;
    }

    private void HandleMouseLook()
    {
        if (!playerCamera || !playerLook)
            return;

        float mouseX = 0;
        float mouseY = 0;
        if(playerCanInteract)
        {
            mouseX = Rewired.ReInput.players.GetPlayer(0).GetAxis("xCamera");
            mouseY = Rewired.ReInput.players.GetPlayer(0).GetAxis("yCamera");
        }
        //Saca el input de rotacion
        //if (mouseX <= 0.02f)
        //    mouseX = 0;

        //if (mouseY <= 0.02f)
        //    mouseY = 0;

        float xRotation = mouseX * mouseSensitivity;
        float yRotation = mouseY * mouseSensitivity;

        float currentXRotation = playerCamera.transform.localEulerAngles.x;
        if (currentXRotation > 180f)
            currentXRotation -= 360f;

        //Rota el player entero en X y solo la camara en Y
        transform.Rotate(new Vector3(0, xRotation, 0));
        float newXRotation = currentXRotation - yRotation;
        newXRotation = Mathf.Clamp(newXRotation, -maxLookAngle, maxLookAngle);
        playerCamera.transform.localRotation = Quaternion.Euler(newXRotation, 0f, 0f);
    }

    private void HandleMovement()
    {
        float horizontal = 0;
        float vertical = 0;
        if(playerCanInteract)
        {
            horizontal = Rewired.ReInput.players.GetPlayer(0).GetAxis("xAxis");
            vertical = Rewired.ReInput.players.GetPlayer(0).GetAxis("yAxis");
        }

        Vector3 moveInput = ((transform.forward * vertical) + (transform.right * horizontal)).normalized;
        _moveDirection = moveInput;
        float movementSpeed = _isDashing ? dashSpeed : (_isSprinting ? sprintSpeed : currentMask_.stats_.realSpeed_);
        transform.position += (_moveDirection * movementSpeed * Time.deltaTime);

        if(_moveDirection != Vector3.zero && !_isMoving)
        {
            _isMoving = true;
            stepsInstance_.start();
        }
        if(_moveDirection == Vector3.zero && _isMoving)
        {
            _isMoving = false;
            stepsInstance_.stop(FMOD.Studio.STOP_MODE.IMMEDIATE);
        }
    }

    private void HandleSprint()
    {
        bool sprintInput = false;
        if(playerCanInteract)
            sprintInput = Rewired.ReInput.players.GetPlayer(0).GetButton("sprint");
        if (sprintInput && _moveDirection.sqrMagnitude > 0.01f)
        {
            if (!_isSprinting) stepsInstance_.setParameterByName("Running", 1.0f);
            _isSprinting = true;
        }
        else
        {
            if (_isSprinting) stepsInstance_.setParameterByName("Running", 0.0f);
            _isSprinting = false;
        }

    }

    private void HandleDash()
    {
        bool dashInput = false;
        if(playerCanInteract)
            dashInput = Rewired.ReInput.players.GetPlayer(0).GetButtonDown("Dash");

        if (dashInput && !_isDashing && _dashCooldownTimer <= 0f && _moveDirection.sqrMagnitude > 0.01f)
        {
            _isDashing = true;
            _dashTimer = dashDuration;
            dashInstance_.start();
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
        float horizontal = 0;
        float vertical = 0;
        if(!playerCanInteract)
        {
            //Saca el lado al que inclinarse segun input
            horizontal = Rewired.ReInput.players.GetPlayer(0).GetAxis("xAxis");
            vertical = Rewired.ReInput.players.GetPlayer(0).GetAxis("yAxis");
        }
        
        Vector3 targetLean = Vector3.zero;
        if (Mathf.Abs(horizontal) > 0.01f)
            targetLean.z -= horizontal * cameraLeanAccumulation;
        if (Mathf.Abs(vertical) > 0.01f)
            targetLean.x += vertical * cameraLeanAccumulation;
        //Inclinate poco a poco
        _currentLean = Vector3.Lerp(_currentLean, targetLean, cameraLeanRecoverySpeed * Time.deltaTime);
        _currentLean.x = Mathf.Clamp(_currentLean.x, -cameraLeanMax, cameraLeanMax);
        _currentLean.z = Mathf.Clamp(_currentLean.z, -cameraLeanMax, cameraLeanMax);
        //Inclina el contenedor de la camara NO la camara directamente
        if (cameraContainer != null)
            cameraContainer.transform.localRotation = Quaternion.Euler(_currentLean.x, 0f, _currentLean.z);
    }

    private void HandleCameraFOV()
    {
        if (!playerCamera)
            return;

        float targetFOV = _isSprinting ? _baseFOV - sprintFOVReduction : _baseFOV;
        screenMaterial.SetFloat("_mask1VisualObstructionStremgth", 1 - ((Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime) - (_baseFOV - sprintFOVReduction)) / sprintFOVReduction));
        playerCamera.fieldOfView = Mathf.Lerp(playerCamera.fieldOfView, targetFOV, fovTransitionSpeed * Time.deltaTime);
    }

    private void HandleSteps()
    {
        //Si te estas moviento saca la altura que te toca segun tu velocidad 
        bool isMoving = _moveDirection.sqrMagnitude > 0.01f && !_isDashing;
        if (isMoving)
        {
            float speedRatio = _isSprinting ? (sprintSpeed / currentMask_.stats_.realSpeed_) : 1f;
            float adjustedCadence = stepCadence * speedRatio;
            _stepCycle += Time.deltaTime * adjustedCadence;
        }
        //Si esta quieto poco a poco ve a la posicion inicial / cero altura de step
        else
            _stepCycle = Mathf.Lerp(_stepCycle, 0f, stepCadence * Time.deltaTime);

        float bobOffset = isMoving ? (Mathf.Sin(_stepCycle * Mathf.PI) * stepHeightDelta) : 0;
        if (playerCamera)
        {
            Vector3 newCameraPosition = _cameraBasePosition;
            newCameraPosition.y += bobOffset;
            playerCamera.transform.localPosition = newCameraPosition;
        }
    }

    private void ChangeMask()
    {
        FlowManager.instance.NextMask();
        currentMask_ = FlowManager.instance.GetCurrentMask();
    }

    private void OnMaskChange(Mask newMask)
    {
        currentMask_ = newMask;
    }
}