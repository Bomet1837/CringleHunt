using System.Collections;
using UnityEngine;

public class NPCEntity : MonoBehaviour
{

    [HideInInspector] public Rigidbody rb;
    public string entityName;
    public float bodyLife = 5f;
    public float startingHealth;
    public float health
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

    void Awake()
    {
        entityName = gameObject.name;
    }
    

    void Start()
    {
        health = startingHealth;
        rb = GetComponent<Rigidbody>();
    }

    IEnumerator DieRagdoll()
    {
        rb.constraints = RigidbodyConstraints.None;
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

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "FallTrigger")
        {
            EntDie();
        }
    }
}
