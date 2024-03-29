using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    //Constants
    public const float SPEED_MULTIPLIER = 5.0f;

    //love velocity increase
    private float loveVelocityMultiplier = 1.0f;
    private float loveVelocityIncrease = 0.04f; //how much the multiplier increases by per NPC collided with

    //win condition stuff
    public TextMeshProUGUI EndGameText;
    public TextMeshProUGUI TargetsTotalReadout;
    public TextMeshProUGUI TargetsCaughtReadout;
    private int TargetsTotal;
    private int TargetsCaught;
    private bool win = false;

    //Animator
    private Animator animator;
    //Grounded, MidAir, Gliding, Dead

    //ParticleSpawner
    public GameObject ParticleSpawner;

    //Timer mechanic
    public TextMeshProUGUI scoreText;
    private TimerBehaviourScript timer;
    private int score = 0;
    private bool alive = true;

    //Physics
    private Rigidbody body;

    //ground check
    private float floatingOffGroundOffset = 2.0f; //CHANGE ME IF YOU WISH THE PLAYER TO FLOAT FURTHER OR CLOSER TO GROUND

    //Camera
    public Camera Camera;
    private CameraBehaviourScript CameraBehaviour;

    //Looking around & Camera offset ability
    private Ray cameraToCursorRay;
    private RaycastHit cameraToCursorHitPos;
    private Vector3 LookAtPosition = Vector3.zero;
    private bool cameraToCursorHit;
    private bool offsetCamera = false;

    //jumping
    private float jumpMultiplier = 75.0f;
    private Vector3 jumpForce;
    private bool midAir = false;
    private bool gliding = false;

    //gliding
    private bool glideInputTriggered = false;

    //death
    private bool deathStuffTriggered = false;

    //debug feedback
    //public Material StateGround;
    //public Material StateAir;
    //public Material StateGlide;
    //private MeshRenderer meshRenderer;


    public void Kill()//kills player
    {
        alive = false;
    }

    public bool IsAlive()
    {
        return alive;
    }

    public bool HasWon()
    {
        return win;
    }

    bool IsGrounded()//returns result of a raycast pointing down and adjustable length.
    {
        return Physics.Raycast(transform.position, Vector3.down, floatingOffGroundOffset, 1 << LayerMask.NameToLayer("Ground"));
    }


    void ToggleGlide(bool toggle)//adjusts the rigid body's drag to allow regular falling, or a slowed fall for gliding.
    {
        if (toggle)
        {
            body.drag = 5.0f;
        }
        else
        {
            body.drag = 0.05f;
        }
    }

    void FallingContraints(bool toggle)//frees rotation during fall, used to also lock y-axis but that was a bandaid solution which was patched out.
    {
        if (toggle)
        {
            
            body.constraints = RigidbodyConstraints.None;
        }
        else //if hit ground
        {
            body.constraints = RigidbodyConstraints.FreezeRotation;


        }
    }

    IEnumerator Jump()//prepares body for and adds force of the jump.
    {
        ToggleGlide(false);
        FallingContraints(true);

        body.AddForce(jumpForce,ForceMode.Force);
        yield return null; 

    }

    // Start is called before the first frame update
    void Start()
    {
        body = GetComponent<Rigidbody>();
        timer = GetComponent<TimerBehaviourScript>();
        CameraBehaviour = Camera.GetComponent<CameraBehaviourScript>();
        animator = GetComponent<Animator>();
        //meshRenderer = GetComponent<MeshRenderer>();

        
        jumpForce = -Physics.gravity * body.mass * jumpMultiplier;
        TargetsTotal = GameObject.FindGameObjectsWithTag("Target").Length;
    }

    // Update is called once per frame
    void Update()
    {
        TargetsCaughtReadout.text = TargetsCaught.ToString();
        TargetsTotalReadout.text = TargetsTotal.ToString();

        if (TargetsCaught == TargetsTotal)
        {
            win = true;
            EndGameText.text = "YOU WIN!";
        }


        scoreText.text = score.ToString();
        
        //Glide ability
        if (Input.GetKeyDown(KeyCode.Mouse0) && midAir && !glideInputTriggered)
        {
            //Bug: toggles on and off rapidly!
            
            gliding = !gliding;
            ToggleGlide(gliding);
            glideInputTriggered = true;
            animator.SetBool("Gliding", gliding);
            //Debug!
            //if (gliding)
            //{
            //    meshRenderer.material = StateGlide;
            //}
            //else
            //{
            //    meshRenderer.material = StateAir;
            //}
        }
        else if (Input.GetKeyUp(KeyCode.Mouse0))
        {
            glideInputTriggered = false;
        }

        //Camera to mouse position ability
        if (Input.GetMouseButtonDown(1) && alive)
        {
            //Debug.Log("Trans Rights!");
            offsetCamera = true;
            CameraBehaviour.SnapToPlayer(!CameraBehaviour.IsSnappingToPlayer());
            CameraBehaviour.SetLocation(cameraToCursorHitPos.point);
        }
        
        if (Input.GetKeyDown(KeyCode.Space) && !midAir)
        {
            StartCoroutine(Jump());
        }

        //body constraint and drag toggles
        if (midAir)//if player is mid-air
        {
            if (IsGrounded())//check if they landed yet
            {
                FallingContraints(false);
                ToggleGlide(true);
                midAir = false;
                animator.SetBool("Gliding", false);
                //meshRenderer.material = StateGround;
            }
        }
        else//if player is on ground
        {
            
            if (!IsGrounded())//if player has no ground underneath them
            {
                FallingContraints(true);
                ToggleGlide(false);
                midAir = true;
                //meshRenderer.material = StateAir;
            }
        }
        animator.SetBool("MidAir", midAir);
        
    }

    private void FixedUpdate()
    {
        if (alive)
        {
            float horizInput = Input.GetAxis("Horizontal"); // -1, 0, or 1 depending on user input
            float vertInput = Input.GetAxis("Vertical"); // -1, 0, or 1 depending on user input

            if (horizInput != 0 || vertInput != 0)
            {
                CameraBehaviour.SnapToPlayer(true);

                //set velocity
                body.velocity = new Vector3(horizInput * SPEED_MULTIPLIER * loveVelocityMultiplier, body.velocity.y, vertInput * SPEED_MULTIPLIER * loveVelocityMultiplier);

                //transform look in direction of velocity
                transform.LookAt(transform.position + Vector3.Normalize(new Vector3(body.velocity.x,0.0f,body.velocity.z)));

                animator.SetBool("Moving", true);
            }
            else if (!midAir)
            {
                //looking around
                cameraToCursorRay = Camera.ScreenPointToRay(Input.mousePosition);
                cameraToCursorHit = Physics.Raycast(cameraToCursorRay, out cameraToCursorHitPos, 50.0f, 1 << LayerMask.NameToLayer("Ground"));
                if (!cameraToCursorHit)
                {
                    cameraToCursorHitPos.point = transform.position;
                }

                LookAtPosition = new Vector3(cameraToCursorHitPos.point.x, transform.position.y, cameraToCursorHitPos.point.z);
                transform.LookAt(LookAtPosition);

                animator.SetBool("Moving", false);
                ////accidentally madea  buggy free-roaming camera, not useful for this project but keeping it in if it may come in useful for any future re-working.
                //if (!CameraBehaviour.IsSnappingToPlayer())
                //{
                //    CameraBehaviour.SetLocation(hitPos.point);
                //}
            }
        }
        else//if dead
        {
            if (!deathStuffTriggered)
            {
                animator.SetBool("Dead", true);
                CameraBehaviour.SnapToPlayer(true);
                Instantiate(ParticleSpawner, new Vector3(transform.position.x, transform.position.y - transform.localScale.y / 2, transform.position.z), transform.rotation);
                EndGameText.text = "GAME OVER";
                deathStuffTriggered = true;
            }

        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        //nom nom
        if (collision.gameObject.tag == "Target")
        {
            int value = collision.gameObject.GetComponent<BasicNPCBehaviourScript>().GetValue();
            score += value;
            TargetsCaught++;
            loveVelocityMultiplier += loveVelocityIncrease;
            timer.AppendTime(value/3);
            value = -1;
            Instantiate(ParticleSpawner, new Vector3(collision.transform.position.x,collision.transform.position.y - collision.transform.localScale.y / 2, collision.transform.position.z ),collision.transform.rotation);

            Destroy(collision.gameObject);
            
        }
    }
}
