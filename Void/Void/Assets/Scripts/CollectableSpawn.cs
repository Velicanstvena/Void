using System.Collections;
using UnityEngine;

public class CollectableSpawn : MonoBehaviour
{
    [SerializeField] GameObject[] collectableToSpawn;

    private float minSpawnTime = 1f;
    private float maxSpawnTime = 5f;

    private bool spawned = false;

    void Start()
    {
        StartCoroutine(SpawnCollectables());
    }

    private IEnumerator SpawnCollectables()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            if (!spawned)
            {
                GameObject newCollectable = ObjectPooler.Instance.SpawnFromPool(collectableToSpawn[Random.Range(0, collectableToSpawn.Length)].name, transform.position);
                newCollectable.transform.parent = gameObject.transform;
                spawned = true;
            }
        }
    }
}
