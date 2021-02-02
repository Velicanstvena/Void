using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPooler : MonoBehaviour
{
    [SerializeField] GameObject parentPos;
    private int x = 30;

    [System.Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
    }

    #region Singleton

    public static ObjectPooler Instance;

    private void Awake()
    {
        Instance = this;
    }

    #endregion

    public List<Pool> pools;
    public Dictionary<string, Queue<GameObject>> poolDictionary;

    void Start()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab, this.gameObject.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);

                if (obj.GetComponent<CollectableSpawn>())
                {
                    obj.GetComponent<CollectableSpawn>().SetPvId(x++);
                }
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position)
    {
        if (!poolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning("Pool with tag does not exist!");
            return null;
        }

        GameObject objectToSpawn = null;
        if (poolDictionary[tag].Count > 0)
        {
            objectToSpawn = poolDictionary[tag].Dequeue();
        }
        else
        {
            foreach (Pool pool in pools)
            {
                if ((pool.tag).Equals(tag))
                {
                    objectToSpawn = Instantiate(pool.prefab, this.gameObject.transform);
                    if (objectToSpawn.GetComponent<CollectableSpawn>())
                    {
                        objectToSpawn.GetComponent<CollectableSpawn>().SetPvId(x++);
                    }
                }
            }
        }

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.parent = parentPos.transform;

        return objectToSpawn;
    }

    public void ReturnToPool(string tag, GameObject usedObject)
    {
        poolDictionary[tag].Enqueue(usedObject);
        usedObject.transform.parent = this.gameObject.transform;
    }
}
