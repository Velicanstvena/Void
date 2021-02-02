using System.Collections;
using Photon.Pun;
using UnityEngine;

public class CollectableSpawn : MonoBehaviour
{
    [SerializeField] GameObject[] collectableToSpawn;
    [SerializeField] GameObject collectableSpawnerPos;
    private float minSpawnTime = 1f;
    private float maxSpawnTime = 5f;
    private bool spawned = false;

    [SerializeField] private PhotonView pv;

    void Start()
    {
        if (pv.IsMine)
        {
            StartCoroutine(SpawnCollectables());
        }
    }

    private IEnumerator SpawnCollectables()
    {
        while (true)
        {
            yield return new WaitForSeconds(Random.Range(minSpawnTime, maxSpawnTime));

            if (!spawned)
            {
                string collectableName = collectableToSpawn[Random.Range(0, collectableToSpawn.Length)].name;
                GameObject newCollectable = ObjectPooler.Instance.SpawnFromPool(collectableName, collectableSpawnerPos.transform.position);
                newCollectable.transform.parent = collectableSpawnerPos.transform;
                spawned = true;
                pv.RPC("OnCollectableSpawned", RpcTarget.OthersBuffered, collectableName, collectableSpawnerPos.transform.position);
            }
        }
    }

    [PunRPC]
    void OnCollectableSpawned(string collectableName, Vector3 platformPos)
    {
        GameObject newCollectable = ObjectPooler.Instance.SpawnFromPool(collectableName, platformPos);
        newCollectable.transform.parent = collectableSpawnerPos.transform;
        spawned = true;
    }

    public void SetPvId(int viewId)
    {
        this.pv.ViewID = viewId;
    }
}
