using UnityEngine;

public class HazardDamageVolume : MonoBehaviour
{

    private void OnTriggerEnter2D(Collider2D other) // this is using the unity ontrigger2d method to fire this when an object's collider hits THIS objects trigger volume collider
                                                    // we use 'other' so we can get information about the collider that entered this one.
    {

        PlayerLifeController lifeController = other.GetComponent<PlayerLifeController>(); // if the object that triggered this collision has a PlayerLifeController component (ie. is the player) store that in lifeController

        if (lifeController == null) // if lifeController is now empty, forget it
        {
            return;
        }

        // if lifeController contains a lifeController then we do stuff to the player

        Collider2D sourceCollider = GetComponent<Collider2D>(); // quickly get the collider of what hit us
        lifeController.ApplyHit(false, sourceCollider); // fire the ApplyHit method in the lifeController script with the boolean (false).
                                                        // which means ApplyHit, and its a respawn type hit (false). so the player will not respawn, they will just lose a life
                                                        // and trigger any additional logic we want to trigger
                                                        // also pass through the details of the thing that hit us - so we can do directional knockback and all that

    }


} // end of class
