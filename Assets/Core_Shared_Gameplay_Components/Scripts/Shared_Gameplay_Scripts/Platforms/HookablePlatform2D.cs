using System.Xml.Serialization;
using UnityEngine;

public class HookablePlatform2D : MonoBehaviour, IHookableTarget2D // here we need to IMPLEMENT this INTERFACE script. that little thing that goes on all hookable stuff and defines what it needs to provide.
{
    // enum for platform type
    private enum PlatformType
    {
        Static,
        Return,
        Patrol
    }

    private enum RailDirection // this controls the directions the rail platforms can go in
    {
        Left,
        Right,
        Up,
        Down
    }


    // Inspector visible variables

    [SerializeField] private Transform attachPoint; // this is where the attach point goes. we can drag that around inside the prefab to wherever the hook on the sprite is.
    [SerializeField] private float pullSpeed = 3f; // this is the speed this object can be pulled at when reeling in. 
    [SerializeField] private PlatformType platformType = PlatformType.Static; // choose the platform type - we've assigned the enum from above for this, default is static
    [SerializeField] private float returnSpeed = 2f; // how fast it returns if it does so
    [SerializeField] private Collider2D playerPullStopZone; // refernce to the players platform exclusion zone (when reeling)
    [SerializeField] private LayerMask platformExclusionMask; // mask to select the platformexclusionlayer
    [SerializeField] private LayerMask platformBlockMask; // this is a secondary mask to stop the platofrms (which are kinematic RBs) from colliding with each other or level geo

    // patrol controls
    [SerializeField] private Vector2 patrolDirection = Vector2.right;
    [SerializeField] private float patrolDistance = 3f;
    [SerializeField] private float patrolSpeed = 1.5f;

    [Header("Rail Constraint")]
    [SerializeField] private bool useRailConstraint = false;
    [SerializeField] private RailDirection railDirection = RailDirection.Right;
    [SerializeField] private float railDistance = 3f;
    [SerializeField] private LineRenderer railLineRenderer;


    // private variables


    private Vector2 homePosition; // where the platform starts on awake

    private bool isReturningHome;

    private Vector2 patrolTarget; // the thing a patrolling platform is heading at (either away, or home)


    private FishingRodController2D hookedByRod; // this stores a reference to the rod that's currently attached to it. its seralized like this for visibility/debugging. this also gives us access
                                                // to the SPECIFIC instances of the scripts that are running on this rod. which means we can interrogate its various methods (like IsLineUnderTension).
                                                // that mean we can run that function from here - interrogating the rod script to see if the line is under tension. if so, we act accordingly. 
    
    private Rigidbody2D rb; // need to scoop in the rigiddbody we're using on the platforms too

    private Collider2D platformCollider; // also nabbing the collider so we can use it to stop platforms crashing into the player when pulled



    // Public variables

    public Transform AttachPoint => attachPoint; // this is fulfilling the contract of the interface script. it REQUIRES an attachpoint with a transform. 
                                                 // this creates a property (AttachPoint) which is public - so it can be accessed by things looking for it
                                                 // we're using a property here instead of just a regular public variable becasue we might want to add logic to it later. properties allow for that.
                                                 // we then assign the value of attachPoint to that property but using the => operator instead of the = operator 
                                                 // we do this to make it a read-only public property, ratehr than a variable





    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>(); // grab the rigidbody component

        platformCollider = GetComponent<Collider2D>(); // also grab the collider component - we're using it to stop platforms crashing into the player

        homePosition = rb.position; // store the platforms home position so it can return there
        patrolTarget = homePosition + patrolDirection.normalized * patrolDistance; // here we're storing the spot the patrol is going to aim for - does the maths as the variable is created

