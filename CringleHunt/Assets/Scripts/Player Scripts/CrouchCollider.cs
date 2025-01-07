using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrouchCollider : MonoBehaviour
{
    public SlidingMovement controller;
    public SphereCollider crouchCollider;
    
    // Start is called before the first frame update
    void Start()
    {
        controller.CanStand = true;
    }

    // Update is called once per frame
    void Update()
    {
        
        
    }

    private void OnTriggerEnter(Collider hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            controller.CanStand = false;
        } 
    }
    
    private void OnTriggerStay(Collider hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            controller.CanStand = false;
        }
    }
    
    private void OnTriggerExit(Collider hit)
    {
        if (hit.gameObject.CompareTag("Ground"))
        {
            controller.CanStand = true;
        }
    }
}
