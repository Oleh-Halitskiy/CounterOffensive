using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Player controls")]
    [SerializeField] private float movementStrafeSpeed = 6f;
    [SerializeField] private float movementForwardSpeed = 6f;
    [SerializeField] private float movementBackwardSpeed = 6f;
    [SerializeField] private float movementSmoothing = 0.5f;
    [SerializeField] private float jumpHeight = 1;
    [Header("Crouch controls")]
    [SerializeField] private float CrouchStrafeSpeed;
    [SerializeField] private float CrouchForwardSpeed;
    [SerializeField] private float CrouchBackwardSpeed;
    [Header("Prone controls")]
    [SerializeField] private float ProneStrafeSpeed;
    [SerializeField] private float ProneForwardSpeed;
    [SerializeField] private float ProneBackwardSpeed;
    [Header("Gravity Control")]
    [SerializeField] private float gravity = -9.18f;
    [SerializeField] private float groundDistance = 0.4f;
    [Header("Player Stances")]
    [SerializeField] private float cameraStandHeight;
    [SerializeField] private CapsuleCollider StandCollider;
    [SerializeField] private float cameraCrouchHeight;
    [SerializeField] private CapsuleCollider CrouchCollider;
    [SerializeField] private float cameraProneHeight;
    [SerializeField] private CapsuleCollider ProneCollider;
    [SerializeField] private float StancePositionSmoothing;
    [SerializeField] private PlayerStances playerStances;
    [Header("Player footsteps")]
    [SerializeField] private float WalkingStepsRate;
    [SerializeField] private float SprintingStepsRate;
    [SerializeField] private float CrouchingStepsRate;

    public LayerMask groundMask;
    public Transform groundCheck;
    public Transform cameraHolder;
    private float xInput, yInput;
    private Vector3 movementVector;
    private Vector3 gravityVelocity;
    private CharacterController characterController;
    private bool isGrounded, jumpInput;
    private Vector3 newMovementSpeedVelocity;
    private Vector3 capsuleSmoothingVelocity;
    private Vector3 newMovementSpeed;
    private Vector3 moveDirection;
    private float capsuleFloatSmoothingVelocity;
    private float animatorSpeed;
    private float cameraHeight, cameraSmoothingVelocity;
    private bool isWalking, canJump, isSprinting, isCrouching;
    private AudioSource audioSource;
    private float nextTimeToStep;
    // default values
    private float defaultForwardSpeed, defaultStrafeSpeed, defaultBackwardSpeed;
    //
    private enum PlayerStances
    {
        Stand,
        Crouch,
        Prone
    }
    public bool IsWalking { get { return isWalking; } }
    public bool IsSprinting { get { return isSprinting; } }
    public float Xinpt { get { return xInput; } }
    public float Yinpt { get { return yInput; } }
    public float AnimatorSpeed { get { return animatorSpeed; } }
    // WASD stuff
    void ControlGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && gravityVelocity.y < 0)
        {
            characterController.stepOffset = 0.35f;
            gravityVelocity.y = -2f;
        }
        else
        {
            characterController.stepOffset = 0;
        }
        gravityVelocity.y += gravity * Time.deltaTime;
        
        characterController.Move(gravityVelocity * Time.deltaTime);

      
    }
    void CalculateMovement()
    {
        if (jumpInput && isGrounded && canJump)
        {
            gravityVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        var verticalSpeed = movementForwardSpeed * yInput * Time.deltaTime;
        var horizontalSpeed = movementStrafeSpeed * xInput * Time.deltaTime;
        movementVector = transform.forward * verticalSpeed + transform.right * horizontalSpeed;
        moveDirection = Vector3.SmoothDamp(moveDirection, movementVector, ref newMovementSpeedVelocity, movementSmoothing);
        if (isSprinting)
        {
            characterController.Move(moveDirection * 1.5f);
        }
        else
        {
            characterController.Move(moveDirection);
        }
        playFootStepts();

    }
    void GetInput()
    {
    
        jumpInput = Input.GetButton("Jump");
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        if (xInput == 0 && yInput == 0 || isSprinting)
        {
            isWalking = false;
        }
        else
        {
            
            isWalking = true; 
        }
        if (Input.GetKeyDown(KeyCode.C))
        {
            if (playerStances == PlayerStances.Crouch)
            {
                playerStances = PlayerStances.Stand;
                return;
            }
            playerStances = PlayerStances.Crouch;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if (playerStances == PlayerStances.Prone)
            {
                playerStances = PlayerStances.Stand;
                return;
            }
            playerStances = PlayerStances.Prone;
        }
        if (Input.GetKey(KeyCode.LeftShift) && playerStances == PlayerStances.Stand && yInput == 1)
        {
            isSprinting = true;
            
        }
        else
        {
            isSprinting = false;
        }
    }
    void CalculateStances()
    {
            isCrouching = false;
            movementForwardSpeed = defaultForwardSpeed;
            movementStrafeSpeed = defaultStrafeSpeed;
            movementBackwardSpeed = defaultBackwardSpeed;
            canJump = true;
            var stanceHeight = cameraStandHeight;
            var capsuleHeight = StandCollider.height;
            var capsuleCenter = StandCollider.center;
    
        if(playerStances == PlayerStances.Crouch)
        {
            isCrouching = true;
            movementForwardSpeed = CrouchForwardSpeed;
            movementStrafeSpeed = CrouchStrafeSpeed;
            movementBackwardSpeed = CrouchBackwardSpeed;
            canJump = false;
            capsuleCenter = CrouchCollider.center;
            capsuleHeight = CrouchCollider.height;
            stanceHeight = cameraCrouchHeight;
        }
        else if(playerStances == PlayerStances.Prone)
        {
            isCrouching = false;
            movementForwardSpeed = ProneForwardSpeed;
            movementStrafeSpeed = ProneStrafeSpeed;
            movementBackwardSpeed = ProneBackwardSpeed;
            canJump = false;
            capsuleCenter = ProneCollider.center;
            capsuleHeight = ProneCollider.height;
            stanceHeight = cameraProneHeight;
        }
        cameraHeight = Mathf.SmoothDamp(cameraHolder.localPosition.y, stanceHeight, ref cameraSmoothingVelocity, StancePositionSmoothing);
        cameraHolder.localPosition = new Vector3(cameraHolder.localPosition.x, cameraHeight, cameraHolder.localPosition.z);
        characterController.height = Mathf.SmoothDamp(characterController.height, capsuleHeight, ref capsuleFloatSmoothingVelocity, StancePositionSmoothing);
        characterController.center = Vector3.SmoothDamp(characterController.center, capsuleCenter, ref capsuleSmoothingVelocity, StancePositionSmoothing);

    }
    private void playFootStepts()
    {
        if (isGrounded == true && characterController.velocity.magnitude > 1f)
        {
                if(nextTimeToStep <= 0)
            {
                nextTimeToStep = 1;
                audioSource.volume = Random.Range(0.8f, 1);
                audioSource.pitch = Random.Range(0.8f, 1.1f);
                audioSource.PlayOneShot(audioSource.clip);
            }
        }
        if (isWalking && !isCrouching)
        {
            nextTimeToStep -= Time.deltaTime * WalkingStepsRate;
        }
        else if(isSprinting)
        {
            nextTimeToStep -= Time.deltaTime * SprintingStepsRate;
        }
        else if (isCrouching)
        {
            nextTimeToStep -= Time.deltaTime * CrouchingStepsRate;
        }
    }
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        characterController = GetComponent<CharacterController>();
        defaultForwardSpeed = movementForwardSpeed;
        defaultStrafeSpeed = movementStrafeSpeed;
        defaultBackwardSpeed = movementBackwardSpeed;
    }
    private void Update()
    {
        animatorSpeed = characterController.velocity.magnitude / (movementStrafeSpeed * 1.5f);
        if (animatorSpeed > 1)
        {
            animatorSpeed = 1;
        }
        ControlGravity();
        CalculateMovement();
        GetInput();
        CalculateStances();
    }

}
