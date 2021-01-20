using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatformType : MonoBehaviour
{
    [SerializeField] GameObject[] platformsToSpawn;
    private ObjectPooler objectPooler;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        StartCoroutine(SpawnPlatform());
    }

    void Update()
    {
        
    }

    private IEnumerator SpawnPlatform()
    {
        while (true)
        {
            //GameObject newPlatform = Instantiate(platformsToSpawn[Random.Range(0, platformsToSpawn.Length)], transform.position, Quaternion.identity);
            GameObject newPlatform = objectPooler.SpawnFromPool(platformsToSpawn[Random.Range(0, platformsToSpawn.Length)].name, transform.position);
            newPlatform.GetComponent<PlatformMovement>().SetPos(transform.position);
            yield return new WaitForSeconds(5);
        }
    }
}