        if (railLineRenderer != null) // initialise the linerenderer on the rails platforms
        {
            railLineRenderer.positionCount = 2; // a line between 2 points
            railLineRenderer.enabled = useRailConstraint; // we only show this line if railconstraints is ON for this object
        }

    } // end awake

    public void OnHooked(FishingRodController2D rod) // when we sucessfully attach to the platform the Rod script passes itself in, so we store it here
    {
        hookedByRod = rod;
        isReturningHome = false;

    } // end OnHooked

    public void OnUnhooked(FishingRodController2D rod) // when the rod unhooks we want to clear that ref
    {
        if (hookedByRod == rod) // first we check to make sure its the same rod (it pretty much alwys will be but this is good practice)
        {
            hookedByRod = null; // we remove the ref

            if (platformType == PlatformType.Return || platformType == PlatformType.Patrol) // we flag the platform as returning home, so we can do stuff with that
                isReturningHome = true;
        }




    } // end OnUnhooked


    private void FixedUpdate()
    {

        if (hookedByRod == null) // if there is no rod in that variable then drop out/do nothing
        {
            if (platformType == PlatformType.Static) // if you are a platform of type STATIC then return out, do nothing
                return;

            // for platforms of the OTHER kinds (return and patrol) move back to your home position while they are in their "returning home" state.
            if (isReturningHome)
            {
                Vector2 returnPosition = Vector2.MoveTowards(rb.position, homePosition, returnSpeed * Time.fixedDeltaTime); // use the movetoward function to move the platform back to its returnposition

                Vector2 returnDelta = returnPosition - rb.position; // calculate how far the platform intends to move this frame
                returnDelta = ApplyMovementBlocking(returnDelta);   // pass that movement through the blocking check so platforms stop at walls/other platforms
                                                                    // this is our little helper method from below

                rb.MovePosition(rb.position + returnDelta); // apply the safe movement delta to the platform's rigidbody

                // once we are close enough to home stop being in the returning-home state
                if (Vector2.Distance(rb.position, homePosition) <= 0.05f)
                    isReturningHome = false;

                return;
            }

            // If this is a PATROL platform AND it is NOT currently returning home, do its normal patrol movement
            if (platformType == PlatformType.Patrol)
            {
                if (Vector2.Distance(rb.position, patrolTarget) <= 0.05f)
                {
                    if (patrolTarget == homePosition)
                        patrolTarget = homePosition + patrolDirection.normalized * patrolDistance;
                    else
                        patrolTarget = homePosition;
                }

                Vector2 patrolNext = Vector2.MoveTowards(rb.position, patrolTarget, patrolSpeed * Time.fixedDeltaTime); // calculate where the platform wants to move next on its patrol path

                Vector2 patrolDelta = patrolNext - rb.position; // calculate how far the platform intends to move this frame
                patrolDelta = ApplyMovementBlocking(patrolDelta); // pass that movement through the blocking check so platforms stop at walls/other platforms - our helper from below

                rb.MovePosition(rb.position + patrolDelta); // then do the actual MOVE, incorporating this new delta
                return;
            }

            return;

        } // end if (hookedByRod == null)

        if (!hookedByRod.IsLineUnderTension()) // if the line is NOT under tension do nothing. otherwise do the stuff below!
            return;

        Debug.Log("Platform is under tension."); // wee console debug so i can see if tension is occuring

        Vector2 rodPosition = hookedByRod.RodPoint.position; // here we're grabbing the position of the rod this frame. it goes into rodPosition.
                                                             // its just a vector2 here because we don't care about z in 2d.
        Vector2 currentAttachPosition = AttachPoint.position; // getting the current position of the platform



        // now we use the MoveTowards function between these positions and using the pullSpeed * time as its max move this frame. we calculate this and put it in newPosition.
        Vector2 newPosition = Vector2.MoveTowards(currentAttachPosition, rodPosition, pullSpeed * Time.fixedDeltaTime);
        Vector2 movementDelta = newPosition - currentAttachPosition; // we need to work out the difference between the platform's rb position and the attachpoint position, so we can offset movement by that


        if (useRailConstraint) // we need to constrain movement onto the right axis if this is a rails platform. we do this before casting to see if there is a collision
            movementDelta = Vector2.Dot(movementDelta, GetRailAxis()) * GetRailAxis(); // it takes the movementdelta from above and takes out anything that isn't on the permitted axis

        // prevent reeling platforms into the player exclusion zone (so we dont push the player around)
        if (hookedByRod != null && hookedByRod.IsReeling && playerPullStopZone != null)  // if the thing is hooked and the rod is reeling and we have a playerpullstop zone
        {
            RaycastHit2D[] hits = new RaycastHit2D[4]; // cast in the direction of movement to see what it would hit next frame.
                                                       // we look for the first FOUR hits to make sure we get everyting (RaycastHit2D[4]).

            int hitCount = platformCollider.Cast(
                movementDelta.normalized,
                hits,
                movementDelta.magnitude,
                true
            );

            for (int i = 0; i < hitCount; i++) // now we have to cycle through all 4 of the possible hits (if any) and decide what to do about it
            {
                Collider2D hitCollider = hits[i].collider; // checking there is a collider
                if (hitCollider == null) continue;

                int hitLayer = hitCollider.gameObject.layer; // grab the layer number of the thing we hit

                // stop when hitting the player exclusion zone (only while reeling!!! exclusion zone doesnt stop platforms just moving into the player, only being reeled)
                if (((1 << hitLayer) & platformExclusionMask) != 0) // compare the layer number to the things on the mask so we can stop if they match
                {
                    movementDelta = Vector2.zero;
                    break; // we use a break here because as soon as we get 1 actual hit we can stop movement, we don't need to check for others.
                }
            }
        }

        movementDelta = ApplyMovementBlocking(movementDelta); // stop the platform moving into walls/ground/other platforms


        if (useRailConstraint) // this is where we stop a rails platform going BEYOND its rail max distance
        {
            Vector2 railAxis = GetRailAxis(); // get its rail axis

            Vector2 desiredRootPosition = rb.position + movementDelta; // where the platform is trying to move to this frame

            Vector2 fromHome = desiredRootPosition - homePosition; // where that desired position is relative to its home position

            float distanceAlongRail = Vector2.Dot(fromHome, railAxis); // measure how far along the rail that desired position is

            float clampedDistance = Mathf.Clamp(distanceAlongRail, 0f, railDistance); // clamp that distance so it can't go behind home or beyond railDistance

            Vector2 constrainedPosition = homePosition + railAxis * clampedDistance; // calculate the final constrained position along the rail

            rb.MovePosition(constrainedPosition); // apply that position
        }
        else
        {
            rb.MovePosition(rb.position + movementDelta);  // then we do the moving of the platform's rigidbody to this new position baking in the delta from above
        }


    } // end fixedupdate


    private void Update()
    {

        UpdateRailVisual();


    } // end update


    private Vector2 ApplyMovementBlocking(Vector2 desiredDelta) // this little helper method used to be in my movement code but its better here
                                                                // this will take the intended movement next frame, then do the raycast to see if it will hit anything on the exclusion mask
                                                                // then block movement if the answer is yes
    {
        if (desiredDelta == Vector2.zero) // if the platform isnt actuall moving (its vector2 is 0) then don't bother with the cast. just return that 0 delta.
        {
            return desiredDelta;
        }

        RaycastHit2D[] hits = new RaycastHit2D[4]; // cast in the direction of movement to see what it would hit next frame.
                                                   // we look for the first FOUR hits to make sure we get everyting (RaycastHit2D[4]).

        int hitCount = platformCollider.Cast( // then we do the checks to see if what we would hit if we moved the desired delta in this frame. 
            desiredDelta.normalized,
            hits,
            desiredDelta.magnitude,
            true
        );


        for (int i = 0; i < hitCount; i++) // now we have to cycle through all 4 of the possible hits (if any) and decide what to do about it
        {
            Collider2D hitCollider = hits[i].collider; // checking there is a collider
            if (hitCollider == null) continue; // if there is keep going

            int hitLayer = hitCollider.gameObject.layer; // grab the layer number of the thing we hit

            // check if this thing is on the exclusion list we set in the mask
            if (((1 << hitLayer) & platformBlockMask) != 0)
            {
                return Vector2.zero; // if it is then we return ZERO ie. STOP MOVING
            }
        }

        return desiredDelta; // and if there's nothing in the way we just return the intended original deisred delta. move as you like!

    } // endd ApplyMovementBlocking


    private void UpdateRailVisual() // this draws the actual line renderer for rails platofrms to show where they can be pulled
    {
        if (!useRailConstraint || railLineRenderer == null)
            return;

        Vector2 axis = GetRailAxis();

        Vector2 start = homePosition;
        Vector2 end = homePosition + axis * railDistance;

        railLineRenderer.SetPosition(0, start);
        railLineRenderer.SetPosition(1, end);
    } // end UpdateRailVisual

    private Vector2 GetRailAxis() // this monstrosity is converting directions chosen in the enum for rails into vector directions.
    {
        switch (railDirection)
        {
            case RailDirection.Left:
                return Vector2.left;
            case RailDirection.Right:
                return Vector2.right;
            case RailDirection.Up:
                return Vector2.up;
            case RailDirection.Down:
                return Vector2.down;
            default:
                return Vector2.right;
        }
    } // end getrailaxis


} //end class