using System.Collections;
using System.Drawing;
using System.Net;
using UnityEngine;

public class FlyingEnemy : MonoBehaviour
{
    //Variables
    Rigidbody2D body;
    GameObject player;
    Vector2 randPoint;
    private Vector2 circleCenter;
    private float circleRadius;
    [SerializeField] private float thrust = 1f;
    private bool foundPlayer = false;
    private bool canPickNewPoint = true;
    private bool inBoundary = true;
    private bool gotToPoint = false;

    //TEMP VAR
    Vector3 pos;

    
    //Update/Awake
    private void Awake()
    {
        body = GetComponent<Rigidbody2D>();    
        pos = transform.position;
    }
    private void FixedUpdate()
    {
        float enemyPos = Mathf.Sqrt(Mathf.Pow(transform.position.x - circleCenter.x, 2) + Mathf.Pow(transform.position.y - circleCenter.y, 2));
        inBoundary = (enemyPos > circleRadius) ? false : true;
        if(inBoundary && canPickNewPoint && !foundPlayer)
            PickRandomPoint();
        else if(!canPickNewPoint && !foundPlayer && inBoundary)
        {
            float x = transform.position.x;
            float y = transform.position.y;
            bool side = !(randPoint.x - x > 0);
            bool level = !(randPoint.y - y > 0);
            if (((randPoint.x - x > 0) != side) && ((randPoint.y - y > 0) != level))
            {
                body.linearVelocity = Vector2.zero;
                gotToPoint = true;
            }
        }
        if (foundPlayer && inBoundary)
        {
            body.AddForce((player.transform.position - transform.position) * thrust/10 * Time.deltaTime, ForceMode2D.Force);
        }
        else if (!foundPlayer && inBoundary)
        {
            body.AddForce((new Vector3(randPoint.x, randPoint.y) - transform.position) * thrust * Time.deltaTime, ForceMode2D.Force);
            
        }
        else if (!inBoundary)
        {
            MoveBackToBoundary();
        }
    }
    //Methods
    public void SetCircleCenter(Vector2 v)
    {
        circleCenter = v;
    }
    public void SetCircleRadius(float f)
    {
        circleRadius = f;
    }
    public void UpdatePlayerPos(GameObject player)
    {
        this.player = player;
        foundPlayer = true;
        canPickNewPoint = false;
    }
    public void StopUpdatingPlayerPos() {  foundPlayer = false; canPickNewPoint = true; }
    private void PickRandomPoint()
    {
        canPickNewPoint = false;
        float x = Random.Range(-circleRadius, circleRadius) + circleCenter.x;
        float y = Random.Range(-circleRadius, circleRadius) + circleCenter.y;
        float distToCenter = Mathf.Sqrt(Mathf.Pow(circleCenter.x - x, 2) + Mathf.Pow(circleCenter.y - y, 2));
        while(distToCenter > circleRadius)
        {
            x = Random.Range(-circleRadius, circleRadius) + circleCenter.x;
            y = Random.Range(-circleRadius, circleRadius) + circleCenter.y;
            distToCenter = Mathf.Sqrt(Mathf.Pow(circleCenter.x - x, 2) + Mathf.Pow(circleCenter.y - y, 2));
        }
        randPoint = new Vector2(x, y);
        pos = transform.position;
        StartCoroutine(WaitToGetToPoint());
    }
    private void MoveBackToBoundary()
    {
        canPickNewPoint = false;
        body.AddForce(new Vector2(circleCenter.x - transform.position.x, circleCenter.y - transform.position.y) * (thrust/1.25f) * Time.deltaTime, ForceMode2D.Force);
    }

    //Enumerators
    IEnumerator WaitToGetToPoint()
    {
        yield return new WaitUntil(() => gotToPoint);
        gotToPoint = false;
        StartCoroutine(WaitToMove());
    }
    IEnumerator WaitToMove()
    {
        yield return new WaitForSeconds(2.5f);
        canPickNewPoint = true;
    }
    //Gizmos
    private void OnDrawGizmos()
    {
        //if(foundPlayer)
        //{
        //    Gizmos.DrawLine(transform.position, player.transform.position);
        //}
        //if(!inBoundary)
        //{
        //    Gizmos.DrawRay(new Ray(transform.position, new Vector3(circleCenter.x - transform.position.x, circleCenter.y - transform.position.y)));
        //}
        //Gizmos.color = UnityEngine.Color.yellow;
        //Gizmos.DrawRay(new Ray(transform.position, (new Vector3(randPoint.x, randPoint.y) - transform.position) * thrust * Time.deltaTime));
        //Gizmos.DrawLine(new Vector3(randPoint.x - pos.x, randPoint.y - pos.y), transform.position);
        //Gizmos.color = UnityEngine.Color.white;
    }
}
