﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    private void OnDisable()
    {
        gameObject.GetComponent<SpriteRenderer>().color = Color.white;
    }
}
