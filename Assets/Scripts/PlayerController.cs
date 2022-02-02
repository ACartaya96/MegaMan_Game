using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public float speed;
    public float jumpHeight;
    float horizontal;


    BoxCollider2D boxCollider2D;
    Rigidbody2D rb;
    Vector2 lookDirection;
    public Transform feet;
    public LayerMask groundLayers;

    // Start is called before the first frame update
    void Start()
    {
        boxCollider2D = GetComponent<BoxCollider2D>();
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontal = Input.GetAxis("Horizontal");    

        if (Input.GetButtonDown("Jump") && isGrounded())
        {
            Jump();
        }
    }

    private void FixedUpdate()
    {
        Vector2 movement = new Vector2(horizontal * speed, rb.velocity.y);

       rb.velocity = movement;
    }

    private void Jump()
    {
        Vector2 movement = new Vector2(rb.velocity.x, jumpHeight);

        rb.velocity = movement;
    }

    public bool isGrounded()
    {
        Collider2D groundCheck = Physics2D.OverlapCircle(feet.position, 0.5f, groundLayers);

        if(groundCheck.gameObject != null)
        {
            return true;
        }
        return false;
    }

}
