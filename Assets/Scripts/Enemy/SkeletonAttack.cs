using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonAttack : MonoBehaviour
{
    [Header("Sight")]
    public BoxCollider2D boxCollider;
    public float inSightRange;
    public float colliderDistance;
    public EnemyPatrol enemyPatrol;

    [Header("Attack")]
    public LayerMask playerLayer;
    public float attackRange;
    public Transform attackPoint;
    public float attackCoolDown;
    private float coolDownTimer = Mathf.Infinity;

    private Animator anim;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        coolDownTimer += Time.deltaTime;

        if (PlayerInSight())
        {
            if (coolDownTimer >= attackCoolDown)
            {
                coolDownTimer = 0;
                anim.SetTrigger("Attack");
                enemyPatrol.canMove = false;
            }
        }

        if(PlayerHealth.instance.dead == true)
        {
            enabled = false;
        }

    }

    public bool PlayerInSight()
    {
        RaycastHit2D hit = Physics2D.BoxCast(boxCollider.bounds.center + transform.right * inSightRange * transform.localScale.x * colliderDistance,
            new Vector2(boxCollider.bounds.size.x * inSightRange, boxCollider.size.y), 0, Vector2.left, 0, playerLayer);
            
        return hit.collider != null;
    }

    public void DamagePlayer(float damage)
    {
        //Detecting enemies in range
        Collider2D[] hitPlayers = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, playerLayer);
        foreach (Collider2D player in hitPlayers)
        {
            player.GetComponent<PlayerHealth>().TakeDamage(damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
        Gizmos.DrawWireCube(boxCollider.bounds.center + transform.right * inSightRange * transform.localScale.x * colliderDistance,
            new Vector2(boxCollider.bounds.size.x * inSightRange, boxCollider.size.y));
    }
}
