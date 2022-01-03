using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [Header("Player controls")]
    [SerializeField] private float movementStrafeSpeed = 6f;
    [SerializeField] private float movementForwardSpeed = 6f;
    [SerializeField] private float movementBackwardSpeed = 6f;
    [SerializeField] private float movementSmoothing = 0.5f;
    [SerializeField] private float jumpHeight = 1;
    [SerializeField] private float gravity = -9.18f;
    [SerializeField] private float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform groundCheck;
    private float xInput, yInput;
    private Vector3 movementVector, moveDirection;
    private Vector3 velocity;
    private CharacterController characterController;
    private bool isGrounded, jumpInput;
    private Vector3 newMovementSpeedVelocity;
    private Vector3 newMovementSpeed;
    private float animatorSpeed;
    private bool isWalking;
    public bool IsWalking { get { return isWalking; } }
    public float Xinpt { get { return xInput; } }
    public float Yinpt { get { return yInput; } }
    public float AnimatorSpeed { get { return animatorSpeed; } }
    // WASD stuff
    void ControlGravity()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(isGrounded && velocity.y < 0)
        {
            characterController.stepOffset = 0.35f;
            velocity.y = -2f;
        }
        else
        {
            characterController.stepOffset = 0;
        }
        velocity.y += gravity * Time.deltaTime;
        
        characterController.Move(velocity * Time.deltaTime);

      
    }
    void CalculateMovement()
    {
        if (jumpInput && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
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
    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(groundCheck.position, groundDistance);
    }

    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        animatorSpeed = characterController.velocity.magnitude / (movementStrafeSpeed * 1.5f);
        Debug.Log(animatorSpeed);
        if (animatorSpeed > 1)
        {
            animatorSpeed = 1;
        }
      ControlGravity();
        CalculateMovement();
      GetInput();
     // MovePlayer();

    }

}
