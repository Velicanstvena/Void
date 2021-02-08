using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class PlayerPlaceBomb : MonoBehaviourPun
{
    [SerializeField] private PhotonView pv;

    private GameController gameController;
    private Button placeBombButton;
    private GameObject currentPlatform;

    void Start()
    {
        if (pv.IsMine)
        {
            gameController = FindObjectOfType<GameController>();
            placeBombButton = gameController.GetBombButton();
            placeBombButton.onClick.AddListener(PlaceBomb);
        }
    }

    void Update()
    {
        if (pv.IsMine)
        {
            BombButton();
        }
    }

    private void BombButton()
    {
        if (gameController.GetNumberOfBombs() > 0 && !gameController.IsFinished())
        {
            placeBombButton.gameObject.SetActive(true);
        }
        else
        {
            placeBombButton.gameObject.SetActive(false);
        }
    }

    public void PlaceBomb()
    {
        GameObject obj = ObjectPooler.Instance.SpawnFromPool("Bomb", transform.position);
        obj.transform.parent = currentPlatform.transform;
        obj.GetComponent<Collider2D>().enabled = false;
        gameController.DecreaseNumberOfBombs();
        obj.GetComponent<Bomb>().isPlaced = true;
        pv.RPC("OnCollectablePlaced", RpcTarget.OthersBuffered, currentPlatform.transform.position);
    }

    [PunRPC]
    void OnCollectablePlaced(Vector3 currPlatPos)
    {
        GameObject obj = ObjectPooler.Instance.SpawnFromPool("Bomb", currPlatPos);
        GameObject parent = null;
        GameObject[] objs = FindObjectsOfType<GameObject>();
        foreach (var o in objs)
        {
            if (Vector2.Distance(currPlatPos, o.transform.position) < 1f)
            {
                parent = o;
            }
        }
        obj.transform.parent = parent.transform;
        obj.GetComponent<Collider2D>().enabled = false;
        obj.GetComponent<Bomb>().isPlaced = true;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!pv.IsMine)
        {
            return;
        }

        currentPlatform = collision.gameObject;
    }
}
