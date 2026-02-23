using UnityEngine;

public class BFw_Detection : NPCBehaviourFwk
{
    public float detectionRadius = 10f;
    public LayerMask detectionLayers;
    [HideInInspector] public bool isPlayerDetected;

    [SerializeField] private Transform playerTransform; // assign in inspector for robustness
    private BFw_Movement _movementScript;
    private BFw_Combat _combatScript;

    private Coroutine _currentCombatCoroutine;
    private bool _isAlerted;

    // Non-alloc buffer for physics queries
    private Collider[] _overlapResults = new Collider[8];

    private new void Awake()
    {
        base.Awake();
        _movementScript = GetComponent<BFw_Movement>();
        _combatScript = GetComponent<BFw_Combat>();

        // Try inspector-assigned transform first, fallback to FindWithTag
        if (playerTransform == null)
        {
            var playerObj = GameObject.FindWithTag("Player");
            if (playerObj != null) playerTransform = playerObj.transform;
        }
    }

    private void FixedUpdate()
    {
        DetectPlayer();
        IsAlerted();
        IsLost();
    }

    private void DetectPlayer()
    {
        // Use non-allocating API to reduce GC pressure
        int hits = Physics.OverlapSphereNonAlloc(transform.position, detectionRadius, _overlapResults, detectionLayers);

        isPlayerDetected = false;
        for (int i = 0; i < hits; i++)
        {
            var c = _overlapResults[i];
            if (c == null) continue;
            if (c.CompareTag("Player") || (playerTransform != null && c.transform.IsChildOf(playerTransform)))
            {
                isPlayerDetected = true;
                break;
            }
        }

        // If buffer was full, consider increasing its size to avoid missed hits
        if (hits == _overlapResults.Length)
        {
            // Double buffer size to handle denser scenes; this keeps the change local and safe.
            System.Array.Resize(ref _overlapResults, _overlapResults.Length * 2);
        }
    }

    public void IsAlerted()
    {
        if (isPlayerDetected)
        {
            _combatScript.AimAtTarget(playerTransform);
        }
        
        if (isPlayerDetected && !_isAlerted)
        {
            
            if (_movementScript == null || playerTransform == null)
            {
                Debug.LogWarning("BFw_Detection: Movement script missing; cannot start combat coroutine.");
                _isAlerted = true; // still mark alerted to avoid repeated attempts
                return;
            }

            // Start the combat coroutine only once 
            if (_currentCombatCoroutine == null)
            {
                _currentCombatCoroutine = StartCoroutine(_movementScript.CombatCoroutine(playerTransform));
            }

            _isAlerted = true;
        }
    }

    private void IsLost()
    {
        if (!isPlayerDetected && _isAlerted)
        {
            // Stop any running combat coroutine when the player is lost
            if (_currentCombatCoroutine != null)
            {
                StopCoroutine(_currentCombatCoroutine);
                _currentCombatCoroutine = null;
            }

            _isAlerted = false;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
}
