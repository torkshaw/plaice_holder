using UnityEngine;
using System;

// this script is based on the script I used in my scrolling shooter/the tutorial we did in class
// TS

public class PlayerLifeController : MonoBehaviour
{
    // configurable inspector variables 

    [Header("Lives")]
    [SerializeField] private int startingLives = 3;
    [SerializeField] private int currentLives; // making this visible in inspector so we can do debugging. prolly remove it when we get a UI in here too 

    [Header("State")] // likewise with these. until we get UI or visual feedback we can see it in the inspector
    [SerializeField] private bool isDead; // this refers to a state when the player has lost a life but before the next thing happens (probably game over). this lets us stop player input in this state or similar. 
    [SerializeField] private bool isInvulnerable; // when the player has been hit and can't be hit again 

    [Header("Damage")] // controls for what happens on damage but NOT respawn
    [SerializeField] private float invulnerabilityDuration = 0.75f;
    [SerializeField] private float invulnerabilityTimer;


    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //

    private void Awake() // applying our base values at the start
    {
        currentLives = startingLives;
        isDead = false;
        isInvulnerable = false;
    }

    // need to make our core variables available as properties to other scripts too, so creating public versions here. "public int" means that these are READ ONLY to those scripts.
    public int CurrentLives => currentLives; // exposing this so that other scripts can use it
    public bool IsDead => isDead; // exposing this so that other scripts can use it
    public bool IsInvulnerable => isInvulnerable; // exposing this so that other scripts can use it


    // Broadcast events: these events allow for stuff like the audiomanager and FX manager to listen for these and respond accordingly

    public event Action RespawnRequested; // this action is a public "event"" taht other scripts can subscribe to. so our respawn script can respond when this action is called. 
                                          // actions can be listened to by other scripts but they can't be <called> but those scripts. they're just listing for this.
                                          // this is just a call. it deosn't contain any data. So when we broadcast this other scripts can respond to this simple request.
                                          // other scripts will contain methods that are added to the "delegate invocation list" for this event action. 
                                          // essnetially they listen for this event action and when it happens they have methods that are added to the list of things to run (with the += operator).
                                          // the reason we're using this Action event instead of calling a respawn method from the respawn script is that this way THIS script doesn't have
                                          // to care about any other specific script and its methods (for example a respawn method in the respawn script).
                                          // instead it can just say "ok I need a respawn please!" and whatever script or scripts that are listening for it can add their respawn realted code to the delegate list for this action.
                                          // This makes it less architecture SPECIFIC. so if we want to change up where repsawn lives or how that script works in the future we can without risking this one.

    public event Action GameOverRequested; // here's another of these public event actions. again, we're using this so that we don't create architecture dependencies.
                                           // this action event will broadcast that we want to do all the game over things and they can be fired from their individual appropriate scripts that are listening. 

    public event Action<Collider2D, Vector2> DamageTaken;       // this action event will broadcast when the player takes a hit from a NON RESPAWN (ApplyHit(false)) enemy.
                                                                // this event can be listened to by the damage reaction script on the player to do flashing, knockback, etc.
                                                                // this event also captures the collider reference and vector 2 (position) of the thing that did damage.

    public static event System.Action OnLifeLost; // player loses a life - this happens whenever the player has their life counter reduced (hitting an enemy OR falling in water)


    public void ApplyHit(bool shouldRespawn, Collider2D sourceCollider) 
        // this function is the main entry point for this system. its called when things "hit" the player.
        // since we want differnt behaviours for the water and enemies we can indicate that from the call - that's what the bool is for
        // so those things would call either ApplyeHit(true) or ApplyHit(false). and that bool will carry through for this script to use when deciding what to do next.
        // the rest of the stuff we're pulling in is the collider refernece (what hit me), which includes its position and stuff (to allow directional knockback)
    {

        if (isDead) // if the player is dead we do nothing
        {
            return;
        }

        if (isInvulnerable) // if the player is invulnerable we do nothing
        {
            return;
        }


        currentLives = Mathf.Max(0, currentLives - 1); // reduce the players current lives by 1. we're using the math.max here to prevent current lives going below 0.
        OnLifeLost?.Invoke(); // call the event for stuff that's listening

        if (currentLives <= 0) // if the player's current lives are less than or equal to 0...
        {
            isDead = true; // turn on the isDead bool
            Debug.Log("Game Over requested");
            GameOverRequested?.Invoke(); // here we're invoking that gameoverrequested we defined above. so that subscribers know to act on it, as appropriate. 
            return;
        }

        if (shouldRespawn) // if the bool for shouldrespawn is 'true' from the ApplyHit call then we do this: 
        {
            isDead = true;
            Debug.Log("Respawn requested");
            RespawnRequested?.Invoke(); // here we're invoking/broadcasting that public respawnRequested we defined above. so if something is subscribed to this event now is the time to call RespawnRequested.
            return;
        }

        // from this point we're doing stuff that happens when shouldRespawn returns FALSE. ie. player takes damage (we're still reducing player lives above) 
        // but does not respawn, then do this stuff: 


                Vector2 hitDirection = Vector2.zero; // calculate the hit direction of the hit we just took
                if (sourceCollider != null) // make sure there is a collider that hit us
                {
                    hitDirection = ((Vector2)transform.position - (Vector2)sourceCollider.transform.position).normalized; // establish the hit direction for that collider, relative to us
                }

                DamageTaken?.Invoke(sourceCollider, hitDirection); // NOW fire the 'damagetaken' event so all the other scripts that are listening for it can do their thing (flashing, FX, etc.)
                                                                   // we need to calcualte the hit direction before we do that so we can pass that through.

                // start temporary invulnerability
                isInvulnerable = true;
                invulnerabilityTimer = invulnerabilityDuration; // and set the timer to the fied above



    } // end ApplyHit

    public void FinishRespawn() // some quick logic to tidy up player states after respawn happens. again, we want to keep it all in this script so that life stuff all lives in one place.
    {
        isDead = false;

    } // end finish respawn


    private void Update() // this is the code that runs every frame
    {
        if (!isInvulnerable) return; // if the player is still invulnerable then drop out
        
        invulnerabilityTimer -= Time.deltaTime; // reduce the timer for invulnerability

        if (invulnerabilityTimer <= 0f) // if the timer is less than or equal to 0...
        {
            isInvulnerable = false; // remove the invulnerability flag from the player
        }
    } // end update




} // end class