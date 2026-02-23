using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;


/// <summary>
/// Spawns NPCs at a specified location with configurable frequency and quantity.
/// </summary>
public class NPCSpawner : MonoBehaviour
{
    [Header("NPC Spawner Settings")]
    public GameObject npcPrefab;
    public int npcQty = 1;
    public int npcQtyLimit = 1;
    public float radius = 5f;
    public float spawnInterval = 2f;

    [Header("Weapon Settings")] 
    public GameObject weaponPrefab;
    public bool isArmed;
    
    private float _timeSinceLastSpawn = 0f;
    private int _npcSpawnedQty;
    private List<GameObject>  _spawnedNpcs = new List<GameObject>();

    public void Awake()
    {
        UpdateNpcs();
    }

    public void FixedUpdate()
    {
        UpdateNpcs();
        _npcSpawnedQty = _spawnedNpcs.Count;
    }
    
    public void Update()
    {
        _timeSinceLastSpawn += Time.deltaTime;

        if (_timeSinceLastSpawn >= spawnInterval && _npcSpawnedQty < npcQtyLimit)
        {
            Spawn();
            _timeSinceLastSpawn = 0f;
        }
    }
    
    public void UpdateNpcs()
    {
        _spawnedNpcs = FindObjectsByType<NPCEntity>(FindObjectsSortMode.None).Select(npc => npc.gameObject).ToList();
    }

    public void Spawn()
    {
        _spawnedNpcs.RemoveAll(x => !x); // Clean up destroyed NPCs
        
        Vector3 spawnVector = (transform.position * Random.Range(0f, radius));

        for(int i = 0; i < npcQty && _spawnedNpcs.Count < npcQtyLimit; i++)
        {
            Vector2 circle = Random.insideUnitCircle * radius;
            Vector3 spawnVector2 = transform.position + new Vector3(circle.x, 0, circle.y);

            float randomYaw = Random.Range(0f, 360f);
            Quaternion npcRotation = Quaternion.Euler(0f, randomYaw, 0f);
            
            GameObject newNpc = Instantiate(npcPrefab, spawnVector2, npcRotation);
            _spawnedNpcs.Add(newNpc);
            
            if(isArmed && weaponPrefab != null)
            {
                Transform gunContainer = newNpc.transform .Find("GunContainer");
                Quaternion gunRotation = Quaternion.LookRotation(newNpc.transform.forward, Vector3.up);
                
                Transform parent = gunContainer != null ? gunContainer : newNpc.transform;
                Vector3 weaponSpawnPos = parent.position;
                
                GameObject newWeapon = Instantiate(weaponPrefab, weaponSpawnPos, gunRotation, parent);
                newWeapon.transform.SetParent(parent, false);
            }
            else
                Instantiate(npcPrefab, spawnVector, new Quaternion(0, Random.value, 0, 1));
        }
    }
    
    
}
