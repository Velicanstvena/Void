using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

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
    private Queue<int> moves = new Queue<int>();
    private int previousStep;

    // vars for fall check
    private float lastPosY = 0f;
    private float fallDistance = 0f;
    private bool broken = false;

    // bombs
    private GameObject currentPlatform;

    // multiplayer vars
    public PhotonView pv;
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
            gameController = FindObjectOfType<GameController>();
            animator = GetComponent<Animator>();
            rb = GetComponent<Rigidbody2D>();
            StartCoroutine(Move());
        }
    }
    
    void Update()
    {
        if (pv.IsMine)
        {
            Swipe();
            CheckFall();
            BombButton();

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
        //if (pv.IsMine)
        //{
            GameObject obj = ObjectPooler.Instance.SpawnFromPool("Bomb", transform.position);
            obj.transform.parent = currentPlatform.transform;
            obj.GetComponent<Collider2D>().enabled = false;
            gameController.DecreaseNumberOfBombs();
            obj.GetComponent<Bomb>().isPlaced = true;
            pv.RPC("OnCollectablePlaced", RpcTarget.Others, currentPlatform.transform.position);
        //}
    }

    public void FinishJump()
    {
        isJumping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (!pv.IsMine)
        {
            return;
        }

        if (collision.gameObject.tag == "Heart")
        {
            DespawnCollectable(collision);
            gameController.IncreaseNumberOfHearts();
            pv.RPC("OnCollectablePicked", RpcTarget.Others, "Heart", collision.gameObject.transform.position);
        }

        if (collision.gameObject.tag == "Bomb")
        {
            DespawnCollectable(collision);
            gameController.IncreaseNumberOfBombs();
            pv.RPC("OnCollectablePicked", RpcTarget.Others, "Bomb", collision.gameObject.transform.position);
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

    [PunRPC]
    void OnCollectablePicked(string objectName, Vector3 objectPos)
    {
        GameObject[] gmts = GameObject.FindGameObjectsWithTag(objectName);
        foreach (var gm in gmts)
        {
            if (gm.transform.position == objectPos)
            {
                DespawnCollectable(gm);
            }
        }
    }

    [PunRPC]
    void OnCollectablePlaced(Vector3 currPlatPos)
    {
        GameObject obj = ObjectPooler.Instance.SpawnFromPool("Bomb", currPlatPos);
        GameObject parent = null;
        GameObject[] objs = FindObjectsOfType<GameObject>();
        foreach (var o in objs)
        {
            if (o.transform.position == currPlatPos)
            {
                parent = o;
            }
        }
        obj.transform.parent = parent.transform;
        obj.GetComponent<Collider2D>().enabled = false;
        obj.GetComponent<Bomb>().isPlaced = true;
    }

    [PunRPC]
    void OnLose()
    {
        FindObjectOfType<GameController>().Win();
    }

    private void DespawnCollectable(Collider2D collision)
    {
        GameObject usedObject = collision.gameObject;
        ObjectPooler.Instance.ReturnToPool(usedObject.name.Replace("(Clone)", ""), usedObject);
        usedObject.SetActive(false);
    }

    private void DespawnCollectable(GameObject usedObject)
    {
        ObjectPooler.Instance.ReturnToPool(usedObject.name.Replace("(Clone)", ""), usedObject);
        usedObject.SetActive(false);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (!pv.IsMine)
        {
            return;
        }

        currentPlatform = collision.gameObject;
        FinishJump();

        if (collision.gameObject.tag == "Glass")
        {
            if (fallDistance > 1 || previousStep == (int)Direction.DOWN)
            {
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                collision.gameObject.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(GlassPlatform(collision.gameObject));
                broken = true;
                pv.RPC("OnGlassPlatformBroken", RpcTarget.Others, collision.gameObject.transform.position);
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
        }

        if (collision.gameObject.tag == "Left")
        {
            MovePlayer(leftDiff);
        }

        ResetFallVars();
    }

    [PunRPC]
    void OnGlassPlatformBroken(Vector3 platformPos)
    {
        GameObject[] objs = GameObject.FindGameObjectsWithTag("Glass");
        foreach (var o in objs)
        {
            if (o.transform.position == platformPos)
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

    private void BombButton()
    {
        if (gameController.GetNumberOfBombs() > 0 && gameController.IsAlive())
        {
            //placeBombButton.gameObject.SetActive(true);
            if (Input.GetKeyDown(KeyCode.Space))
            {
                PlaceBomb();
            }
        }
        else
        {
            //placeBombButton.gameObject.SetActive(false);
            Debug.Log("Bomb is not placed");
        }
    }
}
