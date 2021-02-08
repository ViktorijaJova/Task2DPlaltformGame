using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D coll;

    private enum State {idle,running,jumping,falling,hurt,climb }
    private State state = State.idle;
    [SerializeField] private LayerMask ground;
    [SerializeField] private float speed = 5f;
    [SerializeField] private float jumpForce = 10f;
   // [SerializeField] private Text cherrytext;
    [SerializeField] private float hurtForce = 10f;
   [SerializeField] private AudioSource footsteep;
   [SerializeField] private AudioSource cherry;
    // [SerializeField] private int health;
    //[SerializeField] private Text healthAmount;

    public bool canClimb = false;
    public bool topLadder = false;
    public bool bottomLadder = false;
    public Ladder ladder;

    public int cherries = 0;
    private float naturalGravity;
    [SerializeField] float climbSpeed = 3f;





    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        coll = GetComponent<Collider2D>();
       PermenentUI.perm.healthamount.text = PermenentUI.perm.health.ToString();
        naturalGravity = rb.gravityScale;
    }

    private void Update()
    {
        if(state == State.climb)
        {
            Climb();
        }
       else if(state != State.hurt)
        {
            InputManager();
        }
        VelocityStateSwitch();
        anim.SetInteger("state", (int)state);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Enemy enemy = collision.gameObject.GetComponent<Enemy>();

            if (state == State.falling)
            {
                enemy.JumpedOn();
             //   Destroy(collision.gameObject);
                Jump();
            }
            else
            {
                state = State.hurt;
                HandleHealth();
                if (collision.gameObject.transform.position.x > transform.position.x)
                {
                    //enemy is to my right therefore i shoud be damaged and move left
                    rb.velocity = new Vector2(-hurtForce, rb.velocity.y);
                }
                else
                {
                    //enemy is to my left therefore i should be damaged and moved right
                    rb.velocity = new Vector2(hurtForce, rb.velocity.y);
                }
            }
        }
    }

    private void HandleHealth()
    {
        PermenentUI.perm.health -= 1;
        PermenentUI.perm.healthamount.text = PermenentUI.perm.health.ToString();
        if (PermenentUI.perm.health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            PermenentUI.perm.health = 5;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag == "Collectable")
        {
            cherry.Play();
            Destroy(other.gameObject);
            PermenentUI.perm.cherries += 1;
            PermenentUI.perm.cherrytext.text = PermenentUI.perm.cherries.ToString();
        }

        else if(other.tag == "Powerup")
        {
            Destroy(other.gameObject);
            jumpForce = 18f;
            GetComponent<SpriteRenderer>().color = Color.yellow;
            StartCoroutine(REsetPower());

;        }
    }

    private void InputManager()
    {
        float Hdirection = Input.GetAxis("Horizontal");
        if(canClimb && Mathf.Abs(Input.GetAxis("Vertical")) > .1f)
                {

            state = State.climb;
            rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation;
            transform.position = new Vector3(ladder.transform.position.x, rb.position.y);
            rb.gravityScale = 0f;

        }
        //moving left
        if (Hdirection < 0)
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
            transform.localScale = new Vector2(-1, 1);
        }
        //moving right
        else if (Hdirection > 0)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
            transform.localScale = new Vector2(1, 1);
        }

        //jumping space
        if (Input.GetButtonDown("Jump") && coll.IsTouchingLayers(ground))
        {
            Jump();

        }
    }
    public void FootSteep()
    {
        footsteep.Play();
    }
    private void Jump()
    {
        rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        state = State.jumping;
    }
    private IEnumerator REsetPower()
    {
        yield return new WaitForSeconds(10);
        jumpForce = 16f;
        GetComponent<SpriteRenderer>().color = Color.white;
    }
    private void VelocityStateSwitch()
    {
        if(state == State.climb)
        {

        }
       
        else if(state == State.jumping)
        {
            if(rb.velocity.y < .1f)
            {
                state = State.falling;
            }
        }
       else if(state == State.falling)
        {
            if (coll.IsTouchingLayers(ground))
            {
                state = State.idle;
            }
        }
        else if(state == State.hurt)
        {
            if(Mathf.Abs(rb.velocity.x) < .1f)
            {
                state = State.idle;
            }
        }
         else if(Mathf.Abs(rb.velocity.x)> 2f)
        {
            //moving
            state = State.running;
        }
        else
        {
            state = State.idle;
        }

     
    }


    void Climb()
    {
        if (Input.GetButtonDown("Jump"))
        {
            rb.constraints =  RigidbodyConstraints2D.FreezeRotation;
            canClimb = false;
            rb.gravityScale = naturalGravity;
            Jump();


        }
        float vDirection = Input.GetAxis("Vertical");
        if(vDirection > .1f && !topLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);
        }
        else if (vDirection < -1f && !bottomLadder)
        {
            rb.velocity = new Vector2(0f, vDirection * climbSpeed);

        }
        else
        {

        }
    }

  
}
