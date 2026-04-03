using UnityEngine;
using UnityEngine.Tilemaps;


public class TileBonker : MonoBehaviour
{
    private Tilemap tilemap;
    [SerializeField] private string tileMapOne;
    [SerializeField] private string tileMapTwo;
    [SerializeField] private string tileMapThree;
    [SerializeField] private string tileNameToRemove;
    //public TileBase emptyblock;
    
    //Start
    void Start()
    {
        tilemap = GetComponent<Tilemap>();
    }

    //Methods
    public void Bonk(float a, float b)
    {
        if (gameObject.name == tileMapOne)
        {
            Vector3Int gridPosition = tilemap.WorldToCell(new Vector2(a, b));
            TileBase t = tilemap.GetTile(gridPosition);
            if (t != null)
            {
                if (t.name == tileNameToRemove)
                    tilemap.SetTile(gridPosition, null);
                //if (t.name == "questionblock")
                //    tilemap.SetTile(gridPosition, emptyblock);
            }
        }
        else if (gameObject.name == tileMapTwo)
        {
            Vector3Int gridPosition = tilemap.WorldToCell(new Vector2(a, b));
            TileBase t = tilemap.GetTile(gridPosition);
            Tilemap main = GameObject.Find(tileMapOne).GetComponent<Tilemap>();
            if (t != null)
            {
                tilemap.SetTile(gridPosition, null);
                //main.SetTile(gridPosition, emptyblock);
            }
        }
    }
    public void LavaDetect(GameObject sam)
    {
        print("Detecting LAVA!! AHHH");
        if (this.gameObject.name == tileMapThree)
        {
            sam.GetComponent<NotSamusManager>().DamageNotSamus();            
        }
    }
}
