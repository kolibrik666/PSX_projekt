using DG.Tweening;
using System;
using System.Collections;
using UnityEngine;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    public bool IsAlive => _isAlive;
    public bool IsSprinting => _isSprinting;
    bool _isSprinting => _canSprint && Input.GetKey(sprintKey);
    bool _isGrounded => _characterController.isGrounded;
    bool _shouldJump => Input.GetKeyDown(jumpKey) && _characterController.isGrounded && !_isCrouching;
    bool _shouldCrouch => Input.GetKeyDown(crouchKey) && !_duringCrouchAnimation && _characterController.isGrounded;

    [SerializeField] Camera _playerCamera;
    [SerializeField] CharacterController _characterController;
    [SerializeField] PlayerDeathAnimation _playerDeathAnimation;
    [SerializeField] GameObject _light;

    [Header("Controls")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] KeyCode interactKey = KeyCode.Mouse0;
    [SerializeField] KeyCode zoomKey = KeyCode.Mouse1;

    [Header("Movement Parameters")]
    [SerializeField] float _walkSpeed = 3.0f;
    [SerializeField] float _sprintSpeed = 6.0f;
    [SerializeField] float _crouchSpeed = 1.5f;
    //[SerializeField] float slopeSpeed = 7f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] float _lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] float _lookSpeedY = 2.0f;
    [SerializeField, Range(1, 100)] float _upperLookLimit = 80.0f; //stupne
    [SerializeField, Range(1, 100)] float _lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] float _jumpForce = 8.0f;
    [SerializeField] float _gravity = 30.0f;

    [Header("Crouch Parameters")]
    [SerializeField] float _crouchHeight = 0.5f;
    [SerializeField] float _standingHeight = 1.7f;
    [SerializeField] float _timeToCrouch = 0.25f;
    [SerializeField] Vector3 _crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] Vector3 _standingCenter = new Vector3(0, 0, 0);
    private bool _isCrouching;
    private bool _duringCrouchAnimation;

    [Header("HeadBob Parameters")]
    [SerializeField] float _walkBobSpeed = 10f;
    [SerializeField] float _walkBobAmount = 0.03f;
    [SerializeField] float _sprintBobSpeed = 14f;
    [SerializeField] float _sprintBobAmount = 0.07f;
    [SerializeField] float _crouchBobSpeed = 8f;
    [SerializeField] float _crouchBobAmount = 0.025f;
    float _defaultYPos = 0;
    float _timer;

    [Header("Zoom Parameters")]
    [SerializeField] float _timeToZoom = 0.3f;
    [SerializeField] float _zoomFOV = 30f;
    float _defaultFOV;
    Coroutine _zoomRoutine;

    [Header("Interaction")]
    [SerializeField] Vector3 _interactionRayPoint = default;
    [SerializeField] float _interactionDistance = default;
    [SerializeField] LayerMask _interactionLayer = default;

    Interactable _currentInteractable;

    Vector3 _moveDirection;
    Vector2 _currentInput;

    bool _canSprint = true;
    bool _canJump = true;
    bool _canCrouch = true;
    bool _canUseHeadBob = true;
    bool _canZoom = true;
    bool _canInteract = true;
    bool _canLook = true;

    bool _isAlive = true;
    float _rotationX = 0;

    public static event Action<bool> IsCrouching;

    void Awake()
    {
        _defaultYPos = _playerCamera.transform.localPosition.y;
        _defaultFOV = _playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (!_isAlive) return;
        if (CanMove)
        {
            HandleMovementInput();

            if (_canJump && _shouldJump)
                HandleJump();

            if (_canCrouch && _shouldCrouch)
                StartCoroutine(CrouchStand());

            if (_canUseHeadBob)
                HandleHeadBob();

            if (_canZoom)
                HandleZoom();

            ApplyFinalMovement();
        }

        if (_canLook) HandleMouseLook();

        if (_canInteract)
        {
            HandleInteractionCheck();
            HandleInteractionInput();
        }
    }
    private void OnEnable()
    {
        SurvivalManager.OnPlayerDeath += Death;
        SetSpotlight.OnChangeControl += ChangeMovement;
    }
    private void OnDisable()
    {
        SurvivalManager.OnPlayerDeath -= Death;
        SetSpotlight.OnChangeControl -= ChangeMovement;
    }
    private void Death()
    {
        _isAlive = false;
        _light.SetActive(false);
        _playerDeathAnimation.Play();
    }
    private void ChangeMovement()
    {
        if (CanMove) CanMove = false;
        else CanMove = true;
    }

    private void HandleMovementInput()
    {
        _currentInput = new Vector2((_isCrouching ? _crouchSpeed : _isSprinting ? _sprintSpeed : _walkSpeed) * Input.GetAxis("Vertical"), 
            (_isCrouching ? _crouchSpeed : _isSprinting ? _sprintSpeed : _walkSpeed) * Input.GetAxis("Horizontal"));
        float _moveDirectionY = _moveDirection.y;
        _moveDirection = (transform.TransformDirection(Vector3.forward) * _currentInput.x) + (transform.TransformDirection(Vector3.right) * _currentInput.y);
        _moveDirection.y = _moveDirectionY;
    }

    private void HandleMouseLook()
    {
        _rotationX -= Input.GetAxis("Mouse Y") * _lookSpeedY;
        _rotationX = Mathf.Clamp(_rotationX, -_upperLookLimit, _lowerLookLimit);
        _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _lookSpeedX, 0);
    }

    private void HandleJump()
    {
        _moveDirection.y = _jumpForce;
    }

    private void HandleHeadBob()
    {
        if (!_isGrounded) return;

        if(Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f)
        {
            _timer += Time.deltaTime * (_isCrouching ? _crouchBobSpeed : _isSprinting ? _sprintBobSpeed : _walkBobSpeed);
            _playerCamera.transform.localPosition = new Vector3(
                _playerCamera.transform.localPosition.x,
                _defaultYPos + Mathf.Sin(_timer) * (_isCrouching ? _crouchBobAmount : _isSprinting ? _sprintBobAmount : _walkBobAmount),
                _playerCamera.transform.localPosition.z);
        }
    }

    private void HandleZoom()
    {
        if(Input.GetKeyDown(zoomKey))
        {
            if(_zoomRoutine != null)
            {
                StopCoroutine(_zoomRoutine);
                _zoomRoutine = null;
            }

            _zoomRoutine = StartCoroutine(ToggleZoom(true));
        }

        if (Input.GetKeyUp(zoomKey))
        {
            if (_zoomRoutine != null)
            {
                StopCoroutine(_zoomRoutine);
                _zoomRoutine = null;
            }

            _zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }

    private void HandleInteractionCheck()
    {
        if(Physics.Raycast(_playerCamera.ViewportPointToRay(_interactionRayPoint), out RaycastHit hit, _interactionDistance))
        {
            if(hit.collider.gameObject.layer == 9 && (_currentInteractable == null || hit.collider.gameObject.GetInstanceID() != _currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out _currentInteractable);

                if (_currentInteractable)
                    _currentInteractable.OnFocus();
            }
        }
        else if (_currentInteractable)
        {
            _currentInteractable.OnLoseFocus();
            _currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && _currentInteractable != null && Physics.Raycast(_playerCamera.ViewportPointToRay(_interactionRayPoint), out RaycastHit hit, _interactionDistance, _interactionLayer))
        {
            _currentInteractable.OnInteract();
        }
    }

    private void ApplyFinalMovement()
    {
        if (!_isGrounded)
        {
            _moveDirection.y -= _gravity * Time.deltaTime;
            //_moveDirection.y -= _gravity;
        }

        //if (_characterController.velocity.y < -1 && _characterController.isGrounded)
        //    _moveDirection.y = 0;

        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if (_isCrouching && Physics.Raycast(_playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        _duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = _isCrouching ? _standingHeight : _crouchHeight;
        float currentHeight = _characterController.height;
        Vector3 targetCenter = _isCrouching ? _standingCenter : _crouchingCenter;
        Vector3 currentCenter = _characterController.center;

        while(timeElapsed < _timeToCrouch)
        {
            _characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / _timeToCrouch);
            _characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / _timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _characterController.height = targetHeight;
        _characterController.center = targetCenter;

        _isCrouching = !_isCrouching;
        IsCrouching?.Invoke(_isCrouching);

        _duringCrouchAnimation = false;
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? _zoomFOV : _defaultFOV;
        float startingFOV = _playerCamera.fieldOfView;
        float timeElapsed = 0;

        while(timeElapsed < _timeToZoom)
        {
            _playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / _timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        _playerCamera.fieldOfView = targetFOV;
        _zoomRoutine = null;
    }
}
