using UnityEngine;

// here we're making sure certain characteristics are true of the game object this script lives on. in this case the player, who will have all 3 of these elements listed assigned to them.
[RequireComponent(typeof(Rigidbody2D))] 
[RequireComponent(typeof(PlayerInputReader))]
[RequireComponent(typeof(PlayerGroundDetector))]

public class JBTest_PlayerMovementController : MonoBehaviour
{

    // these privave fields are going to be storing some of the stuff from the player they are attached to; its rigidbody in rb, details from its inputreader and grounddetector scripts.
    private Rigidbody2D rb;
    private PlayerInputReader inputReader;
    private PlayerGroundDetector groundDetector;
    private PlayerRespawnController respawnController;

    // Serialized Fields - Inspector variables

    [Header("Movement")]     // serieliazed fields we can use to adjust movement feel in the inspector
    [SerializeField] private float maxMoveSpeed = 8f;
    [SerializeField] private float acceleration = 60f;
    [SerializeField] private float deceleration = 80f;
    [SerializeField] private float airControlMultiplier = 0.6f; // cuts player movement control while in the air - JasonB 

    [Header("Jump")]    // serieliazed fields we can use to adjust Jump feel in the inspector
    [SerializeField] private float jumpVelocity = 14f; // the speed at which a player is sent vertical. this replaced the 'force' system in the prototytpe so we can be more accurate.
    [SerializeField] private float coyoteTime = 0.15f; // how long the player can jump when they have left a surface
    [SerializeField] private float jumpBufferTime = 0.1f; // how long a 'jump' command can be remembered before landing/being fired
    [SerializeField] private float jumpCutGravityMultiplier = 2f; // how much extra gravity to apply when jump is released
    [SerializeField] private float fallGravityMultiplier = 2.5f; // how much extra gravity to apply when the player is falling
    [SerializeField] private float apexThreshold = 2f; // apexGravity multiplier only takes effect after the player reaches this velocity - JasonB
                                                      // Essentially, the greater this number - the floatier the hang time is. Mess around with it! - JasonB
    [SerializeField] private float apexGravityMultiplier = 0.4f;  // This basically reserves momentum upwards when the jump button is released near the apex of each jump - JasonB
                                                                 //This greatly aids "game feel" and makes the player feel more real within the physics space - Jason B
    [SerializeField] private float maxFallSpeed = -20f; // Clamp fall speed - useful for longer drops - JasonB

    // declare these (private) variables for use in the script
    private float coyoteTimer;
    private float jumpBufferTimer;
    private Collider2D lastGroundCollider;
    private Vector2 lastGroundPosition;



    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //


    private void Awake() // we use awake to then FIND those details and put them in their respective variables defined above
    {
        rb = GetComponent<Rigidbody2D>();
        inputReader = GetComponent<PlayerInputReader>();
        groundDetector = GetComponent<PlayerGroundDetector>();
        respawnController = GetComponent<PlayerRespawnController>();
    }



    private void FixedUpdate() // doing movement in fixedupdate because we're using physics
    {

        if (respawnController != null && respawnController.MovementLockedAfterRespawn) // if we have a controller AND MovementLockedAfterRespawn is TRUE then do: 
        {
            if (inputReader.MoveInput != 0f || inputReader.JumpPressed) // if movement or jump are pressed...
            {
                respawnController.UnlockMovementAfterRespawn(); // return to the respawncontroller and run UnlockMovementAfterRespawn
            }
            else // otherwise do nothing
            {
                return;
            }
        }
        // then we run all the normal movement stuff 
        UpdateJumpTimers(); // each physics frame we want to update the coyote and grace timers
        HandleHorizontalMovement(); // calling the movement funciton instead of putting it in here
        HandleJump(); // calling the jump function from below
        ApplyGravityModifiers(); // this little funciton will do jump-cut and extra gravity for falling
        ApplyApexModifier(); // runs function for Gravity changes at apex of jump - JasonB
    } // end fixedupdate




