using System;
using UnityEngine;

public class BFw_Detection : NPCBehaviourFwk, NPCBehaviourInterface_Detection
{
    public float detectionRadius = 10f;
    public LayerMask detectionLayers;
    [SerializeField] protected bool isPlayerDetected = false;
    [SerializeField] private GameObject player;
    [SerializeField] private BFw_Movement _movementScript;

    private bool _wasPlayerDetected = false;
    
    new private void Awake()
    {
        base.Awake();
        _movementScript = GetComponent<BFw_Movement>();
        player = GameObject.FindWithTag("Player");
    }

    new private void FixedUpdate()
    {
        DetectPlayer();

        if (isPlayerDetected /*&& !_wasPlayerDetected*/)
        {
            OnNPCAlerted();
        }

       // _wasPlayerDetected = isPlayerDetected;
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayers);
        isPlayerDetected = hits.Length > 0;
    }

    public void OnNPCAlerted()
    {
        //Debug.Log(isPlayerDetected);
        _movementScript.MoveTowards(player.transform.position);
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
