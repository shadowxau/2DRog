using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(EnemyController2D))]
public class Enemy : MonoBehaviour
{
    public bool enemyIsHit;
    public bool enemyIsTalking;
    public float talkTimer = 2;
    float defaultTalkTimer;
    public float hitTimer = 1;
    float defaultHitTimer;

    public string enemyDialog;
    public Text enemyText;
    public GameObject enemyCanvas;

    public float maxJumpHeight = 4;
    public float minJumpHeight = 1;
    public float jumpSpeed = .4f;
    public float accelerationSpeedAir = .2f;
    public float accelerationSpeedGround = .1f;
    public float moveSpeed = 6;
    public Vector2 knockBack;
 
    float gravity;
    public Vector3 velocity { get; set; }
    float velocityXSmoothing;
    float maxJumpVelocity;
    float minJumpVelocity;
    public float maxFallVelocity;

    public int facingDirX { get; set; }

    public bool enemyJumping { get; set; }
    bool canDoubleJump;
    int numberOfJumps = 0;

    public bool enemyIsDead { get; set; }
    public bool enemyAttacking { get; set; }

    public Vector2 directionalInput { get; set; }
    public EnemyController2D enemyController;

    public bool enemyCanMove;
    public SpriteRenderer enemySprite;

    // Use this for initialization
    void Start()
    {
        //EnemyController = gameObject.GetComponent<Controller2D>();

        gravity = -(2 * maxJumpHeight) / Mathf.Pow(jumpSpeed, 2);                  // calculate gravity
        maxJumpVelocity = Mathf.Abs(gravity) * jumpSpeed;                          // calculate jumpVelocity
        minJumpVelocity = Mathf.Sqrt(2 * Mathf.Abs(gravity) * minJumpHeight);       // calculate jumpVelocity
        maxFallVelocity = maxJumpVelocity;

        enemyCanvas.SetActive(false);

        defaultHitTimer = hitTimer;
        defaultTalkTimer = talkTimer;
        enemyIsHit = false;
    }

    // Update is called once per frame
    void Update()
    {

        CalculateVelocity();
        enemyController.Move(velocity * Time.deltaTime, directionalInput);

        if (enemyController.collisions.above || enemyController.collisions.below)
        {
            if (enemyController.collisions.below) enemyJumping = false;

            if (enemyController.collisions.slidingDownMaxSlope)
            {
                Vector3 v = velocity;
                v.y += enemyController.collisions.slopeNormal.y * -gravity * Time.deltaTime;
                velocity = v;
            }
            else
            {
                Vector3 v = velocity;
                v.y = 0;
                velocity = v;
            }
        }

        if (!enemyController.collisions.below && numberOfJumps == 0)
        {
            enemyJumping = true;
            canDoubleJump = true;
        }

        if (!enemyController.collisions.below && !enemyController.collisions.above && !enemyController.collisions.left && !enemyController.collisions.right)
        {
            enemyJumping = true;

            if (velocity.y < -maxFallVelocity)
            {
                Vector3 v = velocity;
                v.y = -maxFallVelocity;
                velocity = v;
            }
        }


        CheckIfTalking();
        CheckForAttackCollision();
    }

    // check if enemy has collided with player attack
    void CheckForAttackCollision()
    {
        if (enemyController.collisions.touchPlayerAttack)
        {
            if (!enemyIsHit)
            {
                enemyIsHit = true;
                Debug.Log("EnemyIsHit = true;");

                int knockBackDir = enemyController.collisions.hitDir;

                // Knock back enemy unless a stationary object
                if (enemyCanMove)
                {
                    Vector3 v = velocity;
                    v.x = knockBackDir * knockBack.x;
                    v.y = knockBack.y;
                    velocity = v;
                    Debug.Log("velocity = " + velocity);
                }
            }
        }

        if (enemyIsHit)
        {
            // change color/flash
            enemySprite.material.SetFloat("_FlashAmount", 1);

            hitTimer -= Time.deltaTime;

            if (hitTimer <= 0)
            {
                hitTimer = defaultHitTimer;
                enemyIsHit = false;
            }
        }
    }

    // activated text if enemy is talking
    void CheckIfTalking()
    {
        if (enemyIsTalking)
        {
            // display text on screen
            print("NPC is Talking");

            ShowText(enemyDialog);


            talkTimer -= Time.deltaTime;

            if (talkTimer <= 0)
            {
                enemyCanvas.SetActive(false);

                print("NPC stopped talking");
                enemyIsTalking = false;

                talkTimer = defaultTalkTimer;
            }
        }
    }

    void CalculateVelocity()
    {
        float targetVelocityX = directionalInput.x * moveSpeed;

        Vector3 v = velocity;
        v.x = Mathf.SmoothDamp(velocity.x, targetVelocityX, ref velocityXSmoothing, (enemyController.collisions.below) ? accelerationSpeedGround : accelerationSpeedAir);
        v.y += gravity * Time.deltaTime;
        velocity = v;
    }

    void ShowText(string dialog)
    {
        enemyCanvas.SetActive(true);
        enemyText.text = dialog;
    }

    public int GetFacingDir()
    {
        int direction = enemyController.collisions.facingDir;

        return (direction);
    }

    public void SetDirectionalInput(Vector2 input)
    {
        if (enemyCanMove)
        {
            directionalInput = input;
        }
    }

    // used to display debug info on screen during development
    void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 200, 20), "Velocity X: " + velocity.x.ToString());
        GUI.Label(new Rect(10, 24, 200, 20), "Velocity Y: " + velocity.y.ToString());
        //GUI.Label(new Rect(10, 38, 200, 20), "Item Count: " + itemCount.ToString());
        //GUI.Label(new Rect(10, 52, 200, 20), "ViewPositionX: " + viewPosition.x.ToString());
        //GUI.Label(new Rect(10, 66, 200, 20), "ViewPositionY: " + viewPosition.y.ToString());

        //GUI.Label(new Rect(10, 80, 500, 20), "camera.transform.position" + playerCamera.transform.position);
        //GUI.Label(new Rect(10, 94, 500, 20), "cameraBounds.RightBoundary(" + cameraBounds.RightBoundary + ")");
        //GUI.Label(new Rect(10, 108, 500, 20), "cameraBounds.LeftBoundary(" + cameraBounds.LeftBoundary + ")");
        //GUI.Label(new Rect(10, 122, 500, 20), "cameraBounds.TopBoundary(" + cameraBounds.TopBoundary + ")");
        //GUI.Label(new Rect(10, 136, 500, 20), "cameraBounds.BottomBoundary(" + cameraBounds.BottomBoundary + ")");

        //GUI.Label(new Rect(10, 150, 500, 20), "cameraRightPos = " + cameraRightPos);
        //GUI.Label(new Rect(10, 164, 500, 20), "cameraLeftPos = " + cameraLeftPos);
        //GUI.Label(new Rect(10, 178, 500, 20), "cameraTopPos = " + cameraTopPos);
        //GUI.Label(new Rect(10, 192, 500, 20), "cameraBottomPos = " + cameraBottomPos);

    }

    void OnDrawGizmos()
    {
        // shows player hit box
        Gizmos.color = Color.blue;
        Gizmos.DrawCube(transform.position, new Vector3(this.gameObject.GetComponent<BoxCollider2D>().size.x, this.gameObject.GetComponent<BoxCollider2D>().size.y, 1));
    }
}
