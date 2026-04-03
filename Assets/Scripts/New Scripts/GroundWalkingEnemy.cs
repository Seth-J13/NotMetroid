using System.Collections;
using UnityEngine;

public class GroundWalkingEnemy : MonoBehaviour
{
    private Rigidbody2D body;
    private Vector3 playerPos;
    private bool gotPlayerPos;
    private GameObject player;
    private bool canMove = true;
    [SerializeField] private int moveForce = 100;
    [SerializeField] private float walkForce = 5;

    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();
    }
    private void Update()
    {
        if (gotPlayerPos && player != null)
        {
            canMove = false;
            playerPos = new Vector3(player.transform.position.x, player.transform.position.y,
                player.transform.position.z);
            MoveToPlayer();
            if (playerPos.x > transform.position.x)
            {
                transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
            }
            else
            {
                transform.rotation = Quaternion.AngleAxis(360f, Vector3.up);
            }
        }
        else
        {
            MoveRandomly();
        }

    }

    private void MoveRandomly()
    {
        if(canMove)
        {
            canMove = false;
            int randomNum = Random.Range(0, 9);
            int dir = Random.Range(0, 2);
            if (dir == 0)
            {
                //Go Left
                transform.rotation = Quaternion.AngleAxis(360f, Vector3.up);
                body.AddForceX(-randomNum * walkForce * Time.deltaTime, ForceMode2D.Impulse);
            }
            else
            {
                //Go right
                transform.rotation = Quaternion.AngleAxis(180f, Vector3.up);
                body.AddForceX(randomNum * walkForce * Time.deltaTime, ForceMode2D.Impulse);
            }
            StartCoroutine(MoveWait());
        }
    }
    private void MoveToPlayer()
    {
        if (transform.position.x < playerPos.x && body.linearVelocityX < 40)
        {

            //print("I am moving right");
            //print("This is moveForce * playerPos.x * Time.deltaTime: " + moveForce * playerPos.x * Time.deltaTime);
            body.AddForceX(moveForce * Mathf.Abs(playerPos.x) * Time.deltaTime, ForceMode2D.Force);
        }
        else if(transform.position.x > playerPos.x && body.linearVelocityX < 40)
        {
            //print("I am moving left");
            //print("This is moveForce * playerPos.x * Time.deltaTime: " + -moveForce * playerPos.x * Time.deltaTime);
            body.AddForceX(-moveForce * Mathf.Abs(playerPos.x) * Time.deltaTime, ForceMode2D.Force);
        }
    }

    public void FoundPlayer(Collider2D collision)
    {
        gotPlayerPos = true;
        playerPos = collision.transform.position;
        player = collision.gameObject;
        StopAllCoroutines();
    }

    public void PlayerGone(Collider2D collision)
    {
        StartCoroutine(ShortTermMemmory());
    }

    IEnumerator MoveWait()
    {
        yield return new WaitForSeconds(Random.Range(2.5f, 6f));
        canMove = true;
    }
    IEnumerator ShortTermMemmory()
    {
        yield return new WaitForSeconds(5f);
        gotPlayerPos = false;
        playerPos = new Vector3();
        player = null;
        canMove = true;
        //print("I forgot what I was doing");
    }
}
