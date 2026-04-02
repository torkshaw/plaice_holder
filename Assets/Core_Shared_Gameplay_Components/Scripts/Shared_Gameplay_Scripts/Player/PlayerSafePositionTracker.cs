using UnityEngine;


// TS

public class PlayerSafePositionTracker : MonoBehaviour
{

    // this wee script is just constatly storing the player's location when they are grounded.,so we can respawn them in the last place they were grounded if they die. 


    // these are the configurable elements in the inspector 
    [Header("References")]
    [SerializeField] private PlayerGroundDetector groundDetector; // this is just to call on the groundetector we ALREADY have on the player

    [Header("Safe Position Settings")]
    [SerializeField] private bool requireGroundedToUpdate = true; // this bool lets us set whether we need the player to be GROUNDED to count as a safe position.

    [Header("Debug")]
    [SerializeField] private Vector2 lastSafePosition; // this is just so we can SEE where the last position was when playing/debugging

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
            return;
        }

        lastSafePosition = transform.position; // but otherwise - we do this: putting the player's position into the lastSafePosition variable. 


    } // end update
}