using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class playerScript : MonoBehaviour
{
    Rigidbody2D rb;
    public float speed = 5;
    public float jumpForce = 25;
    private float movement = 0.0f;
    private Animator charAnim;

    //for jumping conditions
    public Transform groundCheckPoint1;
    public Transform groundCheckPoint2;
    public LayerMask groundLayer;
    private bool isGround;
    private bool isGroundLeft;
    private bool isGroundRight;
    public float groundCheckRadius = 0.2f;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        charAnim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        playerMovement();
        checkGround();
        playerAnimation();
    }

    private void playerMovement()
    {
        movement = Input.GetAxis("Horizontal");
        if(movement!=0)
            rb.velocity = new Vector2(movement * speed, rb.velocity.y);
        if (Input.GetButtonDown("Jump") && isGround)
            rb.velocity = new Vector2(rb.velocity.x, jumpForce);
        if (movement < 0)
            transform.eulerAngles = new Vector3(0, -180, 0);
        if (movement > 0)
            transform.eulerAngles = new Vector3(0, 0, 0);
    }

    public void checkGround()
    {
        this.isGroundLeft = Physics2D.OverlapCircle(groundCheckPoint1.position, groundCheckRadius, groundLayer);
        this.isGroundRight = Physics2D.OverlapCircle(groundCheckPoint2.position, groundCheckRadius, groundLayer);
        if (isGroundLeft || isGroundRight)
            this.isGround = true;
        else
            this.isGround = false;
    }

    private void playerAnimation()
    {
        charAnim.SetFloat("Speed", Math.Abs(movement));

        if(isGround)
            charAnim.SetBool("isGround", true);
        else
            charAnim.SetBool("isGround", false);
    }
}
