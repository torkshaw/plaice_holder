using UnityEngine;

// TS

public class FishingRodController2D : MonoBehaviour
{
    // fields for the inspector

    [Header("References")]
    [SerializeField] private Transform rodPoint; // ref for the point on the player's rod the line starts
    [SerializeField] private LineRenderer lineRenderer;
    [SerializeField] private Camera mainCamera;
    [SerializeField] private PlayerInputReader inputReader; // input script ref
    [SerializeField] private PlayerGroundDetector groundDetector; // need this for 'isgrounded'

    [Header("Cast Settings")] // settings for actually clicking stuff
    [SerializeField] private float castSnapRadius = 0.5f; // radius on the mouseclick for grabbing things nearby so you don't have to be pixel perfect
    [SerializeField] private LayerMask hookableMask; // what layers stuff that can be clicked on has to be on

    [Header("Line Settings")]
    [SerializeField] private float maxLineLength = 8f;
    [SerializeField] private float reelSpeed = 4f;
    [SerializeField] private float groundedPlatformReelBuffer = 0.05f; // this is the distance we need to keep the platforms from moving when you're on them

    [Header("Line Visuals")]
    [SerializeField] private Color minRangeColor = Color.white;
    [SerializeField] private Color maxRangeColor = Color.red;

    // non-inspector variables


    // Current hook state
    private Transform currentAttachPoint; // attach point of the thing we're hooked to
        private Component currentTargetComponent; // This stores the reference to the thing we are CURRENTLY connected to. 

        // Current allowed line length - this is the maximum length the line is currently allowed to be. this can never exceed maxLineLength
        private float currentLineLength;

        // Input state bool - we can use this for lots of stuff. a useful bool
        private bool isReeling;


    // exposed public variables

        public bool IsHooked => currentAttachPoint != null; // this little bool tells us if we have another object's attachpoint or not - ie. whether we are connected t something.
        public bool IsReeling => isReeling; // this bool will be set to true if the player is holding the "reel" button (right click)
        public float MaxLineLength => maxLineLength; // making this public
        public float CurrentLineLength => currentLineLength; // making this public 
        public Transform RodPoint => rodPoint; // expose a ref to the actual rodpoint so the one we drag in can be seen by all scripts, not just this one


    // EVENTS TO BROADCAST -----------------------------
    public static event System.Action AttachToObject;
    public static event System.Action ReelingStarted;
    public static event System.Action ReelingStopped; 
    public static event System.Action DetatchObject;


 


    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //


    private void Awake()
    {

        if (mainCamera == null) // quick safety check to assign main camera. 
        {
            mainCamera = Camera.main; // Camera.main means "camera tagged MainCamera in this scene"
        }

        if (lineRenderer != null) // if there is a line renderer (there should be)
        {
            lineRenderer.positionCount = 2; // set number of positions to draw the line between to 2
            lineRenderer.enabled = false; // keep it off until we need it
        }

        if (inputReader == null) // fallback to make sure we get the inputreader
        {
            inputReader = GetComponent<PlayerInputReader>();
        }

        if (groundDetector == null) // get the grounddetector script in case it is not assiend manually
        {
            groundDetector = GetComponent<PlayerGroundDetector>();
        }
    } // end awake

    private void Update()
    {
        if (FishingLevelManager.Instance != null && FishingLevelManager.Instance.IsMinigameActive) // if the level manager says minigame is active PAUSE all other rod behaviours
        {
SetReeling(false);
return;
        }

        HandleInput();
        HandleReeling();
        CheckForLineBreak();

    } // end update


    private void LateUpdate()
    {

        UpdateLineVisual();

    } // end lateupdate






    // Custom Methods Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //



    private void HandleInput()
    {

        if (inputReader != null)
        {
            SetReeling(inputReader.ReelHeld && groundDetector != null && groundDetector.IsGrounded);  // asking each frame if the inputreader has a true value for ReelHeld (RMB is pressed) AND if the player is on the ground. if SO set isReeling to true. 
            // we dont want the player reeling when they are in the air. it makes for weird platform moving behaviours. 
        }
        else
        {
            SetReeling(false); // if not, to false
        }

        if (inputReader != null && inputReader.CastPressed) // asking each frame if CastPressed is TRUE
        {
            TryCastAtMouse();
            inputReader.ConsumeCastPress(); // if so consume the CastPress.
        }


    } // end handleinput



