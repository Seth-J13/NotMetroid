using System;
using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class NotSamusManager : MonoBehaviour
{
    //Samus Info
    private int health = 3;
    private int rocketAmmo = 0;
    [SerializeField] private float beamForce = 10f;
    [SerializeField] private float rocketForce = 20f;

    //Upgrade Bools
    [SerializeField] private bool morphBallUpgrd = false;
    [SerializeField] private bool rocketShotsUpgrd = false;
    [SerializeField] private bool triBeamUpgrd = false;
    [SerializeField] private bool morphBombUpgrd = false;
    [SerializeField] private bool screwAttackUpgrd = false;
    [SerializeField] private bool plasmaShotUpgrd = false;
    [SerializeField] private bool fireSuitUpgrd = false;
    [SerializeField] private bool doubleJumpUpgrd = false;

    //GameObjects
    private GameObject beam;
    private GameObject plasmaBeam;
    private GameObject rocket;
    private GameObject morphBomb;

    //Scripts
    private PlayerMovement samusMovement;
    
    //Animators
    private Animator leftFireAni;
    private Animator rightFireAni;

    //Awake
    private void Awake()
    {
        samusMovement = GetComponent<PlayerMovement>();
        beam = Resources.Load<GameObject>("Prefabs/World Assets/SamusRelated/BeamBullet");
        plasmaBeam = Resources.Load<GameObject>("Prefabs/World Assets/SamusRelated/PlasmaBeamBullet");
        rocket = Resources.Load<GameObject>("Prefabs/World Assets/SamusRelated/Rocket");
        morphBomb = Resources.Load<GameObject>("Prefabs/World Assets/SamusRelated/MorphBomb");
        leftFireAni = GameObject.Find("Left Fire").GetComponent<Animator>();
        rightFireAni = GameObject.Find("Right Fire").GetComponent<Animator>();
    }
    //Methods
    //Health Methods
    public int GetHealth()
    {
        return health;
    }
    public void SetHealth(int health)
    {
        this.health = health;
        CheckIfDead();
    }
    public void AddHealth()
    {
        if(health < 3)
        {
            health++;
            CheckIfDead();
        }
    }
    private void CheckIfDead()
    {
        UpdateHealthUI();
        if (health <= 0)
        {
            GetComponentInChildren<AudioController>().PlayDeathSound();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            StartCoroutine(WaitForSound());
        }
    }
    //Damage Methods
    public void DamageNotSamus()
    {
        health--;
        CheckIfDead();
    }
    public void DamageNotSamus(int damage)
    {
        health -= damage;
        CheckIfDead();
    }
    //Ammo Methods
    //Rockets
    public int GetRocketAmmoAmount()
    {
        return rocketAmmo;
    }

    public void SetRocketAmmo(int rocketAmmo)
    {
        this.rocketAmmo = rocketAmmo;
        UpdateRocketUI();
    }
    public void UseRocketAmmo()
    {
        rocketAmmo--;
        UpdateRocketUI();
    }

    public void AddRocketAmmo()
    {
        rocketAmmo += 2;
        UpdateRocketUI();
    }
    //UI Methods
    private void UpdateRocketUI()
    {
        GameManager.Instance.UpdateRocketAmmoCount();
    }
    private void UpdateHealthUI()
    {
        GameManager.Instance.UpdateHealthUI(health);
    }
    //Upgrade Methods
    public bool HasMorphBall()
    {
        return morphBallUpgrd;
    }
    public void GotMorphBall(bool b)
    {
        morphBallUpgrd = b;
    }

    public void GotRocketShot (bool b)
    {
        rocketShotsUpgrd = b;
    }
    public bool HasRocketShot ()
    {
        return rocketShotsUpgrd;
    }

    public void GotTriBeam(bool b)
    {
        triBeamUpgrd = b;
    }
    public bool HasTriBeam()
    {
        return triBeamUpgrd;
    }

    public void GotMorphBomb(bool b)
    {
        morphBombUpgrd = b;
    }
    public bool HasMorphBomb()
    {
        return morphBombUpgrd;
    }

    public void GotScrewAttack(bool b)
    {
        screwAttackUpgrd = b;
    }
    public bool HasScrewAttack()
    {
        return screwAttackUpgrd;
    }

    public void GotPlasmaShot(bool b)
    {
        plasmaShotUpgrd = b;
    }
    public bool HasPlasmaShot()
    {
        return plasmaShotUpgrd;
    }

    public void GotFireSuit(bool b)
    {
        fireSuitUpgrd = b;
    }
    public bool HasFireSuit()
    {
        return fireSuitUpgrd;
    }

    public void GotDoubleJump(bool b)
    {
        doubleJumpUpgrd = b;
    }
    public bool HasDoubleJump()
    {
        return doubleJumpUpgrd;
    }
    //Samus Action Call Methods
    private void ChangeBeamDir(GameObject beamshot)
    {
        beamshot.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
        beamshot.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
    }
    public void FireBeam(Animator animator,bool lookingLeft = false)
    {
        if (!HasTriBeam())
        {
            //
            //Something is not right, it does not fire any bullets.
            //
            GameObject beamshot;
            if (HasPlasmaShot())
            {
                beamshot = Instantiate(plasmaBeam);
            }
            else
            {
                beamshot = Instantiate(beam);
            }
            beamshot.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
            if (lookingLeft)
            {
                if (HasPlasmaShot())
                    leftFireAni.SetBool("isFiringPlasma?", true);
                else
                    leftFireAni.Play("BeamShotLeft", 0, 1f);
                ChangeBeamDir(beamshot);
                beamshot.GetComponent<Rigidbody2D>().AddForceX(-beamForce, ForceMode2D.Impulse);
            }
            else
            {
                if (HasPlasmaShot())
                    rightFireAni.SetBool("isFiringPlasma?", true);
                else
                    rightFireAni.Play("BeamShot", 0, 1f);
                beamshot.GetComponent<Rigidbody2D>().AddForceX(beamForce, ForceMode2D.Impulse);
            }
            StartCoroutine(ShootBullets(animator));
        }
        else
            FireTriBeam(animator, lookingLeft);
    }

    public void FireTriBeam(Animator animator, bool lookingLeft = false)
    {
        float offset = -0.3f;
        for(int i = 0; i < 3; i++)
        {
            GameObject beamshot;
            if (HasPlasmaShot())
            {
                beamshot = Instantiate(plasmaBeam);
            }
            else
            {
                beamshot = Instantiate(beam);
            }
            beamshot.transform.position = new Vector3(transform.position.x, transform.position.y + offset, transform.position.z);
            if (lookingLeft)
            {
                if (HasPlasmaShot())
                    leftFireAni.SetBool("isFiringPlasma?", true);
                else
                    leftFireAni.Play("BeamShotLeft", 0, 1f);
                ChangeBeamDir(beamshot);
                beamshot.GetComponent<Rigidbody2D>().AddForceX(-beamForce, ForceMode2D.Impulse);
            }
            else
            {
                if (HasPlasmaShot())
                    rightFireAni.SetBool("isFiringPlasma?", true);
                else
                    rightFireAni.Play("BeamShot", 0 , 1f);
                beamshot.GetComponent<Rigidbody2D>().AddForceX(beamForce , ForceMode2D.Impulse);
            }
            offset += 0.5f;
        }
        StartCoroutine(ShootBullets(animator));
    }
    public void FireRocket(Animator animator, bool lookingLeft = false)
    {
        UseRocketAmmo();
        GameObject rocketshot = Instantiate(rocket);
        rocketshot.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        if (lookingLeft)
        {
            leftFireAni.Play("RocketShotLeft", 0, 1f);
            rocketshot.GetComponent<Rigidbody2D>().AddForceX(-rocketForce, ForceMode2D.Impulse);
            rocketshot.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
            rocketshot.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
        }
        else
        {
            rightFireAni.Play("RocketShot", 0, 1f);
            rocketshot.GetComponent<Rigidbody2D>().AddForceX(rocketForce, ForceMode2D.Impulse);
        }
        StartCoroutine(ShootRockets(animator));
    }
    
    public void DropMorphBomb()
    {
        Instantiate(morphBomb).transform.position = transform.position;
        StartCoroutine(WaitToDropMorphBomb());
    }
    //Enumerators
    IEnumerator WaitForSound()
    {
        yield return new WaitForSeconds(0.5f);
        //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        GameManager.Instance.RestartLevel(SceneManager.GetActiveScene().name);
    }
    IEnumerator ShootBullets(Animator animator)
    {
        yield return new WaitForSeconds(0.35f);
        animator.SetBool("isShooting?", false);
        leftFireAni.SetBool("isFiringBeam?", false);
        rightFireAni.SetBool("isFiringBeam?", false);
        leftFireAni.SetBool("isFiringPlasma?", false);
        rightFireAni.SetBool("isFiringPlasma?", false);
        samusMovement.SetShooting(false);
    }
    IEnumerator ShootRockets(Animator animator)
    {
        yield return new WaitForSeconds(0.35f);
        animator.SetBool("isFiringRocket?", false);
        leftFireAni.SetBool("isFiringRocket?", false);
        rightFireAni.SetBool("isFiringRocket?", false);
        samusMovement.SetFiring(false);
    }
    IEnumerator WaitToDropMorphBomb()
    {
        yield return new WaitForSeconds(0.25f);
        samusMovement.SetShooting(false);
    }
}