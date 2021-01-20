using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEnd : MonoBehaviour
{
    private ObjectPooler objectPooler;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player" && collision.gameObject.transform.parent.tag != "Row")
        {
            GameObject usedObject = collision.gameObject;

            objectPooler.ReturnToPool(usedObject.name.Replace("(Clone)", ""), usedObject);
            usedObject.SetActive(false);
            
        }

        if (collision.gameObject.transform.parent.tag == "Row")
        {
            Destroy(collision.gameObject);
        }
    }
}
