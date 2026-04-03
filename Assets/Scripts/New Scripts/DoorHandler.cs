using UnityEngine;
using UnityEngine.SceneManagement;

public class DoorHandler : MonoBehaviour
{
    public string level = "";
    public bool rocketDoor;
    public bool beamDoor;
    public bool plasmaDoor;
    public Vector2 samPlacement;
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "Player" && level != "")
        {
            print("Going to next level: (" + samPlacement.x + ", " + samPlacement.y + ")");
            if (samPlacement != null || samPlacement != Vector2.zero)
            { 
                GameManager.Instance.SetLasPos(samPlacement);

            }
            GameManager.Instance.GoToNextLevel(level);
        }
        else
        {
            if ((collision.gameObject.name == "Rocket" || collision.gameObject.name == "PlasmaBeamBullet")
                && rocketDoor)
            {
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }
            if ((collision.gameObject.name == "BeamBullet" || collision.gameObject.name == "PlasmaBeamBullet") &&
                beamDoor)
            {
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }
            if(collision.gameObject.name == "PlasmaBeamBullet" && plasmaDoor)
            {
                Destroy(collision.gameObject);
                Destroy(this.gameObject);
            }
            if(collision.gameObject.name == "BeamBullet" && (rocketDoor || plasmaDoor))
            {
                    Destroy(collision.gameObject);
            }
            else if(collision.gameObject.name == "Rocket" && (beamDoor || plasmaDoor))
            {
                Destroy(collision.gameObject);
            }
            else if((collision.gameObject.name == "Rocket" || collision.gameObject.name == "BeamBullet") &&
                plasmaDoor)
            {
                Destroy(collision.gameObject);
            }
        }
    }
}
