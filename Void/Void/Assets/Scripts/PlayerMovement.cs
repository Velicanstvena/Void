using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun//, IPunObservable
{
    private Animator animator;
    private bool isJumping;
    private Rigidbody2D rb;
    [SerializeField] private bool jumped = false;
    //[SerializeField] private bool alive = true;

    // vars for slime
    private int movesBeforeUnstuck = 2;
    private bool cantMove;

    // vars for MovePlayer
    private Vector3 rightDiff = new Vector3(3, 0, 0);
    private Vector3 leftDiff = new Vector3(-3, 0, 0);
    private Vector3 upDiff = new Vector3(0, 5, 0);
    private Vector3 downDiff = new Vector3(0, -5, 0);

    // vars for swipe
    private Vector2 startPos, currPos;
    private float swipeRange = 50;

    // vars for moves queue
    private Queue<int> moves = new Queue<int>();
    private int previousStep;

    // vars for fall check
    private float lastPosY = 0f;
    private float fallDistance = 0f;
    private bool broken = false;

    // bombs
    private ObjectPooler objectPooler;
    private GameObject currentPlatform;
    private Vector2 overlapBoxSize1 = new Vector2(6f, 1f);
    private Vector2 overlapBoxSize2 = new Vector2(1f, 10f);
    private Collider2D[] colliders;

    // multiplayer vars
    public PhotonView pv;

    // game controller
    [SerializeField] private GameController gameController;

    private enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }
    
    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        objectPooler = ObjectPooler.Instance;
        StartCoroutine(Move());
    }
    
    void Update()
    {
        Swipe();
        CheckFall();
    }

    IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (moves.Count != 0 && isJumping == false)
            {
                int direction = moves.Dequeue();
                previousStep = direction;

                if (direction == (int)Direction.RIGHT)
                {
                    MovePlayer(rightDiff);
                }
                else if (direction == (int)Direction.LEFT)
                {
                    MovePlayer(leftDiff);
                }
                else if (direction == (int)Direction.UP)
                {
                    MovePlayer(upDiff);
                }
                else if (direction == (int)Direction.DOWN)
                {
                    MovePlayer(downDiff);
                }
            }
        }
    }

    private void Swipe()
    {
        // Input.GetTouch(0).phase == TouchPhase.Began
        if (Input.GetMouseButtonDown(0)) 
        {
            startPos = Input.mousePosition;
        }

        // Input.GetTouch(0).phase == TouchPhase.Ended
        if (Input.GetMouseButtonUp(0))
        {
            currPos = Input.mousePosition;
            Vector2 distance = currPos - startPos;

            // player je na slime platformi
            if (cantMove && movesBeforeUnstuck != 0)
            {
                movesBeforeUnstuck--;

                if (movesBeforeUnstuck == 0)
                {
                    cantMove = false;
                }

                return;
            }

            if (distance.x < -swipeRange)
            {
                moves.Enqueue((int) Direction.LEFT);
            }
            else if (distance.x > swipeRange)
            {
                moves.Enqueue((int) Direction.RIGHT);
            }
            else if (distance.y > swipeRange)
            {
                moves.Enqueue((int) Direction.UP);
            }
            else if (distance.y < -swipeRange)
            {
                moves.Enqueue((int) Direction.DOWN);
            }
        }
    }

    private void MovePlayer(Vector3 diff)
    {
        // provera da player ne ode van scene
        if ((transform.position + diff).x < -0.5 || (transform.position + diff).x > 12.5 || (transform.position + diff).y > 22) return;
        jumped = true;
        animator.SetTrigger("jump");
        isJumping = true;
        transform.position = transform.position + diff;
    }

    private void CheckFall()
    {
        if (rb.velocity.y < -0.1 && jumped)
        {
            if (lastPosY > gameObject.transform.position.y)
            {
                fallDistance += lastPosY - gameObject.transform.position.y;
            }

            lastPosY = gameObject.transform.position.y;

            //if (fallDistance >= 10)
            //{
            //    Debug.Log("Dead from fall");
            //    alive = false;
            //}
        }
    }

    private void ResetFallVars()
    {
        jumped = false;
        lastPosY = 0;
        fallDistance = 0;
    }

    public void PlaceBomb()
    {
        GameObject obj = objectPooler.SpawnFromPool("Bomb", transform.position);
        obj.transform.parent = currentPlatform.transform;
        //obj.GetComponent<Collider2D>().isTrigger = false;
        obj.GetComponent<Collider2D>().enabled = false;
        //obj.tag = "EnemyBomb";
        gameController.DecreaseNumberOfBombs();
        StartCoroutine(ActivateBomb(obj));
    }

    IEnumerator ActivateBomb(GameObject activeBomb)
    {
        yield return new WaitForSeconds(2f);
        activeBomb.GetComponent<SpriteRenderer>().color = Color.red;
        yield return new WaitForSeconds(2f);
        BombExplosion(activeBomb.transform);
        DespawnCollectable(activeBomb);
        //activeBomb.tag = "ActiveBomb";
    }

    public void BombExplosion(Transform bombPos)
    {
        FindAffectedPlatforms(overlapBoxSize1, bombPos);
        FindAffectedPlatforms(overlapBoxSize2, bombPos);
        //gameController.Die();
    }

    private void FindAffectedPlatforms(Vector2 boxSize, Transform bombPos)
    {
        colliders = Physics2D.OverlapBoxAll(bombPos.position, boxSize, 0f);

        if (colliders.Length > 0)
        {
            foreach (Collider2D col in colliders)
            {
                //if (col.tag != "Player" && col.tag != "Slime" && col.tag != "Grass")
                if (col.tag == "Player")
                {
                    Debug.Log(col.gameObject.name);
                    gameController.Die();
                    //DespawnCollectable(col);
                }
            }
        }
    }

    public void FinishJump()
    {
        isJumping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Heart")
        {
            DespawnCollectable(collision);
            gameController.IncreaseNumberOfHearts();
        }

        if (collision.gameObject.tag == "Bomb")
        {
            DespawnCollectable(collision);
            gameController.IncreaseNumberOfBombs();
        }

        if (collision.gameObject.tag == "Slime")
        {
            if (cantMove == false)
            {
                cantMove = true;
                movesBeforeUnstuck = 2;
            }
        }

        if (fallDistance >= 9)
        {
            if (collision.gameObject.tag == "Grass")
            {
                //alive = true;
                //Debug.Log("Survived");
                ResetFallVars();
            }
        }

        //if (collision.gameObject.tag == "ActiveBomb")
        //{
        //    BombExplosion();
        //    DespawnCollectable(collision);
        //}
    }

    private void DespawnCollectable(Collider2D collision)
    {
        GameObject usedObject = collision.gameObject;
        objectPooler.ReturnToPool(usedObject.name.Replace("(Clone)", ""), usedObject);
        usedObject.SetActive(false);
    }

    private void DespawnCollectable(GameObject usedObject)
    {
        objectPooler.ReturnToPool(usedObject.name.Replace("(Clone)", ""), usedObject);
        usedObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        currentPlatform = collision.gameObject;
        FinishJump();

        if (fallDistance != 0) Debug.Log("Fall distance -> " + fallDistance);

        if (collision.gameObject.tag == "Glass")
        {
            if (fallDistance > 1 || previousStep == (int) Direction.DOWN)
            {
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                collision.gameObject.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(GlassPlatform(collision.gameObject));
                broken = true;
                return;
            }
        }

        if (collision.gameObject.tag == "Spikes")
        {
            gameController.Die();
        }

        if (broken && fallDistance > 4)
        {
            gameController.Die();
            broken = false;
        }

        if (fallDistance >= 10)
        {
            //alive = false;
            gameController.Die();
        }

        if (collision.gameObject.tag == "Right")
        {
            MovePlayer(rightDiff);
        }

        if (collision.gameObject.tag == "Left")
        {
            MovePlayer(leftDiff);
        }

        ResetFallVars();
    }

    IEnumerator GlassPlatform(GameObject glassPlatform)
    {
        yield return new WaitForSeconds(3f);
        glassPlatform.GetComponent<SpriteRenderer>().enabled = true;
        glassPlatform.GetComponent<Collider2D>().enabled = true;
    }

    //public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    //{
    //    if (stream.IsWriting)
    //    {
    //        stream.SendNext(transform.position);
    //    }
    //    else if (stream.IsReading)
    //    {
    //        smoothMove = (Vector3) stream.ReceiveNext();
    //    }
    //}
}
