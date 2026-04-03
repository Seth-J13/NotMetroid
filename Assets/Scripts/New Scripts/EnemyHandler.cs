using UnityEngine;
using UnityEngine.SceneManagement;

public class EnemyHandler : MonoBehaviour
{
    public int health = 0;
    public int damage = 2;
    private GameObject rocketAmmo;
    private GameObject healthPack;

    public int GetEnemyHealth()
    {
        return health;
    }

    public void DamageEnemy(int damage)
    {
        health -= damage;
    }

    public int GetDamageDealt()
    {
        return damage;
    }

    private void DropRandItem()
    {
        for (int i = 0; i < Random.Range(1, 4); i++)
        {
            int item = Random.Range(0, 2);
            if(item == 0)
            {
                Instantiate(rocketAmmo).transform.position = new Vector3(this.transform.position.x + Random.Range(-0.5f, 0.5f),
                    this.transform.position.y + Random.Range(-0.5f, 0.5f), this.transform.position.z);
            }
            else
            {
                Instantiate(healthPack).transform.position = new Vector3(this.transform.position.x + Random.Range(-0.5f, 0.5f),
                    this.transform.position.y + Random.Range(-0.5f, 0.5f), this.transform.position.z);
            }

        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            collision.gameObject.GetComponent<NotSamusManager>().DamageNotSamus(damage);
        }
    }

    private void Awake()
    {
        rocketAmmo = Resources.Load<GameObject>("Prefabs/World Assets/SamusRelated/NotSamusRocketAmmo");
        healthPack = Resources.Load<GameObject>("Prefabs/World Assets/SamusRelated/NotSamusMedkit");
    }
    private void Update()
    {
        if(health <= 0)
        {
            DropRandItem();
            Destroy(this.gameObject);
        }
    }
}
