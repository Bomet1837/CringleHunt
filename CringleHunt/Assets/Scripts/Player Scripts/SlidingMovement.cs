using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SlidingMovement : MonoBehaviour
{
    [Header("References")] 
    public Transform orient;
    public Transform playerObj;
    private Rigidbody rb;
    private Controller move;

    [Header("Sliding Settings")] 
    public float slideTimeMax;
    public float slideForce;
    private float slideTimer;
    
    public float slideYScale;
    private float slideYScaleStart;

    [Header("Inputs")] 
    public KeyCode slideKey = KeyCode.LeftShift;
    private float inputX;
    private float inputY;
    private bool isSliding;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        move = GetComponent<Controller>();

        slideYScaleStart = playerObj.localScale.y;
    }
    void Update()
    {
        inputX = Input.GetAxisRaw("Horizontal");
        inputY = Input.GetAxisRaw("Vertical");

        if (Input.GetKeyDown(slideKey))
            SlideStart();
        if(Input.GetKeyUp(slideKey) && isSliding)
            SlideEnd();

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
        rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // slideTimer = slideTimeMax; //<==== Slide Timer

    }

    void SlideEnd()
    {
        isSliding = false;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScaleStart, playerObj.localScale.z);
    }

    void SlideMove()
    {
        Vector3 inputDir = orient.forward * inputY + orient.right * inputY;
        rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);
        // if(slideTimer <=0)
        //     SlideEnd();
    }

}
