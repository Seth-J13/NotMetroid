using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance;
    private NotSamusManager notSamusManager;
    private GameObject sam;
    private TextMeshProUGUI ammoText;
    private TextMeshProUGUI healthText;
    private string currScene;

    //Not Samus Info
    private Vector2 lasPos = Vector2.zero;
    private int health;
    private int rocketAmmo;
    private bool morphBallUpgrd = false;
    private bool rocketShotsUpgrd = false;
    private bool triBeamUpgrd = false;
    private bool morphBombUpgrd = false;
    private bool screwAttackUpgrd = false;
    private bool plasmaShotUpgrd = false;
    private bool fireSuitUpgrd = false;
    private bool doubleJumpUpgrd = false;



    public static GameManager Instance
    {
        get
        {
            if (instance == null)
            {
                GameObject go = new GameObject("GameManager");
                go.AddComponent<GameManager>();
            }
            return instance;
        }
    }
    //OnEnable/Disable 
    private void OnEnable()
    {
        SceneManager.sceneLoaded += ReloadAfterDeath;
    }
    private void OnDisable()
    {
        SceneManager.sceneLoaded -= ReloadAfterDeath;
    }
   
    //Awake
    private void Awake() //Awake runs before start, this makes sure that the game properly loads the game manager before anything tries to mess with it
    {
        if (instance != null && instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        SetCamera();
        if (GameObject.Find("NotSamus") == false)
        {
            Instance.PlaceNotSamus();
        }
        Instance.SetNotSamManager();
        Instance.UpdateHealthUI();
        Instance.UpdateRocketAmmoCount();
    }

    //Set Methods
    private void SetNotSamManager()
    {
        Instance.notSamusManager = Instance.sam.GetComponent<NotSamusManager>();
    }
    private void SetCamera()
    {
        //Set Camera on Samus
        GameObject cam = GameObject.Find("Main Camera");
        if (cam != null)
        {
            try
            {
                PlatformerCamera camComponent = cam.AddComponent<PlatformerCamera>();
                camComponent.minY = -40;
                camComponent.windowSize = 0.1f;
            }
            catch
            {
                print("Critical error adding Platformer Camera component");
            }
        }
    }
    public void SetLasPos(Vector2 samPlacement)
    {
        Instance.lasPos = samPlacement;
    }

    //UI Methods
    public void UpdateRocketAmmoCount()
    {
        ammoText = GameObject.Find("Ammo Count").GetComponent<TextMeshProUGUI>();
        ammoText.text = "Rocket Ammo: " + Instance.notSamusManager.GetRocketAmmoAmount();
    }
    public void UpdateHealthUI()
    {
        healthText = GameObject.Find("Health").GetComponent<TextMeshProUGUI>();
        healthText.text = "Health: " + Instance.notSamusManager.GetHealth();
    }
    public void UpdateHealthUI(int health)
    {
        healthText = GameObject.Find("Health").GetComponent<TextMeshProUGUI>();
        healthText.text = "Health: " + health;
    }

    //Scene Management
    public void GoToNextLevel(string lv)
    {
        Instance.GetNotSamusInfo();
        SceneManager.LoadScene(lv);
        Instance.StartCoroutine(LoadNotSamus(lv));
    }
    public void RestartLevel(string lv)
    {
        SceneManager.LoadScene(lv);
        Instance.StartCoroutine(LoadNotSamus(lv));
    }
    void ReloadAfterDeath(Scene sc, LoadSceneMode mode)
    {
        if (currScene == sc.name)
        {
            if(GameObject.Find("NotSamus") == null)
                Instance.PlaceNotSamus();
            Instance.SetCamera();
            print("About to give sam info");
            Instance.GiveSamusInfo();
        }
    }
    public void GetNotSamusInfo()
    {
        Instance.health = Instance.notSamusManager.GetHealth();
        Instance.rocketAmmo = Instance.notSamusManager.GetRocketAmmoAmount();
        Instance.morphBallUpgrd = Instance.notSamusManager.HasMorphBall();
        Instance.rocketShotsUpgrd = Instance.notSamusManager.HasRocketShot();
        Instance.morphBombUpgrd = Instance.notSamusManager.HasMorphBomb();
        Instance.plasmaShotUpgrd = Instance.notSamusManager.HasPlasmaShot();
        Instance.triBeamUpgrd = Instance.notSamusManager.HasTriBeam();
        Instance.doubleJumpUpgrd = Instance.notSamusManager.HasDoubleJump();
        Instance.fireSuitUpgrd = Instance.notSamusManager.HasFireSuit();
        print("Got sam info, morphUpgrd = " + Instance.morphBallUpgrd);
    }

    public void GiveSamusInfo()
    {
        Instance.notSamusManager.SetHealth(Instance.health);
        Instance.notSamusManager.SetRocketAmmo(Instance.rocketAmmo);
        print("Giving sam info, morphUpgrd = " + Instance.morphBallUpgrd);
        Instance.notSamusManager.GotMorphBall(Instance.morphBallUpgrd);
        Instance.notSamusManager.GotRocketShot(Instance.rocketShotsUpgrd);
        Instance.notSamusManager.GotMorphBomb(Instance.morphBombUpgrd);
        Instance.notSamusManager.GotPlasmaShot(Instance.plasmaShotUpgrd);
        Instance.notSamusManager.GotTriBeam(Instance.triBeamUpgrd);
        Instance.notSamusManager.GotDoubleJump(Instance.doubleJumpUpgrd);
        Instance.notSamusManager.GotFireSuit(Instance.fireSuitUpgrd);

    }
    private void PlaceNotSamus()
    {
        Instance.sam = Instantiate(Resources.Load<GameObject>("Prefabs/Crucial Assests/NotSamus"));
        Instance.sam.transform.position = new Vector3(lasPos.x, lasPos.y);
        Instance.sam.name = "NotSamus";
        Instance.SetNotSamManager();    
    }
    IEnumerator LoadNotSamus(string lv)
    {
        yield return new WaitUntil(() => SceneManager.GetActiveScene().name == lv);
        currScene = lv;
        bool foundNotSam = GameObject.Find("NotSamus");
        bool onCorrectScene = SceneManager.GetActiveScene().name == lv;
        if (!foundNotSam && onCorrectScene && lv != "Win Screen")
        {
            Instance.PlaceNotSamus();
            Instance.GiveSamusInfo();
        }
        else if(foundNotSam && onCorrectScene && lv != "Win Screen")
        {
            Instance.GiveSamusInfo();
        }
        Instance.SetCamera();
    }
}
