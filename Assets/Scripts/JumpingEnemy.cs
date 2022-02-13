using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpingEnemy : MonoBehaviour
{
    EnemyController enemyController;
    Rigidbody2D rb;
    BoxCollider2D box;
    Animator animator;

    bool isFacingRight;
    bool isGrounded;
    bool isJumping;

    float jumpTimer;
    float jumpDelay = 1f;

    int jumpPatternIndex;
    int[] jumpPattern;
    int[][] jumpPatterns = new int[][]
    {
        new int[1] { 1 } ,
        new int[2] {0, 1},
        new int[3] {0, 0, 1}
    };

    int jumpVelocityIndex;
    Vector2 jumpVelocity;
    Vector2[] jumpVelocities =
    {
        new Vector2(2.5f, 4.5f),//Low Jump
        new Vector2(1.5f, 6.5f)//High Jump
    };

    public AudioClip jumpLandedClip;

    public enum MoveDirections {Left, Right};
    [SerializeField] MoveDirections moveDirection = MoveDirections.Right;

    // Start is called before the first frame update

    private void Awake()
    {
        enemyController = GetComponent<EnemyController>();
        rb = enemyController.GetComponent<Rigidbody2D>();
        animator = enemyController.GetComponent<Animator>();
        box = enemyController.GetComponent<BoxCollider2D>();

        isFacingRight = true;
        isJumping = false;
        if (moveDirection == MoveDirections.Left)
        {
            isFacingRight = false;
            enemyController.Flip();
        }

        jumpPattern = null;
    }


    private void FixedUpdate()
    {
        isGrounded = false;
        Color raycastColor;
        RaycastHit2D raycastHit;
        float raycastDistance = 0.05f;
        int layerMask = 1 << LayerMask.NameToLayer("Ground");

        //ground check
        Vector3 box_orgin = box.bounds.center;
        box_orgin.y = box.bounds.min.y + (box.bounds.extents.y / 4f);
        Vector3 box_size = box.bounds.size;
        box_size.y = box.bounds.size.y / 4f;
        raycastHit = Physics2D.BoxCast(box_orgin, box_size, 0f, Vector2.down, raycastDistance, layerMask);
        //Jumping Eye box colliding with ground layer
        if (raycastHit.collider != null)
        {
            isGrounded = true;
            //just landed from jumping/falling
            if (isJumping)
            {
                SoundManager.Instance.Play(jumpLandedClip);
                isJumping = false;
            }
        }
        raycastColor = (isGrounded) ? Color.green : Color.red;
        Debug.DrawRay(box_orgin + new Vector3(box.bounds.extents.x, 0), 
            Vector2.down * (box.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_orgin - new Vector3(box.bounds.extents.x, 0), 
            Vector2.down * (box.bounds.extents.y / 4f + raycastDistance), raycastColor);
        Debug.DrawRay(box_orgin - new Vector3(box.bounds.extents.x, box.bounds.extents.y / 4f + raycastDistance), 
            Vector2.right * (box.bounds.extents.x * 2), raycastColor);

      
    }
    // Update is called once per frame
    void Update()
    {
        if(enemyController.freezeEnemy)
        {
            return;
        }

        GameObject player = GameObject.FindGameObjectWithTag("Player");

        if(isGrounded)
        {
            animator.Play("Jumping_Grounded");
            rb.velocity = new Vector2(0, rb.velocity.y);
            jumpTimer -= Time.deltaTime;
            if(jumpTimer < 0)
            {
                if(jumpPattern == null)
                {
                    jumpPatternIndex = 0;
                    jumpPattern = jumpPatterns[Random.Range(0, jumpPatterns.Length)];
                }
                jumpVelocityIndex = jumpPattern[jumpPatternIndex];
                jumpVelocity = jumpVelocities[jumpVelocityIndex];
                if (player.transform.position.x <= transform.position.x)
                {
                    jumpVelocity.x *= -1;
                }
                rb.velocity = new Vector2(rb.velocity.x, jumpVelocity.y);
                jumpTimer = jumpDelay;
                if(++jumpPatternIndex > jumpPattern.Length-1)
                {
                    jumpPattern = null;
                }
            }
        }
        else
        {
          
            rb.velocity = new Vector2(jumpVelocity.x, rb.velocity.y);
            isJumping = true;
            if(jumpVelocity.x <= 0)
            {
                if(isFacingRight)
                {
                    isFacingRight = !isFacingRight;
                    enemyController.Flip();
                }
            }
            else
            {
                if (!isFacingRight)
                {
                    isFacingRight = !isFacingRight;
                    enemyController.Flip();
                }
            }
            animator.Play("Jumping_Jump");
        }

    }

    
}
