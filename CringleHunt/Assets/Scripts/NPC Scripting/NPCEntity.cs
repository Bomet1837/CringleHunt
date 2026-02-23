using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NPCEntity : MonoBehaviour
{

    [HideInInspector] public Rigidbody rb;
    public string entityName;
    public float bodyLife = 5f;
    public float startingHealth;
    public float Health
    {
        get
        {
            return _health;
        }
        set
        {
            _health = Mathf.Clamp(value, 0, startingHealth);
            Debug.Log($"Health is now {_health}");

            if (_health <= 0)
            {
                StartCoroutine(DieRagdoll());
            }
        }
    }
    
    private float _health;
    private NavMeshAgent _agent;

    void Awake()
    {
        entityName = gameObject.name;
        rb = GetComponent<Rigidbody>();
        _agent = GetComponent<NavMeshAgent>();
        
        if (!rb)
        {
            Debug.LogError("Rigidbody component not found on " + gameObject.name);
        }
    }
    

    void Start()
    {
        Health = startingHealth;
    }

    IEnumerator DieRagdoll()
    {
        if (!_agent != null)
        {
            _agent.isStopped = true;
            _agent.enabled = false;
            Destroy(_agent);
        }

        if (rb != null)
        {
            rb.isKinematic = false;
            rb.useGravity = true;
            rb.constraints = RigidbodyConstraints.None;
        }
        
        var rbChildren = GetComponentsInChildren<Rigidbody>(true);
        foreach (var childRb in rbChildren)
        {
            childRb.isKinematic = false;
            childRb.useGravity = true;
        }
        
        


        Debug.Log("rot constraints should be gone");
        yield return new WaitForSeconds(bodyLife);
        Debug.Log("EntDie called from DieRagdoll");
        EntDie();
    }

    public void EntDie()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
    
    public void EntDespawn()
    {
        Debug.Log($"{gameObject.name} has despawned.");
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FallTrigger")
        {
            EntDie();
        }
    }
}
