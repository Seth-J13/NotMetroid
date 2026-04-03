using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class PlatformMovement : MonoBehaviour
{
    NotSamusManager samManager;
    GameManager gameManager;
    Rigidbody2D body;
    Animator animator;
    Animator leftFireAni;
    Animator rightFireAni;
    GameObject bonk;
    GameObject ground;
    GameObject anchor;
    GameObject beam;
    GameObject rocket;
    public float moveForce = 5;
    public float jumpForce = 2.5f;
    public int beamForce = 12;
    public int rocketForce = 20;
    public int speedLimit = 30;
    float xMovement;
    bool grounded = true;
    bool firing = false;
    bool shooting = false;
    bool crouching = false;
    bool lookingLeft;
    float rotationMovement;
    Quaternion initRot;

    private void Awake()
    {
        samManager = GetComponent<NotSamusManager>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        beam = Resources.Load<GameObject>("Prefabs/BeamBullet");
        rocket = Resources.Load<GameObject>("Prefabs/Rocket");
    }
    void Start()
    {
        body = GetComponent<Rigidbody2D>(); 
        animator = GetComponent<Animator>();
        leftFireAni = GameObject.Find("Left Fire").GetComponent<Animator>();
        rightFireAni = GameObject.Find("Right Fire").GetComponent<Animator>();
        bonk = GameObject.FindGameObjectWithTag("BonkDetector");
        ground = GameObject.FindGameObjectWithTag("GroundDetector");
        anchor = GameObject.FindGameObjectWithTag("Anchor");
        initRot = anchor.transform.localRotation;
        //gameManager.UpdateRocketAmmoCount();
        GameManager.Instance.UpdateRocketAmmoCount();
        //gameManager.UpdateHealthUI();
        GameManager.Instance.UpdateHealthUI();  
    }
    
    void OnRestart(InputValue v)
    {
        if (v.Get<float>() == 1)
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void OnMove(InputValue v)
    {
        xMovement = v.Get<Vector2>().x;
    }

    void OnJump(InputValue v)
    {
        if (grounded && !firing)
        {
            body.AddForceY(jumpForce, ForceMode2D.Impulse);
            grounded = false;
            transform.SetParent(null);
        }
    }
    
    void OnCrouch(InputValue v)
    {
        if(!samManager.HasMorphBall())
        {
            return;
        }
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        BoxCollider2D grounder = transform.GetChild(0).GetChild(1).GetComponentInChildren<BoxCollider2D>();
        animator.SetBool("isCrouching?", !animator.GetBool("isCrouching?"));
        crouching = animator.GetBool("isCrouching?");
        if (crouching)
        {
            box.enabled = false;
            circle.enabled = true;
            grounder.size = new Vector2(0.3f, 1);
            bonk.transform.localPosition = new Vector3(bonk.transform.localPosition.x, bonk.transform.localPosition.y / 2, bonk.transform.localPosition.z);
            ground.transform.localPosition = new Vector3(ground.transform.localPosition.x, ground.transform.localPosition.y / 2, ground.transform.localPosition.z);
        }
        else
        {
            box.enabled = true;
            circle.enabled = false;
            grounder.size = new Vector2(0.9f, 1);
            bonk.transform.localPosition = new Vector3(bonk.transform.localPosition.x, bonk.transform.localPosition.y * 2, bonk.transform.localPosition.z);
            ground.transform.localPosition = new Vector3(ground.transform.localPosition.x, ground.transform.localPosition.y * 2, ground.transform.localPosition.z);
            transform.rotation = Quaternion.identity;
            rotationMovement = 0f;
        }
    }

    void OnRocket(InputValue v)
    {
        if(!samManager.HasRocketShot())
        {
            return;
        }
        if (samManager.GetRocketAmmoAmount() > 0)
        {
            if (v.Get<float>() == 1 && !crouching && !firing)
            {
                animator.SetBool("isFiringRocket?", true);
                firing = true;
            }
            if (v.Get<float>() == 0 && !crouching)
            {
                GetComponentInChildren<AudioController>().PlayRocketShotSound();
                samManager.UseRocketAmmo();
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
                StartCoroutine(ShootRockets());
            }
        }
    }
    void OnShoot(InputValue v)
    {
        if(!crouching && !shooting)
        {
            GetComponentInChildren<AudioController>().PlayBeamShotSound();
            shooting = true;
            animator.SetBool("isShooting?", true);
            GameObject beamshot = Instantiate(beam);
            beamshot.transform.position = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
            if(lookingLeft)
            {
                leftFireAni.Play("BeamShotLeft", 0, 1f);
                beamshot.transform.rotation = Quaternion.AngleAxis(180, Vector3.forward);
                beamshot.transform.rotation = Quaternion.AngleAxis(180, Vector3.up);
                beamshot.GetComponent<Rigidbody2D>().AddForceX(-beamForce, ForceMode2D.Impulse);
            }
            else
            {
                rightFireAni.Play("BeamShot", 0, 1f);
                beamshot.GetComponent<Rigidbody2D>().AddForceX(beamForce, ForceMode2D.Impulse);
            }
            StartCoroutine(ShootBullets());
        }

    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.gameObject.tag == "DeathZone")
        {
            GetComponentInChildren<AudioController>().PlayDeathSound();
            //GameManager.Instance.GetNotSamusInfo();
            GetComponent<SpriteRenderer>().enabled = false;
            GetComponent<PlayerInput>().enabled = false;
            StartCoroutine(WaitForSound());
        }

        if (collision.gameObject.tag == "RocketAmmo")
        {
            samManager.AddRocketAmmo();
            Destroy(collision.gameObject);
        }

        if (collision.gameObject.tag == "HealthPack")
        {
            samManager.AddHealth();
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "MorphUpgrd")
        {
            samManager.GotMorphBall(true);
            Destroy(collision.gameObject);
        }

        if(collision.gameObject.tag == "RocketUpgrd")
        {
            samManager.GotRocketShot(true);
            Destroy(collision.gameObject);
        }
        
        if(collision.gameObject.name == "Riddler Speech")
        {
            GetComponentInChildren<AudioController>().RiddlerSpeech();
        }
    }

    public void SetGrounded(bool g)
    {
        grounded = g;  
    }

    void FixedUpdate()
    {
        animator.SetFloat("yVelocity", body.linearVelocityY);
    }

    void Update()
    {
        animator.SetFloat("xVelocity", Mathf.Round(xMovement * moveForce * Time.deltaTime));
        if (!firing)
        {
            if(!(body.linearVelocityX > speedLimit))
            {
                body.AddForceX(xMovement * moveForce * Time.deltaTime);
            }
            if (crouching)
            {
                rotationMovement -= body.linearVelocityX * 0.2f;
                transform.rotation = Quaternion.AngleAxis(rotationMovement, Vector3.forward);
            }
        }
        if (xMovement != 0 && grounded && !firing)
        {
            animator.SetBool("LookingLeft", xMovement < 0);
            lookingLeft = xMovement < 0;
        }
    }

    void LateUpdate()
    {
        anchor.transform.rotation = initRot;
    }

    IEnumerator WaitForSound()
    {
        yield return new WaitForSeconds(0.5f);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);

    }
    IEnumerator ShootBullets()
    {
        yield return new WaitForSeconds(0.25f);
        animator.SetBool("isShooting?", false);
        leftFireAni.SetBool("isFiringBeam?", false);
        rightFireAni.SetBool("isFiringBeam?", false);
        shooting = false;
    }

    IEnumerator ShootRockets()
    {
        yield return new WaitForSeconds(0.35f);
        animator.SetBool("isFiringRocket?", false);
        leftFireAni.SetBool("isFiringRocket?", false);
        rightFireAni.SetBool("isFiringRocket?", false);
        firing = false;
    }
}