    private void HandleHorizontalMovement() // this is the actual movement function
    {
        float moveInput = inputReader.MoveInput; // here we're creating the moveInput local varibale so that it can hold the incoming movement deets from the inputreader script.
                                                 // specifically its MoveInput property. we're basically reading that out here from that script. this just tells us direction (-1, 0, or 1)
                                                 // this could jsut be "inputReader.MoveInput" instead of making a new local varibale. but its safer to have it as a new variable.
                                                 // having it as a new varibale also allows us to do stuff with it here. 

        // then we want to check if the player is on a platform so we can add any of its movement to the player movement
        float platformVelocityX = 0f; // create this variable

        if (groundDetector.IsGrounded && groundDetector.GroundCollider != null) // if the player is on the ground and the thing has a collider
        {
            Collider2D currentGroundCollider = groundDetector.GroundCollider; // get the collider refernece for the thing we're standing on
            Vector2 currentGroundPosition = currentGroundCollider.transform.position; // find out the current position of that collider

            if (currentGroundCollider == lastGroundCollider) // if in this frame the collider is the same one i was on last frame...
            {
                platformVelocityX = (currentGroundPosition.x - lastGroundPosition.x) / Time.fixedDeltaTime; // calculate how far its moved in X
            }

            lastGroundCollider = currentGroundCollider; // store in variable to remember this for next frame
            lastGroundPosition = currentGroundPosition; // store in variable to remember this for next frame
        }
        else
        {
            lastGroundCollider = null; // if we're NOT on the same colider this frame as last frame then we clear lastGroundCollider
        }

        float targetSpeed = moveInput * maxMoveSpeed; // in this line we're calcualtng the target speed - the speed we want to go - by multiplying the moveinput but the maxmovespeed and storing it in targetSpeed
        float currentSpeed = rb.linearVelocity.x - platformVelocityX; // we also need to capturethe players current speed so we can then move them towards their max speed (the new variable)
                                                                      // this line pulls the linearvelocity.x (horizontal movememnt speed) from the attached rb (rigidbody) and slaps it into currentSpeed

        float controlMultiplier = groundDetector.IsGrounded ? 1f : airControlMultiplier; // Added airControlMultiplier - integrated within code below for accel and decel on ternary operator - JasonB
                                                                                        // This will decrease the amount of control the player has in the air - preventing unintended jumping behaviour - JasonB

        float accelRate = (moveInput != 0 ? acceleration : deceleration) * controlMultiplier; // here we're creating the accelRate variable but it is going to be both accel an decel, in practice-
                                                                        // we're using that ternary operator to choose between two possible variables; acceleration and deceleration
                                                                        // ternery operators look like this : condition (in this case moveInput != 0) ? valueIfTrue (in this case acceleration) : valueIfFalse (deceleration)
                                                                        // so the variable accelRate becomes EITHER the acelleration varibale or the decelration variable, depending on whether the moveinput is NOT zero (0).
                                                                        // if it is NOT zero it returns the acceleration variable - that's the accelRate it should use to change the player's speed
                                                                        // if it IS 0 and the player is providing NO input then it returns the deceleration variable - that's the accelRate it should use to change the player's speed 
                                                                        // once calculated this is us saying 'ok now we know how much to increase/decrease between currentSpeed and targetSpeed
                                                                        // next we'll do the calculation of all this into one newSpeed variable

        float newSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, accelRate * Time.fixedDeltaTime);
        // Mathf.MoveTowards is a way to move between two values over time in the format (current valie, targetvalue, maxchange)
        // so we're mnoving from current speed to targetSpeed at the accelRate maxchange.
        // we have to multiply by Time.fixedDeltaTime because this is running per physics frame, and thats waaaay too fast.
        // the resulting "newSpeed" variable is the amount we want to mvoe the character for this frame. 



