using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] GameObject endPos;
    [SerializeField] float speed;
    private Vector3 endPosY;

    void Start()
    {
        endPosY = new Vector3(transform.position.x, endPos.transform.position.y);
    }

    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPosY, speed * Time.deltaTime);
    }

    public void SetPos(Vector3 spawnedPos)
    {
        endPosY = new Vector3(spawnedPos.x, endPos.transform.position.y);
    }
}
