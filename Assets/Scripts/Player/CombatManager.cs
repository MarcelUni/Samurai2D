using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CombatManager : MonoBehaviour
{
    public static CombatManager instance;

    [Header("Input")]
    public bool canReceiveInput;
    public bool inputReceived;

    [Header("Attack")]
    public Transform attackPoint;
    public float attackRange = 0.5f;
    public int attackDamage = 10;
    public bool isAttacking = false;

    [Header("Enemy")]
    public LayerMask EnemyLayers;

    private void Awake()
    {  
        instance = this;
    }

    public void DamageEnemy(float damage)
    {
        //Detecting enemies in range
        Collider2D[] hitEnemies = Physics2D.OverlapCircleAll(attackPoint.position, attackRange, EnemyLayers);
        //Applying damage
        foreach (Collider2D enemy in hitEnemies)
        {
            enemy.GetComponent<EnemyHealth>().EnemyTakeDamage(damage);
        }
    }

    //Attacking
    public void Attack(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            if (canReceiveInput)
            {
                inputReceived = true;
               
                canReceiveInput = false;
            }
            else
            {
                return;
            }
        }
    }

    public void InputManager()
    {
        if (!canReceiveInput)
        {
            canReceiveInput = true;
        }
        else
        {
            canReceiveInput = false;
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null)
        {
            return;
        }
        
        Gizmos.DrawWireSphere(attackPoint.position, attackRange);
    }

}
