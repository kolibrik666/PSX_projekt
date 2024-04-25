using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Runtime.CompilerServices;
using UnityEngine;
using Zenject;

public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    public bool IsAlive => _isAlive;
    public bool IsSprinting => _isSprinting;
    public bool CanSprint => _canSprint;
    bool _isSprinting => _canSprint && Input.GetKey(_sprintKey);
    bool _isMovingBackwards => Input.GetAxis("Vertical") < 0f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0f && Input.GetAxis("Vertical") < 0f || Mathf.Abs(Input.GetAxis("Horizontal")) > 0f && Input.GetAxis("Vertical") == 0f;
    bool _isMoving => CanMove && (Mathf.Abs(Input.GetAxis("Horizontal")) > 0f || Mathf.Abs(Input.GetAxis("Vertical")) > 0f);
    bool _isGrounded => _characterController.isGrounded;
    bool _shouldJump => Input.GetKeyDown(_jumpKey) && _characterController.isGrounded && !_isCrouching;
    bool _shouldCrouch => Input.GetKeyDown(_crouchKey) && !_duringCrouchAnimation && _characterController.isGrounded;

    [SerializeField] Camera _playerCamera;
    [SerializeField] CharacterController _characterController;
    [SerializeField] PlayerDeathAnimation _playerDeathAnimation;

    [SerializeField] Crosshair _crosshair;

    [Header("Controls")]
    [SerializeField] KeyCode _sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode _jumpKey = KeyCode.Space;
    [SerializeField] KeyCode _crouchKey = KeyCode.LeftControl;
    [SerializeField] KeyCode _interactKey = KeyCode.E;

    [Header("Movement Parameters")]
    [SerializeField] float _walkSpeed = 3.0f;
    [SerializeField] float _sprintSpeed = 6.0f;
    [SerializeField] float _crouchSpeed = 1.5f;
    float _backwardSpeedMultiplier = 0.65f;

    //[SerializeField] float slopeSpeed = 7f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] float _lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] float _lookSpeedY = 2.0f;
    [SerializeField, Range(1, 100)] float _upperLookLimit = 80.0f; //stupne
    [SerializeField, Range(1, 100)] float _lowerLookLimit = 80.0f;

    [Header("Flashlight Parameters")]
    [SerializeField] GameObject _light;
    [SerializeField] GameObject _flashlight;
    [SerializeField] GameObject _head;
    float _flashlightRotationSpeed = 8f;

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

    [Header("Headbob Parameters")]
    [SerializeField] float _walkBobSpeed = 10f;
    [SerializeField] float _walkBobAmount = 0.03f;
    [SerializeField] float _sprintBobSpeed = 14f;
    [SerializeField] float _sprintBobAmount = 0.07f;
    [SerializeField] float _crouchBobSpeed = 8f;
    [SerializeField] float _crouchBobAmount = 0.025f;
    float _defaultYPos = 0;
    float _timer;

    [Header("Interaction")]
    [SerializeField] Vector3 _interactionRayPoint = default;
    [SerializeField] float _interactionDistance = default;
    [SerializeField] LayerMask _interactionLayer = default;
    [SerializeField] LayerMask _interactionHoldingLayer = default;

    float _holdInterval = 80f;
    float _holdTimer;

    [Inject] GameSetupData _gameSetupData;
    [Inject] CommonSounds _commonSounds;
    [Inject] AudioManager _audioManager;
    float _footstepInterval = 0.6f;
    float _footstepSprintInterval = 0.35f;
    float _footstepTimer;

    Interactable _currentInteractable;

    Vector3 _moveDirection;
    Vector2 _currentInput;

    bool _canSprint = true;
    bool _canJump = true;
    bool _canCrouch = true;
    bool _canUseHeadBob = true;
    bool _canInteract = true;
    bool _canLook = true;

    bool _isAlive = true;
    float _rotationX = 0;

    public static event Action<bool> IsCrouching;

    void Awake()
    {
        _defaultYPos = _playerCamera.transform.localPosition.y;
        LockCursor();
    }
    private void OnEnable()
    {
        MainMenuSettings.SettingsChanged += UpdateSettings;
        SurvivalManager.OnPlayerDeath += Death;
        SetSpotlight.OnChangeControl += ChangeMovement;
        HUD.OnChangeControl += ChangeMouseLook;
        SurvivalManager.CanSprint += ChangeSprint;
        UpdateSettings();
    }
    private void OnDisable()
    {
        MainMenuSettings.SettingsChanged -= UpdateSettings;
        SurvivalManager.OnPlayerDeath -= Death;
        SetSpotlight.OnChangeControl -= ChangeMovement;
        HUD.OnChangeControl -= ChangeMouseLook;
        SurvivalManager.CanSprint -= ChangeSprint;
    }
 
    void Update()
    {
        if (!_isAlive) return;
        if (CanMove)
        {
            HandleMovementInput();

            if (_canJump && _shouldJump) HandleJump();
            if (_canCrouch && _shouldCrouch) StartCoroutine(CrouchStand());
            if (_canUseHeadBob) HandleHeadBob();
            if (_isMoving) HandleFootsteps();

            ApplyFinalMovement();
        }

        if (_canLook) HandleMouseLook();

        if (_canInteract)
        {
            HandleInteractionCheck();
            HandleInteractionInput();
        }
    }
    private void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void UpdateSettings()
    {
        _canUseHeadBob = _gameSetupData.Headbob;
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
        else
        {
            LockCursor();
            CanMove = true;
        }
    }
    private void ChangeMouseLook()
    {
        if (_canLook) _canLook = false;
        else _canLook = true;
    }
    private void ChangeSprint(bool b)
    {
        _canSprint = b;
    }
    private void HandleFootsteps()
    {
        if (!_isGrounded) return;
        _footstepTimer += Time.unscaledDeltaTime;
        if (_footstepTimer >= (_isSprinting && !_isCrouching ? _footstepSprintInterval : _footstepInterval))
        {
            _audioManager.PlayOneShot(_commonSounds.Footsteps);
            _footstepTimer = 0;
        }
    }
    private void HandleMovementInput()
    {
        _currentInput = new Vector2((_isCrouching ? _crouchSpeed : _isSprinting ? _sprintSpeed : _walkSpeed) * Input.GetAxis("Vertical"),
            (_isCrouching ? _crouchSpeed : _isSprinting ? _sprintSpeed : _walkSpeed) * Input.GetAxis("Horizontal"));
        float _moveDirectionY = _moveDirection.y;

        if (_isMovingBackwards && _isSprinting) _currentInput *= _backwardSpeedMultiplier;

        _moveDirection = (transform.TransformDirection(Vector3.forward) * _currentInput.x) + 
                           (transform.TransformDirection(Vector3.right) * _currentInput.y);
        _moveDirection.y = _moveDirectionY;
    }

    private void HandleMouseLook()
    {
        _rotationX -= Input.GetAxis("Mouse Y") * _lookSpeedY;
        _rotationX = Mathf.Clamp(_rotationX, -_upperLookLimit, _lowerLookLimit);
        _playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * _lookSpeedX, 0);

        _flashlight.transform.position = _head.transform.position;
        Quaternion targetRotation = Quaternion.Euler(_rotationX, transform.eulerAngles.y, 0);
        _flashlight.transform.rotation = Quaternion.Lerp(_flashlight.transform.rotation, targetRotation, _flashlightRotationSpeed * Time.deltaTime);
    }

    private void HandleJump()
    {
        _moveDirection.y = _jumpForce;
        _audioManager.PlayOneShot(_commonSounds.Footsteps);
    }

    private void HandleHeadBob()
    {
        if (!_isGrounded) return;

        if(Mathf.Abs(_moveDirection.x) > 0.1f || Mathf.Abs(_moveDirection.z) > 0.1f)
        {
            _timer += Time.deltaTime * (_isCrouching ? _crouchBobSpeed : _isSprinting && !_isMovingBackwards ? _sprintBobSpeed : _walkBobSpeed);
            _playerCamera.transform.localPosition = new Vector3(
                _playerCamera.transform.localPosition.x,
                _defaultYPos + Mathf.Sin(_timer) * (_isCrouching ? _crouchBobAmount : _isSprinting && !_isMovingBackwards ? _sprintBobAmount : _walkBobAmount),
                _playerCamera.transform.localPosition.z);
        }
    }

    private void HandleInteractionCheck()
    {
        if (Physics.Raycast(_playerCamera.ViewportPointToRay(_interactionRayPoint), out RaycastHit hit, _interactionDistance))
        {
            if ((hit.collider.gameObject.layer == 9 || hit.collider.gameObject.layer == 10) && (_currentInteractable == null || hit.collider.gameObject.GetInstanceID() != _currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out _currentInteractable);

                if (_currentInteractable)
                {
                    _currentInteractable.OnFocus();
                    _crosshair.CanInteract();
                }                         // volame sa na HUD kde menime obrazok ked je on focus
            }
            else _crosshair.ResetDot();
        }
        else ResetInteraction();      
    }
    private void ResetInteraction()
    {
        if (_currentInteractable)
        {
            _currentInteractable.OnLoseFocus();
            _currentInteractable = null;
        }
        _crosshair.ResetDot();
    }

    private void HandleInteractionInput()
    {
        async void HandleHoldInput()
        {
            if (Input.GetKey(_interactKey))
            {
                await UniTask.Delay(TimeSpan.FromMilliseconds(300));
                if (_currentInteractable == null) return;
                _holdTimer += Time.timeScale;
                _crosshair.Holding(_holdTimer);
                if (_holdTimer >= _holdInterval)
                {
                    _currentInteractable.OnInteract();
                    _holdTimer = 0;
                    _crosshair.Holding(_holdTimer);
                }
            }
            else
            {
                _holdTimer = 0;
                _crosshair.ResetSlider();
            }  
        }

        if (_holdTimer == 0 && Input.GetKeyUp(_interactKey) && _currentInteractable != null && (Physics.Raycast(_playerCamera.ViewportPointToRay(_interactionRayPoint), out RaycastHit hit, _interactionDistance, _interactionLayer)
            || Physics.Raycast(_playerCamera.ViewportPointToRay(_interactionRayPoint), out _, _interactionDistance, _interactionHoldingLayer)))
        {
            _currentInteractable.OnInteract();
            _crosshair.Interacted();
        }
        else if (Physics.Raycast(_playerCamera.ViewportPointToRay(_interactionRayPoint), out _, _interactionDistance, _interactionHoldingLayer)) HandleHoldInput();
    }
    

    private void ApplyFinalMovement()
    {
        if (!_isGrounded) _moveDirection.y -= _gravity * Time.deltaTime;
        _characterController.Move(_moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if (_isCrouching && Physics.Raycast(_playerCamera.transform.position, Vector3.up, 1.5f)) yield break;
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

}
