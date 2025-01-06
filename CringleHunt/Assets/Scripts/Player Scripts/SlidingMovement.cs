using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;

public class SlidingMovement : MonoBehaviour
{
    [Header("References")] 
    public Transform orient;
    public Transform playerObj;
    public CharacterController playerController;
    public Rigidbody rb;


    
    [Header("Sliding Settings")] 
    public float slideTimeMax;
    public float slideForce;
    private float slideTimer;
    
    public float slideYScale;
    private float slideYScaleStart;
    private int playerLayer = 6;
    private int layersExclPlayer;

    
    

    [Header("Inputs")] 
    public KeyCode slideKey = KeyCode.LeftShift;
    private float inputX;
    private float inputY;
    private bool isSliding;
    private bool CanStand;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        slideYScaleStart = playerObj.localScale.y;

        playerLayer = LayerMask.NameToLayer("Player");
        layersExclPlayer = (1 << playerLayer);
        layersExclPlayer = ~layersExclPlayer;

    }
    void Update()
    {
        Physics.CheckCapsule(
            transform.position + Vector3.up * (0.5f + Physics.defaultContactOffset),
            transform.position + Vector3.up * (1f - 0.5f),
            0.5f - Physics.defaultContactOffset,
            layersExclPlayer,
            QueryTriggerInteraction.Ignore);
        
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetKey(slideKey))
        {
            SlideStart();
        }
        else if (!Input.GetKey(slideKey) && CanStand == true)
        {
            SlideEnd();
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit other)
    {
        switch (other.gameObject.layer)
        {
            case 10 :
                CanStand = false;
                break;
            
            case 7 :
                CanStand = true;
                break;
        }
    }


    private void FixedUpdate()
    {
        if(isSliding)
        {
            SlideMove();
        }
    }

    void SlideStart()
    {
        isSliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        playerController.height = 1f;
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // slideTimer = slideTimeMax; //<==== Slide Timer

    }

    void SlideEnd()
    {
        {
            isSliding = false;
            playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScaleStart, playerObj.localScale.z);
            playerController.height = 2f;
        }
            
        
        
        
        
    }

    void SlideMove()
    {
        Vector3 inputDir = orient.forward * inputY + orient.right * inputY;
        rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);
        // if(slideTimer <=0)
        //     SlideEnd();
    }

}
