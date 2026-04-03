using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

public class RiddlerAI : MonoBehaviour
{
    private GameObject sam;
    public GameObject leftBounds;
    public GameObject rightBounds;
    public GameObject questions;
    private EnemyHandler enemyHandler;
    public float moveSpeed = 1;
    public float widthToMove;
    public float waitTime = 1.25f;
    private void Awake()
    {
        enemyHandler = GetComponent<EnemyHandler>();
        sam = GameObject.Find("NotSamus");
        leftBounds = GameObject.Find("LeftBounds");
        rightBounds = GameObject.Find("RightBounds");
        transform.position = rightBounds.transform.position;
        widthToMove = leftBounds.transform.position.x;
    }

    private void Update()
    {
        MoveDirection();
        StartCoroutine(IHaveAQuestion());
        if(enemyHandler.GetEnemyHealth() >=750)
        {
            waitTime = 1.0f;
        }
        else if(enemyHandler.GetEnemyHealth() >= 500)
        {
            waitTime = 0.75f;
        }
        else
        {
            waitTime = 0.45f;
        }
    }

    private void MoveDirection()
    {
        if (transform.position.x < leftBounds.transform.position.x)
            widthToMove = -widthToMove;
        if (transform.position.x > rightBounds.transform.position.x)
            widthToMove = -widthToMove;
        transform.position = new Vector3(transform.position.x + widthToMove * moveSpeed, transform.position.y, transform.position.z);
    }



    private void FireProjectiles()
    {
        GameObject projectile = Instantiate(questions);
        projectile.transform.position = transform.position;
        Rigidbody2D body = projectile.GetComponent<Rigidbody2D>();
        try
        {
            body.AddForce(new Vector2(sam.transform.position.x * (2 / waitTime), sam.transform.position.y * (2 / waitTime)), ForceMode2D.Impulse);
        }
        catch
        {
            sam = GameObject.Find("NotSamus");
        }

    }

    IEnumerator IHaveAQuestion()
    {
        yield return new WaitForSeconds(waitTime);
        FireProjectiles();
    }

}