    private void UpdateLineVisual()
    {
        if (lineRenderer == null) // sfety check
            return;

        if (!IsHooked || rodPoint == null || currentAttachPoint == null) // this hides the line if there are no current valid attachments to it
        {
            lineRenderer.enabled = false;
            return;
        }


        // activating the line renderer and hooking it to the rihgt points
        lineRenderer.enabled = true;
        lineRenderer.SetPosition(0, rodPoint.position);
        lineRenderer.SetPosition(1, currentAttachPoint.position);

        // next we need to measure and store the distance of this connection
        float distance = GetCurrentTargetDistance(); ; // creates the variable in which to store it AND immediately assigns the distance using the GetCurrentTargetDistance function below
        float t = Mathf.InverseLerp(0f, maxLineLength, distance); // here we do some maths to convert the distance into a value between 0 and 1 so we can use that for line colouration 


        // and then we change the colour of the line to match that 0-1, going from the colorus we defined above in the serialized fields
        Color lineColor = Color.Lerp(minRangeColor, maxRangeColor, t); // this blends between 2 colours, using the "t" from above (the 0-1 version of the distance)
        lineRenderer.startColor = lineColor; // this is split into start and end of line in case because these are things we can configure separately.
        lineRenderer.endColor = lineColor;

    } // update endlinevisual



    private void CheckForLineBreak()
    {
        if (!IsHooked || rodPoint == null || currentAttachPoint == null) // dont run the check unless something is actually attached
            return;

        float distance = GetCurrentTargetDistance(); ; // this gives us the distance between the rodpoint and attachpoint using our GetCurrentTargetDistance function from below
        if (distance > maxLineLength) // if that distance is greateer than max length do the detatch method
        {
            Detach();
        }

    } // end checkforlinebreak




    public void Detach() // break the connections to attachopints, resetting current line length, hiding the line, and clearing is reeling to be safe
    {
        IHookableTarget2D hookableTarget = currentTargetComponent as IHookableTarget2D; // this line takes what we're currently considering the currentTargetComponent and stores it in the local variable hookableTarget
        if (hookableTarget != null) // provided we actually got something there we now run the target's OnUnhook function
        {
            hookableTarget.OnUnhooked(this); // so here we're asking the object that is currently stored in hookableTarget (the thing we're attached to) to run its own OnUnhooked method, passing it this instance of the rod
        }                                    // that allows the target object to do its own uncoupling from the rod - most likely just nulling the reference to it.   
        currentAttachPoint = null;
        currentTargetComponent = null;
        SetReeling(false);
        currentLineLength = 0f;

        DetatchObject?.Invoke(); // call the event for stuff that's listening so it knows we've detached

        if (lineRenderer != null)
        {
            lineRenderer.enabled = false;
        }

    } //end detatch


    private Vector3 GetMouseWorldPosition() // this method returns a Vector3 (3d position)
    {
        if (mainCamera == null) // fallback in case no camera is assigned
            return Vector3.zero;

        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(Input.mousePosition); // "Input.mousePosition" is a unity thing. it returns a Vector3 of where the mouse is right now on the SCREEN.
                                                                                         // "mainCamera.ScreenToWorldPoint" is another unity function that converts screen space position to WORLD position (related to the specified camera)
                                                                                         // we scoop that into "mouseWorldPosition" - a local varibale for this function (a Vector3 as well)
        mouseWorldPosition.z = 0f; // we can set the Z to 0 because this is a 2d game and we only care about X and Y

        return mouseWorldPosition; // this is what then gives this calculated position back to the code that called this method. so we can pull this in other code with "Vector3 mouseWorld = GetMouseWorldPosition();"

    } // end GetmouseWorldPosition


