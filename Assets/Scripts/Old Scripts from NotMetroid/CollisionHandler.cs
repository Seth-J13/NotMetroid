using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Tilemaps;


public class CollisionHandler : MonoBehaviour
{
    public string eventType;
    void Start()
    {

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (eventType == "bonk")
        {
            if (transform.parent.parent.gameObject.GetComponent<Rigidbody2D>().linearVelocityY >= 0)
            {
                HandleBonk(collision.gameObject, transform.position.x - 0.1f, transform.position.y + 0.8f);
                HandleBonk(collision.gameObject, transform.position.x + 0.1f, transform.position.y + 0.8f);
            }
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if(eventType == "ground")
        {
            HandleGround(collision.gameObject);
        }
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        if (eventType == "ground" && collision.isActiveAndEnabled)
        {
            transform.parent.parent.gameObject.GetComponent<PlatformMovement>().SetGrounded(false);
            if (collision.gameObject.tag == "MovingPlatform")
            {
                transform.parent.parent.SetParent(null);
            }
        }
    }
    private void HandleGround(GameObject other)
    {
        if (transform.parent.parent.gameObject.GetComponent<Rigidbody2D>().linearVelocity.y < 0.01f)
        {
            if(other.name != "Collider")
                transform.parent.parent.gameObject.GetComponent<PlatformMovement>().SetGrounded(true);
            if(other.tag == "MovingPlatform")
            {
                transform.parent.parent.SetParent(other.transform);
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

    // Update is called once per frame
    void Update()
    {
        
    }
}
