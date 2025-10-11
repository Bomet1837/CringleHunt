using UnityEngine;

public class Entity : MonoBehaviour
{
    public float startingHealth;
    private float _health;
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
                EntDie();
            }
        }
    }

    void Start()
    {
        health = startingHealth;
    }

    public void EntDie()
    {
        Debug.Log($"{gameObject.name} has died.");
        Destroy(gameObject);
    }
}
