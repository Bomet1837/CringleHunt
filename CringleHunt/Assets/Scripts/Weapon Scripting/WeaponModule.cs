using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class WeaponModule : MonoBehaviour
{
    [Header("Weapon Module - Event Hooks")]
    public UnityEvent onAttack;

    private UnityEvent[] _eventHooks = new UnityEvent[] { };

    [Header("Weapon Properties")]
    public float atkCooldown;
    public bool isAutomatic;
    public bool isMelee;
    public bool heldByPlayer;
    
    [Header("NPC Attack Settings")]
    public float npcAttackInterval;

    [SerializeField] private int npcShotsPerBurst = 3;
    [SerializeField] private float npcBurstInterval = 2.5f;

    private float _currentCooldown;
    [HideInInspector] public bool isAttacking;
    private BFw_Detection _detectionScript;
    private BFw_Combat _combatScript;

    private void Awake()
    {
        _eventHooks = new UnityEvent[] {onAttack};

        foreach (var evhook in _eventHooks)
        {
            if (evhook == null)
            {
                Debug.LogWarning("One of the event hooks is not assigned in " + gameObject.name + ". May be intentional.");
            }
        }
        
        if (!heldByPlayer)
        {
                _detectionScript = GetComponentInParent<BFw_Detection>();
                _combatScript = GetComponent<BFw_Combat>();
                if (_detectionScript == null)
                {
                    Debug.LogError("BFw_Detection component not found in parent of " + gameObject.name);
                    Debug.LogError("BFw_Combat component not found in " + gameObject.name);
                } 
        }
        else
        {
            _detectionScript = null;
            _combatScript = null;
        }
    }

    void Start()
    {
        _currentCooldown = atkCooldown;
    }

    void Update()
    {
        foreach (var evhook in _eventHooks)
        {
            switch (evhook)
            {
                case var _ when evhook == onAttack:
                    switch (heldByPlayer)
                    {
                        case true:
                            HandlePlayerAttack(evhook);
                            break;
                        
                        case false:
                            StartCoroutine(HandleNpcAttack(evhook));
                            break;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        _currentCooldown -= Time.deltaTime;
    }

    private void HandlePlayerAttack(UnityEvent evHook)
    {
        if (isAutomatic)
        {
            if (Input.GetKey(KeyCode.Mouse0) && _currentCooldown <= 0)
            {
                evHook.Invoke();
                _currentCooldown = atkCooldown;
                isAttacking = true;
            }
            else { isAttacking = false; }
        }
        else
        {
            if (Input.GetKeyDown(KeyCode.Mouse0) && _currentCooldown <= 0)
            {
                evHook.Invoke();
                _currentCooldown = atkCooldown;
                isAttacking = true;
            }
            else { isAttacking = false; }
        }
    }

    private IEnumerator HandleNpcAttack(UnityEvent evHook)
    {
        if (_currentCooldown <= 0 && _detectionScript.isPlayerDetected)
        {
            int shotsFired = 0;
            isAttacking = true;

            for (int i = 0; i < Mathf.Max(1, npcShotsPerBurst); i++)
            {
                if (!_detectionScript.isPlayerDetected)
                {
                    break;
                }
                
                evHook?.Invoke();
                shotsFired++;
                
                if (i < npcShotsPerBurst - 1)
                {
                    yield return new WaitForSeconds(Mathf.Max(0, npcBurstInterval));
                }
            }
            
            _currentCooldown = atkCooldown;
            isAttacking = shotsFired > 0;
        }
        else
        {
            isAttacking = false;
        }
        yield return new WaitForSeconds(Mathf.Max(0, npcAttackInterval));
    }
    
}

    
