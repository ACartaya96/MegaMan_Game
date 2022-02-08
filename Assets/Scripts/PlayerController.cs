using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    BoxCollider2D box2d;
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] Transform bulletPos;
    [SerializeField] GameObject bulletPrefab;

    bool jumpPressed;
    bool keyShoot;
    bool isShooting;
    bool isFacingRight;
    bool isGrounded;
    bool keyShootRelease;
    bool isTakingDamage;
    bool isInvincible;
    bool hitSideRight;

    [SerializeField] int bulletDamage = 1;
    [SerializeField] float bulletSpeed = 5f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 2f;
    public int currentHealth;
    public int maxHealth =28;

    float shootTime;
    float horizontal;
    public float speed; // 1.5f
    public float jumpHeight;

    // Start is called before the first frame update
    void Start()
    {
        box2d = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator =GetComponent<Animator>();
        isFacingRight = true;
        currentHealth =maxHealth;
    }

    // Update is called once per frame

    void Update()
    {  
       if(isTakingDamage)
       {
           animator.Play("Player_Hit");
           return;
       }
       PlayerDirectionInput();
       PlayerShootInput();
       PlayerMovement();
    }

    private void FixedUpdate()
    {
        isGrounded =  false;
        Color raycastColor;
        RaycastHit2D raycastHit;
        float raycastDistance = 0.05f;
        int layerMask = 1 << LayerMask.NameToLayer("ground");

        //ground check
        Vector3 box_orgin = box2d.bounds.center;
        box_orgin.y = box2d.bounds.min.y + (box2d.bounds.extents.y / 4f);
        Vector3 box_size = box2d.bounds.size;
        box_size.y = box2d.bounds.size.y / 4f ;
        raycastHit = Physics2D.BoxCast(box_orgin, box_size, 0f, Vector2.down, raycastDistance, layerMask);
        if (raycastHit.collider != null)
        {
            isGrounded = true;
        }
        raycastColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawRay(box_orgin + new Vector3(box2d.bounds.extents.x,0), Vector2.down * (box2d.bounds.extents.y /4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_orgin - new Vector3(box2d.bounds.extents.x,0), Vector2.down * (box2d.bounds.extents.y /4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_orgin - new Vector3(box2d.bounds.extents.x,box2d.bounds.extents.y / 4f + raycastDistance), Vector2.down * (box2d.bounds.extents.x * 2), raycastColor);

        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !jumpPressed)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
    }

    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180F,0);
    }

    void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab,bulletPos.position, Quaternion.identity ); 
        bullet.name = bulletPrefab.name;
        bullet.GetComponent<Bullet_Script>().SetDamageValue(bulletDamage);
        bullet.GetComponent<Bullet_Script>().SetBulletSpeed(bulletSpeed);
        bullet.GetComponent<Bullet_Script>().SetBulletDirection((isFacingRight) ? Vector2.right : Vector2.left );
        bullet.GetComponent<Bullet_Script>().Shoot();
    }

    void PlayerDirectionInput()
    {
        horizontal = Input.GetAxis("Horizontal");
       
    }
    void PlayerMovement()
    {
        if (horizontal < 0 )
       {
           if(isFacingRight)
           {
               Flip();
           }
           if(isGrounded)
           {
               if(isShooting)
               {
                   // animator.Play("Player_RunShoot");
               }
               else
               {
                   // animator.Play("Player_Run");
               }
           }
            rb.velocity = new Vector2(-speed, rb.velocity.y);
       }
       else if (horizontal > 0 )
       {
            if(!isFacingRight)
           {
               Flip();
           }
            if(isGrounded)
           {
               if(isShooting)
               {
                   // animator.Play("Player_RunShoot");
               }
               else
               {
                   // animator.Play("Player_Run");
               }
           }
            rb.velocity = new Vector2(speed, rb.velocity.y);
       }
       else 
       {
            if(isGrounded)
           {
                if(isShooting)
                {
                    // animator.Play("Player_Shoot");
                }
                else
                {
                   // animator.Play("Player_Idle");
                }
           }
            rb.velocity = new Vector2(0f, rb.velocity.y);
       }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            if(isShooting)
               {
                    // animator.Play("Player_JumpShoot");
               }
            else
               {
                   // animator.Play("Player_Jump");
               }
            rb.velocity = new Vector2(rb.velocity.x, jumpHeight); //jump heght should be 4.875f
        }
        if (!isGrounded)
        {
            if(isShooting)
               {
                    // animator.Play("Player_JumpShoot");
               }
            else
               {
                   // animator.Play("Player_Jump");
               }
        }
        if (Input.GetButton("Jump"))
        {
            jumpPressed = true;
        }
        else
        {
            jumpPressed = false;
        }
    }

    void PlayerShootInput()
    {
        float shootTimeLength= 0;
        float keyShootReleaseTimeLength = 0;

        keyShoot = Input.GetKey(KeyCode.C); // enter key

        if(keyShoot && keyShootRelease )
        {
            isShooting =true;
            keyShootRelease = false;
            shootTime = Time.time;
            Invoke("ShootBullet",01f);
        }
        if(!keyShoot && !keyShootRelease)
        {
            keyShootReleaseTimeLength =  Time.time -  shootTime;
        }
        if(isShooting)
        {
            shootTimeLength = Time.time - shootTime;
            if(shootTimeLength >= 0.25f || keyShootReleaseTimeLength >= 0.15f)
            {
                isShooting = false;
            }
        }

    }
    public void HitSide(bool rightSide)
    {
        hitSideRight = rightSide ;

    }
    public void Invincible(bool invincibility)
    {
        isInvincible = invincibility;
    }
    public void TakeDamage(int damage)
    {
        if (!isInvincible)
        {
            currentHealth -=damage;
            Mathf.Clamp(currentHealth,0,maxHealth);
            //UIHealthBar.instance.setValue(currentHealth / (float)maxHealth); waiting for health bar
            if(currentHealth <= 0)
            {
                Defeat();
            }
            else
            {
                StartDamageAnimation();
            }
        }
    }
    void StartDamageAnimation()
    {
        if(!isTakingDamage)
        {
            isTakingDamage = true;
            isInvincible = true;
            float hitForceX = 0.50f;
            float hitForceY = 1.5f;
            if(hitSideRight) hitForceX = -hitForceX;
            rb.velocity = Vector2.zero;
            rb.AddForce(new Vector2(hitForceX, hitForceY),ForceMode2D.Impulse);
        }
    }
    void StopDamageAnimation()
    {
        isTakingDamage = false ;
        isInvincible = false ;
        //animator.Play("Player_Hit",-1,0f);
    }
    void Defeat()
    {
        Destroy(gameObject);
    }
}

