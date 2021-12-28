using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstPersonMovement : MonoBehaviour
{
    [SerializeField] private float movementSpeed = 6f;
    [SerializeField] private float jumpHeight = 1;
    [SerializeField] private float gravity = -9.18f;
    [SerializeField] private float groundDistance = 0.4f;
    public LayerMask groundMask;
    public Transform groundCheck;
    private float xInput, yInput;
    private Vector3 movementVector;
    private Vector3 velocity;
    private CharacterController characterController;
    private bool isGrounded, jumpInput;
    private Vector3 moveVelocity;
    private Vector3 moveDirection;
    private float vel = 0.15f;
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
            velocity.y = -2f;
        }
        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }
    void MovePlayer()
    {
        if(jumpInput && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        characterController.Move(moveDirection * movementSpeed * Time.deltaTime);
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
        movementVector = transform.forward * yInput + transform.right * xInput;
        moveDirection = Vector3.SmoothDamp(moveDirection, movementVector.normalized, ref moveVelocity, vel);
        
    }
   
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
        animatorSpeed = characterController.velocity.magnitude / movementSpeed;
        if (animatorSpeed > 1)
        {
            animatorSpeed = 1;
        }
      ControlGravity();
      GetInput();
      MovePlayer();

    }

}
