using System;
using UnityEngine;
using UnityEngine.Events;

public class WeaponModule : MonoBehaviour
{
    [Header("Weapon Module - Event Hooks")]
    public UnityEvent onAttack;
    public UnityEvent onReload;
    public UnityEvent onEquip;
    public UnityEvent onUnequip;

    UnityEvent[] _eventHooks = new UnityEvent[] { };

    [Header("Weapon Properties")]
    public float atkCooldown;
    public bool isAutomatic;

    private float currentCooldown;
    [HideInInspector] public bool isAttacking;

    void Awake()
    {
        _eventHooks = new UnityEvent[] { onAttack, onReload, onEquip, onUnequip };
    }

    void Start()
    {
        currentCooldown = atkCooldown;
    }

    void Update()
    {
        foreach (var evhook in _eventHooks)
        {
            switch (evhook)
            {
                case var _ when evhook == null:
                    Debug.LogWarning("One of the event hooks is not assigned in " + gameObject.name + ". May be intentional.");
                    break;

                case var _ when evhook == onAttack:
                    if (isAutomatic)
                    {
                        if (Input.GetKey(KeyCode.Mouse0) && currentCooldown <= 0)
                        {
                            evhook.Invoke();
                            currentCooldown = atkCooldown;
                            isAttacking = true;
                        }
                        else { isAttacking = false; }
                    }
                    else
                    {
                        if (Input.GetKeyDown(KeyCode.Mouse0) && currentCooldown <= 0)
                        {
                            evhook.Invoke();
                            currentCooldown = atkCooldown;
                            isAttacking = true;
                        }
                        else { isAttacking = false; }
                    }
                    break;
                case var _ when evhook == onReload:
                    if (Input.GetKeyDown(KeyCode.R))
                    {
                        evhook.Invoke();
                    }
                    break;
    
                case var _ when evhook == onEquip:
                    // Placeholder for equip logic
                    break;
                case var _ when evhook == onUnequip:
                    // Placeholder for unequip logic
                    break;

                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        currentCooldown -= Time.deltaTime;
    }
}
