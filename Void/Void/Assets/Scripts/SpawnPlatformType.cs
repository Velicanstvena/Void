using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatformType : MonoBehaviour
{
    [SerializeField] GameObject[] platformsToSpawn;
    private ObjectPooler objectPooler;

    // Start is called before the first frame update
    void Start()
    {
        objectPooler = ObjectPooler.Instance;
        StartCoroutine(SpawnPlatform());
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator SpawnPlatform()
    {
        while (true)
        {
            //GameObject newPlatform = Instantiate(platformsToSpawn[Random.Range(0, platformsToSpawn.Length)], transform.position, Quaternion.identity);
            GameObject newPlatform = objectPooler.SpawnFromPool(platformsToSpawn[Random.Range(0, platformsToSpawn.Length)].name, transform.position);
            yield return new WaitForSeconds(5);
        }
    }
}
