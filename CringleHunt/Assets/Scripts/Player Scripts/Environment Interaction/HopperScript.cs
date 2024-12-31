using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HopperScript : MonoBehaviour
{
    [Header("Hopper Properties")] 
    public float verticalForceMultiplier = 2f;
    public CharacterController playerController;
    public Rigidbody playerRB;
    public int testCount = 0;

    void Start()
    { 
        playerController = playerController.GetComponent<CharacterController>();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            playerRB.AddForce(Vector3.up * verticalForceMultiplier);
            testCount += testCount;
        }
    }

    void Update()
    {

    }
}
