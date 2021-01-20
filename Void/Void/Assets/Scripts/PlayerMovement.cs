using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    private Animator animator;
    private bool isJumping;

    int movesBeforeUnstuck = 2;
    bool cantMove;

    //private Rigidbody2D rb;

    // var for swipe
    Vector2 startPos, currPos;
    public float swipeRange = 50;

    // moves list
    //List<int> moves;
    Queue<int> moves;

    private int previousStep; // za glass platform
    
    void Start()
    {
        //rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        moves = new Queue<int>();
    }

    
    void Update()
    {
        /*
        if (Input.GetKeyDown(KeyCode.UpArrow) && !isJumping)
        {
            MovePlayer(new Vector3(0, 5, 0));
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow) && !isJumping)
        {
            MovePlayer(new Vector3(-3, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow) && !isJumping)
        {
            MovePlayer(new Vector3(3, 0, 0));
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) && !isJumping)
        {
            MovePlayer(new Vector3(0, -5, 0));
        }
        */

        //CheckIfAbyss();
        Swipe();
        if (moves.Count != 0) Move();
    }

    private void Move()
    {
        int direction = moves.Dequeue();
        previousStep = direction;
        Debug.Log(direction);

        // 1 -> move right
        if (direction == 1)
        {
            MovePlayer(new Vector3(3, 0, 0));
        }
        // -1 -> move left
        else if (direction == -1)
        {
            MovePlayer(new Vector3(-3, 0, 0));
        }
        // 2 -> move up
        else if (direction == 2)
        {
            MovePlayer(new Vector3(0, 5, 0));
        }
        // -2 -> move down
        else if (direction == -2)
        {
            MovePlayer(new Vector3(0, -5, 0));
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

            if (distance.x < -swipeRange)
            {
                Debug.Log("Left");
                if (cantMove == false) moves.Enqueue(-1);
                //MovePlayer(new Vector3(-3, 0, 0));
            }
            else if (distance.x > swipeRange)
            {
                Debug.Log("Right");
                if (cantMove == false) moves.Enqueue(1);
                //MovePlayer(new Vector3(3, 0, 0));
            }
            else if (distance.y > swipeRange)
            {
                Debug.Log("Up");
                if (cantMove == false) moves.Enqueue(2);
                //MovePlayer(new Vector3(0, 5, 0));
            }
            else if (distance.y < -swipeRange)
            {
                Debug.Log("Down");
                if (cantMove == false) moves.Enqueue(-2);
                //MovePlayer(new Vector3(0, -5, 0));
            }
        }

        // ova provera if (cantMove == false) treba da spreci da se u queue moves doda move ukoliko je player na slime-u
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
        if ((transform.position + diff).x < -0.5 || (transform.position + diff).x > 12.5 || (transform.position + diff).y > 22) return;
        if (cantMove && movesBeforeUnstuck != 0)
        {
            movesBeforeUnstuck--;
            return;
        }
        animator.SetTrigger("jump");
        isJumping = true;
        transform.position = transform.position + diff;

        //rb.MovePosition(transform.position + diff);
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
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Right")
        {
            MovePlayer(new Vector3(3, 0, 0));
        }

        if (collision.gameObject.tag == "Left")
        {
            MovePlayer(new Vector3(-3, 0, 0));
        }

        if (collision.gameObject.tag == "Glass")
        {
            if (previousStep == -2)
            {
                Destroy(collision.gameObject);
            }
        }

        if (collision.gameObject.tag == "Slime")
        {
            if (cantMove == false)
            {
                cantMove = true;
                movesBeforeUnstuck = 2;
            }
        }

        if (collision.gameObject.tag == "Spikes")
        {
            //Destroy(gameObject);
            Debug.Log("You are dead!");
        }
    }
}
