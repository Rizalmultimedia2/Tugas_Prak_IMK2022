﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private float playerSpeed = 10f;
    [SerializeField] private float turnSmoothTime = 0.1f;
    [SerializeField] private float jumpHeight = 1.0f;
    [SerializeField] private float gravityValue = -9.81f;
    [SerializeField] private bool usePhysics = true;
    [SerializeField] private float rotationSpeed;
    float turnSmoothVelocity;

    private Camera _mainCamera;
    public Transform cam;
    private Rigidbody _rb;
    private Controls _controls;
    private Vector3 playerVelocity;
    private Animator _animator;
    private static readonly int IsWalking = Animator.StringToHash("isWalking");

    private void Awake()
    {
        _controls = new Controls();
    }

    private void OnEnable()
    {
        Cursor.lockState = CursorLockMode.Locked;
        _controls.Enable();
    }

    private void OnDisable()
    {
        Cursor.lockState = CursorLockMode.None;
        _controls.Disable();
    }

    private void Start()
    {
        _mainCamera = Camera.main;
        _rb = gameObject.GetComponent<Rigidbody>();
        _animator = gameObject.GetComponentInChildren<Animator>();
    }
    private void Update()
    {

        if (usePhysics)
        {
            return;
        }

        Vector2 input = _controls.Player.Move.ReadValue<Vector2>();

        if (_controls.Player.Move.IsPressed())
        {
            _animator.SetBool(IsWalking, true);
            Vector3 target = HandleInput(input);
            Move(target);
        }
        else
            _animator.SetBool(IsWalking, false);

        if (_controls.Player.Jump.IsPressed())
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            playerVelocity.y += gravityValue * Time.deltaTime;
            Vector3 targetJump = transform.position + playerVelocity * Time.deltaTime;
            Move(targetJump);
        }

    }

    private void FixedUpdate()
    {

        if (!usePhysics)
        {
            return;
        }

        Vector2 input = _controls.Player.Move.ReadValue<Vector2>();

        if (_controls.Player.Move.IsPressed())
        {
            _animator.SetBool(IsWalking, true);
            Vector3 target = HandleInput(input);
            MovePhysics(target);
        }
        else
            _animator.SetBool(IsWalking, false);

        if (_controls.Player.Jump.IsPressed())
        {
            playerVelocity.y += Mathf.Sqrt(jumpHeight * -3.0f * gravityValue);
            playerVelocity.y += gravityValue * Time.deltaTime;
            Vector3 targetJump = transform.position + playerVelocity * Time.deltaTime;
            MovePhysics(targetJump);
        }
      
    }

    private Vector3 HandleInput(Vector2 input)
    {
        Vector3 forward = _mainCamera.transform.forward;
        Vector3 right = _mainCamera.transform.right;

        forward.y = 0;
        right.y = 0;

        forward.Normalize();
        right.Normalize();

        Vector3 direction = right * input.x + forward * input.y;
        // Vector3 direction = new Vector3(input.x, 0f, input.y).normalized;

        // float targetAngle = Mathf.Atan2(direction.x, direction.z) + Mathf.Rad2Deg + cam.eulerAngles.y;
        // float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime);
        // transform.rotation = Quaternion.Euler(0f, targetAngle, 0f);
        // Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        if(direction != Vector3.zero){
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        return transform.position + direction * playerSpeed * Time.deltaTime;
    }

    private void Move(Vector3 target)
    {
        transform.position = target;

    }

    private void MovePhysics(Vector3 target)
    {
        transform.position = target;
    }
}
