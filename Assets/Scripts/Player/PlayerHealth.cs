using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth instance;

    public float maxHealth;
    public float currentHealth;
    public Animator anim;
    private Rigidbody2D rb;
    public bool takingHit = false;
    public bool dead;
    public float knockBack;

    public HealthBar healthBar;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        currentHealth = maxHealth;
        instance = this;
        healthBar.SetMaxHealth(maxHealth);
    }

    //Function for taking damage
    public void TakeDamage(float damage)
    {
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        healthBar.SetHealth(currentHealth);

        if (currentHealth > 0)
        {
            anim.SetTrigger("Takehit");
            if(Pc.instance.facingLeft == true)
            {
                rb.AddForce(Vector2.right * knockBack);
            }
            else
            {
                rb.AddForce(Vector2.left * knockBack);
            }
        }
        else
        {
            if (!dead)
            {
                dead = true;
                anim.SetTrigger("Death");
                
            }
        }
    }

    //Function for adding health
    public void AddHealth(float _value)
    {
        currentHealth = Mathf.Clamp(currentHealth + _value, 0, maxHealth);
    }
}
