using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;

public class PlayerMovement : MonoBehaviour
{
    NotSamusManager samusManager;
    Rigidbody2D body;
    Animator animator;
    Animator screwAni;
    AudioController audioController;
    GameObject bonk;
    GameObject ground;
    GameObject anchor;
    [SerializeField] private float moveForce = 5;
    [SerializeField] private float morphForce = 10;
    [SerializeField] private float jumpForce = 2.5f;
    private float xMovement;
    private float tempForce;
    private float speedLimit = 30f;
    private int jumpCount = 1;
    private bool grounded = true;
    private bool firing = false;
    private bool shooting = false;
    private bool canHurtFromLava = true;
    private bool crouching = false;
    private bool jumping = false;
    private bool inSmallTunnel = false;
    private bool lookingLeft;
    private float rotationMovement;
    private Quaternion initRot;

    private void Awake()
    {
        samusManager = GetComponent<NotSamusManager>();
        body = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        screwAni = transform.GetChild(1).GetComponent<Animator>();
        audioController = GetComponentInChildren<AudioController>();
        bonk = GameObject.FindGameObjectWithTag("BonkDetector");
        ground = GameObject.FindGameObjectWithTag("GroundDetector");
        anchor = GameObject.FindGameObjectWithTag("Anchor");
        initRot = anchor.transform.localRotation;
    }
    //Update
    private void Update()
    {
        animator.SetFloat("xVelocity", Mathf.Round(xMovement * moveForce * Time.deltaTime));
        if (!firing)
        {
            if (!(body.linearVelocityX > speedLimit))
            {
                body.AddForceX(xMovement * moveForce * Time.deltaTime);
            }
            if ((crouching || jumping) && !shooting)
            {
                rotationMovement -= body.linearVelocityX * (moveForce/2) * 0.2f * Time.deltaTime;
                transform.rotation = Quaternion.AngleAxis(rotationMovement, Vector3.forward);
            }
        }
        if (xMovement != 0 && grounded && !firing)
        {
            animator.SetBool("LookingLeft", xMovement < 0);
            lookingLeft = xMovement < 0;
        }
    }
    private void FixedUpdate()
    {
        animator.SetFloat("yVelocity", body.linearVelocityY);
    }
    private void LateUpdate()
    {
        anchor.transform.rotation = initRot;
    }
    //Methods
    public void SetGrounded(bool g)
    {
        grounded = g;
        jumpCount = 1;
    }
    public void SetFiring(bool b)
    {
        firing = b;
        screwAni.SetBool("isFiring?", false);
        screwAni.SetBool("isJumping?", false);

    }
    public void SetShooting(bool b)
    {
        shooting = b;
        screwAni.SetBool("isShooting?", false);
        screwAni.SetBool("isJumping?", false);
    }
    public void SetInSmallTunnel(bool b)
    {
        inSmallTunnel = b;
    }
    public void Morph()
    {
        BoxCollider2D box = GetComponent<BoxCollider2D>();
        CircleCollider2D circle = GetComponent<CircleCollider2D>();
        //Getting ground detector
        BoxCollider2D grounder = transform.GetChild(0).GetChild(1).GetComponentInChildren<BoxCollider2D>();
            tempForce = jumpForce;
            jumpForce = morphForce;
            box.enabled = false;
            circle.enabled = true;
            grounder.size = new Vector2(0.3f, 1);
            bonk.transform.localPosition = new Vector3(0, 0.575f, 0);
            ground.transform.localPosition = new Vector3(0, -0.475f, 0);
    }
    private void UnMorph()
    {
        if(!crouching)
        {
            BoxCollider2D box = GetComponent<BoxCollider2D>();
            CircleCollider2D circle = GetComponent<CircleCollider2D>();
            //Getting ground detector
            BoxCollider2D grounder = transform.GetChild(0).GetChild(1).GetComponentInChildren<BoxCollider2D>();
            jumpForce = tempForce;
            box.enabled = true;
            circle.enabled = false;
            grounder.size = new Vector2(0.9f, 1);
            bonk.transform.localPosition = new Vector3(0, 0.95f, 0);
            ground.transform.localPosition = new Vector3(0, -0.95f, 0);
            transform.rotation = Quaternion.identity;
            rotationMovement = 0f;
        }
    }
    public void LandUnMorph()
    {
        if(!crouching)
        {
            animator.SetBool("isCrouching?", !animator.GetBool("isCrouching?"));
            jumping = animator.GetBool("isCrouching?");
            if (samusManager.HasScrewAttack())
            {
                screwAni.SetBool("isJumping?", false);
                screwAni.SetBool("LookingLeft", false);
            }
            UnMorph();
        }
    }
    public void LavaTouch()
    {
        if(canHurtFromLava)
        {
            canHurtFromLava = false;
            StartCoroutine(LavaHurt());
        }
    }
    //Trigger Interactions
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "DeathZone")
        {
            audioController.PlayDeathSound();
            GameManager.Instance.GetNotSamusInfo();
            samusManager.SetHealth(0);
        }
        else if (collision.gameObject.tag == "RocketAmmo")
        {
            samusManager.AddRocketAmmo();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "HealthPack")
        {
            samusManager.AddHealth();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "MorphUpgrd")
        {
            samusManager.GotMorphBall(true);
            GameManager.Instance.GetNotSamusInfo();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.tag == "RocketUpgrd")
        {
            samusManager.GotRocketShot(true);
            GameManager.Instance.GetNotSamusInfo();
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.tag == "TriBeamUpgrd")
        {
            samusManager.GotTriBeam(true);
            GameManager.Instance.GetNotSamusInfo();
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.name == "MorphBombUpgrd")
        {
            samusManager.GotMorphBomb(true);
            GameManager.Instance.GetNotSamusInfo();
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.name == "ScrewAttackUpgrd")
        {
            samusManager.GotScrewAttack(true);
            GameManager.Instance.GetNotSamusInfo();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.name == "DoubleJumpUpgrd")
        {
            samusManager.GotDoubleJump(true);
            GameManager.Instance.GetNotSamusInfo();
            Destroy(collision.gameObject);
        }
        else if (collision.gameObject.name == "PlasmaUpgrd")
        {
            samusManager.GotPlasmaShot(true);
            GameManager.Instance.GetNotSamusInfo();
            Destroy(collision.gameObject);
        }
        else if(collision.gameObject.name == "HeatSuitUpgrd")
        {
            samusManager.GotFireSuit(true);
            GameManager.Instance.GetNotSamusInfo();
            Destroy(collision.gameObject);
        }
    }
    //Action Maps
    void OnRestart(InputValue v)
    {
        if (v.Get<float>() == 1)
            GameManager.Instance.GoToNextLevel(SceneManager.GetActiveScene().name);
            //SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    void OnMove(InputValue v)
    {
        xMovement = v.Get<Vector2>().x;
    }

    void OnJump(InputValue v)
    {
        int force = lookingLeft ? -150 : 150;
        if (xMovement == 0)
            force = 0;
        
        if (grounded && !firing)
        {
            if(!crouching)
            {
                animator.SetBool("isCrouching?", !animator.GetBool("isCrouching?"));
                jumping = animator.GetBool("isCrouching?");
                Morph();
            }
            body.AddForce(new Vector2(force * Time.deltaTime,jumpForce), ForceMode2D.Impulse);
            grounded = false;
            transform.SetParent(null);
        }
        if(samusManager.HasDoubleJump() && jumpCount > 0 && !grounded)
        {
            if (body.linearVelocityY >= -2.5f && body.linearVelocityY <= 10 && !firing)
            {
                if (!jumping && !crouching)
                    Morph();
                body.linearVelocityY = 0;
                if(!(body.linearVelocityY > speedLimit -20))
                    body.AddForce(new Vector2(force * Time.deltaTime, jumpForce), ForceMode2D.Impulse);
                grounded = false;
                transform.SetParent(null);
                jumpCount--;
            }
        }
        StartCoroutine(WaitForJump());
        if (samusManager.HasScrewAttack() && jumping)
        {
            if (lookingLeft)
                screwAni.SetBool("LookingLeft", true);
            screwAni.SetBool("isJumping?", jumping);
        }
    }

    void OnCrouch(InputValue v)
    {
        if (!samusManager.HasMorphBall())
            return;
        if(!inSmallTunnel && grounded)
        {
            animator.SetBool("isCrouching?", !animator.GetBool("isCrouching?"));
            crouching = animator.GetBool("isCrouching?");
            if (crouching)
                Morph();
            else
                UnMorph();
        }
    }

    void OnShoot(InputValue v)
    {
        if(!crouching && !shooting)
        {
            if (animator.GetBool("isCrouching?") && jumping)
            {
                jumping = false;
                transform.GetChild(0).GetChild(2).GetComponent<BoxCollider2D>().enabled = false;
                animator.SetBool("isCrouching?", false);
                UnMorph();
            }
            transform.rotation = Quaternion.identity;
            rotationMovement = 0f;
            audioController.PlayBeamShotSound();
            shooting = true;
            animator.SetBool("isShooting?", true);
            screwAni.SetBool("isShooting?", true);
            samusManager.FireBeam(animator, lookingLeft);
        }
        else if(crouching && !shooting)
        {
            shooting = true;
            samusManager.DropMorphBomb();
        }    
    }

    void OnRocket(InputValue v)
    {
        if (!samusManager.HasRocketShot())
            return;
        if(samusManager.GetRocketAmmoAmount() > 0)
        {
            if (v.Get<float>() == 1 && !crouching && !firing)
            {
                animator.SetBool("isFiringRocket?", true);
                screwAni.SetBool("isFiring?", true);
                firing = true;
            }
            if(v.Get<float>() == 0 && !crouching)
            {
                audioController.PlayRocketShotSound();
                samusManager.FireRocket(animator, lookingLeft);

            }
        }
    }
    //Corutines
    IEnumerator WaitForJump()
    {
        yield return new WaitForSeconds(0.15f);
        transform.GetChild(0).GetChild(2).GetComponent<BoxCollider2D>().enabled = true;
    }
    IEnumerator LavaHurt()
    {
        yield return new WaitForSeconds(1f);
        samusManager.DamageNotSamus();
        canHurtFromLava = true;
    }
}