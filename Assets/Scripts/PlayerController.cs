using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerController : MonoBehaviour
{
    BoxCollider2D box2d;
    Rigidbody2D rb;
    Animator animator;
    [SerializeField] Transform bulletPos;
    [SerializeField] GameObject bulletPrefab;

    [SerializeField] AudioClip jumpLandedClip;
    [SerializeField] AudioClip shootBulletClip;
    [SerializeField] AudioClip takingDamageClip;
    [SerializeField] AudioClip walkingClip;

    [SerializeField] AudioClip explodeEffectClip;

    bool jumpPressed;
    public bool keyShoot = false;
    bool isShooting;
    bool isFacingRight;
    bool isGrounded;
    bool isJumping;
    bool isClimbing;
    public bool keyShootRelease = true;
    bool isTakingDamage;
    bool isThrowing;

    //ladder
    float transformY;
    float transformHY;
    bool isClimbingDown;
    bool atLaddersEnd;
    bool hasStartedClimbing;
    bool startedClimbTransition;
    bool finishedClimbTransition;



    bool hitSideRight;
    public bool isInvincible;

    bool freezeInput;
    bool freezePlayer;

    ColorSwap colorSwap;
    private enum SwapIndex
    {
        Primary = 64,
        Secondary = 128
    };
    public enum PlayerWeapons
    {
        Default,
        GutsMan
    };
    public PlayerWeapons playerWeapon = PlayerWeapons.Default;
    [System.Serializable]

    public struct PlayerWeaponsStruct
    {
        public PlayerWeapons weapon;
        public int currentEnergy;
        public int maxEnergy;
    }
    public PlayerWeaponsStruct[] playerWeaponStructs;


    RigidbodyConstraints2D rbConstraints;

    [HideInInspector] public LadderScript ladder;
    [SerializeField] int bulletDamage = 1;
    [SerializeField] float bulletSpeed = 5f;
    [SerializeField] float climbSpeed = .35f;
    public float fallMultiplier = 2.5f;
    public float lowJumpMultiplier = 1.5f;
    public int currentHealth;
    public int maxHealth = 28;
    public int bulletIndex = 0;
    public int maxBullets = 3;

    float shootTime;
    float horizontal;
    float keyVertical;
    public float speed; // 1.5f
    public float jumpHeight;

    Vector2 direction;
    [Header("Ladder Setting")]
    [SerializeField] float climbSpriteHeight = 0.24f;



    // Start is called before the first frame update
    void Start()
    {
        box2d = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        isFacingRight = true;
        isInvincible = false;
        currentHealth = maxHealth;
        for (int i = 0; i < playerWeaponStructs.Length; i++)
        {
            playerWeaponStructs[i].currentEnergy = playerWeaponStructs[i].maxEnergy;
        }
        colorSwap = GetComponent<ColorSwap>();
        SetWeapon(playerWeapon);
    }

    // Update is called once per frame

    void Update()
    {

        if (isTakingDamage)
        {
            //animator.Play("Player_Hit");
            return;
        }
        PlayerDirectionInput();
        PlayerShootInput();
        PlayerMovement();

    }

    private void FixedUpdate()
    {
        isGrounded = false;
        Color raycastColor;
        RaycastHit2D raycastHit;
        float raycastDistance = 0.05f;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");

        //ground check
        Vector3 box_orgin = box2d.bounds.center;
        box_orgin.y = box2d.bounds.min.y + (box2d.bounds.extents.y / 4f);
        Vector3 box_size = box2d.bounds.size;
        box_size.y = box2d.bounds.size.y / 4f;
        raycastHit = Physics2D.BoxCast(box_orgin, box_size, 0f, Vector2.down, raycastDistance, layerMask);
        if (raycastHit.collider != null)
        {
            isGrounded = true;
            if (isJumping)
            {
                SoundManager.Instance.Play(jumpLandedClip);
                isJumping = false;
            }
        }
        raycastColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawRay(box_orgin + new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_orgin - new Vector3(box2d.bounds.extents.x, 0), Vector2.down * (box2d.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_orgin - new Vector3(box2d.bounds.extents.x, box2d.bounds.extents.y / 4f + raycastDistance), Vector2.down * (box2d.bounds.extents.x * 2), raycastColor);
        
        if (rb.velocity.y < 0)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (fallMultiplier - 1) * Time.fixedDeltaTime;
        }
        else if (rb.velocity.y > 0 && !jumpPressed)
        {
            rb.velocity += Vector2.up * Physics2D.gravity.y * (lowJumpMultiplier - 1) * Time.fixedDeltaTime;
        }
        
    }

    public static float Half(float value)
    {
        return Mathf.Floor(value) + 0.5f;
    }

    private void ResetClimbing()
    {
        if (isClimbing)
        {
            isClimbing = false;
            atLaddersEnd = false;
            startedClimbTransition = false;
            finishedClimbTransition = false;
            animator.speed = 1;
            rb.bodyType = RigidbodyType2D.Dynamic;
            rb.velocity = Vector2.zero;
        }
    }
    void Flip()
    {
        isFacingRight = !isFacingRight;
        transform.Rotate(0f, 180F, 0);
    }
    public void SetWeapon(PlayerWeapons weapon)
    {
        playerWeapon = weapon;
        int currentEnergy = playerWeaponStructs[(int)playerWeapon].currentEnergy;
        int maxEnergy = playerWeaponStructs[(int)playerWeapon].maxEnergy;
        float weaponEnergyValue = (float)currentEnergy / (float)maxEnergy;

        switch (playerWeapon)
        {
            case PlayerWeapons.Default:
                colorSwap.SwapColor((int)SwapIndex.Primary, ColorSwap.ColorFromInt(0x000000));
                colorSwap.SwapColor((int)SwapIndex.Secondary, ColorSwap.ColorFromInt(0xFFD700));
                UIEnergyBars.Instance.SetImage(UIEnergyBars.EnergyBars.PlayerWeapon, UIEnergyBars.EnergyBarsTypes.PlayerLife);
                UIEnergyBars.Instance.SetVisibility(UIEnergyBars.EnergyBars.PlayerWeapon, false);
                break;
            case PlayerWeapons.GutsMan:
                colorSwap.SwapColor((int)SwapIndex.Primary, ColorSwap.ColorFromInt(0xFFD700));
                colorSwap.SwapColor((int)SwapIndex.Secondary, ColorSwap.ColorFromInt(0x000000));
                UIEnergyBars.Instance.SetImage(UIEnergyBars.EnergyBars.PlayerWeapon, UIEnergyBars.EnergyBarsTypes.SuperArm);
                UIEnergyBars.Instance.SetValue(UIEnergyBars.EnergyBars.PlayerWeapon, weaponEnergyValue);
                UIEnergyBars.Instance.SetVisibility(UIEnergyBars.EnergyBars.PlayerWeapon, true);
                break;
        }
        colorSwap.ApplyColor();
    }

    void ShootBullet()
    {
        GameObject bullet = Instantiate(bulletPrefab, bulletPos.position, Quaternion.identity);

        bullet.GetComponent<Bullet_Script>().SetDamageValue(bulletDamage);
        bullet.GetComponent<Bullet_Script>().SetBulletSpeed(bulletSpeed);
        bullet.GetComponent<Bullet_Script>().SetBulletDirection((isFacingRight) ? Vector2.right : Vector2.left);
        bullet.GetComponent<Bullet_Script>().SetCollideWithTags("Enemy");
        bullet.GetComponent<Bullet_Script>().Shoot();



        SoundManager.Instance.Play(shootBulletClip);
    }

    void PlayerDirectionInput()
    {
        if (!freezeInput)
        {
            horizontal = Input.GetAxis("Horizontal");
            keyVertical = Input.GetAxisRaw("Vertical");
        }
    }

    void PlayerMovement()
    {
        transformY = transform.position.y;
        transformHY = transformY + climbSpriteHeight;
        if (isClimbing)
        {
            //debug lines
            Debug.DrawLine(new Vector3(ladder.posX - 2f, ladder.posTopHandlerY, 0),
                new Vector3(ladder.posX + 2f, ladder.posTopHandlerY, 0), Color.blue);
            Debug.DrawLine(new Vector3(ladder.posX - 2f, ladder.posBottomHandlerY, 0),
                new Vector3(ladder.posX + 2f, ladder.posBottomHandlerY, 0), Color.blue);
            Debug.DrawLine(new Vector3(transform.position.x - 2f, transformHY, 0),
                new Vector3(transform.position.x + 2f, transformHY, 0), Color.magenta);
            Debug.DrawLine(new Vector3(transform.position.x - 2f, transformY, 0),
                new Vector3(transform.position.x + 2f, transformY, 0), Color.magenta);

            //passed the top transform position
            if (transformHY > ladder.posTopHandlerY)
            {
                if (!isClimbingDown)
                {
                    if (!startedClimbTransition)
                    {
                        startedClimbTransition = true;
                        ClimbTransition(true);
                    }
                    else if (finishedClimbTransition)
                    {
                        finishedClimbTransition = false;
                        isJumping = false;
                        animator.Play("Player_Run");
                        transform.position = new Vector2(ladder.posX, ladder.posPlatformY + 0.005f);// was .005
                        if (!atLaddersEnd)
                        {
                            atLaddersEnd = true;
                            ResetClimbing();
                        }
                    }
                }
            }
            else if (transformHY < ladder.posBottomHandlerY)
            {
                ResetClimbing();
            }
            else
            {
                if (!isClimbingDown)
                {
                    if (Input.GetButtonDown("Jump") && keyVertical == 0)
                    {
                        ResetClimbing();
                    }
                    else if (isGrounded && !hasStartedClimbing)
                    {
                        isJumping = false;
                        animator.Play("idle");
                        transform.position = new Vector2(ladder.posX, ladder.posBottomY - 0.005f);
                        if (!atLaddersEnd)
                        {
                            atLaddersEnd = true;
                            Invoke("ResetClimbing", .01f);
                        }
                    }
                    else
                    {
                        animator.speed = Mathf.Abs(keyVertical);
                        if (keyVertical != 0 && !isShooting)
                        {
                            Vector3 climbDirection = new Vector3(0, climbSpeed) * keyVertical;
                            transform.position = transform.position + climbDirection * Time.deltaTime;
                        }
                        if (isShooting || isThrowing)
                        {
                            if (horizontal < 0)
                            {
                                if (isFacingRight)
                                {
                                    Flip();
                                }
                            }
                            else if (horizontal > 0)
                            {
                                if (!isFacingRight)
                                {
                                    Flip();
                                }
                            }
                            if (isShooting)
                            {
                                animator.Play("climbing_shooting");
                            }
                            else if (isThrowing)
                            {
                                //animator.Play("Player_ClimbThrow");
                            }
                        }
                        else
                        {
                            animator.Play("climbing");
                        }
                    }
                }
            }
        }

        else
        {
            if (horizontal < 0)
            {
                if (isFacingRight)
                {
                    Flip();
                }
                if (isGrounded)
                {
                    if (isShooting)
                    {
                        animator.Play("Player_RunShoot");
                    }
                    else if (isThrowing)
                    {
                        speed = 0f;
                        //animator.Play("Player_Throw");
                    }
                    else
                    {
                        animator.Play("Player_Run");
                        SoundManager.Instance.Play(walkingClip);
                    }
                }
                rb.velocity = new Vector2(-speed, rb.velocity.y);

            }
            else if (horizontal > 0)
            {
                if (!isFacingRight)
                {
                    Flip();
                }
                if (isGrounded)
                {
                    if (isShooting)
                    {
                        animator.Play("Player_RunShoot");
                    }
                    else if (isThrowing)
                    {
                        speed = 0f;
                        //animator.Play("Player_Throw");
                    }
                    else
                    {
                        animator.Play("Player_Run");
                        SoundManager.Instance.Play(walkingClip);
                    }
                }
                rb.velocity = new Vector2(speed, rb.velocity.y);
            }
            else
            {
                if (isGrounded)
                {

                    if (isShooting)
                    {
                        animator.Play("Player_Shoot");
                    }
                    else if (isThrowing)
                    {
                        //animator.Play("Player_Throw");
                    }
                    else
                    {
                        animator.Play("Player_Idle");
                    }
                }
                rb.velocity = new Vector2(0f, rb.velocity.y);
            }
            //Jump Code
            if (Input.GetButtonDown("Jump") && isGrounded)
            {
                if (isShooting)
                {
                    animator.Play("Player_JumpShoot");
                }
                else if (isThrowing)
                {
                    //animator.Play("Player_JumpThrow");
                }
                else
                {
                    animator.Play("Player_Jump");
                }
                rb.velocity = new Vector2(rb.velocity.x, jumpHeight); //jump height should be 4.875f
            }
            //Move Code
            if (!isGrounded)
            {
                isJumping = true;
                if (isShooting)
                {
                    animator.Play("Player_JumpShoot");
                }
                else if (isThrowing)
                {
                    //animator.Play("Player_JumpThrow");
                }
                else
                {
                    animator.Play("Player_Jump");
                   // Debug.Log("error 2");
                }
            }
            if (ladder != null)
            {
                if (ladder.isNearLadder && keyVertical > 0 && transformHY < ladder.posTopHandlerY)
                {
                    isClimbing = true;
                    isClimbingDown = false;
                    animator.speed = 0;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    rb.velocity = Vector2.zero;
                    transform.position = new Vector3(ladder.posX, transformY + 0.025f, 0);
                    StartedClimbing();
                }
                if (ladder.isNearLadder && keyVertical < 0 && isGrounded && transformHY > ladder.posTopHandlerY)
                {
                    isClimbing = true;
                    isClimbingDown = true;
                    animator.speed = 0;
                    rb.bodyType = RigidbodyType2D.Kinematic;
                    rb.velocity = Vector2.zero;
                    transform.position = new Vector3(ladder.posX, transformY, 0);
                    ClimbTransition(false);
                }
            }
        }
        if (!freezeInput)
        {
            if (Input.GetButton("Jump"))
            {
                jumpPressed = true;
            }
            else
            {
                jumpPressed = false;
            }
        }
    }

    void StartedClimbing()
    {
        StartCoroutine(StartedClimbingCo());
    }
    private IEnumerator StartedClimbingCo()
    { 
        hasStartedClimbing = true;
        yield return new WaitForSeconds(0.1f);
        hasStartedClimbing = false;
    }
    void ClimbTransition(bool movingUp)
    {
        StartCoroutine(ClimbTransitionCo(movingUp));
    }
    private IEnumerator ClimbTransitionCo(bool movingUp)
    { 
        freezeInput = true;
        finishedClimbTransition = false;
        Vector3 newPos = Vector3.zero;
        if (movingUp)
        {
            newPos = new Vector3(ladder.posX, transformY + ladder.handlerTopOffset, 0);
        }
        else
        {
            transform.position = new Vector3(ladder.posX, ladder.posTopHandlerY - climbSpriteHeight + ladder.handlerTopOffset, 0);
            newPos = new Vector3(ladder.posX, ladder.posTopHandlerY - climbSpriteHeight, 0);
        }
        while (transform.position != newPos)
        {
            transform.position = Vector3.MoveTowards(transform.position, newPos, climbSpeed * Time.deltaTime);
            animator.speed = 1;
            animator.Play("climbing");//if we had Player_ClimbTop replace
            yield return null;
        }
        isClimbingDown = false;
        finishedClimbTransition = true;
        FreezeInput(true);
    }

    void PlayerShootInput()
    {
        float shootTimeLength = 0;
        float keyShootReleaseTimeLength = 0;

        if (!freezeInput)
        {
           keyShoot = Input.GetKeyDown(KeyCode.C); // enter key
        }

        if(keyShoot && keyShootRelease )
        { 
                isShooting = true;
                keyShootRelease = false;
                shootTime = Time.time;
                Invoke("ShootBullet", 0.01f);
          
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
                keyShootRelease = true;
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
            currentHealth -= damage;
            currentHealth = Mathf.Clamp(currentHealth,0,maxHealth);
            UIEnergyBars.Instance.SetValue(UIEnergyBars.EnergyBars.PlayerHealth, currentHealth / (float)maxHealth);
            
            if (currentHealth <= 0)
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
            Invincible(true);
            ResetClimbing();
            float hitForceX = 0.50f;
            float hitForceY = 1.5f;
            if(hitSideRight) hitForceX = -hitForceX;
            rb.velocity = Vector2.zero;
            animator.Play("Player_Hit", -1, 0f);
            rb.AddForce(new Vector2(hitForceX, hitForceY),ForceMode2D.Impulse);
            SoundManager.Instance.Play(takingDamageClip);
        }
    }
    void StopDamageAnimation()
    {
        isTakingDamage = false ;
        Invincible(false);
        animator.Play("Player_Hit",-1,0f);
    }
    void Defeat()
    {
        Destroy(gameObject);
    }

    public void FreezeInput(bool freeze)
    {
        freezeInput = freeze;
    }

    public void FreezePlayer(bool freeze)
    {
        if (freeze)
        {
            freezePlayer = true;
            rbConstraints = rb.constraints;
            animator.speed = 0;
            rb.constraints = RigidbodyConstraints2D.FreezeAll;

        }
        else
        {
            freezePlayer = false;
            rb.constraints = rbConstraints;
            animator.speed = 1;

        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Spawn Zones"))
        {
            GameManager.Instance.DespawnEnemies();
        }
    }
}

