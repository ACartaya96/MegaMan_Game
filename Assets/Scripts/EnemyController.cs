using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    bool isInvincible;
    public bool freezeEnemy;

    public int enemyPoints = 500;
    public int currentHealth;
    public int maxHealth = 1;
    public int contactDamage = 1;
    public int bulletDamage = 1;
    public int bulletSpeed = 1;

    RigidbodyConstraints2D rbConstraints;
    Animator animator;
    Rigidbody2D rb;

    [SerializeField] AudioClip enemyHit;
    [SerializeField] AudioClip enemyDie;
    [SerializeField] AudioClip enemyDink;

    public AudioClip shootBulletClip;
    public GameObject bulletPrefab;
    public Transform bulletShootPos;
    


    // Start is called before the first frame update
    void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
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
        if (!isInvincible)
        {
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
            if (currentHealth <= 0)
            {
                Defeated();
            }

            SoundManager.Instance.Play(enemyHit);
        }
        else
        {
            SoundManager.Instance.Play(enemyDink);
        }
    }

    public void FreezeEnemy(bool freeze)
    {
        Debug.Log(freeze.ToString());
        if(freeze)
        {
            freezeEnemy = true;
            rbConstraints = rb.constraints;
            animator.speed = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

        }
        else
        {
            freezeEnemy = false;
            rb.constraints = rbConstraints;
            animator.speed = 1;
        
        }
    }

    void Defeated()
    {
        SoundManager.Instance.Play(enemyDie);
        Destroy(gameObject);
        GameManager.Instance.AddScorePoints(this.enemyPoints);
        GameManager.Instance.enemyCount--;
        GameManager.Instance.enemyCount = Mathf.Clamp(GameManager.Instance.enemyCount, 0, 1);
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
