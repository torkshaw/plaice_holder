using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))] //ensures gameobject will always have rigidbody2d attached
public class CoconutDrop : MonoBehaviour
{

    public Transform player; // holds the player's transform position
    public float maxDistance = 6.5f; // maximum distance before hazard becomes active, defaults to 6.5 units
    private Rigidbody2D coconutRB; // variable for holding the rigidbody2d component

    void Start()
    {
        coconutRB = GetComponent<Rigidbody2D>(); // gets the rb2d attached to game object and stores it in coconutRB        
    } // end start

    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) < maxDistance) // Checks the distance between the player and the coconut
        {            
            coconutRB.WakeUp(); // wakes up the rb2d attached to the coconut
        }
    } //end update
}