    private void TryCastAtMouse() // this fires the cast at the mouse position from above
    {

        if (groundDetector == null || !groundDetector.IsGrounded) // this if looks for 2 conditions: either no groundDetector OR IsGrounded is FALSE (! (not) groundDetector.IsGrounded)
        {                                                         // if either of these are met then we drop out of TryCastAtMouse (return). so players can't cast if they are not grounded.
            return; 
        }

        Vector3 mouseWorldPosition = GetMouseWorldPosition(); // nabbing the mouse position from the medthod above this one. the Vector3 is what that method 'returns' in its last line.

                // DEBUG: these two lines will show a gizmo that indicates where the mouse click happened on scree
                //Debug.DrawLine(mouseWorldPosition + Vector3.left * 0.25f, mouseWorldPosition + Vector3.right * 0.25f, Color.green, 1f);
                //Debug.DrawLine(mouseWorldPosition + Vector3.up * 0.25f, mouseWorldPosition + Vector3.down * 0.25f, Color.green, 1f);

        Collider2D hit = Physics2D.OverlapCircle( // this is asking if there are any colliders inside the OverlapCircle defined here. If so, the reference will be stored in the local variable "hit"
            mouseWorldPosition, // do it at the mouse
            castSnapRadius, // use the snap radius
            hookableMask // only pay attention to stuff in the right layers
            );


        if (hit == null) // if nothing gets picked up then forget it
        {
            return;
        }

        IHookableTarget2D hookableTarget = hit.GetComponentInParent<IHookableTarget2D>(); // GetComponent is the thing that does "look on this objeect for <specific component>". 
                                                                                          // we're using it here to establish whether the object with the collider we hit implements the IHookableTarget2D interface
                                                                                          // we're checking its PARENT since its likely thats going to hold that script.
                                                                                          // the reason IHookableTarget2D is on the LEFT side too is because the hookableTarget local varibale we're making here is 
                                                                                          // of the TYPE IHookableTarget2D. Which means that this variable holds a reference to a THING that can do the stuff defined in the Interface.
                                                                                          // in this case; an object that has a property called "AttachPoint" of the type "transform". as defined in the interface.

        if (hookableTarget == null) // drop out if this thing isn't hookable. ie. if the line above didn't put anyting into hookableTarget because the object didn't meet the interface requirements/implement IHookableTarget2D
        {
            return;
        }

        if (currentTargetComponent == hookableTarget as Component) // here we compare the thing we're attachd to directly to the thing we just clicked. if they are the same reference (object) then we DETATCH.
                                                                   // note: we have to do "as component" here because currentTargetComponent is a commponent and hookableTarget is of type IHookableTarget2D
                                                                   // Unity can handle this, but doesn't like to. so we can force it to recognise hookableTarget as a component to prevent warnings.
        {
            Detach();
            return;
        }

        // we also need to check if the thing is within the range of the rod's max range and not connect if it is not. 
        float distanceToTarget = Vector3.Distance(rodPoint.position,hookableTarget.AttachPoint.position); // so we work out the distance between the rodPoint (player) and hookableTarget attach point.
        if (distanceToTarget > maxLineLength) // then compare it to max line. if its more we drop out of this before we get to the attach.
        {
            return;
        }

        AttachToTarget(hookableTarget); // now we call the Attach method and pass it the refence to the specific target it should attach to


    } // end TryCastAtMouse



    public void AttachToTarget(IHookableTarget2D target) // this is the medhot that actually makes the cnnection happen
                                                         // it's brackets at the end are described this way because it ONE input parameter, called "target", whose type is IHookableTarget2D.
                                                         // so this means unlike many methods which have () at the end and take NO inputs, this method takes one defined input
                                                         // this one input is the "thing it should attach to" - the target of our "trycastatmouse" method above. 
                                                         // that method is calling AttachToTarget(hookableTarget) - initiating this method and passing ITS variable "hookableTarget" on as the "target" for this one
                                                         // its written as "IHookableTarget2D target" because as usual we have to decalare what kind of input that thing is. its type is "IHookableTarget2D".
    {
        if (target == null) // safety dropout
        {
            return;
        }

        Transform attachPoint = target.AttachPoint; // creating a local variable of type transform (attachPoint) and filling it with the attachpoint reference from the "target" object we just got from above.

        if (attachPoint == null) // quick safety check to ensure we actually get a transform out of the attachpoint. we already safety checked that there was a 'target' above, but not a transform specifically.
        {
            return;
        }

        if (IsHooked) // this checks to see if we're connected to something ALREADY and if so, now we know the next/new target is valid, we detatch from the old target.
        {
            Detach();
        }


        currentTargetComponent = target as Component; // here we want to store the reference to the object that we've got stored in 'target' as a COMPONENT type (rather than a IHookableTarget2D type)
                                                      // so we write it to the currentTargetComponent variable we declared at the top of the script.
                                                      // we make this conversion becase will want to compare this object to other things that are more likely to be components in the future.
                                                      // we can do differnt things with a component type than we can with a IHookableTarget2D type.
        currentAttachPoint = attachPoint; // assign the attachPoint we just got to currentAttachPoint - thats the hero varibale we declared at the start of the script and refers to 'thing we're attached to'
                                          // this way we ensure its using the validated version that's been through all the safety checks above


        if (rodPoint != null) // make sure there is a rodpoint (on the player)
        {
            currentLineLength = GetCurrentTargetDistance(); // use our GetCurrentTargetDistance funciton to assign currentLineLength. 
                                                            // currentLineLength is a variable set at the top of this script and is used by a few methods in this script.
        }
        else // if for some reason the player does NOT have a rodPoint then make the lineLength zero
        {
            currentLineLength = 0f;
        }

        target.OnHooked(this); // this helps things we hook to know what's going on. in this case "target" refers to "the thing we just hooked onto" and "this" refers to THIS FISHING ROD that did the attaching
                               // this is essentially going to the "target" and running its OnHooked function, passing THIS thing (this rod) as the rod that function is looking for.  
                               // this format is common in c# - object.Function(argument). Which means Ask object to run Function giving it argument.
                               // so this is "ask the platform (or whater we hooked) to run OnHooked, giving it the details of this rod as the argument".

        AttachToObject?.Invoke(); // announce that we connected to something

    } // end AttachToTarget



