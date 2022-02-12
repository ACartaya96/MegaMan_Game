using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlasterEnemyController : MonoBehaviour
{
    Animator animator;
    BoxCollider2D box;
    Rigidbody2D rb;
    EnemyController enemyController;

    int bulletIndex = 0;

    float closedTimer;
    public float closedDelay = 2f;

    bool doAttack;
    public float playerRange = 2f;

    public enum BlasterState { Closed, Open }
    [SerializeField] BlasterState blasterState = BlasterState.Closed;

    public enum BlasterOrientation { Bottom, Top, Left, Right }
    [SerializeField] public BlasterOrientation blasterOrientation;


    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        animator = enemyController.GetComponent<Animator>();
        rb = enemyController.GetComponent<Rigidbody2D>();
        box = enemyController.GetComponent<BoxCollider2D>();
       
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (enemyController.freezeEnemy)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        switch(blasterState)
        {
            case BlasterState.Closed:
                animator.Play("Blaster_Closed");
                if(player != null && !doAttack)
                {
                    float distance = Vector2.Distance(transform.position, player.transform.position);
                    if(distance <= playerRange)
                    {
                        doAttack = true;
                        closedTimer = closedDelay;
                    }
                }
                if(doAttack)
                {
                    closedTimer -= Time.deltaTime;
                    if(closedTimer <= 0)
                    {
                        blasterState = BlasterState.Open;
                    }
                }
                break;
            case BlasterState.Open:
                animator.Play("Blaster_Open");
                break;
        }
        SetOrientation();
    }

    public void SetOrientation()
    {
        transform.rotation = Quaternion.identity;
        
        switch (blasterOrientation)
        {
            case BlasterOrientation.Bottom:
                transform.Rotate(0, 0, -90f);
                break;
            case BlasterOrientation.Top:
                transform.Rotate(0, 0, 90f);
                break;
            case BlasterOrientation.Left:
                transform.Rotate(0, 180f, 0);
                break;
            case BlasterOrientation.Right:
                transform.Rotate(0, 0, 0);
                break;
        }
    }

    private void ShootBullet()
    {
        GameObject bullet;
        Vector2[] bulletVectors =
        {
            new Vector2(0.75f,0.75f),
            new Vector2(1f,0.15f),
            new Vector2(1f,-0.15f),
            new Vector2(0.75f,-0.75f)
        };

        switch (blasterOrientation)
        {
            case BlasterOrientation.Left:
               
                break;
            case BlasterOrientation.Right:
                bulletVectors[bulletIndex].x *= -1;
                break;
            case BlasterOrientation.Top:
                bulletVectors[bulletIndex] = UtilityFunction.RotateByAngle(bulletVectors[bulletIndex], 90f);
                break;
            case BlasterOrientation.Bottom:
                bulletVectors[bulletIndex] = UtilityFunction.RotateByAngle(bulletVectors[bulletIndex], -90f);
                break;
        }
        //Needs code
        bullet = Instantiate(enemyController.bulletPrefab);
        bullet.name = enemyController.bulletPrefab.name;
        bullet.transform.position = enemyController.bulletShootPos.transform.position;
        bullet.GetComponent<Bullet_Script>().SetDamageValue(enemyController.bulletDamage);
        bullet.GetComponent<Bullet_Script>().SetBulletSpeed(enemyController.bulletSpeed);
        bullet.GetComponent<Bullet_Script>().SetBulletDirection(bulletVectors[bulletIndex]);
        bullet.GetComponent<Bullet_Script>().SetCollideWithTags("Player");
        bullet.GetComponent<Bullet_Script>().SetDestroyDelay(5f);
        bullet.GetComponent<Bullet_Script>().Shoot();

        if (++bulletIndex > bulletVectors.Length - 1)
        {
            bulletIndex = 0;
        }

        SoundManager.Instance.Play(enemyController.shootBulletClip);
    }

    private void InvinvibleAnimationStart()
    {
        enemyController.Invicible(true);
    }

    private void OpenAnimationStart()
    {
        enemyController.Invicible(false);
    }    

    private void OpenAnimationStop()
    {
        doAttack = false;
        blasterState = BlasterState.Closed;
    }
}

