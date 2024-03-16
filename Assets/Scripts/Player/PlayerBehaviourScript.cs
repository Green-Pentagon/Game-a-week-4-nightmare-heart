using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;

public class PlayerBehaviourScript : MonoBehaviour
{
    public const float SPEED_MULTIPLIER = 5.0f;
    public Camera Camera;
    public TextMeshProUGUI scoreText;

    private Rigidbody body;
    private TimerBehaviourScript timer;
    private CameraBehaviourScript CameraBehaviour;
    private int score = 0;
    private bool alive = true;

    //ground check
    private float floatingOffGroundOffset = 1.0f; //CHANGE ME IF YOU WISH THE PLAYER TO FLOAT FURTHER OR CLOSER TO GROUND

    //Looking around where cursor is
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



    // Start is called before the first frame update

    public void Kill()
    {
        alive = false;
    }

    public bool IsAlive()
    {
        return alive;
    }

    bool IsGrounded()
    {
        return Physics.Raycast(transform.position, Vector3.down, floatingOffGroundOffset, 1 << LayerMask.NameToLayer("Ground"));
    }


    void ToggleGlide(bool toggle)
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

    void FallingContraints(bool toggle)
    {
        if (toggle)
        {
            body.constraints = RigidbodyConstraints.None;//unfreezes y motion
            body.constraints = RigidbodyConstraints.FreezeRotation;
        }
        else //if hit ground
        {
            body.constraints = RigidbodyConstraints.FreezePositionY | RigidbodyConstraints.FreezeRotation;
            
        }
    }

    IEnumerator Jump()
    {
        ToggleGlide(false);
        FallingContraints(true);

        body.AddForce(jumpForce,ForceMode.Force);
        yield return null; 
        //yield return new WaitForSeconds(0.2f); 
        //midAir = true;

    }


    void Start()
    {
        body = GetComponent<Rigidbody>();
        timer = GetComponent<TimerBehaviourScript>();
        CameraBehaviour = Camera.GetComponent<CameraBehaviourScript>();

        jumpForce = -Physics.gravity * body.mass * jumpMultiplier;
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = score.ToString();

        if(Input.GetMouseButton(0) && midAir)
        {
            gliding = !gliding;
            ToggleGlide(gliding);
        }

        if (Input.GetMouseButtonDown(1) && alive)
        {
            //Debug.Log("Trans Rights!");
            offsetCamera = true;
            CameraBehaviour.SnapToPlayer(!CameraBehaviour.IsSnappingToPlayer());
            CameraBehaviour.SetLocation(cameraToCursorHitPos.point);
        }
        else if (Input.GetKeyDown(KeyCode.Space) && !midAir)
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
            }
        }
        else//if player is on ground
        {
            if (!IsGrounded())//if player has no ground underneath them
            {
                FallingContraints(true);
                ToggleGlide(false);
                midAir = true;
            }
        }

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
                body.velocity = new Vector3(horizInput * SPEED_MULTIPLIER, body.velocity.y, vertInput * SPEED_MULTIPLIER);

                //transform look in direction of velocity
                transform.LookAt(transform.position + Vector3.Normalize(body.velocity));
            }
            else
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

                ////accidentally madea  buggy free-roaming camera, not useful for this project but keeping it in if it may come in useful for any future re-working.
                //if (!CameraBehaviour.IsSnappingToPlayer())
                //{
                //    CameraBehaviour.SetLocation(hitPos.point);
                //}
            }
        }
        else
        {
            CameraBehaviour.SnapToPlayer(true);
        }
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Target")
        {
            int value = collision.gameObject.GetComponent<BasicNPCBehaviourScript>().GetValue();
            score += value;
            timer.AppendTime(value);
            value = -1;
            Destroy(collision.gameObject);
        }
    }
}
