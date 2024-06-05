using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    public float maxHealth = 25;
    public float currentHealth;
    public EnemyHealthbar enemyHealthbar;
    public EnemyPatrol enemyPatrol;

    private Rigidbody2D rb;
    private Animator anim;
    private Collider2D myCollider;


    //Setting health and healthbar
    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        enemyHealthbar.SetMaxHealth(maxHealth);
        myCollider = GetComponent<BoxCollider2D>();
        anim = GetComponent<Animator>();
    }

    //Enemy losing health function
    public void EnemyTakeDamage(float damage)
    { 
        anim.SetTrigger("TakeHit");
        currentHealth = Mathf.Clamp(currentHealth - damage, 0f, maxHealth);
        enemyHealthbar.SetHealth(currentHealth);

        //Enemy dead
        if (currentHealth <= 0)
        {
            rb.gravityScale = 0;
            myCollider.enabled = false;
            anim.SetFloat("Health", currentHealth);
            enemyHealthbar.gameObject.SetActive(false);
            enemyPatrol.enabled = false;
        }
    }

}
