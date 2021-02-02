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
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(0f, 0f, 0f), playerPrefab.transform.rotation);
        }
        else if (!PhotonNetwork.IsMasterClient)
        {
            PhotonNetwork.Instantiate(playerPrefab.name, new Vector3(6f, 0f, 0f), playerPrefab.transform.rotation);
        }
    }

    void SpawnPlayer(Vector3 pos)
    {
        PhotonNetwork.Instantiate(playerPrefab.name, pos, playerPrefab.transform.rotation);
    }
}
