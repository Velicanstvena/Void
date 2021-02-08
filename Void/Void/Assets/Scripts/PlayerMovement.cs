using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun, IPunObservable
{
    #region Variables

    private Animator animator;
    private bool isJumping;
    private Rigidbody2D rb;
    private bool jumped = false;

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
    private Queue<int> moves;
    private int previousStep;

    // vars for fall check
    private float lastPosY = 0f;
    private float fallDistance = 0f;
    private bool broken = false;

    // multiplayer vars
    [SerializeField] private PhotonView pv;
    public static GameObject LocalPlayerInstance;
    private Vector3 smoothMove;

    // game controller
    private GameController gameController;

    #endregion

    private enum Direction
    {
        LEFT,
        RIGHT,
        UP,
        DOWN
    }

    private void Awake()
    {
        if (photonView.IsMine)
        {
            PlayerMovement.LocalPlayerInstance = this.gameObject;
        }
    }

    void Start()
    {
        if (pv.IsMine)
        {
            previousStep = -1;
            gameController = FindObjectOfType<GameController>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            moves = new Queue<int>();
            StartCoroutine(Move());
        }
    }
    
    void Update()
    {
        if (pv.IsMine)
        {
            Swipe();
            CheckFall();

            if (!gameController.IsAlive())
            {
                pv.RPC("OnLose", RpcTarget.Others);
            }
        }
        else
        {
            SmoothMovement();
        }
    }

    IEnumerator Move()
    {
        while (!gameController.IsFinished())
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
        if (Input.GetMouseButtonDown(0)) 
        {
            startPos = Input.mousePosition;
        }

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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(transform.position);
        }
        else if (stream.IsReading)
        {
            smoothMove = (Vector3)stream.ReceiveNext();
        }
    }

    private void SmoothMovement()
    {
        transform.position = Vector3.Lerp(transform.position, smoothMove, Time.deltaTime * 20);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pv.IsMine)
        {
            return;
        }

        if (collision.gameObject.tag == "Slime")
        {
            if (cantMove == false)
            {
                cantMove = true;
                movesBeforeUnstuck = 2;
            }
        }

        if (collision.gameObject.tag == "Grass")
        {
            ResetFallVars();
        }
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
        }
    }

    private void ResetFallVars()
    {
        jumped = false;
        lastPosY = 0;
        fallDistance = 0;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!pv.IsMine)
        {
            return;
        }

        isJumping = false;

        if (collision.gameObject.tag == "Glass")
        {
            if (fallDistance > 1 || previousStep == (int)Direction.DOWN)
            {
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                collision.gameObject.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(GlassPlatform(collision.gameObject));
                broken = true;
                pv.RPC("OnGlassPlatformBroken", RpcTarget.OthersBuffered, collision.gameObject.transform.position);
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

        if (fallDistance > 3 && previousStep == (int)Direction.DOWN)
        {
            gameController.Die();
        }

        if (fallDistance >= 10)
        {
            gameController.Die();
        }

        if (collision.gameObject.tag == "Right")
        {
            MovePlayer(rightDiff);
            return;
        }

        if (collision.gameObject.tag == "Left")
        {
            MovePlayer(leftDiff);
            return;
        }

        ResetFallVars();
    }

    [PunRPC]
    void OnGlassPlatformBroken(Vector3 platformPos)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Glass");
        foreach (var o in objs)
        {
            if (Vector2.Distance(platformPos, o.transform.position) < 1f)
            {
                o.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                o.gameObject.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(GlassPlatform(o.gameObject));
                broken = true;
            }
        }
    }

    IEnumerator GlassPlatform(GameObject glassPlatform)
    {
        yield return new WaitForSeconds(3f);
        glassPlatform.GetComponent<SpriteRenderer>().enabled = true;
        glassPlatform.GetComponent<Collider2D>().enabled = true;
    }

    [PunRPC]
    void OnLose()
    {
        FindObjectOfType<GameController>().Win();
    }
}
