using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.TextCore.Text;

public class PlayerController : MonoBehaviour
{
    private const float Gravity = 9.0f;
    
    
    public CharacterController characterController;
    public Vector3 characterVelocity;
    
    

    [Header("Controller Parameters")] 
    public float gravityMult;
    public float movementSpeed = 10f;
    public float jumpHeight;
    private bool _isCrouching;

    [Header("Ground Collision Parameters")]
    public GameObject collisionFlag;
    public float distance = 0.1f;
    public bool isGrounded;
    public LayerMask groundMask;
    
    
    // Start is called before the first frame update
    void Start()
    {
        characterController = GetComponent<CharacterController>();
    }

    // Update is called once per frame
    void Update()
    {
        //raycast functions
        RaycastHit hit;

        if (Physics.Raycast(collisionFlag.transform.position, collisionFlag.transform.TransformDirection(Vector3.down),
                out hit, distance, groundMask))
        {
            isGrounded = true;
        }
        else
        {
            isGrounded = false;
        }
        
        Debug.DrawRay(collisionFlag.transform.position, collisionFlag.transform.TransformDirection(Vector3.down) * distance, Color.red);

        if (isGrounded) //player is on the ground
        {
            HandleGroundedMovement();

            if (Input.GetKeyDown(KeyCode.Space)) //jump input and command
            {
               // characterVelocity += new Vector3();
                characterVelocity += Vector3.up * jumpHeight;
            }

        }
        else //player is not on the ground
        {
            HandleAirMovement();

        }
        
        //update controller movement on every frame
        characterController.Move(characterVelocity * Time.deltaTime);
        characterVelocity += Vector3.down * (Gravity * Time.deltaTime * gravityMult);

        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl))
        {
            _isCrouching = true;
        }
        else
        {
            _isCrouching = false;
        }
        
        if (Input.GetKeyDown(KeyCode.C) || Input.GetKeyDown(KeyCode.LeftControl) && _isCrouching)
        {
            CrouchFunction();
        }

        
    }

    private void CrouchFunction()
    {
        Vector2 inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 inputSpaceMovement = new Vector3(inputAxis.x - 2.5f, 0, inputAxis.y - 2.5f);
        Vector3 worldSpaceMovement = transform.TransformVector(inputSpaceMovement);

        transform.localScale = new Vector3(1f, 0.5f, 1f);
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);

        characterVelocity = worldSpaceMovement * movementSpeed;
    }

    private void HandleGroundedMovement()
    {
        Vector2 inputAxis = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        Vector3 inputSpaceMovement = new Vector3(inputAxis.x, 0, inputAxis.y);
        Vector3 worldSpaceMovement = transform.TransformVector(inputSpaceMovement);

        characterVelocity = worldSpaceMovement * movementSpeed;
        
    }
    
    private void HandleAirMovement()
    {
        Vector2 inputAxis = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
        Vector3 inputSpaceMovement = new Vector3(inputAxis.x, 0, inputAxis.y);
        Vector3 worldSpaceMovement = transform.TransformVector(inputSpaceMovement);
        
        characterController.Move(movementSpeed * Time.deltaTime * worldSpaceMovement);
        
    }
}
