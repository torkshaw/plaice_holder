using UnityEngine;


// TS

public class HookableEnemy2D : MonoBehaviour, IHookableTarget2D // must remeber to implement the INTERFACE script (IHookableTarget2D)
{

    // this script does a LOT of the same stuff as the hookableplatform2d script. but instead of doing movement with position we do it with the rigidbody.


    private enum PullMode // enum to choose between flying and grounded type of target for when pulling
    {
        Full2D,
        HorizontalOnly
    }


    // serlialzied fields

    [SerializeField] private Transform attachPoint;
    [SerializeField] private float pullSpeed = 3f;
    [SerializeField] private PullMode pullMode = PullMode.Full2D; // choose between Full2D (ignores gravity when pulled) and HorixontalOnly (gets pulled along the ground).
    [SerializeField] private EnemyPatrol2D enemyPatrol; // refernce for its patrol IF it is using one
    [SerializeField] private float verticalLiftUnlockDistance = 0.5f; // when the rod is this close on X allow vertical lift for normally horizontal-only enemies
                                                                      // this sort of kind of lets us fake gravity a little bit for 'heavy' enemies.


    // private variables
    private FishingRodController2D hookedByRod; // hold the rod that hooked us
    private Rigidbody2D rb; // hold the rigid body for this


    // public components
    public Transform AttachPoint => attachPoint; // make my attachpoint public component
    public bool IsHooked => hookedByRod != null; // does this thing currently have a 'hookedbyrod' ref, and if so - TRUE




    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //



    private void Awake() 
    {
        rb = GetComponent<Rigidbody2D>(); // get the rigidbody for this object
    }

    public void OnHooked(FishingRodController2D rod) // what we do when a rod has hooked us
    {
        hookedByRod = rod; // assign the rod that hooked to us to "rod"

    } // end onhooked

    public void OnUnhooked(FishingRodController2D rod) // what we do when a rod unhooks us
    {
        if (hookedByRod == rod)
        {
            hookedByRod = null; // zero out our rod field

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }

            if (enemyPatrol != null)
            {
                enemyPatrol.ReturnToPatrolStart(); // tell the patrol script to go back to its patrol centre
            }
        }

    } // end onunhooked

    private void FixedUpdate()
    {
        if (hookedByRod == null) // if we have no rod refernce then drop out
        {
            return;
        }

        if (!hookedByRod.IsLineUnderTension()) // check to see if the line is NOT currently under tension
        {
            rb.linearVelocity = Vector2.zero; // zero out the movement 
            return;
        }

        // if the line IS under tension then do this: 
        Vector2 rodPosition = hookedByRod.RodPoint.position; // work out where the rod is
        Vector2 enemyPosition = rb.position; // work out where this enemy currently is

        Vector2 direction = (rodPosition - enemyPosition).normalized; // work out what direction it is to the rod

        if (pullMode == PullMode.Full2D) // if this enemy uses full 2D pull movement (ignoring gravity basically)
        {
            rb.linearVelocity = direction * pullSpeed; // move towards the rod in full 2D at this enemy's pull speed
        }
        else // otherwise this enemy uses horizontal-only pull (so it will be affected by vertical gravity)
        {
            float horizontalDistanceToRod = Mathf.Abs(rodPosition.x - enemyPosition.x); // work out how far away the rod is on the X axis only

            if (horizontalDistanceToRod <= verticalLiftUnlockDistance) // if the rod is nearly above the enemy on the X axis...
            {
                rb.linearVelocity = direction * pullSpeed; // unlock full 2D pull so the enemy can now be lifted
            }
            else
            {
                rb.linearVelocity = new Vector2(direction.x * pullSpeed, rb.linearVelocity.y); // otherwise move toward the rod on X only
                                                                                               // but keep the current Y velocity so gravity/falling still work
            }
        }

        Debug.DrawLine(rb.position, hookedByRod.RodPoint.position, Color.red);
    } // end fixedupdate

} // end classs