        // next we want to actually adjust the player object's linearvelocity by this newSpeed calculation. 
        Vector2 velocity = rb.linearVelocity; // this capture the current velocity from the player object. note: linearVelocity is a Vector2 (so it has X and Y) but for horizontal movement I only want to target x here.
        velocity.x = newSpeed + platformVelocityX; // so we're only switching velocity.x for the newSpeed variable calculated above AND adding platform speed if there is some
        rb.linearVelocity = velocity; // then here the varibale 'velocity' with the x part changed is written back to the rb.linearVelocity - this is what tells the physics engine to move that rigidbody (the player) by that much, this frame.

    } // end HandleHorizontalMovement


    private void UpdateJumpTimers() // this is the function that updates the timers, called from Fixedupdate. this enables coyote time and the jump buffer.
                                    // NOTE!! This function also wraps in an 'isGrounded' check and uses that to reset the coyote time timer. that means we donb't need to check 'isgrounded' separately for each jump.
                                    // we also check for jump input here in this funciton - to start the timers
    {
        // handling Coyote Time
        if (groundDetector.IsGrounded) // if the player is grounded ->
        {
            coyoteTimer = coyoteTime; // - refresh the coyoteTimer to the coyoteTime value
                                      // NOTE - since we're checking in the line above to see if the player isGrounded this wraps being grounded into the coyotetimer. 
        }
        else // if the player is NOT grounded ->
        {
            coyoteTimer -= Time.fixedDeltaTime; // - reduce the coyoteTimer by deltatime. ie. start counting it down. player can still jump while it counts down, even tho they are not grounded.
                                                // if the timer reaches 0 then the coyoteTimer window is over, and player can't jump
        }


        // handling the jumpbuffer - making sure that jump presses still fire a jump even if they're pressed a tiny bit earlier than a player can actually jump
        if (inputReader.JumpPressed) // if the player pressed jump this frame ->
        {
            jumpBufferTimer = jumpBufferTime; // - set the jumpbuffertimer to jumpbuffertime
            inputReader.ConsumeJumpPress(); // this is where we set the jumppressed back to false, since we've registered it here and we're dealing with it

        }
        else // if the player didn't press jump this frame ->
        {
            jumpBufferTimer -= Time.fixedDeltaTime; // - we reduce the jumpbuffertimer this frame
        }

    } // end updatejumptimers

    private void HandleJump() // this is what actually does the jump called from Fixedupdate
    {

        if (jumpBufferTimer > 0f && coyoteTimer > 0f) // check to see if both buffer timer and coyotetimer (and implicity 'isGrounded') have time left
        {

            Vector2 velocity = rb.linearVelocity; // here we're getring the rb velocity of the player again
            velocity.y = jumpVelocity; // here we're swapping its y velocity for the jumpVelocity
            rb.linearVelocity = velocity; // here we're writign that back to lineaer velocity for this frame, initiating the vertical speed change

            jumpBufferTimer = 0f; // then we reset the buffer timer so jump can only happen once per input
            coyoteTimer = 0f; // and we reset coyote timer so jump can only happen once per input

        }

    } // end HandleJump

    private void ApplyGravityModifiers() // here's where the gravity stuff happens
    {

        if (rb.linearVelocity.y < 0f) // this checks to see if the player is moving DOWN - ie. falling (linearVelocity y is in the negatives)
        {

            Vector2 velocity = rb.linearVelocity; // grabbing the current player velocity

            float extraGravity = Physics2D.gravity.y * (fallGravityMultiplier - 1f) * Time.fixedDeltaTime; // does a wee calculation of current gravity * our modifier (-1 since gravity is already 1) * time (to convert acelleration into velocity change per physics step/update). 
                                                                                                           // this is stored as the variable 'extraGravity'
            velocity.y += extraGravity; // then we set the y velocity to itself PLUS that extraGravity. the "+=" here is a shorthand for "x = (becomes) x + y". where x is the stuff before the operator and y is the stuff after

            velocity.y = Mathf.Max(velocity.y, maxFallSpeed); // Clamps fall speed to prevent player falling too fast - JasonB

            rb.linearVelocity = velocity; // then we write that back to the rb.linearVelocity. much as we did with the movement above.

        }

        else if (rb.linearVelocity.y > 0f && !inputReader.JumpHeld) // so if the player is moving UP (y > 0) AND the jump button is NOT being held (&& !)
                                                                    // ie. the player is still rising from a jump but the button is no longer held
        { // then we do the same calculations as above, but with our jumpcut multiplier

            Vector2 velocity = rb.linearVelocity;

            float extraGravity = Physics2D.gravity.y * (jumpCutGravityMultiplier -1f) * Time.fixedDeltaTime;

            velocity.y += extraGravity;

            rb.linearVelocity = velocity;

        }


    } // end ApplyGravityModifiers

    private void ApplyApexModifier() // Adds an upward nudge at the apex of jumping, helps gravity feel "real".
    {
        Vector2 velocity = rb.linearVelocity; // Get players current velocity and assigns to local variable - JasonB

        float apexCounterGravity = Physics2D.gravity.y * (1f - apexGravityMultiplier) * Time.fixedDeltaTime; // Cancels out the gravity multiplier, reducing the pull of gravity at the apex of the jump  -JasonB
        velocity.y -= apexCounterGravity; // Applies to the players y velocity variable - JasonB
        rb.linearVelocity = velocity; // Applies to the rigidbody velocity to actually apply the movement - JasonB
    }

} // end class


