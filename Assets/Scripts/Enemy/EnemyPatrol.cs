using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPatrol : MonoBehaviour
{
    [Header("Moving")]
    public Transform leftEdge;
    public Transform rightEdge;
    public bool canMove;
    public float speed = 1f;
    public float stillSpeed = 0f;

    private float moveSpeed;
    private SkeletonAttack skeletonAttack;
    private bool facingLeft;
    private Vector2 scale;
    private Animator anim;

    [Header("Player Behind")]
    public float inSightRange;
    public BoxCollider2D boxCollider;
    public float colliderDistance;
    public LayerMask playerLayer;

    void Start()
    {
        skeletonAttack = GetComponent<SkeletonAttack>();
        anim = GetComponent<Animator>();
        scale = transform.localScale;
        facingLeft = false;
        canMove = true;
    }

    public void Update()
    {
        if (PlayerBehind())
        {
            facingLeft = !facingLeft;
        }

        if (leftEdge != null || rightEdge != null)
        {
            //Stop movement when taking hit
            if (anim.GetCurrentAnimatorStateInfo(0).IsName("TakeHit") || skeletonAttack.PlayerInSight() == true)
            {
                canMove = false;
            }
            else
            {
                canMove = true;
            }

            //Patrolling
            if (skeletonAttack.PlayerInSight() == false && canMove == true)
            {
                if (!facingLeft)
                {
                    if (transform.position.x >= leftEdge.position.x)
                    {
                        MoveInDirection(-1);
                    }
                    else
                    {
                        DirectionChange();
                    }
                }
                else
                {
                    if (transform.position.x <= rightEdge.position.x)
                    {
                        MoveInDirection(1);
                    }
                    else
                    {
                        DirectionChange();
                    }
                }
            }
            if (canMove == false)
            {
                anim.SetBool("Run", false);
                moveSpeed = stillSpeed;
            }
            else
            {
                moveSpeed = speed;
            }
        }

        else
        {
            canMove = false;
            return;
        }

        //Debug.Log(PlayerBehind());
    }

    private bool PlayerBehind()
    {
        Vector2 boxOrigin = (Vector2)boxCollider.bounds.center + (Vector2)transform.right * inSightRange * transform.localScale.x * colliderDistance;
        Vector2 boxSize = new Vector2(boxCollider.bounds.size.x * inSightRange, boxCollider.bounds.size.y);

        Collider2D hit = Physics2D.OverlapBox(boxOrigin, boxSize, 0, playerLayer);

        if (hit != null)
        {
            //Debug.Log("Player position: " + hit.transform.position);
            return true;
        }
        return false;
    }

    private void DirectionChange()
    {
        facingLeft = !facingLeft;
    }

    private void MoveInDirection(int _direction)
    {
        //Set anim parameter
        anim.SetBool("Run", true);

        //Make enemy face direction
        transform.localScale = new Vector2(Mathf.Abs(scale.x) * _direction, scale.y);

        //Move in that direction
        transform.position = new Vector3(transform.position.x + Time.deltaTime * _direction * moveSpeed, transform.position.y, transform.position.z);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * inSightRange * transform.localScale.x * colliderDistance,
            new Vector2(boxCollider.bounds.size.x * inSightRange, boxCollider.size.y));
    }
}
