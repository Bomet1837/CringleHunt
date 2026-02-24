using System.Collections;
using UnityEngine;

public class BFw_Combat : NPCBehaviourFwk
{
    [Header("Combat Settings")]
    public float attackCooldown = 1f;
    public LayerMask targetLayers;
    public LayerMask friendlyLayers;

     public bool friendlyFire = false;
    
    private BFw_Detection _detectionScript;
    private GameObject _npcWeapon;
    private Vector3 _lastAimDir = Vector3.zero;
    
    
    private GameObject FindChildGameObjectByPrefix(Transform parent, string prefix)
    {
        if (parent == null || string.IsNullOrEmpty(prefix)) return null;

        // Use GetComponentsInChildren to search recursively and include inactive objects
        foreach (Transform t in parent.GetComponentsInChildren<Transform>(true))
        {
            if (t == parent) continue; // skip the parent itself
            if (t.name.StartsWith(prefix, System.StringComparison.OrdinalIgnoreCase))
            {
                return t.gameObject;
            }
        }

        return null;
    }
    
    private new void Awake()
    {
        base.Awake();
        _detectionScript = GetComponent<BFw_Detection>();
        if (_detectionScript == null)
        {
            Debug.LogError("BFw_Detection component not found on " + gameObject.name);
        }

        // Try to find an NPC weapon in children by prefix (handles variations like "NPCGun_SMG(Clone)")
        _npcWeapon = FindChildGameObjectByPrefix(transform.Find("GunContainer"), "NPCGun");

        if (_npcWeapon == null)
        {
            // As a fallback try finding any object with a "Gun" prefix
            _npcWeapon = FindChildGameObjectByPrefix(transform, "Gun");
        }

        if (_npcWeapon == null)
        {
            Debug.LogError("GunModule component not found as child of " + gameObject.name);
        }
        
    }

    public void AimAtTarget(Transform target)
    {
        if (target == null || _npcWeapon == null) return;

        if (target != null && _npcWeapon != null)
        {

            Vector3 directionToTarget = target.position - _npcWeapon.transform.position;
            _lastAimDir = directionToTarget;

            if (directionToTarget.sqrMagnitude > 0.01f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);
                _npcWeapon.transform.rotation =
                    Quaternion.Slerp(_npcWeapon.transform.rotation, targetRotation,
                        Time.deltaTime * 5f);
            }
            
            Ray npcGunRay = new Ray(_npcWeapon.transform.position, directionToTarget.normalized);
            //Debug.DrawRay(npcGunRay.origin, npcGunRay.direction * directionToTarget.magnitude, friendlyFire ? Color.green : Color.magenta);
            if (Physics.Raycast(npcGunRay, out RaycastHit hitInfo, directionToTarget.magnitude))
            {
                if (hitInfo.collider.gameObject.layer == targetLayers && !friendlyFire)
                {
                    friendlyFire = false;
                    StartCoroutine(Attack());
                } 
                else if (hitInfo.collider.gameObject.layer == friendlyLayers)
                {
                    Debug.Log("Friendly fire will not be tolerated! Ceasing fire and re-aiming...");
                    friendlyFire = true;
                    _lastAimDir = hitInfo.normal;
                }
                else
                {
                    friendlyFire = false;
                }
            }
            
            /*Debug.DrawRay(_npcWeapon.transform.position, directionToTarget.normalized * 10f,
                Color.magenta, 0.1f);*/
        }
    }
    
    IEnumerator Attack()
    {
        Debug.Log("NPC Attacks!");
        WaitForSeconds wait = new WaitForSeconds(attackCooldown);
        // Implement attack logic here (e.g., reduce player health, play attack animation, etc.)
        yield return wait;
    }
    
    public void OnDeath()
    {
        Debug.Log("NPC has died!");
        // Implement death logic here (e.g., play death animation, disable NPC, etc.)
    }
}
