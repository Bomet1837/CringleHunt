using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

[RequireComponent(typeof(CharacterController))]

public class Controller : MonoBehaviour
{
    [Header("Movement Settings")] public float slowerWalkingSpeed = 4.5f;
    public float walkingSpeed = 7.5f;
    public float runningSpeed = 11.5f;
    public float jumpSpeed = 8.0f;
    public float gravity = 20.0f;
    public float hopperStrength = 0.125f;
    public float slideSpeedScale = 0.125f;

    private float _jv;
    private float _ws;
    private float _rs;

    private bool isFloating;
    private bool isPlayable;

    [Header("Camera Settings")] public Camera playerCamera;
    public float lookSpeed = 2.0f;
    public float lookXLimit = 45.0f;

    CharacterController characterController;
    [HideInInspector] public Vector3 moveDirection = Vector3.zero;
    float rotationX = 0;

    [HideInInspector] public bool canMove = true;
    public int jumpCounter = 2;

    [SerializeField] private Rigidbody rb;
    [SerializeField] private GameObject PauseScreen;


    void Start()
    {
        characterController = GetComponent<CharacterController>();
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;

        //storing original movement values, traversal gadgets overwrite the entered values.
        _jv = jumpSpeed;
        _ws = walkingSpeed;
        _rs = runningSpeed;
        
        isFloating = false;
        isPlayable = true;

        


        // Lock cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        // We are grounded, so recalculate move direction based on axes
        Vector3 forward = transform.TransformDirection(Vector3.forward);
        Vector3 right = transform.TransformDirection(Vector3.right);

        // Press Left Shift to run, Left Alt to walk
        bool isRunning = Input.GetKey(KeyCode.LeftShift);
        bool isWalking = Input.GetKey(KeyCode.LeftAlt);
        float curSpeedX = canMove
            ? (isRunning ? runningSpeed : (isWalking ? slowerWalkingSpeed : walkingSpeed)) * Input.GetAxis("Vertical")
            : 0;
        float curSpeedY = canMove
            ? (isRunning ? runningSpeed : (isWalking ? slowerWalkingSpeed : walkingSpeed)) * Input.GetAxis("Horizontal")
            : 0;
        float movementDirectionY = moveDirection.y;
        moveDirection = (forward * curSpeedX) + (right * curSpeedY);


        // Jumping controls and config, double jump functionality
        Mathf.Clamp(jumpSpeed, 0f, 50f);
        if (Input.GetButtonDown("Jump") && canMove && characterController.isGrounded ||
            Input.GetButtonDown("Jump") && jumpCounter > 0)
        {
            moveDirection.y = jumpSpeed;
            jumpCounter -= 1;
        }
        else
        {
            moveDirection.y = movementDirectionY;
        }

        if (characterController.isGrounded && jumpCounter <= 0)
        {
            jumpCounter = 2;
        }

        // Apply gravity. Gravity is multiplied by deltaTime twice (once here, and once below
        // when the moveDirection is multiplied by deltaTime). This is because gravity should be applied
        // as an acceleration (ms^-2)
        if (!characterController.isGrounded && !isFloating)
        {
            moveDirection.y -= gravity * Time.deltaTime;
        }
        else if (isFloating)
        {
            moveDirection.y = 10f;
        }
        
      

        // Move the controller
        characterController.Move(moveDirection * Time.deltaTime);

        // Player and Camera rotation
        if (canMove && isPlayable)
        {
            rotationX += -Input.GetAxis("Mouse Y") * lookSpeed;
            rotationX = Mathf.Clamp(rotationX, -lookXLimit, lookXLimit);
            playerCamera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            transform.rotation *= Quaternion.Euler(0, Input.GetAxis("Mouse X") * lookSpeed, 0);
        }
        
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            isPlayable = !isPlayable;
            if (isPlayable)
            {
                Time.timeScale = 1;
                PauseScreen.SetActive(false);
                Cursor.lockState = CursorLockMode.Locked;
            }
            else
            {
                Time.timeScale = 0; 
                PauseScreen.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }
    

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        float slideStrength = slideSpeedScale + walkingSpeed;
        switch (hit.gameObject.tag)
        {
            case "isHopper":
                jumpSpeed = hopperStrength;
                break;
            case "isHopper2":
                moveDirection = new Vector3(0f, hopperStrength / 2f, 0f);
                break;
            case "Ground":
                jumpSpeed = _jv;
                walkingSpeed = _ws;
                runningSpeed = _rs;
                break;
            case "isSlide":
                walkingSpeed = Mathf.Clamp(slideStrength, 0f, 70f);
                runningSpeed = Mathf.Clamp(slideStrength, 0f, 70f);
                break;
        }

        
    }

    private void OnTriggerEnter(Collider hit)
    {
        switch (hit.gameObject.tag)
        {
            case "Test":
                Debug.Log("This trigger works!");
                break;
            case "FallTrigger":
                characterController.transform.position = new Vector3(0, 0, 0);
                break;
            case "isDrifter":
                isFloating = true;
                if(isFloating){Debug.Log("Floating 1...");}
                break;
        }
        

    }

    private void OnTriggerExit(Collider hit)
    {
        switch (hit.gameObject.tag)
        {
            case "isDrifter":
                isFloating = false;
                if(!isFloating){Debug.Log("Not floating.");}
                break; 
        }
    }
}
