using UnityEngine;
using System.Collections;


public class EnemyController2D : RaycastController
{

    public float maxSlopeAngle = 80;                                                                                                                    // max angle can walk on

    public CollisionInfo collisions;
    [HideInInspector]
    public Vector2 moveInput;

    public override void Start()
    {
        base.Start();
        collisions.facingDir = 1;
        collisions.hitDir = 1;
    }

    public void Move(Vector2 velocity, bool isGrounded)
    {
        Move(velocity, Vector2.zero, isGrounded);
    }

    public void Move(Vector2 velocity, Vector2 input, bool isGrounded = false)
    {
        UpdateRaycastOrigins();
        collisions.ResetCollisionInfo();
        collisions.velocityOld = velocity;
        moveInput = input;

        if (velocity.y < 0)
        {
            DescendSlope(ref velocity);
        }

        if (velocity.x != 0)
        {
            collisions.facingDir = (int)Mathf.Sign(velocity.x);
        }

        HorizontalCollisions(ref velocity);

        if (velocity.y != 0)
        {
            VerticalCollisions(ref velocity);
        }

        //Move the object
        transform.Translate(velocity);

        if (isGrounded)
        {
            collisions.below = true;
        }
    }


    void HorizontalCollisions(ref Vector2 velocity)
    {
        float dirX = collisions.facingDir;                                                                                    // if moving down = -1, if moving up = +1
        float rayLength = Mathf.Abs(velocity.x) + skinWidth;

        if (Mathf.Abs(velocity.x) < skinWidth)
        {
            rayLength = 2 * skinWidth;
        }

        #region check front ray
        for (int i = 0; i < horRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight;                          // if moving down start rays from bottom left otherwise if moving up start rays from top left
            rayOrigin += Vector2.up * (horRaySpacing * i);
            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.green);
            
            #region enemyHit
            RaycastHit2D enemyHit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, enemyCollisionMask);
            if (enemyHit)
            {
                CheckObjectCollisions(enemyHit);
            }
            #endregion

