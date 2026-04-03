using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class MorphBombHandler : MonoBehaviour
{
    [SerializeField] private int damage = 2;
    [SerializeField] private float bombDelay = 1.75f;
    [SerializeField] private float pushForce = 3f;
    private bool exploded = false;

    private void Awake()
    {
        StartCoroutine(BombSpawn());
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            collision.gameObject.GetComponent<EnemyHandler>().DamageEnemy(damage);
        }
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player") && exploded)
        {
            exploded = false;

            collision.gameObject.GetComponent<Rigidbody2D>().linearVelocityY = 0;
            collision.gameObject.GetComponent<Rigidbody2D>().AddForce(new Vector2(collision.transform.position.x - transform.position.x,
                collision.transform.position.y - transform.position.y).normalized * new Vector2(pushForce, pushForce), ForceMode2D.Impulse);
        }
    }

    IEnumerator BombSpawn()
    {
        yield return new WaitForSeconds(bombDelay);
        exploded = true;
        GetComponent<CircleCollider2D>().enabled = true;
        GetComponent<Animator>().SetBool("hasExploded?", true);
        yield return new WaitForSeconds(0.25f);
        Destroy(gameObject);
    }
}
