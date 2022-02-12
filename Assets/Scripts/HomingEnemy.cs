using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingEnemy : MonoBehaviour
{
    public int moving;
    public float speed;
    public float radius;
    public float distance;
    public float changeTime;
    public bool canSeePlayer;
    float timer;

    [SerializeField] Transform player;
    [SerializeField] Vector2 direction;

    Rigidbody2D rb;
    GameObject kg;
    Animator animator;
    EnemyController enemyController;

    bool isFacingRight;
    bool isFollowingPath;
    Vector3 startPos;
    Vector3 endPos;
    Vector3 midPoint;
    float pathTimeStart;

    public float bezierTime = 1f;
    public float bezierDistance = 1f;
    public Vector3 bezierHeight = new Vector3(0, 0.8f, 0);

    public enum MoveDirection { Left, Right };
    [SerializeField] MoveDirection moveDirection = MoveDirection.Left;

    //State Machine
    const int PATROLLING = 0;
    const int HOMING = 1;
    public int state = PATROLLING;



    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        rb = enemyController.GetComponent<Rigidbody2D>();
        animator = enemyController.GetComponent<Animator>();
        kg = GameObject.FindGameObjectWithTag("Player");

        isFacingRight = true;
        if (moveDirection == MoveDirection.Left)
        {
            isFacingRight = false;
            enemyController.Flip();
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if(enemyController.freezeEnemy)
        {
            pathTimeStart += Time.deltaTime;
            return;
        }
        canSeePlayer = CanSeePlayer(distance);
        //AI Behavior
        switch (state)
        {
            case PATROLLING:
                   Patrol();
                if (canSeePlayer)
                {
                    state = HOMING;
                }
                break;
            case HOMING:
               
                HomeInPlayer();
                if (kg.GetComponent<PlayerController>().isInvincible)
                {
                    state = PATROLLING;
                    ResetFollowingPath();
                    SetMoveDirection(MoveDirection.Left);
                }
                break;
        }

        animator.Play("Homing_Flying");

    }
    // Update is called once per frame

    public void SetMoveDirection(MoveDirection direction)
    {
        moveDirection = direction;
        if(moveDirection == MoveDirection.Left)
        {
            if(isFacingRight)
            {
                isFacingRight = !isFacingRight;
                enemyController.Flip();
            }
        }
        else
        {
            if(!isFacingRight)
            {
                isFacingRight = !isFacingRight;
                enemyController.Flip();
            }
        }
    }

    public void ResetFollowingPath()
    {
        isFollowingPath = false;
    }

   

     void Patrol()
     {

        if(!isFollowingPath)
        {
            float distance = (isFacingRight) ? bezierDistance : -bezierDistance;
            startPos = rb.transform.position;
            endPos = new Vector3(startPos.x + distance, startPos.y, startPos.z);
            midPoint = startPos + (((endPos - startPos) /2) + bezierHeight);
            pathTimeStart = Time.time;
            isFollowingPath = true;
        }
        else
        {
            float percentage = (Time.time - pathTimeStart) / bezierTime;
            rb.transform.position = UtilityFunction.CalculateQuadraticBezierPoint(startPos, midPoint, endPos, percentage);
            if (percentage >= 1f)
            {
                bezierHeight *= -1;
                isFollowingPath = false;
            }
        }
     }

     void HomeInPlayer()
     {
        pathTimeStart += Time.deltaTime;
        transform.position = Vector2.MoveTowards(transform.position, kg.GetComponent<PlayerController>().transform.position, speed * Time.deltaTime);
        
        if(kg.GetComponent<PlayerController>().transform.position.x > transform.position.x)
        {
            SetMoveDirection(MoveDirection.Right);
        }
        else
        {
            SetMoveDirection(MoveDirection.Left);
        }
     }

     bool CanSeePlayer(float distance)
     {
         bool val = false;
         RaycastHit2D hit = Physics2D.CircleCast(rb.position, radius, direction);
        
         if (hit.collider != null)
         {
             if (hit.collider.tag == "Player")
             {
                val = true;
             }
             else
             {
                 val = false;
             }
         }
         return val;
     }
}
