using Mirror;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawner : NetworkBehaviour
{
    [System.Serializable]
    public struct SpawnableObject
    {
        public GameObject prefab;
        [Range(0f, 1f)] 
        public float spawnChance;
    }

    public SpawnableObject[] gameObjects;
    public Transform despawnLimit;

    public float minSpawnRate = 1f;
    public float maxSpawnRate = 2f;

    private void OnEnable()
    {
        if (isServer)
        {
            Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
        }
    }

    private void Spawn()
    {
        Debug.Log("Spawning object...");

        float spawnchance = Random.value;

        foreach (var obj in gameObjects)
        {
            if(spawnchance < obj.spawnChance)
            {
                GameObject obstacle = Instantiate(obj.prefab, transform.position, transform.rotation);
                NetworkServer.Spawn(obstacle);
                break;
            }

            if(minSpawnRate >= 0.2f && maxSpawnRate >= 0.5f)
            {
                minSpawnRate -= 0.01f;
                maxSpawnRate -= 0.01f;
            }
            

            spawnchance -= obj.spawnChance;
        }
        Invoke(nameof(Spawn), Random.Range(minSpawnRate,maxSpawnRate));
    }

    private void OnDisable()
    {
        CancelInvoke();
    }


}
