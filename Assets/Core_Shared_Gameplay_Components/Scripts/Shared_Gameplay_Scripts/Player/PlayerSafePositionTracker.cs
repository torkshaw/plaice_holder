using UnityEngine;


// TS (the complicated 'check in boxes around me' stuff came from chatGTP. My original script was just 'store my last _isGrounded position' but it had 
// really weird results - respawning you RIGHT on an edge. So I got help from chatGTP to extend it out to include multiple frames and a 'safe zone' detector.

public class PlayerSafePositionTracker : MonoBehaviour
{

    // this wee script is just constatly storing the player's location when they are grounded.,so we can respawn them in the last place they were grounded if they die. 


    // these are the configurable elements in the inspector 
    [Header("References")]
    [SerializeField] private PlayerGroundDetector groundDetector; // this is just to call on the groundetector we ALREADY have on the player

    [Header("Safe Position Settings")]
    [SerializeField] private bool requireGroundedToUpdate = true; // this bool lets us set whether we need the player to be GROUNDED to count as a safe position.
    [SerializeField] private Transform groundCheckPoint; // this is the actual transform of the spot we're checking 'safe' from
    [SerializeField] private Vector2 edgeCheckOffset = new Vector2(0.5f, 0f); // check for 'ground' nearby using our ground detector box
    [SerializeField] private Vector2 edgeCheckSize = new Vector2(0.15f, 0.1f); // (this is the size of the actual box used for checking for ground, above)
    [SerializeField] private LayerMask groundLayerMask; // this specifies the layers that count as ground
    [SerializeField] private int groundedFramesRequired = 5; // how many frame syou have to have been grounded by to count as safe
    [SerializeField] private float secondsBetweenSafePositionUpdates = 0.15f; // we don't want to be saving EVERY frame so this limits it to just every 0.15seconds

    [Header("Debug")]
    [SerializeField] private Vector2 lastSafePosition; // this is just so we can SEE where the last position was when playing/debugging


    private int groundedFrameCount; // counts up how many times in a row the player has been grounded
    private float nextAllowedSafePositionUpdateTime; // this is the next time the script is allowed to update the safe position

    public Vector2 LastSafePosition => lastSafePosition; // this is where we're storing where to return the player to in the event of death/respawn
                                                         // its a property (read only for scripts other than this one) for the same reason we're using properties in movement/inputs.


    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //



    private void Awake()
    {
        if (groundDetector == null) // in awake we're grabbing the playergrounddetector from the player in case we didn't assign it (to avoid errors).
        {
            groundDetector = GetComponent<PlayerGroundDetector>();
        }

        lastSafePosition = transform.position; // we set the safe position here to the initial player spawn position, so there is one from the get-go.

    } // end awake



    private void Update()
    {
        if (groundDetector == null) // safety check - if there's no grounddtetector we don't crash, just return
        {
            return;
        }

        if (requireGroundedToUpdate && !groundDetector.IsGrounded) // if the player is NOT grounded (and we set 'grounding required' in the inspector) then we don't do anytihng
        {
            groundedFrameCount = 0;
            return;
        }

        groundedFrameCount++; // this is where the chatGPT stuff kicks in. Here we're incrementing the frame count

        if (groundedFrameCount < groundedFramesRequired) // if player has not been grounded for the number of frames required we return
        {
            return;
        }

        if (Time.time < nextAllowedSafePositionUpdateTime) // if we updtaed in the allowed time already then return
        {
            return;
        }

        if (!HasGroundOnBothSides()) // if the player is NOT grounded on both sides return
        {
            return;
        }

        lastSafePosition = transform.position; // but otherwise - we do this: putting the player's position into the lastSafePosition variable. 
        nextAllowedSafePositionUpdateTime = Time.time + secondsBetweenSafePositionUpdates; // this now sets the time we're allowed to do this again



    } // end update


    private bool HasGroundOnBothSides() // this method checks to see fhat the player is safe all around, using their bounding box. it returns a bool.
    {
        if (groundCheckPoint == null) // safety check for grondcheckpoint
        {
            return true;
        }

        // these are the two checkpoints using our offset
        Vector2 leftCheckPosition = (Vector2)groundCheckPoint.position - edgeCheckOffset; 
        Vector2 rightCheckPosition = (Vector2)groundCheckPoint.position + edgeCheckOffset;

        // these two bools indicate whether BOTH/EACH of these points are safe
        bool hasGroundLeft = Physics2D.OverlapBox(leftCheckPosition, edgeCheckSize, 0f, groundLayerMask);
        bool hasGroundRight = Physics2D.OverlapBox(rightCheckPosition, edgeCheckSize, 0f, groundLayerMask);

        return hasGroundLeft && hasGroundRight; // if they are BOTH true then this method returns TRUE
    } // end HasGroundOnBothSides



} // end class