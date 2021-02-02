using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnPlatformType : MonoBehaviour
{
    [SerializeField] GameObject[] platformsToSpawn;
    private float spawnTime = 5f;

    public PhotonView pv;

    void Start()
    {
        if (pv.IsMine)
        {
            StartCoroutine(SpawnPlatform());
        }
    }
   
    private IEnumerator SpawnPlatform()
    {
        while (true)
        {
            string platformName = platformsToSpawn[Random.Range(0, platformsToSpawn.Length)].name;
            GameObject newPlatform = ObjectPooler.Instance.SpawnFromPool(platformName, transform.position);
            newPlatform.GetComponent<PlatformMovement>().SetPos(transform.position);
            pv.RPC("OnPlatformSpawned", RpcTarget.OthersBuffered, platformName, transform.position);
            yield return new WaitForSeconds(spawnTime);
        }
    }

    [PunRPC]
    void OnPlatformSpawned(string platformName, Vector3 spawnerPos)
    {
        GameObject newPlatform = ObjectPooler.Instance.SpawnFromPool(platformName, spawnerPos);
        newPlatform.GetComponent<PlatformMovement>().SetPos(spawnerPos);
    }
}
