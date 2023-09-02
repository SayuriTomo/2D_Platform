using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Security.Cryptography;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Rigidbody2D rb;
    private Animator anim;

    private float MovementInputDiection;
    private float DashTimeLeft;
    private float LastImageXPos;
    private float LastDash = -100f;

    private int AmountOfJumpRemain;
    private int FaceDirection = 1;

    private bool bIsWalking;
    private bool bIsFacingRight = true;
    private bool bIsGrounded;
    private bool bIsTouchingWall;
    private bool bIsWallSliding;
    private bool bCanJump;
    private bool bIsDashing = false;
    private bool bCanFlip = true;
    private bool bCanMove = true;
    private bool bIsButterfly = false;

    public int AmountOfJumps = 2;
    public float MovementSpeed = 10.0f;
    public float JumpForce = 16.0f;
    public float GroundCheckRadius;
    public float WallCheckDistance;
    public float WallSlideSpeed;
    //public float MovementForceInAir;
    public float AirDragMultiplier = 0.95f;
    public float VariableJumpHeightMultiplier = 0.5f;
    //public float WallHopeForce;
    public float WallJumpForce;
    public float DashTime;
    public float DashSpeed;
    public float DistanceBetweenImages;
    public float DashCoolDown;
    
    public bool bIsDead = false;
    //public Vector2 WallHopeDirection;
    public Vector2 WallJumpDirection;
    public Vector2 RespawnPosition;

    public LevelManager LevelManager;

    public Transform GroundCheck;
    public Transform WallCheck;
    public LayerMask WhatIsGround;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.sharedMaterial = new PhysicsMaterial2D() { friction = 0f, bounciness = 0f };
        anim = GetComponent<Animator>();
        AmountOfJumpRemain = AmountOfJumps;
        //WallHopeDirection.Normalize();
        WallJumpDirection.Normalize();

        RespawnPosition = transform.position;
        LevelManager = FindObjectOfType<LevelManager>();
    }

    // Update is called once per frame
    void Update()
    {
        CheckInput();
        CheckMovementDirection();
        UpdateAnimations();
        CheckIfCanJump();
        CheckIfWallSliding();
        CheckDash();
        CheckDie();
    }

    private void FixedUpdate()
    {
        ApplyMovement();
        CheckSurrondings();
        
    }

    private void CheckIfWallSliding()
    {
        if (bIsTouchingWall && rb.velocity.y < 0&&!bIsButterfly)
        {
            bIsWallSliding = true;
        }
        else
        {
            bIsWallSliding = false;
        }
    }
    private void CheckIfCanJump()
    {
        if ((bIsGrounded && rb.velocity.y <= 0)||bIsWallSliding||bIsButterfly)
        {
            AmountOfJumpRemain = AmountOfJumps;
        }

        if(AmountOfJumpRemain <= 0)
        {
            bCanJump = false;
        }
        else
        {
            bCanJump = true;
        }
    }
    private void CheckSurrondings()
    {
        bIsGrounded = Physics2D.OverlapCircle(GroundCheck.position, GroundCheckRadius, WhatIsGround);

        bIsTouchingWall = Physics2D.Raycast(WallCheck.position,transform.right, WallCheckDistance, WhatIsGround);
    }

    private void CheckMovementDirection()
    {
        if (bIsFacingRight && MovementInputDiection < 0&&bCanFlip)
        {
            Flip();
        }
        else if (!bIsFacingRight && MovementInputDiection > 0&&bCanFlip)
        {
            Flip();
        }

        if(rb.velocity.x != 0)
        {
            bIsWalking = true;
        }
        else
        {
            bIsWalking = false;
        }
    }

    private void UpdateAnimations()
    {
        anim.SetBool("bIsWalking", bIsWalking);
        anim.SetBool("bIsWallSliding", bIsWallSliding);
        anim.SetBool("bIsDashing", bIsDashing);
        anim.SetBool("bIsButterfly", bIsButterfly);
        anim.SetBool("bIsDead", bIsDead);
    }

    private void CheckInput()
    {
        MovementInputDiection = Input.GetAxisRaw("Horizontal");

        if (Input.GetButtonDown("Jump"))
        {
            Jump();
        }

        if (Input.GetButtonUp("Jump"))
        {
            rb.velocity = new Vector2(rb.velocity.x, rb.velocity.y * VariableJumpHeightMultiplier);
        }

        if (Input.GetButtonDown("Dash"))
        {
            if (Time.time >= (LastDash + DashCoolDown))
            {
                AttemptToDash();
            }
            
        }

    }

    private void AttemptToDash()
    {
        if (!bIsButterfly)
        {
            bIsDashing = true;
            DashTimeLeft = DashTime;
            LastDash = Time.time;

            PlayerAfterImagePool.Instance.GetFromPool();
            LastImageXPos = transform.position.x;
        }
    }

    private void CheckDash()
    {
        if (!bIsDead)
        {
            if (bIsDashing)
            {
                if (DashTimeLeft > 0)
                {
                    bCanMove = false;
                    bCanFlip = false;

                    rb.velocity = transform.right * DashSpeed;

                    DashTimeLeft -= Time.deltaTime;

                    if (Mathf.Abs(transform.position.x - LastImageXPos) < DistanceBetweenImages)
                    {
                        PlayerAfterImagePool.Instance.GetFromPool();
                        LastImageXPos = transform.position.x;
                    }
                }

                if (DashTimeLeft <= 0 || bIsTouchingWall)
                {
                    bIsDashing = false;
                    bCanMove = true;
                    bCanFlip = true;
                }
            }
        }
    }
    private void Jump()
    {
        if (!bIsDead)
        {
            if ((bIsWallSliding || bIsTouchingWall) && MovementInputDiection != 0 && bCanJump && MovementInputDiection != FaceDirection)
            {
                bIsWallSliding = false;
                AmountOfJumpRemain -= 1;
                Vector2 ForceToAdd = new Vector2(
                    WallJumpForce * WallJumpDirection.x * MovementInputDiection,
                    WallJumpForce * WallJumpDirection.y);
                rb.AddForce(ForceToAdd, ForceMode2D.Impulse);
            }
            else if (bCanJump && !bIsWallSliding)
            {
                rb.velocity = new Vector2(MovementSpeed * MovementInputDiection, JumpForce);
                AmountOfJumpRemain -= 1;
            }
        }
        /*
        else if (bIsWallSliding&&MovementInputDiection==0&&bCanJump) //Wall Hope
        {
            bIsWallSliding = false;
            AmountOfJumpRemain -= 1;
            Vector2 ForceToAdd = new Vector2(
                WallHopeForce * WallHopeDirection.x * -FaceDirection,
                WallHopeForce * WallHopeDirection.y);
            rb.AddForce(ForceToAdd, ForceMode2D.Impulse);
        }*/
       
        
    }

    private void ApplyMovement()
    {
        if (!bIsDead)
        {
            if (((!bIsWallSliding && bIsGrounded) || bIsButterfly) && bCanMove)
            {
                rb.velocity = new Vector2(MovementSpeed * MovementInputDiection, rb.velocity.y);
            }
            else if (bIsWallSliding)
            {
                if (rb.velocity.y < -WallSlideSpeed)
                {
                    rb.velocity = new Vector2(0, -WallSlideSpeed);
                }
            }
            else if (!bIsGrounded && !bIsWallSliding && MovementInputDiection == 0)
            {
                rb.velocity = new Vector2(rb.velocity.x * AirDragMultiplier, rb.velocity.y);
            }
        }
        

        /*
        if (!bIsGrounded&&!bIsWallSliding&&MovementInputDiection!=0)
        {
            Vector2 ForceToAdd = new Vector2(MovementForceInAir * MovementInputDiection, 0);
            rb.AddForce(ForceToAdd);

            if (Mathf.Abs(rb.velocity.x) > MovementSpeed)
            {
                rb.velocity = new Vector2(MovementSpeed * MovementInputDiection, rb.velocity.y);
            }
        }
        */

    }

    private void Flip()
    {
        if (!bIsWallSliding&&!bIsDead)
        {
            FaceDirection *= -1;
            bIsFacingRight = !bIsFacingRight;
            transform.Rotate(0.0f, 180.0f, 0.0f);
        }
        
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(GroundCheck.position, GroundCheckRadius);
    
        Gizmos.DrawLine(WallCheck.position,
            new Vector3(WallCheck.position.x + WallCheckDistance, WallCheck.position.y, WallCheck.position.z));
    }

    private void OnTriggerEnter2D(Collider2D Collision)
    {
        if(Collision.tag == "Traps"||Collision.tag == "Enemy")
        {
            bIsDead = true;
            LevelManager.Respawn();
        }
        else if(Collision.tag == "Conversion")
        {
           RespawnPosition = Collision.transform.position;
           ApplyConversion();
        }
    }

    private void ApplyConversion()
    {
        if (bIsButterfly)
        {
            bIsButterfly = false;
            JumpForce = 16.0f;
            rb.gravityScale = 3.0f;
        }
        else
        {
            bIsButterfly = true;
            JumpForce = 10.0f;
            rb.gravityScale = 1.0f;
        }
    }

    private void CheckDie()
    {
        if (bIsDead)
        {
            rb.velocity = new Vector2(0, 0);
            if (bIsButterfly)
            {
                bIsButterfly = false;
                JumpForce = 16.0f;
                rb.gravityScale = 3.0f;
            }
        }
    }
}