            #region triggerHit
            RaycastHit2D triggerHit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, triggerCollisionMask);
            if (triggerHit)
            {
                Debug.Log("front triggerHit = " + triggerHit.collider.name);
                CheckObjectCollisions(triggerHit);
            }
            #endregion

            #region solidHit
            RaycastHit2D solidHit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, solidCollisionMask);
            if (solidHit)
            {
                if (solidHit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(solidHit.normal, Vector2.up);                                                       // find the angle of the slope

                if (i == 0 && slopeAngle <= maxSlopeAngle)                                                                      // move character onto slope instead of hovering above
                {
                    if (collisions.descendingSlope)                                                                             // prevent change of speed when moving between two high angle slopes
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlope = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlope = solidHit.distance - skinWidth;
                        velocity.x -= distanceToSlope * dirX;
                    }
                    ClimbSlope(ref velocity, slopeAngle, solidHit.normal);
                    velocity.x += distanceToSlope * dirX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    velocity.x = (solidHit.distance - skinWidth) * dirX;
                    rayLength = solidHit.distance;                                                                                   // reduce ray length if hit

                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);                  // prevent jitter when hitting side of character on slope
                    }

                    collisions.left = dirX == -1;                                                                               // if hit and going left then collisions.left is true
                    collisions.right = dirX == 1;
                }
            }
            #endregion
        }
        #endregion

        #region Check Back Ray 
        for (int i = 0; i < horRayCount; i++)
        {
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;                          // if moving down start rays from bottom left otherwise if moving up start rays from top left
            rayOrigin += Vector2.up * (horRaySpacing * i);
            Debug.DrawRay(rayOrigin, Vector2.right * dirX * rayLength, Color.green);
            
            #region enemyHit
            RaycastHit2D enemyHit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, enemyCollisionMask);
            if (enemyHit)
            {
                CheckObjectCollisions(enemyHit);
            }
            #endregion
            
            #region triggerHit
            RaycastHit2D triggerHit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, triggerCollisionMask);
            if (triggerHit)
            {
                Debug.Log(" back triggerHit = " + triggerHit.collider.name);
                CheckObjectCollisions(triggerHit);
            }
            #endregion

            #region solidHit
            RaycastHit2D solidHit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, solidCollisionMask);
            if (solidHit)
            {
                if (solidHit.distance == 0)
                {
                    continue;
                }

                float slopeAngle = Vector2.Angle(solidHit.normal, Vector2.up);                                                       // find the angle of the slope

                if (i == 0 && slopeAngle <= maxSlopeAngle)                                                                      // move character onto slope instead of hovering above
                {
                    if (collisions.descendingSlope)                                                                             // prevent change of speed when moving between two high angle slopes
                    {
                        collisions.descendingSlope = false;
                        velocity = collisions.velocityOld;
                    }
                    float distanceToSlope = 0;
                    if (slopeAngle != collisions.slopeAngleOld)
                    {
                        distanceToSlope = solidHit.distance - skinWidth;
                        velocity.x -= distanceToSlope * dirX;
                    }
                    ClimbSlope(ref velocity, slopeAngle, solidHit.normal);
                    velocity.x += distanceToSlope * dirX;
                }

                if (!collisions.climbingSlope || slopeAngle > maxSlopeAngle)
                {
                    velocity.x = (solidHit.distance - skinWidth) * dirX;
                    rayLength = solidHit.distance;                                                                                   // reduce ray length if hit

                    if (collisions.climbingSlope)
                    {
                        velocity.y = Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x);                  // prevent jitter when hitting side of character on slope
                    }

                    collisions.left = dirX == -1;                                                                               // if hit and going left then collisions.left is true
                    collisions.right = dirX == 1;
                }
            }
            #endregion

        }
        #endregion
    }

    void VerticalCollisions(ref Vector2 velocity)
    {
        float dirY = Mathf.Sign(velocity.y);                                                                                    // if moving down = -1, if moving up = +1
        float rayLength = Mathf.Abs(velocity.y) + skinWidth;

        for (int i = 0; i < vertRayCount; i++)
        {
            Vector2 rayOrigin = (dirY == -1) ? raycastOrigins.bottomLeft : raycastOrigins.topLeft;                              // if moving down start rays from bottom left otherwise if moving up start rays from top left
            rayOrigin += Vector2.right * (vertRaySpacing * i + velocity.x);
            //DEBUG - Show Rays
            Debug.DrawRay(rayOrigin, Vector2.up * dirY * rayLength, Color.green);

            #region solidHit
            // Check if object collided with "Solid" collision mask object
            RaycastHit2D solidHit = Physics2D.Raycast(rayOrigin, Vector2.up * dirY, rayLength, solidCollisionMask);

            // check against collidables
            if (solidHit)
            {
                // check if object has hit a collision object with Through tag
                if (solidHit.collider.tag == "Through")
                {
                    if (dirY == 1 || solidHit.distance == 0)
                    {
                        continue;
                    }
                    if (collisions.dropDown)
                    {
                        continue;
                    }
                    if (moveInput.y == -1)
                    {
                        collisions.dropDown = true;
                        Invoke("ResetDropDown", .1f);
                        continue;
                    }
                }

                velocity.y = (solidHit.distance - skinWidth) * dirY;
                rayLength = solidHit.distance;                                                                                           // reduce ray length if hit

                if (collisions.climbingSlope)
                {
                    velocity.x = velocity.y / Mathf.Tan(collisions.slopeAngle * Mathf.Deg2Rad) * Mathf.Sign(velocity.x);            // prevent jitter when hitting top of character on slope
                }

                collisions.below = dirY == -1;                                                                                      // if hit and going down then collisions.below is true
                collisions.above = dirY == 1;
            }
            #endregion
        }

        if (collisions.climbingSlope)                                                                                               // prevent player moving inside slope where two slopes meet
        {
            float dirX = Mathf.Sign(velocity.x);
            rayLength = Mathf.Abs(velocity.x) + skinWidth;
            Vector2 rayOrigin = ((dirX == -1) ? raycastOrigins.bottomLeft : raycastOrigins.bottomRight) + Vector2.up * velocity.y;


            #region solidHit
            // Check if object collided with "Solid" collision mask object
            RaycastHit2D solidHit = Physics2D.Raycast(rayOrigin, Vector2.right * dirX, rayLength, solidCollisionMask);
            if (solidHit)
            {
                float slopeAngle = Vector2.Angle(solidHit.normal, Vector2.up);
                if (slopeAngle != collisions.slopeAngle)
                {
                    velocity.x = (solidHit.distance - skinWidth) * dirX;
                    collisions.slopeAngle = slopeAngle;
                    collisions.slopeNormal = solidHit.normal;
                }
            }
            #endregion
        }
    }

    bool CheckObjectCollisions(RaycastHit2D hit)
    {
        /*
        // check if object has hit an collision object with Item tag
        if (hit.collider.gameObject.tag == "Item")
        {
            Debug.Log(gameObject.ToString() + " hit Item object: " + hit.collider.gameObject.ToString());

            // testing (refreshing stored items in rooms)
            RoomController room = GameObject.FindGameObjectWithTag("RoomController").GetComponent<RoomController>();
            room.items.Remove(hit.collider.gameObject);

            //room.CollectItemsUpdate(hit.collider.name);
            GameController gameControl = GameObject.FindGameObjectWithTag("GameController").GetComponent<GameController>();
            gameControl.CollectItemsUpdate(hit.collider.name);

            collisions.touchItem = true;
            Destroy(hit.collider.gameObject);
            return true;
        }
        */

        // check if object has hit a collision object with Hazard tag
        if (hit.collider.gameObject.tag == "Hazard")
        {
            Debug.Log(gameObject.ToString() + " hit Hazard object: " + hit.collider.gameObject.ToString());
            collisions.touchHazard = true;
            return true;
        }

        /*
        // check if object has hit a collision object with Checkpoint tag
        else if (hit.collider.gameObject.tag == "Checkpoint")
        {
            Debug.Log(gameObject.ToString() + " hit CheckPoint object: " + hit.collider.gameObject.ToString());
            // store the trigger object collided with so that we can check attached scripts (used to change scenes)
            collisions.collidedWith = hit.collider.gameObject;
            collisions.touchCheckpoint = true;
            return true;
        }

        
        // check if object has hit a collision object with the NPC tag
        else if (hit.collider.gameObject.tag == "NPC")
        {
            Debug.Log(gameObject.ToString() + " hit NPC object: " + hit.collider.gameObject.ToString());
            collisions.collidedWith = hit.collider.gameObject;
            collisions.touchNPC = true;
            return true;
        }
        */

        // check if this object has hit a collision object with the PlayerShot tag
        if (hit.collider.gameObject.tag == "PlayerShot")
        {
            Debug.Log(gameObject.ToString() + " hit PlayerShot object: " + hit.collider.gameObject.ToString());
            collisions.collidedWith = hit.collider.gameObject;
            collisions.touchPlayerShot = true;

            playerShotScript shot = hit.collider.GetComponent<playerShotScript>();
            collisions.stunLockTime = shot.stunLockTime;
            collisions.hitDir = shot.moveDirX;
            if (!shot.isPiercing)
            {
                Destroy(hit.collider.gameObject);
            }
            return true;
        }

        // check if this object has hit a collision object with the Player tag
        if (hit.collider.gameObject.tag == "Player")
        {
            Debug.Log(gameObject.ToString() + " hit Player object: " + hit.collider.gameObject.ToString());
            collisions.collidedWith = hit.collider.gameObject;
            collisions.touchPlayer = true;

            collisions.hitDir = GameController.gameControl.prevPlayerDirX;
            
        }

        /*
        // check if this object has collided with an object with EnemyAttack tag
        else if (hit.collider.gameObject.tag == "EnemyAttack")
        {
            Debug.Log(gameObject.ToString() + " hit EnemyAttack object: " + hit.collider.gameObject.ToString());
            collisions.collidedWith = hit.collider.gameObject;
            collisions.touchEnemyAttack = true;
            collisions.hitDir = hit.collider.GetComponent<HitCollider>().hitBoxDir;
            return true;
        }

        
        // check if this object has collided with an object with Enemy tag
        else if (hit.collider.gameObject.tag == "Enemy")
        {
            if (hit.collider.gameObject != gameObject)
            {
                Debug.Log(gameObject.ToString() + " hit Enemy object: " + hit.collider.gameObject.ToString());
                collisions.collidedWith = hit.collider.gameObject;
                collisions.touchEnemy = true;
                collisions.hitDir = hit.collider.GetComponent<Enemy>().enemyController.collisions.facingDir;
                return true;
            }
        }
        */

        return false;
    }

    void ClimbSlope(ref Vector2 velocity, float slopeAngle, Vector2 slopeNormal)                                                                         // climb slope
    {
        float moveDistance = Mathf.Abs(velocity.x);
        float climbVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;

        if (velocity.y <= climbVelocityY)
        {
            velocity.y = climbVelocityY;
            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
            collisions.below = true;
            collisions.climbingSlope = true;
            collisions.slopeAngle = slopeAngle;
            collisions.slopeNormal = slopeNormal;
        }
    }

    void DescendSlope(ref Vector2 velocity)
    {
        RaycastHit2D maxSlopeHitLeft = Physics2D.Raycast(raycastOrigins.bottomLeft, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, solidCollisionMask);
        RaycastHit2D maxSlopeHitRight = Physics2D.Raycast(raycastOrigins.bottomRight, Vector2.down, Mathf.Abs(velocity.y) + skinWidth, solidCollisionMask);
        if (maxSlopeHitLeft ^ maxSlopeHitRight)
        {
            SlideDownMaxSlope(maxSlopeHitLeft, ref velocity);
            SlideDownMaxSlope(maxSlopeHitRight, ref velocity);
        }

        if (!collisions.slidingDownMaxSlope)
        {
            float dirX = Mathf.Sign(velocity.x);
            Vector2 rayOrigin = (dirX == -1) ? raycastOrigins.bottomRight : raycastOrigins.bottomLeft;
            RaycastHit2D hit = Physics2D.Raycast(rayOrigin, -Vector2.up, Mathf.Infinity, solidCollisionMask);

            if (hit)
            {
                float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
                if (slopeAngle != 0 && slopeAngle <= maxSlopeAngle)
                {
                    if (Mathf.Sign(hit.normal.x) == dirX)
                    {
                        if (hit.distance - skinWidth <= Mathf.Tan(slopeAngle * Mathf.Deg2Rad) * Mathf.Abs(velocity.x))
                        {
                            float moveDistance = Mathf.Abs(velocity.x);
                            float descendVelocityY = Mathf.Sin(slopeAngle * Mathf.Deg2Rad) * moveDistance;
                            velocity.x = Mathf.Cos(slopeAngle * Mathf.Deg2Rad) * moveDistance * Mathf.Sign(velocity.x);
                            velocity.y -= descendVelocityY;

                            collisions.slopeAngle = slopeAngle;
                            collisions.descendingSlope = true;
                            collisions.below = true;
                        }
                    }
                }
            }
        }

    }

    void SlideDownMaxSlope(RaycastHit2D hit, ref Vector2 moveAmount)
    {

        if (hit)
        {
            float slopeAngle = Vector2.Angle(hit.normal, Vector2.up);
            if (slopeAngle > maxSlopeAngle)
            {
                moveAmount.x = Mathf.Sign(hit.normal.x) * (Mathf.Abs(moveAmount.y) - hit.distance) / Mathf.Tan(slopeAngle * Mathf.Deg2Rad);

                collisions.slopeAngle = slopeAngle;
                collisions.slidingDownMaxSlope = true;
                collisions.slopeNormal = hit.normal;
            }
        }
    }

    void ResetDropDown()
    {
        collisions.dropDown = false;
    }

    public struct CollisionInfo
    {
        public bool above, below;
        public bool left, right;

        public bool climbingSlope;
        public bool descendingSlope;
        public bool slidingDownMaxSlope;

        public float slopeAngle, slopeAngleOld;
        public Vector2 slopeNormal;
        public Vector3 velocityOld;
        public int facingDir;
        public int hitDir;
        public bool dropDown;
        public float stunLockTime;

        public bool touchItem;
        public bool touchHazard;
        public bool touchCheckpoint;
        public bool touchNPC;
        public bool touchEnemy;
        public bool touchPlayerShot;
        public bool touchEnemyAttack;
        public bool touchPlayer;

        public GameObject collidedWith;

        public void ResetCollisionInfo()
        {
            above = below = false;
            left = right = false;
            climbingSlope = false;
            descendingSlope = false;
            slidingDownMaxSlope = false;
            slopeNormal = Vector2.zero;
            slopeAngleOld = slopeAngle;
            slopeAngle = 0;

            touchItem = false;
            touchHazard = false;
            touchCheckpoint = false;
            touchNPC = false;
            touchEnemy = false;
            touchEnemyAttack = false;
            touchPlayerShot = false;
            touchPlayer = false;

            collidedWith = null;
        }
    }

}
