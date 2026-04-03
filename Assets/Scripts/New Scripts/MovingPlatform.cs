using System.Collections;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    private BoxCollider2D waitBox;
    public bool vertical = false;
    public bool horizontal = false;
    public bool waitForPlayer = false;
    [HideInInspector] public bool canMove = true;
    private bool canChangeDir = true;
    public int maxHeight = 20;
    public int maxWidth = 20;
    public float heightToMove = 5;
    public float widthToMove = 5;
    private float startPos;
    private void Start()
    {
        canMove = true;
        if (waitForPlayer && gameObject.name == "WaitBox")
        {
            waitBox = GetComponent<BoxCollider2D>();
        }
        else if (waitForPlayer)
        {
            canMove = false;
        }
        if (vertical)
        {
            startPos = transform.position.y;
        }
        else
        {
            startPos = transform.position.x;
        }
    }
    public void SetCanMove()
    { canMove = true; }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        print("I have collided");
        if (collision.CompareTag("Player") && gameObject.name == "WaitBox")
        {
            print("Setting Can Move");
            transform.parent.GetComponent<MovingPlatform>().SetCanMove();

        }
    }

    private void Update()
    {
        print("canMove: " + canMove);
        Vector3 pos = transform.position;
        if (vertical && canMove)
        {
            if (transform.position.y > maxHeight && canChangeDir)
            {
                canChangeDir = false;
                heightToMove = -heightToMove;
                StartCoroutine(WaitPlatform());
            }
            if(transform.position.y < startPos && canChangeDir)
            {
                canChangeDir = false;
                heightToMove = -heightToMove;
                StartCoroutine(WaitPlatform());
            }
            transform.position = new Vector3(pos.x, pos.y + (heightToMove * Time.deltaTime), pos.z); 
        }
        else if(horizontal && canMove)
        {
            if(transform.position.x >= maxWidth)
            {
                canChangeDir = false;
                widthToMove = -widthToMove;
                StartCoroutine(WaitPlatform());
            }
            if (transform.position.x <= startPos && canChangeDir)
            {
                canChangeDir = false;
                widthToMove = -widthToMove;
                StartCoroutine(WaitPlatform());
            }
            transform.position = new Vector3(pos.x + (widthToMove * Time.deltaTime), pos.y, pos.z);
        }
    }

    IEnumerator WaitPlatform()
    {
        if (vertical && heightToMove < 0)
            yield return new WaitUntil(() => transform.position.y < maxHeight);
        else if (vertical && heightToMove > 0)
            yield return new WaitUntil(() => transform.position.y > startPos);
        else if (horizontal && widthToMove < 0)
            yield return new WaitUntil(() => transform.position.x < maxWidth);
        else
            yield return new WaitUntil(() => transform.position.x > startPos);
        canChangeDir = true;
    }
}
