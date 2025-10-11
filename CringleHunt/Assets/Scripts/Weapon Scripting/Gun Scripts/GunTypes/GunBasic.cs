using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
public class GunBasic : MonoBehaviour
{
    [Header("Gun Properties")]
    public string gunName;
    public int damage;
    public float fireRate, spread, range, reloadTime;
    public int magSize, bulletsPerTap;
    int bulletsLeft, bulletsShot;

   [SerializeField] bool shooting, readyToShoot, reloading;

    [Header("References")]
    public TextMeshProUGUI ammoDisplay;
    private Transform playerCamera;
    [SerializeField] private Transform gunTransform;
    private WeaponModule thisWeaponModule;

    private void Awake()
    {
        playerCamera = Camera.main.transform;
        gunTransform = this.gameObject.transform;
        thisWeaponModule = this.gameObject.GetComponent<WeaponModule>();
        bulletsLeft = magSize;
        readyToShoot = true;

    }

    private void Update()
    {
        if (ammoDisplay != null) ammoDisplay.SetText(bulletsLeft + " / " + magSize);
        shooting = thisWeaponModule.isAttacking;


    }

    public void ShootInput()
    {
        if (readyToShoot && shooting && !reloading && bulletsLeft > 0)
        {
            bulletsShot = bulletsPerTap;
            Shoot();
        }
    }

    public void ReloadInput()
    {
        if (bulletsLeft < magSize && !reloading) Reload();
    }

    public void Shoot()
    {
        float x = Random.Range(-spread, spread);
        float y = Random.Range(-spread, spread);
        Vector3 direction = gunTransform.forward + new Vector3(x, y, 0);
        direction.Normalize();

        readyToShoot = false;

        Ray gunRay = new Ray(gunTransform.position, direction);
        if (Physics.Raycast(gunRay, out RaycastHit hitInfo, range))
        {
            if (hitInfo.collider.gameObject.TryGetComponent(out Entity enemy) && hitInfo.collider.gameObject.layer == LayerMask.NameToLayer("Enemy"))
            {
                enemy.health -= damage;
            }
        }

        bulletsLeft--;
        Invoke("ResetShot", fireRate);

        if (bulletsShot > 0 && bulletsLeft > 0) Invoke("Shoot", thisWeaponModule.atkCooldown);
    }

    private void ResetShot()
    {
        readyToShoot = true;
    }

    public void Reload()
    {
        Invoke("ReloadFinished", reloadTime);
        reloading = true;
    }

    private void ReloadFinished()
    {
        bulletsLeft = magSize;
        reloading = false;
    }

    private void OnDrawGizmos()
    {
        if (gunTransform != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(gunTransform.position, gunTransform.forward * range);
        }

        Gizmos.DrawWireSphere(gunTransform.position, spread);

        

    }


}
