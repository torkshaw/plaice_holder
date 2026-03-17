using UnityEngine;

public class PlayerRespawnController : MonoBehaviour
{

    // define the various scripts and components this script needs access to. these will appear in the inspector even tho they are autoassigned to make debugging easier.

    [Header("References")]
    [SerializeField] private PlayerLifeController lifeController; // the lifecontroller script to drive when respawns happen. it gets assigned to lifeController
    [SerializeField] private PlayerSafePositionTracker safePositionTracker; // where to respawn. it gets assigned to safePositionTracker
    [SerializeField] private Rigidbody2D rb; // the player's rigidbody, to move the player. it gets assigned to rb.
    [SerializeField] private FishingRodController2D rodController; // put the rod in too so we can detach on respawn

    // private variables

    private bool movementLockedAfterRespawn; // this is the flag we throw on when the player has respawned and their movement is temporaily locked



    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //



    private void Awake() // using awake to safely find the references above if they were not assigned.
    {
        if (lifeController == null)
        {
            lifeController = GetComponent<PlayerLifeController>();
        }

        if (safePositionTracker == null)
        {
            safePositionTracker = GetComponent<PlayerSafePositionTracker>();
        }

        if (rb == null)
        {
            rb = GetComponent<Rigidbody2D>();
        }
        if (rodController == null) // autofind rod controller in case it hasnt been assigned in the serialised field
        {
            rodController = GetComponent<FishingRodController2D>();
        }

    } // end awake


    public bool MovementLockedAfterRespawn => movementLockedAfterRespawn; // make the movementlocked variable available so the playermovement script knows when it can move



    private void OnEnable() // this runs when this component becomes active. its going to be listening for the action event from the lifecontroller script.
    {
        Debug.Log($"PlayerRespawnController OnEnable. lifeController null? {lifeController == null}");
        
        if (lifeController != null)
        {
            lifeController.RespawnRequested += HandleRespawnRequested; // this means 'if RespawnRequested is raised call the HandleRespawenRequested method (below). 
                                                                       // += ADDS the function that comes after it to the "delegate invocation list" - the list of functions that are meant to run when RespawnRequested is raised
                                                                       // this is a feature of Actions (action events). that "invoke" that happens in the lifecontroller means "run the functionso n the delegate invocation list"
        }

    } // end onEnable 

    private void OnDisable() // this runs when the component is disabled or becomes inactive. 
    {
        if (lifeController != null)
        {
            lifeController.RespawnRequested -= HandleRespawnRequested; // this unsubscribes from the event
        }
    } //end onDisable


    private void HandleRespawnRequested() // the stuff we actually do when respawn is requested
    {
        Debug.Log("PlayerRespawnController received RespawnRequested"); // quick debug to make sure this is firing

        Vector2 respawnPosition = safePositionTracker.LastSafePosition; // get the last SAFE position from the safepositiontracker and store it in the temp variable respawnPosition
        transform.position = respawnPosition; // apply that as the new transform position of the player object
        rodController.Detach(); // call detach from the rod script
        rb.linearVelocity = Vector2.zero; // but then also quickly reset the velocity on both axis to ZERO so we dont carry over movement happening just before respawn






        movementLockedAfterRespawn = true; // we set the movement lock bool here, then do the actual movement lock
        rb.constraints = RigidbodyConstraints2D.FreezePositionX | RigidbodyConstraints2D.FreezeRotation; // this is the rigidbody movement lock - we're only locking VERTICAL and rotation
        Debug.Log("Player movement locked after respawn"); // wee debug to show its going on in the console 
        lifeController.FinishRespawn(); // here we call that tiny little method from lifecontroller to reset the player 'isDead' bool to false


    } // end HandleRespawnRequest


    public void UnlockMovementAfterRespawn()
    {
        if (!movementLockedAfterRespawn) // check whether its currently true. if not, drop out. 
        {
            return;
        }

        movementLockedAfterRespawn = false; // set it to false
        rb.constraints = RigidbodyConstraints2D.FreezeRotation; // unfreeze rigidbody movement EXCEPT for rotation (Z - so the sprite doesnt tumble, normall we do that in inspector)
                                                                // this REPLACES any current constraints. so we don't have to "unfreeze" Y and X. we just replace what is constrained
                                                                // with Z only (rotation).
        Debug.Log("Player movement UNlocked after respawn"); // wee debug to show its going on in the console 


    } // end UnlockMovementAfterRespawn



} // end class