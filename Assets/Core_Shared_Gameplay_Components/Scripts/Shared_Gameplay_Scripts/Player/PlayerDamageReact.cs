using UnityEngine;

// TS

public class PlayerDamageReact : MonoBehaviour
{


    //Serialized Fields

    [SerializeField] private PlayerLifeController lifeController; //listening for the damagetaken
    [SerializeField] private Rigidbody2D rb; // so we can move the player for knockback
    [SerializeField] private SpriteRenderer spriteRenderer; // so we can flash the player sprite
    [SerializeField] private float flashInterval = 0.05f; // how fast the flash is

    [Header("Knockback")] // what happens when you get knocked back
    [SerializeField] private float knockbackStrength = 8f; // strength of knockback
    [SerializeField] private Vector2 knockbackMultiplier = new Vector2(1f, 1f); // this allows for scaling on X and Y in case we need to adjust that



    // private variables

    private float flashTimer;
    private bool isFlashing;



    // Functions Start From Here
    // ---------------------------------------------------------------------------------------------------------------------------------- //


    private void OnEnable() // this method just starts this script listening for the DamageTaken EVENT when the player object is enabled
                            // and adds HandleDamageTaken to the things it needs to execute when it hears it.
                            // in this case we're doing this in OnEnable instead of Start because we may toggle this component on and off 
    {
        if (lifeController != null)
        {
            lifeController.DamageTaken += HandleDamageTaken;
        }
    } // end onenable



    private void OnDisable()// this method removes HandleDamageTaken from the list of things to do if this object (player) is disables
    {
        if (lifeController != null)
        {
            lifeController.DamageTaken -= HandleDamageTaken;
        }
    } // end ondisable



    private void Update() // this is the meat of the code, running every frame
    {
        if (lifeController == null || spriteRenderer == null) // chgeck to make sure we have the lifecontroller script and a spriterendered assigned
        { 
            return;
        }

        if (lifeController.IsInvulnerable) // if the player is currently invulnerable
        {
            isFlashing = true; // set the flashing bool to true

            flashTimer -= Time.deltaTime; // reduce the flash timer 

            if (flashTimer <= 0f) // if the flash timer is less than or equal to 1 then...
            {
                spriteRenderer.enabled = !spriteRenderer.enabled; // enable the sprite
                flashTimer = flashInterval; // set the flash timer to flash interval
            }
        }  // end the else for invulnerable

        else // if the player is NOT invulnerable
        {
            if (isFlashing) // and if the player isflashing
            {
                isFlashing = false; // reset isFlashing to flase
                spriteRenderer.enabled = true; // set the sprite renderer back on again
                flashTimer = 0f; // set the flash timer to 0
            }
        } // end the else for NOT invulnerable


    } // end update


    private void HandleDamageTaken(Collider2D sourceCollider, Vector2 hitDirection)
    {

        // Debug.Log("PlayerDamageReact received damage event");

        // this is the knockback if
        if (rb != null) // make sure we have an rb captured
        {
            Vector2 knockback = new Vector2(                    // calcualte a knockback vector using the hitdirection and the multiplier for that vector..
                hitDirection.x * knockbackMultiplier.x,             // use direction x
                hitDirection.y * knockbackMultiplier.y              // and direction y
            ).normalized * knockbackStrength;                   // then multiply normalised direction by strenght

            rb.linearVelocity = knockback;                      // then change the rbs linear velocity accordingly
        } // end knockback if


    } // end handledamagetaken




} // end class
