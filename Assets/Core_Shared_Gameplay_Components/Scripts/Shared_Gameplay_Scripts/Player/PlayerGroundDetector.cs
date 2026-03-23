using Mono.Cecil.Cil;
using UnityEngine;

public class PlayerGroundDetector : MonoBehaviour
{

    //Properties - again we're using properties not straight up variables to protect them, and so we can potentially do cool stuff with them
    public bool IsGrounded {  get; private set; } // this is just our basic 'is this thing on the ground' bool
    public Collider2D GroundCollider {  get; private set; } // this stores the ID of the collider we're actually standing on. so we can identify what we're standing on and maybe do different stuff as a result. 
    public Rigidbody2D GroundRigidBody { get; private set; }  // this stores the ID of a rigid body if the thing the player is standing on has one. this is gonna be useful for making the player move WITH platforms.

    // inspector fields
    [SerializeField] private Transform groundCheckPoint; // this is where we'll assign the 'groundcheck' object. the reference to 'transform' means 'grab the location (world transform position) of this thing'
    [SerializeField] private Vector2 groundCheckSize = new Vector2(0.6f, 0.1f); // this is a ground check box so we need an X and a Y to define the box
    [SerializeField] private LayerMask groundLayerMask; // this is how we can specify what layers count as ground. LayerMask is a unity feature that will give a dropdown of layers we have in the project
    public LayerMask GroundLayerMask; // Exposes layer to other scripts publicly - Jason


    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //

    private void Awake()
    {
        GroundLayerMask = groundLayerMask; // Public LayerMask will always = the private LayerMask (I did this as I don't want to mess up other parts of the ground code) - Jason
                                           // This is duplicated, can probably be merged once it's working! - Jason
    }

    private void FixedUpdate()
    {
        PerformGroundCheck(); // we're calling the function that lives below. it is just neater to have fixedupdate call a function than putting the function all in fixedupdate.

    } // end fixedupdate

    private void PerformGroundCheck() // this is the actual funciton that does the check
    {

        if (groundCheckPoint == null) // this wee thing just makes sure we have a groundcheckpoint object assigned. so we don't crash out if its missing. 
        {
            Debug.LogError("GroundCheckPoint not assigned!"); 
            return;
        }

        // here's where we're using an overlapcircle check to see if/what we're colliding with.
        // This varibale we define here - Collider2D hit - is going to store a reference to the collider that we did "hit" if there was one
        // what's being assigned to that varibale is the result of the 'Physics2D.OverlapCircle' function. we give it some info in its brackets (the location of the ground check, the radius, and the layers that count).
        Collider2D hit = Physics2D.OverlapBox(
            groundCheckPoint.position, // call out the groundcheck point
            groundCheckSize, // define its size
            0f, // rotation angle of the ground check box (should not be any)
            groundLayerMask // refer to the layer mask 
        );

        IsGrounded = hit != null; // this line sets the isGrounded bool. if hit (from above) is NOT EQUAL to NULL then this little snippet (hit != null) returns a bool (true/false)
                                  // which is then assigned to IsGrounded - much the samw way the GetKeyCode(down) function returns a bool we can scoop right into our variable. 

        GroundCollider = hit; // here 'wre assigning the thing we identified above and assigned to "hit" to the GroundCollider property. we're storing the identity of what we collided with. 
                              // since this thing is running on every fixedupdate tick we constantly know what the player is standing on (collider, or null).

        GroundRigidBody = hit != null ? hit.attachedRigidbody : null; // this line is where we're capturing the rigidbody refernce of the thing we're standing on and assigning it to the GroundRigidBody property
                                                                      // here im using Unity's built in function  'Collider2D.attachedRigidbody' (here as hit(the collider referenced in hit).attachedRigidbody)
                                                                      // to then get ahold of a reference to the rigidbody the thing we collided with has (if it has one). 


    } // end performgroundcheck



    private void OnDrawGizmosSelected() // gizmo to see the size of the groundcheck
    {
        if (groundCheckPoint == null) return;

        Gizmos.color = Color.yellow;
        Gizmos.DrawWireCube(groundCheckPoint.position, groundCheckSize);
    }


} // end class
