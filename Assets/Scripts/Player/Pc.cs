using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Pc : MonoBehaviour
{
    public static Pc instance;
    public Animator anim;
    public PlayerHealth playerHealth;

    [Header("Run")]
    public float moveSpeed;
    public bool facingLeft;
    public float sprintSpeed;
    private float inputX;
    private Rigidbody2D rb;

    [Header("Jump")]
    public float jumpForce;
    public float doublejumpForce;
    public LayerMask groundLayer;
    public bool canDoubleJump = true;
    public float extraHeight = 1f;
    public bool canPickupJump = false;
    private BoxCollider2D boxCollider2D;

    [Header("Dash")]
    public float dashTime = 0.2f;
    public float dashCoolDown = 1f;
    public float dashSpeed = 16f;
    private bool canDash = true;
    private bool isDashing;
    public TrailRenderer tr;
    private bool canPickupDash = false;

    [Header("Unlocks")]
    public Animator jumpAnim;
    public Animator dashAnim;
    public GameObject PressETextJump;
    public GameObject PressETextDash;
    public GameObject dJUtext;
    public GameObject dashULText;
    public float textDisappear;
    public bool doubleJumpUnlocked;
    public bool dashUnlocked;

    private void Start()
    {
        facingLeft = false;
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        moveSpeed = sprintSpeed;
        instance = this;
        doubleJumpUnlocked = false;
        dashUnlocked = false;
    }

    void FixedUpdate()
    {
        //Move player
        rb.velocity = new Vector2(inputX * moveSpeed, rb.velocity.y);

        //Dashing
        if(isDashing == true)
        {
            rb.velocity = new Vector2(dashSpeed * transform.localScale.x, 0f);
        }

        //Flip player to right direction
        if (inputX > 0 && facingLeft)
        {
            Flip();
        }
        if (inputX < 0 && !facingLeft)
        {
            Flip();
        }

        //Setting parameter for run animation
        anim.SetFloat("Speed", Mathf.Abs(inputX));

        //Playing jump animation if not grounded
        if(IsGrounded() == false)
        {
            anim.SetBool("Jumping", true);
        }
        if(IsGrounded() == true)
        {
            anim.SetBool("Jumping", false);
        }

        //Updating the yVelocity in the animator for the jump/fall animation
        anim.SetFloat("yVelocity", rb.velocity.y);

        //Locking x-axis movement when attacking or taking hit unless in the air
        if (IsGrounded() && CombatManager.instance.isAttacking == true || IsGrounded() && playerHealth.takingHit == true)
        {
            moveSpeed = 0;
        }
        else
        {
            moveSpeed = sprintSpeed;
        }

        //Pickup logic
        if(doubleJumpUnlocked == true)
        {
            PressETextJump.SetActive(false);
        }

    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("DoubleJumpUnlock"))
        {
            PressETextJump.SetActive(true);
            canPickupJump = true;
           
        }
        else if (collision.gameObject.CompareTag("DashUnlock"))
        {
            PressETextDash.SetActive(true);
            canPickupDash = true;

        }
        else
        {
            canPickupJump = false;
            PressETextJump.SetActive(false);
            canPickupDash = false;
            PressETextDash.SetActive(false);
        }
    }

    //For item pickup
    public void PressE(InputAction.CallbackContext context)
    {
        if(context.started && canPickupJump == true)
        {
            doubleJumpUnlocked = true;
            PressETextJump.SetActive(false);
            jumpAnim.Play("ChestOpen");
            StartCoroutine(JumpText());
        }
        else if(context.started && canPickupDash == true)
        {
            dashUnlocked = true;
            PressETextDash.SetActive(false);
            dashAnim.Play("ChestOpen");
            StartCoroutine(DashText());
        }
    }

    //Running
    public void Move(InputAction.CallbackContext context)
    {
        inputX = context.ReadValue<Vector2>().x;
        anim.SetFloat("Speed", Mathf.Abs(inputX));
    }

    //Jumping
    public void Jump(InputAction.CallbackContext context)
    {
        if (IsGrounded() && context.phase == InputActionPhase.Started)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
            canDoubleJump = true;
        }
        else if (context.phase == InputActionPhase.Started && IsGrounded() == false && canDoubleJump == true && doubleJumpUnlocked == true)
        {
            rb.velocity = new Vector2(rb.velocity.x, doublejumpForce);
            canDoubleJump = false;
        }
    }

    //Dashing with coroutine
    public void Dash(InputAction.CallbackContext context)
    {
        if (context.started && canDash == true && dashUnlocked == true)
        {
            isDashing = true;
            StartCoroutine(Dash());
        }
    }

    //Flipping function
    void Flip()
    {
        Vector3 currentScale = gameObject.transform.localScale;
        currentScale.x *= -1f;
        gameObject.transform.localScale = currentScale;
        facingLeft = !facingLeft;
    }

    //Checking ground with raycast
    private bool IsGrounded()
    {
        Color rayColor;

        //Checking ground by casting the boxcollider below player
        RaycastHit2D raycastHit = Physics2D.BoxCast(boxCollider2D.bounds.center, boxCollider2D.bounds.size, 0f, Vector2.down, extraHeight, groundLayer);

        //Drawing raycast in sceneview
        if(raycastHit.collider != null)
        {
            rayColor = Color.green; 
        }
        else
        {
            rayColor = Color.red;
        }
        Debug.DrawRay(boxCollider2D.bounds.center + new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, 0), Vector2.down * (boxCollider2D.bounds.extents.y + extraHeight), rayColor);
        Debug.DrawRay(boxCollider2D.bounds.center - new Vector3(boxCollider2D.bounds.extents.x, boxCollider2D.bounds.extents.y + extraHeight), Vector2.right * boxCollider2D.bounds.extents.x, rayColor);

        return raycastHit.collider != null;
    }

    //Dashing coroutine
    private IEnumerator Dash()
    {
        canDash = false;

        //Storing gravity
        float originalGravity = rb.gravityScale;

        //Setting new gravity
        rb.gravityScale = 0f;
        tr.emitting = true;

        //Dashing while waiting
        yield return new WaitForSeconds(dashTime);

        //Ending the dash
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;

        //Waiting to be able to dash again
        yield return new WaitForSeconds(dashCoolDown);
        yield return new WaitUntil(IsGrounded);
        canDash = true;
    }

    private IEnumerator JumpText()
    {
        dJUtext.SetActive(true);
        yield return new WaitForSeconds(textDisappear);
        dJUtext.SetActive(false);
    }

    private IEnumerator DashText()
    {
        dashULText.SetActive(true);
        yield return new WaitForSeconds(textDisappear);
        dashULText.SetActive(false);
    }
}
