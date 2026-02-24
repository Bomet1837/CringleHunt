using System.Collections;
using TMPro;
using Unity.Entities;
using UnityEditor;
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

    [Header("Graphics Settings")] 
    public float bulletImpactDuration = 10f;
    public LayerMask bulletImpactLayers;


    [Header("References")]
    [SerializeField] private Transform _gunTransform;
    [SerializeField] public TextMeshProUGUI ammoDisplay;
    [SerializeField] public TextMeshProUGUI gunNameDisplay;
    private WeaponModule _weaponModule;
    [SerializeField] private TrailRenderer _bulletTracer;
    [SerializeField] private ParticleSystem _muzzleFlash;
    [SerializeField] private SpriteRenderer _bulletImpact;

    private void Awake()
    {
        //fallbacks for future variations
        if (_bulletTracer == null) _bulletTracer = AssetDatabase.LoadAssetAtPath("Assets/Visual Effects/Weapon FX/Bullet Tracers/BulletTracerDefault.prefab", typeof(TrailRenderer)) as TrailRenderer;
        if (_muzzleFlash == null) _muzzleFlash = AssetDatabase.LoadAssetAtPath("Assets/Visual Effects/Weapon FX/Muzzle Flashes/MuzzleFlashDefault.prefab", typeof(ParticleSystem)) as ParticleSystem;
        if (_bulletImpact == null) _bulletImpact = AssetDatabase.LoadAssetAtPath("Assets/Visual Effects/Weapon FX/Impact Marks/BulletImpactDefault.prefab", typeof(SpriteRenderer)) as SpriteRenderer;
        
        // bulletImpactLayers = LayerMask.GetMask("Ground", "upperCollision");
        
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
        Vector3 baseDirection;

        if (_heldByPlayer && Camera.main != null)
        {
            Ray camRay = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
            baseDirection = camRay.direction;
        }
        else
        {
            baseDirection = _gunTransform.forward;
        }

        if (spread <= 0f)
        {
            return baseDirection.normalized;
        }

        Vector2 rndCircle = Random.insideUnitCircle;
        float x = rndCircle.x * spread;
        float y = rndCircle.y * spread;

        Vector3 right = Vector3.Cross(Vector3.up, baseDirection).normalized;
        if (right.sqrMagnitude < 0.001f) right = _gunTransform.right;
        Quaternion rot = Quaternion.AngleAxis(x, Vector3.up) * Quaternion.AngleAxis(y, Vector3.right);

        Vector3 finalDirection = rot * baseDirection;
        return finalDirection.normalized;
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
            // StartCoroutine(DrawMuzzleFlash()); current muzzle flash placeholder is AWFUL, will add back later but works
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
                    TrailRenderer tracer = Instantiate(_bulletTracer, gunRay.origin, Quaternion.identity);
                    StartCoroutine(DrawBulletTracer(tracer, gunRay.direction));
                  //  Debug.DrawRay(gunRay.origin, gunRay.direction * range, _heldByPlayer ? Color.red : Color.orange, 1f);
                    
                    if (Physics.Raycast(gunRay, out RaycastHit hitInfo, range))
                    {
                        DetermineHitRecipient(gunRay, hitInfo);
                        
                        if (_bulletTracer != null)
                            if (Physics.Raycast(gunRay, out RaycastHit tracerHit, range, bulletImpactLayers)) 
                                StartCoroutine(DrawBulletImpact(hitInfo.point, hitInfo.normal));
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
    
    private IEnumerator DrawBulletImpact(Vector3 position, Vector3 normal)
    {
        SpriteRenderer impact = Instantiate(_bulletImpact, position, Quaternion.LookRotation(normal));
        yield return new WaitForSeconds(bulletImpactDuration);
        Destroy(impact.gameObject);
    }

    private IEnumerator DrawMuzzleFlash()
    {
            ParticleSystem flash = Instantiate(_muzzleFlash, _gunTransform.position, _gunTransform.rotation);
            flash.Play();
            yield return new WaitForSeconds(flash.main.duration + 0.2f);
            Destroy(flash.gameObject);
    }

    private IEnumerator DrawBulletTracer(TrailRenderer tracer, Vector3 direction)
    {
        float time = 0;
        Vector3 startPos = tracer.transform.position;
        Vector3 endPos = startPos + direction.normalized * range;

        while (time < 1)
        {
            tracer.transform.position = Vector3.Lerp(startPos, endPos, time);
            time += Time.deltaTime / tracer.time;

            yield return null;
        }
        tracer.transform.position = endPos;
        
        Destroy(tracer.gameObject, tracer.time + 0.1f);
    }
    
    private void DetermineHitRecipient(Ray gunRay, RaycastHit hitInfo)
    {
        var enemy = hitInfo.collider.GetComponentInParent<NPCEntity>();
        var player = hitInfo.collider.GetComponentInParent<Controller>();
            if (enemy != null && _heldByPlayer)
            {
                enemy.Health -= CalculateDamage(hitInfo.distance);
                if (enemy.rb.constraints == RigidbodyConstraints.None)
                {
                    Debug.Log("Applying force to ragdoll");
                    enemy.rb.AddForceAtPosition(gunRay.direction * 500f, hitInfo.point);
                }
            }
            else if (player != null && !_heldByPlayer)
            {
                player.Health -= CalculateDamage(hitInfo.distance);
                if (player.rb.constraints == RigidbodyConstraints.None)
                {
                    Debug.Log("Applying force to player ragdoll");
                    player.rb.AddForceAtPosition(gunRay.direction * 500f, hitInfo.point);
                }
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
