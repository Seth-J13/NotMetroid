using UnityEngine;

public class TriggerHandler : MonoBehaviour
{
    [SerializeField] private bool GroundEnemy;
    [SerializeField] private bool FlyingEnemy;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player")
        {
            if (GroundEnemy)
            {
                GetComponentInParent<GroundWalkingEnemy>().FoundPlayer(collision);
            }
            else if (FlyingEnemy)
            {
                //GetComponentInParent<FlyingEnemy>()
            }
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(GroundEnemy)
        {
            try
            {
                //print("Got component");
                GetComponentInParent<GroundWalkingEnemy>().PlayerGone(collision);
            }
            catch
            {
                //print("couldn't get component");
            }
        }
        else if(FlyingEnemy)
        {
            //
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 5);
    }
}
