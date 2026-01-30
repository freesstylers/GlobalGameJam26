using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 10f;
    public Camera playerCamera;
    public float cameraLeanAccumulation = 0.15f;
    public float cameraLeanMax = 30f;
    public float cameraLeanRecoverySpeed = 5f;

    public float stepBobSpeed = 5f;
    public float stepHeight = 0.1f;
    public float stepBobSmoothing = 5f;

    public Vector3 _currentLean;
    public Vector3 _moveDirection;
    private Vector3 _cameraBasePosition;
    private float _stepCycle;

    void Start()
    {
        if (playerCamera != null)
            _cameraBasePosition = playerCamera.transform.localPosition;
    }

    void Update()
    {
        HandleMovement();
        HandleCameraLean();
        HandleSteps();
        ApplyCameraLean();
    }

    private void HandleMovement()
    {
        float horizontal = Rewired.ReInput.players.GetPlayer(0).GetAxis("xAxis");
        float vertical = Rewired.ReInput.players.GetPlayer(0).GetAxis("yAxis");
        _moveDirection = new Vector3(horizontal, 0f, vertical);
        _moveDirection.Normalize();
        transform.position += _moveDirection * moveSpeed * Time.deltaTime;
    }

    private void HandleCameraLean()
    {
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
            _stepCycle += Time.deltaTime * stepBobSpeed;
        else
            _stepCycle = Mathf.Lerp(_stepCycle, 0f, stepBobSmoothing * Time.deltaTime);

        float bobOffset = Mathf.Sin(_stepCycle * Mathf.PI) * stepHeight;
        if (playerCamera)
        {
            Vector3 newCameraPosition = _cameraBasePosition;
            newCameraPosition.y += bobOffset;
            playerCamera.transform.localPosition = newCameraPosition;
        }
    }

    private void ApplyCameraLean()
    {
        if (!playerCamera)
            return;

        Vector3 cameraEuler = playerCamera.transform.localEulerAngles;
        cameraEuler.x = _currentLean.x;
        cameraEuler.z = _currentLean.z;
        playerCamera.transform.localEulerAngles = cameraEuler;
    }
}