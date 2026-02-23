using UnityEngine;

public class AmmunitionModule : MonoBehaviour
{
    public string ammoType;
    public int ammoQty;

    private void OnCollisionEnter(Collision other)
    {
        GunModule gunModule = other.gameObject.GetComponentInChildren<GunModule>();
        if (other.gameObject.CompareTag("Player") && gunModule != null)
        {
            AddAmmo(gunModule, ammoType, ammoQty);
            Debug.Log("Picked up " + ammoType + " ammo.");
        }
        else if (other.gameObject.CompareTag("Player") && !gunModule)
        {
            Debug.Log("Player collided with ammo pickup, but no compatible gun module found.");
        }
    }

    public void AddAmmo(GunModule gm, string type, int qty)
    {
        if (type == ammoType)
        {
            gm.ammoCapacity += qty;
            if (gm.ammoCapacity > gm.maxAmmoCapacity)
            {
                gm.ammoCapacity = gm.maxAmmoCapacity;
            }
            Destroy(gameObject);
            Debug.Log("Picked up " + qty + " " + type + " ammo.");
        }
        else
        {
            Debug.Log("Incompatible ammo type.");
        }
    }
}
