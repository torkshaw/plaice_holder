using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody2D))]
public class CoconutDrop : MonoBehaviour
{

    public Transform player;
    public float maxDistance = 7f;
    private Rigidbody2D coconutRB;
    void Start()
    {
        coconutRB = GetComponent<Rigidbody2D>();        
    }

    void Update()
    {
        if (Vector3.Distance(player.position, transform.position) < maxDistance)
        {            
            coconutRB.WakeUp();
        }
    }
}