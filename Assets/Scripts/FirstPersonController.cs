using System;
using System.Collections;
using UnityEngine;
public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    bool _isSprinting => _canSprint && Input.GetKey(sprintKey);
    bool _isGrounded => characterController.isGrounded;
    bool _shouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded && !_isCrouching;
    bool _shouldCrouch => Input.GetKeyDown(crouchKey) && !_duringCrouchAnimation && characterController.isGrounded;

    [SerializeField] Camera playerCamera;
    [SerializeField] CharacterController characterController;

    [Header("Fuctional Options")]
    [SerializeField] bool _canSprint = true;
    [SerializeField] bool _canJump = true;
    [SerializeField] bool _canCrouch = true;
    [SerializeField] bool _canUseHeadBob = true;
    [SerializeField] bool _canZoom = true;
    [SerializeField] bool _canInteract = true;

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
    [SerializeField] float _standingHeight = 2.0f;
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

    float _rotationX = 0;

    void Awake()
    {
        _defaultYPos = playerCamera.transform.localPosition.y;
        _defaultFOV = playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
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

        HandleMouseLook();

        if (_canInteract)
        {
            HandleInteractionCheck();
            HandleInteractionInput();
        }
    }
    private void OnEnable()
    {
        SetSpotlight.OnChangeControl += ChangeMovement;
    }
    private void OnDisable()
    {
        SetSpotlight.OnChangeControl -= ChangeMovement;
    }

    private void ChangeMovement()
    {
        if (CanMove) CanMove = false;
        else CanMove = true;
    }

    private void HandleMovementInput()
    {
        _currentInput = new Vector2((_isCrouching ? _crouchSpeed : _isSprinting ? _sprintSpeed : _walkSpeed) * Input.GetAxis("Vertical"), (_isCrouching ? _crouchSpeed : _isSprinting ? _sprintSpeed : _walkSpeed) * Input.GetAxis("Horizontal"));

        float _moveDirectionY = _moveDirection.y;
        _moveDirection = (transform.TransformDirection(Vector3.forward) * _currentInput.x) + (transform.TransformDirection(Vector3.right) * _currentInput.y);
        _moveDirection.y = _moveDirectionY;
    }

    private void HandleMouseLook()
    {
        _rotationX -= Input.GetAxis("Mouse Y") * _lookSpeedY;
        _rotationX = Mathf.Clamp(_rotationX, -_upperLookLimit, _lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(_rotationX, 0, 0);
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
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                _defaultYPos + Mathf.Sin(_timer) * (_isCrouching ? _crouchBobAmount : _isSprinting ? _sprintBobAmount : _walkBobAmount),
                playerCamera.transform.localPosition.z);
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
        if(Physics.Raycast(playerCamera.ViewportPointToRay(_interactionRayPoint), out RaycastHit hit, _interactionDistance))
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
        if (Input.GetKeyDown(interactKey) && _currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(_interactionRayPoint), out RaycastHit hit, _interactionDistance, _interactionLayer))
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

        //if (characterController.velocity.y < -1 && characterController.isGrounded)
        //    _moveDirection.y = 0;

        characterController.Move(_moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if (_isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        _duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = _isCrouching ? _standingHeight : _crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = _isCrouching ? _standingCenter : _crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < _timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / _timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / _timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        _isCrouching = !_isCrouching;

        _duringCrouchAnimation = false;
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? _zoomFOV : _defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while(timeElapsed < _timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / _timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        _zoomRoutine = null;
    }
}
