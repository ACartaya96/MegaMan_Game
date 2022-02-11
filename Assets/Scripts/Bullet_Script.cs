using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet_Script : MonoBehaviour
{
    Rigidbody2D rb;
    SpriteRenderer sprite;
    Animator animator;
    

    float destroyTime;

    bool freezeBullet;

    public int damage = 1;

    RigidbodyConstraints2D rbConstraints;

    [SerializeField] float bulletSpeed;
    [SerializeField] Vector2 bulletDirection;
    [SerializeField] float destroyDelay;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        sprite = GetComponent<SpriteRenderer>();
    }

    public void SetBulletSpeed(float speed)
    {
        this.bulletSpeed = speed;
    }

      public void SetBulletDirection(Vector2 direction)
    {
        this.bulletDirection = direction;
    }

      public void SetDamageValue(int damage)
    {
        this.damage = damage;
    }

      public void SetDestroyDelay(float delay)
    {
        this.destroyDelay = delay;
    }

    public void Shoot()
    {
        sprite.flipX = (bulletDirection.x < 0);
        rb.velocity = bulletDirection * bulletSpeed;
        destroyTime = destroyDelay;
    }

    public void FreezeBullet(bool freeze)
    {
       if(freeze)
        {
            freezeBullet = true;
            rbConstraints = rb.constraints;
            animator.speed = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
            rb.velocity = Vector2.zero;
         }
        else
        {
            freezeBullet = false;
            animator.speed = 1;
            rb.constraints = rbConstraints;
            rb.velocity = bulletDirection * bulletSpeed;
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.CompareTag("Enemy"))
        {
            EnemyController enemy = other.gameObject.GetComponent<EnemyController>();
            if(enemy != null)
            {
                enemy.TakeDamage(this.damage);
            }
            Destroy(gameObject, 0.01f);
        }
        else if(other.gameObject.CompareTag("Floor"))
        {
            Destroy(gameObject, 0.01f);
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        if (freezeBullet) return;

        destroyTime -= Time.deltaTime;
        if(destroyTime < 0)
        {
            Destroy(gameObject);
        }

    }
}