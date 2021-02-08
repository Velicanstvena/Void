using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerCollectCollectables : MonoBehaviour
{
    [SerializeField] private PhotonView pv;
    [SerializeField] GameObject heartPickedParticle;
    private GameController gameController;

    void Start()
    {
        if (pv.IsMine)
        {
            gameController = FindObjectOfType<GameController>();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pv.IsMine)
        {
            return;
        }

        if (collision.gameObject.tag == "Heart")
        {
            DespawnCollectable(collision.gameObject);
            gameController.IncreaseNumberOfHearts();
            GameObject particle = Instantiate(heartPickedParticle, gameObject.transform.position, Quaternion.identity);
            Destroy(particle, 1);
            pv.RPC("OnCollectablePicked", RpcTarget.OthersBuffered, "Heart", collision.gameObject.transform.position);
        }

        if (collision.gameObject.tag == "Bomb")
        {
            DespawnCollectable(collision.gameObject);
            gameController.IncreaseNumberOfBombs();
            pv.RPC("OnCollectablePicked", RpcTarget.OthersBuffered, "Bomb", collision.gameObject.transform.position);
        }
    }

    [PunRPC]
    void OnCollectablePicked(string objectName, Vector3 objectPos)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(objectName);
        foreach (var o in objects)
        {
            if (Vector2.Distance(objectPos, o.transform.position) < 1f)
            {
                DespawnCollectable(o);
            }
        }
    }

    private void DespawnCollectable(GameObject usedObject)
    {
        ObjectPooler.Instance.ReturnToPool(usedObject.name.Replace("(Clone)", ""), usedObject);
        usedObject.SetActive(false);
    }
}
