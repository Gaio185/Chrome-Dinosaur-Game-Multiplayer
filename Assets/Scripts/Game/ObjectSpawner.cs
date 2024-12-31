using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ObjectSpawner : MonoBehaviour
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
        Invoke(nameof(Spawn), Random.Range(minSpawnRate, maxSpawnRate));
    }

    private void Spawn()
    {
        float spawnchance = Random.value;

        foreach (var obj in gameObjects)
        {
            if(spawnchance < obj.spawnChance)
            {
                GameObject obstacle = Instantiate(obj.prefab);
                obstacle.transform.position += transform.position;
                break;
            }

            spawnchance -= obj.spawnChance;
        }

        Invoke(nameof(Spawn), Random.Range(minSpawnRate,maxSpawnRate));
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void Update()
    {
        
    }

}
