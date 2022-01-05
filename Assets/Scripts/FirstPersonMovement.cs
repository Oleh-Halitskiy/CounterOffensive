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
    [SerializeField] private playerStances PlayerStances;

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
    private float capsuleFloatSmoothingVelocity;
    private float animatorSpeed;
    private float cameraHeight, cameraSmoothingVelocity;
    private bool isWalking, canJump;
    // default values
    private float defaultForwardSpeed, defaultStrafeSpeed, defaultBackwardSpeed;
    //
    private enum playerStances
    {
        Stand,
        Crouch,
        Prone
    }
    public bool IsWalking { get { return isWalking; } }
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
       
        if (Input.GetKeyDown(KeyCode.C))
        {
            if(PlayerStances == playerStances.Crouch)
            {
                PlayerStances = playerStances.Stand;
                return;
            }
            PlayerStances = playerStances.Crouch;
        }
        else if (Input.GetKeyDown(KeyCode.Z))
        {
            if(PlayerStances == playerStances.Prone)
            {
                PlayerStances = playerStances.Stand;
                return;
            }
            PlayerStances = playerStances.Prone;
        }
 
        if (jumpInput && isGrounded && canJump)
        {
            gravityVelocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        var verticalSpeed = movementForwardSpeed * yInput * Time.deltaTime;
        var horizontalSpeed = movementStrafeSpeed * xInput * Time.deltaTime;
        movementVector.z = verticalSpeed;
        movementVector.x = horizontalSpeed;
        newMovementSpeed = Vector3.SmoothDamp(newMovementSpeed, movementVector, ref newMovementSpeedVelocity, movementSmoothing);
        var movementSpeed = transform.TransformDirection(newMovementSpeed);
        characterController.Move(movementSpeed);
    }  
    void GetInput()
    {
        jumpInput = Input.GetButton("Jump");
        xInput = Input.GetAxisRaw("Horizontal");
        yInput = Input.GetAxisRaw("Vertical");
        if (xInput == 0 && yInput == 0)
        {
            isWalking = false;
        }
        else
        {
            
            isWalking = true; 
        }
    }
    void CalculateStances()
    {
            movementForwardSpeed = defaultForwardSpeed;
            movementStrafeSpeed = defaultStrafeSpeed;
            movementBackwardSpeed = defaultBackwardSpeed;
            canJump = true;
            var stanceHeight = cameraStandHeight;
            var capsuleHeight = StandCollider.height;
            var capsuleCenter = StandCollider.center;
    
        if(PlayerStances == playerStances.Crouch)
        {
            movementForwardSpeed = CrouchForwardSpeed;
            movementStrafeSpeed = CrouchStrafeSpeed;
            movementBackwardSpeed = CrouchBackwardSpeed;
            canJump = false;
            capsuleCenter = CrouchCollider.center;
            capsuleHeight = CrouchCollider.height;
            stanceHeight = cameraCrouchHeight;
        }
        else if(PlayerStances == playerStances.Prone)
        {
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
    void Start()
    {
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
