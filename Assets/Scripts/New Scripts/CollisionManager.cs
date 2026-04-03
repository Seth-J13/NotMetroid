using UnityEngine;

public class CollisionManager : MonoBehaviour
{
    [SerializeField] private string eventType;
    GameObject samus;
    PlayerMovement samusMovement;

    //Awake
    private void Awake()
    {
        samus = transform.parent.parent.gameObject;
        samusMovement = samus.GetComponent<PlayerMovement>();
    }
    //Trigger Checkers
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(!collision.name.StartsWith("CircleBounds"))
        {
            if (eventType == "bonk")
            {
                if (samus.GetComponent<Rigidbody2D>().linearVelocityY >= 0)
                {
                    HandleBonk(collision.gameObject, transform.position.x - 0.1f, transform.position.y + 0.8f);
                    HandleBonk(collision.gameObject, transform.position.x + 0.1f, transform.position.y + 0.8f);
                    samusMovement.SetInSmallTunnel(true);
                }
            }
            if (eventType == "land" && (collision.gameObject.CompareTag("Ground") || collision.gameObject.CompareTag("MovingPlatform")))
            {
                samusMovement.LandUnMorph();
                GetComponent<BoxCollider2D>().enabled = false;
            }

        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(!collision.name.StartsWith("CircleBounds"))
        {
            if (eventType == "ground")
            {
                HandleGround(collision.gameObject);
            }
            else if (eventType == "bonk")
            {
                samusMovement.SetInSmallTunnel(true);
            }
            else if(eventType == "lava")
            {
                if(collision.gameObject.CompareTag("Lava") && !samus.GetComponent<NotSamusManager>().HasFireSuit())
                {
                    samus.GetComponent<PlayerMovement>().LavaTouch();
                }
            }
        }

    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!collision.name.StartsWith("CollisionBounds"))
        {
            if (eventType == "bonk")
            {
                samusMovement.SetInSmallTunnel(false);
            }
            if (eventType == "ground" && collision.isActiveAndEnabled)
            {
                samus.GetComponent<PlayerMovement>().SetGrounded(false);
                if (collision.gameObject.tag == "MovingPlatform")
                {
                    samus.transform.SetParent(null);
                }
            }
        }
    }

    //Methods
    private void HandleGround(GameObject other)
    {
        if (samus.GetComponent<Rigidbody2D>().linearVelocity.y < 0.01f)
        {
            if (other.name != "Collider")
                samus.GetComponent<PlayerMovement>().SetGrounded(true);
            if (other.tag == "MovingPlatform")
            {
                samus.transform.SetParent(other.transform);
            }
        }
    }
    private void HandleBonk(GameObject go, float x, float y)
    {
        if (go.tag == "Bonkable")
        {
            go.GetComponent<TileBonker>().Bonk(x, y);
        }
    }
}
