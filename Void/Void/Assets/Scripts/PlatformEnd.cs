﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformEnd : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag != "Player")
        {
            GameObject usedObject = collision.gameObject;
            ObjectPooler.Instance.ReturnToPool(usedObject.name.Replace("(Clone)", ""), usedObject);
            usedObject.SetActive(false);
        }

        if (collision.gameObject.tag == "Player" && PlayerMovement.LocalPlayerInstance == collision.gameObject)
        {
            gameController.Die();
        }
    }
}
