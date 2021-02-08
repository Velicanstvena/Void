using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Manager : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer(new Vector3(0f, 0f, 0f));
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            SpawnPlayer(new Vector3(6f, 0f, 0f));
        }
    }

    void SpawnPlayer(Vector3 pos)
    {
        PhotonNetwork.Instantiate(playerPrefab.name, pos, playerPrefab.transform.rotation);
    }
}
