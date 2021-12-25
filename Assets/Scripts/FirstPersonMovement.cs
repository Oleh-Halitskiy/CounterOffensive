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
    private float _xInput, _yInput;
    private Vector3 _movementVector;
    private Vector3 _velocity;
    private CharacterController _characterController;
    private bool _isGrounded, _jumpInput;
    private Vector3 _moveVelocity;
    private Vector3 _moveDirection;
    private float vel = 0.15f;
    // WASD stuff
    void ControlGravity()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        if(_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f;
        }
        _velocity.y += gravity * Time.deltaTime;
        _characterController.Move(_velocity * Time.deltaTime);
    }
    void MovePlayer()
    {
        if(_jumpInput && _isGrounded)
        {
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }
        _characterController.Move(_moveDirection * movementSpeed * Time.deltaTime);
    }
    void GetInput()
    {
        _jumpInput = Input.GetButton("Jump");
        _xInput = Input.GetAxisRaw("Horizontal");
        _yInput = Input.GetAxisRaw("Vertical");
        _movementVector = transform.forward * _yInput + transform.right * _xInput;
        _moveDirection = Vector3.SmoothDamp(_moveDirection, _movementVector.normalized, ref _moveVelocity, vel);
        
    }
   
    void Start()
    {
        _characterController = GetComponent<CharacterController>();
    }
    private void Update()
    {
      ControlGravity();
      GetInput();
      MovePlayer();

    }

}
