using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class FirstPersonController : MonoBehaviour
{
    public bool CanMove { get; private set; } = true;
    bool isSprinting => canSprint && Input.GetKey(sprintKey);
    bool isGrounded => characterController.isGrounded;
    bool shouldJump => Input.GetKeyDown(jumpKey) && characterController.isGrounded && !isCrouching;
    bool shouldCrouch => Input.GetKeyDown(crouchKey) && !duringCrouchAnimation && characterController.isGrounded;

    [SerializeField] Camera playerCamera;
    [SerializeField] CharacterController characterController;

    [Header("Fuctional Options")]
    [SerializeField] bool canSprint = true;
    [SerializeField] bool canJump = true;
    [SerializeField] bool canCrouch = true;
    [SerializeField] bool canUseHeadBob = true;
    [SerializeField] bool canZoom = true;
    [SerializeField] bool canInteract = true;

    [Header("Controls")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;
    [SerializeField] KeyCode interactKey = KeyCode.Mouse0;
    [SerializeField] KeyCode zoomKey = KeyCode.Mouse1;

    [Header("Movement Parameters")]
    [SerializeField]  float walkSpeed = 3.0f;
    [SerializeField] float sprintSpeed = 6.0f;
    [SerializeField] float crouchSpeed = 1.5f;
    //[SerializeField] float slopeSpeed = 7f;

    [Header("Look Parameters")]
    [SerializeField, Range(1, 10)] float lookSpeedX = 2.0f;
    [SerializeField, Range(1, 10)] float lookSpeedY = 2.0f;
    [SerializeField, Range(1, 100)] float upperLookLimit = 80.0f; //stupne
    [SerializeField, Range(1, 100)] float lowerLookLimit = 80.0f;

    [Header("Jumping Parameters")]
    [SerializeField] float jumpForce = 8.0f;
    [SerializeField] float gravity = 30.0f;

    [Header("Crouch Parameters")]
    [SerializeField] float crouchHeight = 0.5f;
    [SerializeField] float standingHeight = 2.0f;
    [SerializeField] float timeToCrouch = 0.25f;
    [SerializeField] Vector3 crouchingCenter = new Vector3(0, 0.5f, 0);
    [SerializeField] Vector3 standingCenter = new Vector3(0, 0, 0);
    private bool isCrouching;
    private bool duringCrouchAnimation;

    [Header("HeadBob Parameters")]
    [SerializeField] float walkBobSpeed = 10f;
    [SerializeField] float walkBobAmount = 0.03f;
    [SerializeField] float sprintBobSpeed = 14f;
    [SerializeField] float sprintBobAmount = 0.07f;
    [SerializeField] float crouchBobSpeed = 8f;
    [SerializeField] float crouchBobAmount = 0.025f;
    float defaultYPos = 0;
    float timer;

    [Header("Zoom Parameters")]
    [SerializeField] float timeToZoom = 0.3f;
    [SerializeField] float zoomFOV = 30f;
    float defaultFOV;
    Coroutine zoomRoutine;

    [Header("Interaction")]
    [SerializeField] Vector3 interactionRayPoint = default;
    [SerializeField] float interactionDistance = default;
    [SerializeField] LayerMask interactionLayer = default;
    Interactable currentInteractable;

    Vector3 moveDirection;
    Vector2 currentInput;

    float rotationX = 0;

    public event Action OnSprint;

    void Awake()
    {
        defaultYPos = playerCamera.transform.localPosition.y;
        defaultFOV = playerCamera.fieldOfView;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (CanMove)
        {
            HandleMovementInput();
            HandleMouseLook();

            if (canJump && shouldJump)
                HandleJump();

            if (canCrouch && shouldCrouch)
                StartCoroutine(CrouchStand());

            if (canUseHeadBob)
                HandleHeadBob();

            if (canZoom)
                HandleZoom();

            if (canInteract)
            {
                HandleInteractionCheck();
                HandleInteractionInput();
            }

            ApplyFinalMovement();
        }
    }
    private void OnEnable()
    {

    }

    private void HandleMovementInput()
    {
        currentInput = new Vector2((isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Vertical"), (isCrouching ? crouchSpeed : isSprinting ? sprintSpeed : walkSpeed) * Input.GetAxis("Horizontal"));

        float moveDirectionY = moveDirection.y;
        moveDirection = (transform.TransformDirection(Vector3.forward) * currentInput.x) + (transform.TransformDirection(Vector3.right) * currentInput.y);
        moveDirection.y = moveDirectionY;
    }

    private void HandleMouseLook()
    {
        rotationX -= Input.GetAxis("Mouse Y") * lookSpeedY;
        rotationX = Mathf.Clamp(rotationX, -upperLookLimit, lowerLookLimit);
        playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
        transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeedX, 0);
    }

    private void HandleJump()
    {
        moveDirection.y = jumpForce;
    }


    private void HandleHeadBob()
    {
        if (!isGrounded) return;

        if(Mathf.Abs(moveDirection.x) > 0.1f || Mathf.Abs(moveDirection.z) > 0.1f)
        {
            timer += Time.deltaTime * (isCrouching ? crouchBobSpeed : isSprinting ? sprintBobSpeed : walkBobSpeed);
            playerCamera.transform.localPosition = new Vector3(
                playerCamera.transform.localPosition.x,
                defaultYPos + Mathf.Sin(timer) * (isCrouching ? crouchBobAmount : isSprinting ? sprintBobAmount : walkBobAmount),
                playerCamera.transform.localPosition.z);
        }
    }
  

    private void HandleZoom()
    {
        if(Input.GetKeyDown(zoomKey))
        {
            if(zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(true));
        }

        if (Input.GetKeyUp(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }

            zoomRoutine = StartCoroutine(ToggleZoom(false));
        }
    }

    private void HandleInteractionCheck()
    {
        if(Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            if(hit.collider.gameObject.layer == 9 && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                    currentInteractable.OnFocus();
            }
        }
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void HandleInteractionInput()
    {
        if (Input.GetKeyDown(interactKey) && currentInteractable != null && Physics.Raycast(playerCamera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        }
    }

    private void ApplyFinalMovement()
    {
        if (!isGrounded)
        {
            moveDirection.y -= gravity * Time.deltaTime;
            //moveDirection.y -= gravity;
        }

        //if (characterController.velocity.y < -1 && characterController.isGrounded)
        //    moveDirection.y = 0;

        characterController.Move(moveDirection * Time.deltaTime);
    }

    private IEnumerator CrouchStand()
    {
        if (isCrouching && Physics.Raycast(playerCamera.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnimation = true;

        float timeElapsed = 0;
        float targetHeight = isCrouching ? standingHeight : crouchHeight;
        float currentHeight = characterController.height;
        Vector3 targetCenter = isCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = characterController.center;

        while(timeElapsed < timeToCrouch)
        {
            characterController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            characterController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        characterController.height = targetHeight;
        characterController.center = targetCenter;

        isCrouching = !isCrouching;

        duringCrouchAnimation = false;
    }

    private IEnumerator ToggleZoom(bool isEnter)
    {
        float targetFOV = isEnter ? zoomFOV : defaultFOV;
        float startingFOV = playerCamera.fieldOfView;
        float timeElapsed = 0;

        while(timeElapsed < timeToZoom)
        {
            playerCamera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        playerCamera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }
}
