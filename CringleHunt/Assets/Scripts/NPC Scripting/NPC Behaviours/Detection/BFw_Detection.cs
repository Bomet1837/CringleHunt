using System;
using UnityEngine;

public class BFw_Detection : NPCBehaviourFwk, NPCBehaviourInterface_Detection
{
    public float detectionRadius = 10f;
    public LayerMask detectionLayers;
    [SerializeField] protected bool isPlayerDetected = false;
    [SerializeField] private GameObject player;
    [SerializeField] private BFw_Movement _movementScript;
    
    private new void Awake()
    {
        base.Awake();
        _movementScript = GetComponent<BFw_Movement>();
        player = GameObject.FindWithTag("Player");
    }

    private new void FixedUpdate()
    {
        DetectPlayer();

        if (isPlayerDetected)
        {
            OnNPCAlerted();
        }
    }

    private void DetectPlayer()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, detectionRadius, detectionLayers);
        isPlayerDetected = hits.Length > 0;
    }

    public void OnNPCAlerted()
    {
        StartCoroutine(_movementScript.CombatCoroutine(player.transform.position));
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
