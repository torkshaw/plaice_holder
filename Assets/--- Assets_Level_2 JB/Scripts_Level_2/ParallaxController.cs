using UnityEngine;


// Script created by Jason 
public class ParallaxController : MonoBehaviour
{
    private float startPos; // Objects starting position in the world (x)
    private float startCamPos; // Main cams starting x position
    public GameObject cam;  // Field for Main Cam
    [SerializeField] private float parallaxEffect; // 0 = static, 1 = follows cam, in between for varied movement speeds



    void Start()
    {
        startPos = transform.position.x;
        startCamPos = cam.transform.position.x; // Parallax movement will be dependant on the starting position
                                                // This fixed objects jumpin backwards on spawning
    }

    void Update()
    {
        float camTravel = cam.transform.position.x - startCamPos;
        transform.position = new Vector3(startPos + (camTravel * parallaxEffect),
                                         transform.position.y,
                                         transform.position.z);
    }
}