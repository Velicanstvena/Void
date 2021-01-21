using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private bool isJumping;
    private Rigidbody2D rb;
    [SerializeField] private bool jumped = false;
    [SerializeField] private bool alive = true;

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
    private int previousStep; // za glass platform

    // vars for fall check
    private float lastPosY = 0f;
    private float fallDistance = 0f;

    // number of bombs
    [SerializeField] private int numOfBombs = 0;

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
        StartCoroutine(Move());
    }

    
    void Update()
    {
        //CheckIfAbyss();
        Swipe();
        CheckFall();
    }

    IEnumerator Move()
    {
        while (true)
        {
            yield return new WaitForSeconds(0.1f);

            if (moves.Count != 0)
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

    private void CheckIfAbyss()
    {
        if (transform.position.y < -6)
        {
            Debug.Log("Dead");
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

            if (fallDistance >= 10)
            {
                Debug.Log("Dead from fall");
                alive = false;
            }
        }
    }

    private void ResetFallVars()
    {
        jumped = false;
        lastPosY = 0;
        fallDistance = 0;
    }

    public void FinishJump()
    {
        isJumping = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Heart")
        {
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "Bomb")
        {
            Destroy(collision.gameObject);
            numOfBombs++;
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
            alive = true;
            Debug.Log("Survived");

            ResetFallVars();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        ResetFallVars();

        if (collision.gameObject.tag == "Right")
        {
            MovePlayer(rightDiff);
        }

        if (collision.gameObject.tag == "Left")
        {
            MovePlayer(leftDiff);
        }

        if (collision.gameObject.tag == "Glass")
        {
            if (previousStep == (int) Direction.DOWN)
            {
                //collision.gameObject.SetActive(false);
                collision.gameObject.GetComponent<SpriteRenderer>().enabled = false;
                collision.gameObject.GetComponent<Collider2D>().enabled = false;
                StartCoroutine(GlassPlatform(collision.gameObject));
            }
        }

        if (collision.gameObject.tag == "Spikes")
        {
            //Destroy(gameObject);
            Debug.Log("You are dead!");
        }
    }

    IEnumerator GlassPlatform(GameObject glassPlatform)
    {
        yield return new WaitForSeconds(3f);

        //if (!glassPlatform.active) glassPlatform.SetActive(true);

        glassPlatform.GetComponent<SpriteRenderer>().enabled = true;
        glassPlatform.GetComponent<Collider2D>().enabled = true;
    }
}