    private void HandleReeling() // this function activates to reduce the currentLineLength while the RMB is held. 
                                 // this doesn't do anything to stuff in the world - objects will respond to this with their OWN scripts
                                 // NOTE:: isReeling is set by the input controller. this script runs every frame but DROPS OUT if they are NOT reeling, and keeps going (pulling in line) if they ARE.
    {

        if (!IsHooked) // if we're not hooked onto something then forget it
        {
            return;
        }

        float actualDistance = GetCurrentTargetDistance(); // nab the current distance from the function below and assign to "actualDistance"


        // if the player is NOT currently pressing RMB.
        if (!isReeling)
        {

            if (actualDistance > currentLineLength) // we only want to do this if the actualDistance is GREATER than the currentLineLength. otherwise the spooled out line means nothing.
            {
                currentLineLength = Mathf.Min(actualDistance, maxLineLength); // change the currentlinelength to be actualDistance OR maxLineLength. we're using a Matthf.Min for this (return the smaller one of these 2 things)
            }

            return;

        }


        // if the player IS pressing RMB/reeling (i.e they are not-not-reeling)
        currentLineLength -= reelSpeed * Time.deltaTime; // setting currentLineLength to be: currentLineLength - (reelSpeed * time).
                                                         // the -= operator here does the subtraction WHILE changing the variable 

        // we need a minimumn allowed lenghtt, but this also should incorporate the check to prevent the player 'riding the platforms'
        float minimumAllowedLineLength = 0.1f; // generic minimum
        if (IsStandingOnHookedPlatform()) // but IF they are standing on the platform they are hooked to...
        {
            minimumAllowedLineLength = actualDistance + groundedPlatformReelBuffer; // the minimum length is the distance between them plus the small buffer
        }
        currentLineLength = Mathf.Max(currentLineLength, minimumAllowedLineLength); // then we set to the max of the allowed two variables


    } // end HandleReeling



    private float GetCurrentTargetDistance() // this little helper just works out teh current distance from the player's rodPoint to the thing its attached to (if attached)
    {
        if (rodPoint == null || currentAttachPoint == null)
        {
            return 0f;
        }

        return Vector3.Distance(rodPoint.position, currentAttachPoint.position); // measueing and returning the distance between the specified points. so this function will spit this out when called. 
    } // end getcurrenttargetdistance



    public bool IsLineUnderTension() // this is the function that alerts other systems that the line is stretched beyond its ALLOWED length (currently reeled/spooled length).
    {
        if (!IsHooked) // only bother with this when something is attached
        {
            return false;
        }

        float actualDistance = GetCurrentTargetDistance(); // gets the distance from our GetCurrentTargetDistance method

        return actualDistance > currentLineLength; // compares distance to the currentLineLength (the permitted length of the line when factoring in reeling).
                                                   // If its GREATER this function returns a TRUE bool.
                                                   // so other scripts that call this function can get the currentLineLength from this sctript and know they have to try and reposition towards it.
    } // endislineundertension


    private bool IsStandingOnHookedPlatform() // this lets us identify if the thing we're standing on is the SAME as the thing we're hooked to
                                              // we need this to stop players being able to like, ride the platforms
    {
        if (!IsHooked) // if we're hooked to nothing return false
            return false; 

        if (groundDetector == null || !groundDetector.IsGrounded) // if we're not grounded (ie. standing on a platform, return false
            return false;

        if (groundDetector.GroundRigidBody == null) // if the thing we're standing on doesn't have a rigid body return false
            return false;

        Rigidbody2D hookedRb = currentTargetComponent != null ? currentTargetComponent.GetComponent<Rigidbody2D>() : null; // if the currently hooked target has not rigid body return false

        if (hookedRb == null) // if the hookedrb is null return false
            return false;

        return groundDetector.GroundRigidBody == hookedRb; // if the grounddetector RB is the same as rigid body return TRUE
    } // end IsStandingOnHookedPlatform

    private void SetReeling(bool shouldBeReeling) // this littl helper came straight from chatGTP. i needed a way to BROADCAST the start and stop reeling stats for SFX.
    {
        if (isReeling == shouldBeReeling)
        {
            return;
        }

        isReeling = shouldBeReeling;

        if (isReeling)
        {
            ReelingStarted?.Invoke();
        }
        else
        {
            ReelingStopped?.Invoke();
        }
    } // end SetReeling







    //private void OnGUI() // so we can see what all the variables are, live. remove this later.

    //{
    //    GUILayout.Label($"Hooked: {IsHooked}");
    //    GUILayout.Label($"Reeling: {IsReeling}");
    //    GUILayout.Label($"Current Line Length: {currentLineLength:F2}");
    //    GUILayout.Label($"Actual Distance: {GetCurrentTargetDistance():F2}");
    //    GUILayout.Label($"Under Tension: {IsLineUnderTension()}");
    //}


} // end class
