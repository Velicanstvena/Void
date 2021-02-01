using System.Collections;
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
        if (PhotonNetwork.CountOfPlayers == 1)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 0f, 0f), playerPrefab.transform.rotation);
        }
        else if (PhotonNetwork.CountOfPlayers == 2)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(6f, 0f, 0f), playerPrefab.transform.rotation);
        }
    }

    void SpawnPlayer(Vector3 pos)
    {
        PhotonNetwork.Instantiate(playerPrefab.name, pos, playerPrefab.transform.rotation);
    }
}
