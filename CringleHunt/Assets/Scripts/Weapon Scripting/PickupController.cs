using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PickupController : MonoBehaviour
{
    public GunModule gmScript;
    public WeaponModule weapModule;
    public Rigidbody rb;
    public BoxCollider coll;
    public Transform player, gunContainer, fpsCam;
    public float pickUpRange;
    public float dropForwardForce, dropUpwardForce;
    public bool equipped;
    public static bool slotFull = false;

    public int slotIndex = 0;
    public static bool[] invSlots = new bool[5];
    public static int currentSlot = 0;
    public static int maxSlots = 5;

    [SerializeField] private GameObject _floatingText;

    private void Awake()
    {
        _floatingText = transform.Find("FloatingNameDisp").gameObject;
        if (!_floatingText)
        {
            Debug.LogError("Floating text object not found!");
        }

        if (!equipped)
        {
            gmScript.enabled = false;
            weapModule.enabled = false;
            rb.isKinematic = false;
            coll.isTrigger = false;
            _floatingText.SetActive(true);
        }

        if (equipped)
        {
            gmScript.enabled = true;
            weapModule.enabled = true;
            rb.isKinematic = true;
            coll.isTrigger = true;
            _floatingText.SetActive(false);
            slotFull = true;
        }
    }

    private void Update()
    {

        Vector3 distanceToPlayer = player.position - transform.position;
        if (!equipped && distanceToPlayer.magnitude <= pickUpRange && Input.GetKeyDown(KeyCode.E) && slotFull == false)
        {
            Pickup();
        }

        if (equipped && Input.GetKeyDown(KeyCode.Q))
        {
            Drop();
        }
    }

    private void Pickup()
    {

        equipped = true;
        slotFull = true;
        _floatingText.SetActive(false);

        transform.SetParent(gunContainer);
        transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.Euler(Vector3.zero);
        transform.localScale = Vector3.one;

        rb.isKinematic = true;
        coll.isTrigger = true;

        gmScript.enabled = true;
        weapModule.enabled = true;

    }

    private void Drop()
    {
        equipped = false;
        slotFull = false;
        _floatingText.SetActive(true);

        transform.SetParent(null);

        rb.isKinematic = false;
        coll.isTrigger = false;

        rb.linearVelocity = player.GetComponent<Rigidbody>().linearVelocity;
        rb.AddForce(fpsCam.forward * dropForwardForce, ForceMode.Impulse);
        rb.AddForce(fpsCam.up * dropUpwardForce, ForceMode.Impulse);
        rb.AddTorque(new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(-1, 1)) * 10);

        gmScript.gunNameDisplay.SetText("No Gun");
        gmScript.ammoDisplay.SetText("∞");
        gmScript.enabled = false;
        weapModule.enabled = false;

    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickUpRange);
    }
}
