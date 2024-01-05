using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour {
    // Update is called once per frame
    [SerializeField] Rigidbody2D rb;
    [SerializeField] FXManager fx; 
    [SerializeField] float deadzone; 
    [SerializeField] float moveForce, maxSpeed, jumpForce, damageForce;
    [SerializeField, Range(0, 1)] float walkSlow, airDrag, attackSlow; 
    [SerializeField] AnimationCurve jumpCurve; 
    [SerializeField] float jumpCooldown, attackCooldown, respawnTimer; 
    [SerializeField] int totalJumpFrames; 
    public int playerId;
    FightManager fightManager; 
    bool facingRight = true; 
    public int blocking = 0; 
    public bool onLeftSide; 
    Vector3 leftScale = new Vector3(.35f, .35f, .35f);
    Vector3 rightScale = new Vector3(-.35f, .35f, .35f);
    [SerializeField] float damageLockDelay = 1.5f; 
    [SerializeField] float launchForce; 
    int activeJumpFrame;

    float movementInput; 
    bool moveStart, moveLeft, moveRight, moveJump, moveAttack, moveDuck, moveBlock, jumpActive = false, canAttack = true, canJump = true, canHurt = true, controllerWalk, keyboardWalk, moveLock, flipLock, charGrounded, damageLock, dead;

    Animator activeAnimator; 
    GameObject activeSister; 
    public int activeSisterId; 
    [SerializeField]
    GameObject[] sisters; 

    void Awake() {
        fightManager = GameObject.Find("FightManager").GetComponent<FightManager>(); 
    }

    public void Activate() {
        GetComponent<PlayerInput>().SwitchCurrentActionMap("Fight"); 
        GetComponent<OrderPicker>().Deactivate(); 
        rb.simulated = true; 
    }

    public void Reset(Vector3 pos) {
        GetComponent<PlayerInput>().SwitchCurrentActionMap("OrderPicking"); 
        GetComponent<OrderPicker>().Activate(); 
        transform.position = pos; 
        rb.simulated = false; 
        activeSister.SetActive(false); 
    }

    public void DeathSequence(int sId, Vector3 pos, bool respawn) {
        activeAnimator.SetTrigger("Losing"); 
        canHurt = false;
        dead = true;
        StartCoroutine(DelayDeath(sId, pos, respawn)); 
    }

    public void SetSister(int sId, Vector3 pos, bool respawn) {
        if(activeSister != null) {
            activeSister.SetActive(false); 
        }
        // move the player object to the spawn position and turn off physics momentarily
        transform.position = pos; 
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f; 

        // activate the correct sister object and assign the animator
        activeSister = sisters[sId];
        activeSisterId = sId; 
        activeSister.SetActive(true);
        activeAnimator = activeSister.GetComponent<Animator>(); 

        if(respawn) {
            StartCoroutine(RespawnRoutine()); 
        }

        // set the correct facing direction based on player spawn
        if(playerId == 1) {
            activeSister.transform.localScale = rightScale;
            facingRight = true; 
        }
        if(playerId == 2) {
            activeSister.transform.localScale = leftScale;
            facingRight = false; 
        }
    }
    
    public void MovementInput(InputAction.CallbackContext c) {
        movementInput = c.ReadValue<float>(); 
    }

    public void JumpInput(InputAction.CallbackContext c) {
        if(c.ReadValue<float>() > 0 && !moveLock) {
            moveJump = true; 
        } else {
            moveJump = false;
            jumpActive = false; 
        }
    }

    public void AttackInput(InputAction.CallbackContext c) {
        if(c.ReadValue<float>() > 0) {
            moveAttack = true; 
        } else {
            moveAttack = false; 
        }
    }

    public void DuckInput(InputAction.CallbackContext c) {
        bool mD = false; 
        if(c.ReadValue<float>() > .75F) {
            mD = true; 
        }
        if(mD != moveDuck && !dead) {
            activeAnimator.SetBool("Ducking", mD); 
        } 
        moveDuck = mD; 
    }

    public void BlockInput(InputAction.CallbackContext c) {
        bool mB = false; 
        if(c.ReadValue<float>() > 0) {
            mB = true; 
        }
        if(mB != moveBlock && !dead) {
            activeAnimator.SetBool("Blocking", mB); 
        } 
        moveBlock = mB; 
    }

    public void WalkInput(InputAction.CallbackContext c) {
        if(c.ReadValue<float>() > 0) {
            keyboardWalk = true; 
        } else {
            keyboardWalk = false;
        }
    }

    public void BlockBack() {
        Debug.Log("blockback"); 
        if(facingRight) {
            rb.AddForce(transform.right * (-1f * damageForce)); 
        } else {
            rb.AddForce(transform.right * (1f * damageForce)); 
        }
    }

    void Update() {
        if(activeSister) {
            // lock appropriate controls based on the animation
            // no moving when attacking, no turning around when in the air, etc
            moveLock = activeSister.GetComponent<AnimationLocker>().moveLocked; 
            flipLock = activeSister.GetComponent<AnimationLocker>().flipLocked; 
            // check for grounded
            if(!charGrounded) {
                charGrounded = GetComponent<GroundCheck>().isGrounded;
                if(charGrounded) {
                    fx.GetComponent<FXManager>().GetFX(7, facingRight);   
                }
            }
            charGrounded = GetComponent<GroundCheck>().isGrounded; 
            // double check flipLock, as being unable to turn around while grounded is frustrating
            if(charGrounded && canAttack) {
                flipLock = false; 
            }
            // determine move direction
            if(movementInput < 0) {
                if(!moveLeft && movementInput < -.25f) {
                    // fx.GetComponent<FXManager>().GetFX(6, facingRight);   
                }
                moveLeft = true;
                moveRight = false;
            } else {
                moveLeft = false;
                if(!moveRight && movementInput > .25f) {
                    // fx.GetComponent<FXManager>().GetFX(6, facingRight);   
                }
                moveRight = true; 
            }
            // determine if the control stick is being angled to walk, not fully pressed
            if(Mathf.Abs(movementInput) < .9f) {
                controllerWalk = true; 
            } else {
                controllerWalk = false; 
            }
            // cancel movement if input is in deadzone, we're locked, we're ducking, or we're blocking
            if(Mathf.Abs(movementInput) < deadzone || moveLock || moveDuck || moveBlock || !canAttack) {
                moveLeft = false;
                moveRight = false; 
            } 

            if(!dead) {
                // do the actual moving around
                // check direction and if it's a new direction, animate the turnaround
                if(moveRight) {
                    if(!facingRight && charGrounded) {
                        activeAnimator.SetTrigger("Turnaround"); 
                    }
                    activeSister.transform.localScale = rightScale;
                    facingRight = true; 
                } else if (moveLeft) {
                    if(facingRight && charGrounded) {
                        activeAnimator.SetTrigger("Turnaround"); 
                    }
                    activeSister.transform.localScale = leftScale; 
                    facingRight = false; 
                } else {
                // if we're not moving, autoflip. could be enhanced to not autoflip block
                // } else if(!moveBlock) {
                    if(onLeftSide) {
                        activeSister.transform.localScale = rightScale; 
                        facingRight = true; 
                    } else {
                        activeSister.transform.localScale = leftScale; 
                        facingRight = false; 
                    }
                }
                // set all animation parameters
                activeAnimator.SetBool("Moving", moveLeft | moveRight); 
                activeAnimator.SetBool("Grounded", charGrounded); 
                activeAnimator.SetBool("Blocking", moveBlock); 
            }
        }
    }

    void FixedUpdate() {
        Vector2 movementVector = Vector2.zero;
        if(!dead) {
            movementVector = Vector2.zero; 
            float _airDrag = 1; 
            float _walkSlow = 1; 
            float _attackSlow = 1;
            if(!charGrounded) {
                _airDrag = airDrag; 
            }
            if(keyboardWalk || controllerWalk) {
                _walkSlow = walkSlow; 
                activeAnimator.SetBool("Walking", true); 
            } else if(activeAnimator) {
                activeAnimator.SetBool("Walking", false); 
            }
            if(moveDuck) {
                _walkSlow = walkSlow; 
            }
            if(!canAttack) {
                _attackSlow = attackSlow; 
            }
            if (moveLeft) {
                movementVector += (Vector2.left * moveForce) * _airDrag * _walkSlow * _attackSlow; 
                if(rb.velocity.x < -maxSpeed) {
                    float brakeForce = (Mathf.Abs(rb.velocity.x) - maxSpeed) * moveForce; 
                    movementVector += (Vector2.right * brakeForce); 
                }
            }
            if (moveRight) {
                movementVector += (Vector2.right * moveForce) * _airDrag * _walkSlow * _attackSlow; 
                if(rb.velocity.x > maxSpeed) {
                    float brakeForce = (rb.velocity.x - maxSpeed) * moveForce; 
                    movementVector += (Vector2.left * brakeForce); 
                }
            }
            if (moveAttack) {
                if(canAttack) {
                    if (charGrounded) {
                        if (!moveDuck) {
                            activeAnimator.SetTrigger("HiAttack"); 
                            StartCoroutine(JumpCooldown()); 
                            StartCoroutine(AttackCooldown()); 
                        } else {
                            activeAnimator.SetTrigger("LowAttack");
                            StartCoroutine(JumpCooldown()); 
                            StartCoroutine(AttackCooldown()); 
                        }
                    } else {
                        activeAnimator.SetTrigger("AirAttack");
                        StartCoroutine(JumpCooldown()); 
                        StartCoroutine(AttackCooldown()); 
                    }
                }
            }
            if (moveJump) {
                if(charGrounded && canJump && !jumpActive && !moveBlock && !moveDuck) {
                    Jump(); 
                }
            }
            if (jumpActive && activeJumpFrame < totalJumpFrames) {
                movementVector += (Vector2.up * jumpForce * jumpCurve.Evaluate(activeJumpFrame/totalJumpFrames)); 
                activeJumpFrame++;
            }
            rb.AddForce(movementVector); 
        } else { 
            movementVector = Vector3.zero; 
        }
    }

    void Jump() {
        if(!jumpActive && !dead) {
            activeAnimator.SetTrigger("Jump");
            fx.GetComponent<FXManager>().GetFX(5, facingRight);   
        }
        jumpActive = true; 
        StartCoroutine(JumpCooldown()); 
        activeJumpFrame = 0; 
    }

    public void Damage(Vector3 attackDirection, HitDetection.HitType hitType, int sisterId) {
        // if we're not in temp invincibility
        if(canHurt && !dead) {
            activeAnimator.SetTrigger("Damage");
            Debug.Log("hit by sister " + sisterId);
            StartCoroutine(JumpCooldown()); 
            StartCoroutine(DamageCooldown()); 
            StartCoroutine(DamageLock()); 
            rb.AddForce(attackDirection * damageForce); 
            int d = 0;

            // if we're not blocking, or we're high and they're low, or vice versa
            if(blocking == 0 || hitType == HitDetection.HitType.High && blocking == 2 || hitType == HitDetection.HitType.Low && blocking == 1) {
                if(hitType == HitDetection.HitType.Low) {
                    fx.GetComponent<FXManager>().GetFX(1, facingRight);
                } else if(hitType == HitDetection.HitType.High) {
                    fx.GetComponent<FXManager>().GetFX(0, facingRight); 
                } else {
                    fx.GetComponent<FXManager>().GetFX(2, facingRight); 
                }
                switch(activeSisterId) {
                    // we are ROCK
                    case 0:
                        switch(sisterId) {
                            // hit by ROCK
                            case 0: 
                                d = 2;
                                break;
                            // hit by PAPER
                            case 1:
                                d = 3;
                                break;
                            // hit by SCISSORS
                            case 2:
                                d = 1;
                                break;
                        }
                        break;
                    // we are PAPER
                    case 1:
                        switch(sisterId) {
                            // hit by ROCK
                            case 0: 
                                d = 1;
                                break;
                            // hit by PAPER
                            case 1:
                                d = 2;
                                break;
                            // hit by SCISSORS
                            case 2:
                                d = 3;
                                break;
                        }
                        break;
                    // we are SCISSORS
                    case 2:
                        switch(sisterId) {
                            // hit by ROCK
                            case 0: 
                                d = 3;
                                break;
                            // hit by PAPER
                            case 1:
                                d = 1;
                                break;
                            // hit by SCISSORS
                            case 2:
                                d = 2;
                                break;
                        }
                        break;
                }
                fightManager.PlayerHit(playerId, d); 
            } else {
                if(hitType == HitDetection.HitType.Low) {
                    fx.GetComponent<FXManager>().GetFX(3, facingRight);
                    fightManager.BlockBack(playerId); 
                } else {
                    fx.GetComponent<FXManager>().GetFX(4, facingRight); 
                    fightManager.BlockBack(playerId); 
                }
            }
        }
    }

    private IEnumerator JumpCooldown() { canJump = false; yield return new WaitForSeconds(jumpCooldown); canJump = true; }
    private IEnumerator DamageCooldown() { canHurt = false; yield return new WaitForSeconds(jumpCooldown); canHurt = true; }
    private IEnumerator AttackCooldown() { canAttack = false; yield return new WaitForSeconds(attackCooldown); canAttack = true; }
    private IEnumerator DamageLock() { damageLock = true; yield return new WaitForSeconds(damageLockDelay); damageLock = false; }
    private IEnumerator DelayDeath(int sId, Vector3 pos, bool respawn) {
        yield return new WaitForSeconds(3f);
        // activeAnimator.SetTrigger("Reset"); 
        SetSister(sId, pos, respawn);
        dead = false; 
        canHurt = true; 
    }

    private IEnumerator RespawnRoutine() {
        rb.bodyType = RigidbodyType2D.Kinematic; 
        rb.simulated = false;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f; 
        yield return new WaitForSeconds(respawnTimer);
        rb.bodyType = RigidbodyType2D.Dynamic; 
        rb.simulated = true;
        rb.velocity = Vector3.zero;
        rb.angularVelocity = 0f; 
        activeAnimator.SetTrigger("Respawn");
    }
}