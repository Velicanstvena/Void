using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatformType : MonoBehaviour
{
    [SerializeField] GameObject[] platformsToSpawn;
    private float spawnTime = 5f;

    void Start()
    {
        StartCoroutine(SpawnPlatform());
    }

    private IEnumerator SpawnPlatform()
    {
        while (true)
        {
            GameObject newPlatform = ObjectPooler.Instance.SpawnFromPool(platformsToSpawn[Random.Range(0, platformsToSpawn.Length)].name, transform.position);
            newPlatform.GetComponent<PlatformMovement>().SetPos(transform.position);
            yield return new WaitForSeconds(spawnTime);
        }
    }
}
