using UnityEngine;


// TS
public class HazardKillVolume : MonoBehaviour
{


    private void OnTriggerEnter2D(Collider2D other) // this is using the unity ontrigger2d method to fire this when an object's collider hits THIS objects trigger volume collider
                                                    // we use 'other' so we can get information about the collider that entered this one.
    {

        PlayerLifeController lifeController = other.GetComponent<PlayerLifeController>(); // if the object that triggered this collision has a PlayerLifeController component (ie. is the player) store that in lifeController

        if (lifeController != null) // if lifeController contains a PlayerLifeController then do stuff to the player
        {
            Collider2D sourceCollider = GetComponent<Collider2D>(); // quickly get the collider of what hit us
            lifeController.ApplyHit(true, sourceCollider); // fire the ApplyHit method in the lifeController script with the boolean (true).
                                                           // which means ApplyHit, and its a respawn type hit (true).
                                                           // and also passes in the collider info
            return;
        }

        // if it wasn't the player, check if it was an enemy

        EnemyLifeController enemyLife = other.GetComponent<EnemyLifeController>(); // see if the object has an EnemyLifeController

        if (enemyLife != null) // if it does, destroy the enemy
        {
            Destroy(enemyLife.gameObject);
        }

    }



} // end class