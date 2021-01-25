using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEnd : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    private ObjectPooler objectPooler;

    void Start()
    {
        objectPooler = ObjectPooler.Instance;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            GameObject usedObject = collision.gameObject;
            objectPooler.ReturnToPool(usedObject.name.Replace("(Clone)", ""), usedObject);
            usedObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Player")
        {
            gameController.Die();
        }
    }
}
