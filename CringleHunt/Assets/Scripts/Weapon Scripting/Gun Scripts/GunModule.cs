using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GunModule : MonoBehaviour
{
    [Header("Gun Properties")]
    public string gunName;
    public string gunDescription;
    public string ammoType;
    public float damage;
    public float range;
    public float spread;
    public int ammoCapacity;
    public int bulletQtyPerShot;
    [HideInInspector] public int maxAmmoCapacity;


    [Header("References")]
    [SerializeField]
    private Transform _gunTransform;
    public Collider playerCollider;
    public TextMeshProUGUI ammoDisplay;
    public TextMeshProUGUI gunNameDisplay;

    private void Awake()
    {
        maxAmmoCapacity = ammoCapacity;
        if (ammoDisplay == null)
        {
            GameObject ammoDisplayObj = GameObject.Find("GunAmmoCount");
            if (ammoDisplayObj != null)
            {
                ammoDisplay = ammoDisplayObj.GetComponent<TextMeshProUGUI>();
            }
        }
        if (gunNameDisplay == null)
        {
            GameObject gunNameDisplayObj = GameObject.Find("GunName");
            if (gunNameDisplayObj != null)
            {
                gunNameDisplay = gunNameDisplayObj.GetComponent<TextMeshProUGUI>();
            }
        }
        _gunTransform = this.gameObject.transform;
    }

    private void Update()
    {
        if (ammoDisplay != null)
        {
            ammoDisplay.SetText(ammoCapacity.ToString());
        }
        if (gunNameDisplay != null)
        {
            gunNameDisplay.SetText(gunName);
        }
    }

    public Vector3 GetShootDirection()
    {
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 direction = _gunTransform.forward + new Vector3(x, y, 0);
        direction.Normalize();
        return direction;
    }

    public float CalculateDamage(float distance)
    {
        float damageFalloffStart = range * 0.3f; // Start falloff after 30% of range
        float damageFalloffEnd = range; // End falloff at max range

        if (distance <= damageFalloffStart)
        {
            return damage; // Full damage within the start range
        }
        else if (distance >= damageFalloffEnd)
        {
            return damage * 0.5f; // Minimum damage at max range
        }
        else
        {
            // Linear interpolation between full damage and minimum damage
            float falloffFactor = (distance - damageFalloffStart) / (damageFalloffEnd - damageFalloffStart);
            return Mathf.Lerp(damage, damage * 0.5f, falloffFactor);
        }
    }

    public void Shoot()
    {
        if (ammoCapacity > 0 && bulletQtyPerShot > 0)
        {
            for (int i = 0; i < bulletQtyPerShot; i++)
            {
                Ray gunRay = new Ray(_gunTransform.position, GetShootDirection());
                Debug.DrawRay(gunRay.origin, gunRay.direction * range, Color.red, 1f);
                if (Physics.Raycast(gunRay, out RaycastHit hitInfo, range))
                {
                    if (hitInfo.collider.gameObject.TryGetComponent(out NPCEntity enemy))
                    {
                        enemy.health -= CalculateDamage(hitInfo.distance);
                        if (enemy.rb.constraints == RigidbodyConstraints.None)
                        {
                            Debug.Log("Applying force to ragdoll");
                            enemy.rb.AddForceAtPosition(gunRay.direction * 500f, hitInfo.point);
                        }
                    }
                }
            }
            ammoCapacity--;
        }
        else if (ammoCapacity <= 0 && bulletQtyPerShot > 0)
        {
            Debug.Log("Out of Ammo!");
        }
        else
        {
            Debug.Log("No bullets per shot defined!");
        }
    }

    private void OnDrawGizmos()
    {
        if (_gunTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(_gunTransform.position, _gunTransform.forward * range);
        }

        Gizmos.DrawWireSphere(_gunTransform.position, spread);
    }

}
