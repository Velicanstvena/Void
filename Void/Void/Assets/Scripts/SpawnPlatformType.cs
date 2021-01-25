using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatformType : MonoBehaviour
{
    [SerializeField] GameObject[] platformsToSpawn;
    private ObjectPooler objectPooler;
    private float spawnTime = 5f;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        StartCoroutine(SpawnPlatform());
    }

    private IEnumerator SpawnPlatform()
    {
        while (true)
        {
            GameObject newPlatform = objectPooler.SpawnFromPool(platformsToSpawn[Random.Range(0, platformsToSpawn.Length)].name, transform.position);
            newPlatform.GetComponent<PlatformMovement>().SetPos(transform.position);
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
