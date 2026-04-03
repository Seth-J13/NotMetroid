using UnityEngine;

public class BulletHandler : MonoBehaviour
{
    [SerializeField] private int damage = 0;
    private void Awake()
    {
        if (gameObject.name.StartsWith("BeamBullet"))
            gameObject.name = "BeamBullet";
        else if (gameObject.name.StartsWith("Rocket"))
            gameObject.name = "Rocket";
        else if (gameObject.name.StartsWith("PlasmaBeamBullet"))
            gameObject.name = "PlasmaBeamBullet";
            Destroy(this.gameObject, 3f);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            collision.gameObject.GetComponent<EnemyHandler>().DamageEnemy(damage);
            Destroy(this.gameObject);
        }        
    }

}
