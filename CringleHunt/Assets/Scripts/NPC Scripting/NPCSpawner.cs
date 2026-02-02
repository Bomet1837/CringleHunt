using System;
using System.Collections.Generic;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using Random = UnityEngine.Random;

public class NPCSpawner : MonoBehaviour
{
    public GameObject npcPrefab;
    public int npcQty = 1;
    public int npcQtyLimit = 1;
    public float radius = 5f;
    public float spawnInterval = 2f;
    
    private float _timeSinceLastSpawn = 0f;
    [SerializeField] private int _npcSpawnedQty;
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
        _spawnedNpcs = FindObjectsByType<NPCEntity>(FindObjectsSortMode.None).Select(npc => npcPrefab).ToList();
    }

    public void Spawn()
    {
        _spawnedNpcs.RemoveAll(x => !x); // Clean up destroyed NPCs
        Vector3 spawnVector = (transform.position * Random.Range(0f, radius));

        for(int i = 0; i < npcQty && i < npcQtyLimit; i++)
        {
            Instantiate(npcPrefab, spawnVector, new Quaternion(0, Random.value, 0, 1));
        }
    }
    
    
}
