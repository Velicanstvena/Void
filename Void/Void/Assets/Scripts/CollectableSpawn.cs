using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectableSpawn : MonoBehaviour
{
    [SerializeField] GameObject[] collectableToSpawn;
    private ObjectPooler objectPooler;

    private float minSpawnTime = 5f;
    private float maxSpawnTime = 15f;

    private bool spawned = false;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        StartCoroutine(SpawnCollectables());
    }

    private IEnumerator SpawnCollectables()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));
            GameObject newCollectable;

            if (!spawned)
            {
                newCollectable = objectPooler.SpawnFromPool(collectableToSpawn[Random.Range(0, collectableToSpawn.Length)].name, transform.position);
                newCollectable.transform.parent = gameObject.transform;
                spawned = true;
            }
        }
    }
}
