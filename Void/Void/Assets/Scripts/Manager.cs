﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;
    private int numberOfPlayers;
    private bool spawned1, spawned2;

    void Start()
    {
    }

    void Update()
    {
        numberOfPlayers = PhotonNetwork.CountOfPlayers;
        if (numberOfPlayers == 1 && spawned1 == false)
        {
            SpawnPlayer(playerPrefab.transform.position);
            spawned1 = true;
        }
        else if (numberOfPlayers == 2 && spawned2 == false)
        {
            SpawnPlayer(new Vector3(6f, 10f, 0f));
            spawned2 = true;
        }
    }

    void SpawnPlayer(Vector3 pos)
    {
        PhotonNetwork.Instantiate(playerPrefab.name, pos, playerPrefab.transform.rotation);
    }
}