using UnityEngine;

public class BoundsManager : MonoBehaviour
{
    //Variables
    private Collider2D collider;
    [SerializeField] private GameObject[] enemys;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {
            foreach(GameObject enemy in enemys)
            {
                if (enemy != null && enemy.name.StartsWith("Kisser"))
                {
                    enemy.GetComponent<FlyingEnemy>().UpdatePlayerPos(collision.gameObject);
                }
            }
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(collision.CompareTag("Player"))
        {      
            foreach(GameObject enemy in enemys)
            {
                if (enemy != null && enemy.name.StartsWith("Kisser"))
                {
                    enemy.GetComponent<FlyingEnemy>().StopUpdatingPlayerPos();
                }
            }
        }
    }

    private void Awake()
    {
        
        collider = GetComponent<Collider2D>();
        foreach(GameObject enemy in enemys)
        {
            if (enemy != null && enemy.name.StartsWith("Kisser"))
            {
                enemy.GetComponent<FlyingEnemy>().SetCircleCenter(collider.bounds.center);
                enemy.GetComponent<FlyingEnemy>().SetCircleRadius(collider.bounds.size.x / 2);
            }
        }
    }
    private void OnDrawGizmos()
    {
        try
        {
            Gizmos.DrawWireSphere(collider.bounds.center, collider.bounds.size.x/2);
        }
        catch { }

    }
}
