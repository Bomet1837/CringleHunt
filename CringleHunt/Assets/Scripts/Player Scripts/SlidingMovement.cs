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
    public CrouchCollider crouchCollider;
    public Controller controller;
    public Transform[] preserveScaleTransforms;


    
    [Header("Sliding Settings")] 
    public float slideTimeMax;
    public float slideForce;
    private float slideTimer;
    
    public float slideYScale;
    private float slideYScaleStart;
    private int playerLayer = 6;
    private int layersExclPlayer;
    private Vector3 _originalPlayerScaleLocal;
    private Vector3[] _originalPlayerScalesLocalPreserved;

    [Header("Inputs")] 
    public KeyCode slideKey = KeyCode.LeftShift;
    private float inputX;
    private float inputY;
    private bool isSliding;
    [HideInInspector] public bool CanStand;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        slideYScaleStart = playerObj.localScale.y;

        playerLayer = LayerMask.NameToLayer("Player");
        layersExclPlayer = (1 << playerLayer);
        layersExclPlayer = ~layersExclPlayer;

        _originalPlayerScaleLocal = playerObj != null ? playerObj.localScale : Vector3.one;
        if (preserveScaleTransforms != null && preserveScaleTransforms.Length > 0)
        {
            _originalPlayerScalesLocalPreserved = new Vector3[preserveScaleTransforms.Length];
            for (int i = 0; i < preserveScaleTransforms.Length; i++)
            {
                if (preserveScaleTransforms[i] != null)
                    _originalPlayerScalesLocalPreserved[i] = preserveScaleTransforms[i].localScale;
                else
                {
                    _originalPlayerScalesLocalPreserved[i] = Vector3.one;
                }
            }
            
        }

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

    /*private void OnControllerColliderHit(ControllerColliderHit other)
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
    }*/


    private void FixedUpdate()
    {
        if(isSliding)
        {
            //SlideMove();
        }
    }

    void SlideStart()
    {
        isSliding = true;
        playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScale, playerObj.localScale.z);
        SlideMove();
        //rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);

        // slideTimer = slideTimeMax; //<==== Slide Timer

    }

    void SlideEnd()
    {
        {
            isSliding = false;
            if (playerObj != null)
            {
                Vector3 newScale = new Vector3(playerObj.localScale.x, slideYScaleStart, playerObj.localScale.z);
                playerObj.localScale = newScale;
                MaintainPreservedScales(newScale);
            }
            
            //playerObj.localScale = new Vector3(playerObj.localScale.x, slideYScaleStart, playerObj.localScale.z);
            
            
        }
            
        
        
        
        
    }

    void SlideMove()
    {
        Vector3 inputDir = orient.forward * inputY + orient.right * inputX;
        rb.AddForce(inputDir.normalized * slideForce, ForceMode.Force);
        // if(slideTimer <=0)
        //     SlideEnd();
    }

    void MaintainPreservedScales(Vector3 newParentScale)
    {
        if (preserveScaleTransforms == null || _originalPlayerScalesLocalPreserved == null) return;
        
        Vector3 factor = new Vector3(
            SafeDiv(_originalPlayerScaleLocal.x, newParentScale.x),
            SafeDiv(_originalPlayerScaleLocal.y, newParentScale.y),
            SafeDiv(_originalPlayerScaleLocal.z, newParentScale.z)
                );

        for (int i = 0; i < preserveScaleTransforms.Length; i++)
        {
            var t = preserveScaleTransforms[i];
            if (t != null) continue;
            
            Vector3 origLocal = _originalPlayerScalesLocalPreserved.Length > i ? _originalPlayerScalesLocalPreserved[i] : t.localScale;
            t.localScale = new Vector3(origLocal.x, origLocal.y, origLocal.z * factor.z);
        }
    }

    float SafeDiv(float a, float b)
    {
        return Mathf.Approximately(b, 0f) ? 1f : a / b;
    }

}
