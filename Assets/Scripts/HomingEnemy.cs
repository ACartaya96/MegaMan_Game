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

    // Start is called before the first frame update
    void Start()
    {
        enemyController = GetComponent<EnemyController>();
        rb = enemyController.GetComponent<Rigidbody2D>();
        animator = enemyController.GetComponent<Animator>();

        isFacingRight = true;
        if(moveDirection == MoveDirection.Left)
        {
            isFacingRight = false;
            enemyController.Flip();
        }
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
                break;
        }

        //Animation.Play("Homing_Flying");


        /*timer -= Time.deltaTime;
        
        if(timer < 0)
        {
            moving = -moving;
            timer = changeTime;
        }*/
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
        /*Vector2 position = rb.position;
        position.x = position.x + speed * moving * Time.deltaTime;
        rb.MovePosition(position);*/
     }

     void HomeInPlayer()
     {
         if(Vector2.Distance(transform.position, player.position) > 0.5)
         {
             transform.position = Vector2.MoveTowards(transform.position, player.position, speed * Time.deltaTime);
         }
         
     }

     bool CanSeePlayer(float distance)
     {
         bool val = false;
         RaycastHit2D hit = Physics2D.CircleCast(rb.position, radius, direction);
         Debug.DrawRay(rb.position, direction, Color.red);
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
