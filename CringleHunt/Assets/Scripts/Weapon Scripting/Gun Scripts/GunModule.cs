using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public enum GunPhysType { Hitscan, Projectile }

public class GunModule : MonoBehaviour
{
    [Header("Gun Properties")]
    public string gunName;
    public string gunDescription;
    public string ammoType;
    public GunPhysType gunPhysType;
    public float damage;
    public float range;
    public float spread;
    public float projVelocity;
    public int ammoCapacity;
    public int bulletQtyPerShot;
    [HideInInspector] public int maxAmmoCapacity;
    private bool _heldByPlayer;


    [Header("References")]
    [SerializeField] private Transform _gunTransform;
    [SerializeField] public TextMeshProUGUI ammoDisplay;
    [SerializeField] public TextMeshProUGUI gunNameDisplay;
    private WeaponModule _weaponModule;

    private void Awake()
    {
        maxAmmoCapacity = ammoCapacity;
        
        _weaponModule = GetComponent<WeaponModule>();
        if (_weaponModule == null)
        {
            Debug.LogError("WeaponModule component not found on " + gameObject.name);
        }
        else
        {
            _heldByPlayer = _weaponModule.heldByPlayer;
        }

        if (_heldByPlayer)
        {
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
        }
        else if (!_heldByPlayer)
        {
            ammoDisplay = null;
            gunNameDisplay = null;
        }

        
            
        _gunTransform = gameObject.transform;            
    }

    private void Update()
    {
        if (_heldByPlayer)
        {
            AmmoDisplay();
        }

        if (!_heldByPlayer)
        {
            ammoCapacity = maxAmmoCapacity; // NPC guns have infinite ammo 
        }
    }

    private void AmmoDisplay()
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
                if (gunPhysType == GunPhysType.Projectile)
                {
                    
                    // Implement projectile instantiation and behaviour here
                    Debug.Log("Projectile shooting not implemented yet.");
                    continue;
                }
                
                if (gunPhysType == GunPhysType.Hitscan)
                {
                    Ray gunRay = new Ray(_gunTransform.position, GetShootDirection());
                    Debug.DrawRay(gunRay.origin, gunRay.direction * range, _heldByPlayer ? Color.red : Color.orange, 1f);
                    if (Physics.Raycast(gunRay, out RaycastHit hitInfo, range))
                    {
                        DetermineHitRecipient(gunRay, hitInfo);
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
            Debug.LogWarning("No bullets per shot defined!");
        }
    }
    
    private void DetermineHitRecipient(Ray gunRay, RaycastHit hitInfo)
    {
            if (hitInfo.collider.gameObject.TryGetComponent(out NPCEntity enemy) && _heldByPlayer)
            {
                enemy.Health -= CalculateDamage(hitInfo.distance);
                if (enemy.rb.constraints == RigidbodyConstraints.None)
                {
                    Debug.Log("Applying force to ragdoll");
                    enemy.rb.AddForceAtPosition(gunRay.direction * 500f, hitInfo.point);
                }
            }
            else if (hitInfo.collider.gameObject.TryGetComponent(out Controller player) && !_heldByPlayer)
            {
                player.Health -= CalculateDamage(hitInfo.distance);
                if (player.rb.constraints == RigidbodyConstraints.None)
                {
                    Debug.Log("Applying force to player ragdoll");
                    player.rb.AddForceAtPosition(gunRay.direction * 500f, hitInfo.point);
                }
                // Implement player damage logic here
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
