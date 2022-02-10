using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    bool isInvincible;
    public bool freezeEnemy;

    public int currentHealth;
    public int maxHealth = 1;
    public int contactDamage = 1;

    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;    
    }

    public void Flip()
    {
        transform.Rotate(0, 180f, 0);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void Invicible(bool invincibility)
    {
        isInvincible = invincibility;
    }

    public void TakeDamage(int damage)
    {
        if(!isInvincible)
        {
            currentHealth -= damage;
            Mathf.Clamp(currentHealth, 0, maxHealth);
            if(currentHealth <= 0)
            {
                Defeated();
            }
        }
    }

    void Defeated()
    {
        Destroy(gameObject);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Player"))
        {

            PlayerController player = other.gameObject.GetComponent<PlayerController>();
            player.HitSide(transform.position.x > player.transform.position.x);
            player.TakeDamage(this.contactDamage);
        }
    }
}
