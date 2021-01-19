using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMovement : MonoBehaviour
{
    [SerializeField] GameObject endPos;
    [SerializeField] float speed;

    private Vector3 endPosY;

    // Start is called before the first frame update
    void Start()
    {
        endPosY = new Vector3(transform.position.x, endPos.transform.position.y);
    }

    // Update is called once per frame
    void Update()
    {
        Move();
    }

    private void Move()
    {
        transform.position = Vector3.MoveTowards(transform.position, endPosY, speed * Time.deltaTime);
    }
}
