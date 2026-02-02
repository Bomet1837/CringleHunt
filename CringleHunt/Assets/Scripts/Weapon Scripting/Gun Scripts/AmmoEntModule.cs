using UnityEngine;

public class AmmunitionModule : MonoBehaviour
{
    public string ammoType;
    public int ammoQty;

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<GunModule>() != null)
        {
            AddAmmo(gunModule, ammoType, ammoQty);
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
