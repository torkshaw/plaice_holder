using UnityEngine;

public class EnemyPatrol2D : MonoBehaviour
{

    // enums for holding the kind of patrol
    private enum PatrolAxis
    {
        Horizontal,
        Vertical
    }
    private enum PatrolStartDirection
    {
        Positive,
        Negative
    }

    // serialized fields

    [SerializeField] private HookableEnemy2D hookableEnemy; // for the ref to its own hookable script
    [SerializeField] private Rigidbody2D rb; // for a ref to its own rigid body
    [SerializeField] private SpriteRenderer spriteRenderer; // get a ref to the sprite so we can flip it on patrol

    [Header("Patrol")]
    [SerializeField] private float patrolSpeed = 2f;
    [SerializeField] private float patrolDistance = 3f; // this is BOTH ways from its starting postion
    [SerializeField] private PatrolAxis patrolAxis = PatrolAxis.Horizontal; // choose patrol axis
    [SerializeField] private PatrolStartDirection startDirection = PatrolStartDirection.Positive; // choose direction it starts in
    [SerializeField] private float returnToPatrolSpeed = 3f; // how fast it goes 'home' after being pulled



    //private variables

    private Vector2 startPosition;
    private int moveDirection = 1; // default is positive
    private bool isReturningToStart; // so we knw when its going home




    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //

    private void Awake()
    {
        startPosition = transform.position; // even tho this is a rigidbody enemy we're still storing its starting postiion as a transform
        moveDirection = startDirection == PatrolStartDirection.Positive ? 1 : -1; // get movedireciton. if start direction in inspector is positive use 1, if not use -1

    } // end awake

    private void FixedUpdate()
    {
        if (rb == null) // make sure there is a rigid body, other wise drop out
        {
            return;
        }

        if (hookableEnemy != null && hookableEnemy.IsHooked) // if there is a hookableEnemy component AND this thing is currently hooked then STOP MOVEMENT from this script
        {
            return;
        }


        if (isReturningToStart) // if enemy is currently in its return to start state
        {
            Vector2 currentPosition = rb.position; // capture where the rigidbody is now
            Vector2 targetPosition = startPosition; // the position we want to return to
            Vector2 directionToStart = (targetPosition - currentPosition).normalized; // get the direction back toward the patrol start point

            bool hasArrivedAtStart; // use this to decide if we've arrived back at patrol start

            if (patrolAxis == PatrolAxis.Horizontal) // if this is a horizontal patrol enemy...
            {
                hasArrivedAtStart = Mathf.Abs(currentPosition.x - targetPosition.x) <= 0.05f; // only care about getting back to the right X position
            }
            else // otherwise this is a vertical patrol enemy
            {
                hasArrivedAtStart = Mathf.Abs(currentPosition.y - targetPosition.y) <= 0.05f; // only care about getting back to the right Y position
            }

            if (hasArrivedAtStart) // if we've arrived back at patrol start on the relevant axis...
            {
                isReturningToStart = false; // stop the return state
                rb.linearVelocity = Vector2.zero; // stop movement cleanly when it arrives
                return;
            }

            if (patrolAxis == PatrolAxis.Horizontal) // if this is a horizontal patrol enemy (i.e we want it to use gravity too)
            {
                if (spriteRenderer != null) // if there IS a sprite renderer assigned...
                {
                    spriteRenderer.flipX = directionToStart.x < 0f; // flip the sprite to face the direction it is returning in (this only works for horizontal enemies)
                }

                rb.linearVelocity = new Vector2(directionToStart.x * returnToPatrolSpeed, rb.linearVelocity.y); // return on X only
                                                                                                                // keep the current Y velocity so gravity/falling still works
            }
            else // otherwise this is a vertical patrol enemy
            {
                rb.linearVelocity = directionToStart * returnToPatrolSpeed; // vertical/flying enemies can still return in full 2D
            }

            return;
        }



        // if there is NOT a hookableEnemy component OR if this thing is not currently hooked then do its patrol:

        if (patrolAxis == PatrolAxis.Horizontal) // if this enemy patrols horizontally...
        {
            float leftLimit = startPosition.x - patrolDistance; // capture a left limit (start - distance)
            float rightLimit = startPosition.x + patrolDistance; // capture a right limit (start + distance)

            if (transform.position.x <= leftLimit) // while its position is less than or equal to its leftlimit...
            {
                moveDirection = 1; // move right
            }
            else if (transform.position.x >= rightLimit) // while its position is greater than or equal to its rightlimit...
            {
                moveDirection = -1; // move left
            }

            if (spriteRenderer != null) // if there IS a sprite renderer assigned...
            {
                spriteRenderer.flipX = moveDirection < 0; // flip the sprite on the X to reflect the direction change
            }


            rb.linearVelocity = new Vector2(moveDirection * patrolSpeed, rb.linearVelocity.y); // then do the actual movement using its patrol speed
                                                                                               // NOTE! we keep the y velocity too, even tho we're only patrolling left and right
                                                                                               // this is so we don't end up hovering
        }
        else // otherwise this enemy patrols vertically
        {
            float lowerLimit = startPosition.y - patrolDistance; // capture a lower patrol limit (start - distance)
            float upperLimit = startPosition.y + patrolDistance; // capture an upper patrol limit (start + distance)

            if (transform.position.y <= lowerLimit) // while its position is less than or equal to its lower limit...
            {
                moveDirection = 1; // move up
            }
            else if (transform.position.y >= upperLimit) // while its position is greater than or equal to its upper limit...
            {
                moveDirection = -1; // move down
            }

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, moveDirection * patrolSpeed); // move vertically instead of horizontally
                                                                                               // NOTE! we keep the x velocity here for the same reason as above
                                                                                               // patrol only controls one axis
        }

    } // end fixedupdate



    public void ReturnToPatrolStart() // method for returning to start position if it gets pulled way off course
    {
        isReturningToStart = true; // set the rerurning flag

    }



} // end class
