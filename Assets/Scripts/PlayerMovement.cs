using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public GameObject player;
    Rigidbody2D playerRB;
    public float speed;

    public int movementType;
    public float jumpForce;


    public Transform groundCheck;
    private float groundCheckRadius = 0.2f;
    private bool isGrounded;

    void Start()
    {
        playerRB = player.GetComponent<Rigidbody2D>();
    }

    void Update()
    {

        if (movementType == 1)
        {
            topDownMovement();
        }
        else if (movementType == 2)
        {
            platformMovment();
        }

    }

    void FixedUpdate()
    {

    }

    void topDownMovement()
    {
        playerRB.gravityScale = 0;

        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");

        Vector2 direction = new Vector2(horizontal, vertical).normalized;
        Vector2 movement = speed * Time.deltaTime * direction;

        player.transform.Translate(movement.x, movement.y, 0);
    }

    void platformMovment()
    {
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius);

        playerRB.gravityScale = 5;
        float horizontal = Input.GetAxisRaw("Horizontal");
        Vector2 direction = new Vector2(horizontal, 0).normalized;
        Vector2 movement = speed * Time.deltaTime * direction;

        player.transform.Translate(movement.x, 0, 0);


        jump();

    }

    void jump()
    {
        if (Input.GetKeyDown(KeyCode.Z) && isGrounded)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, jumpForce);
        }

        if (Input.GetKeyUp(KeyCode.Z) && playerRB.linearVelocity.y > 0)
        {
            playerRB.linearVelocity = new Vector2(playerRB.linearVelocity.x, playerRB.linearVelocity.y * 0.5f);
        }
    }
}
