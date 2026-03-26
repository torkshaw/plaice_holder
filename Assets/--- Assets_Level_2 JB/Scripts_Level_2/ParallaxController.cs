using UnityEngine;

public class ParallaxController : MonoBehaviour
{
    private float length, startpos; // Variable for asset width and x/y start position
    public GameObject cam; // Field for the main camera
    [SerializeField] private float parallaxEffect; // How strong the parallax feels, higher is slower.

    void Start()
    {
        startpos = transform.position.x; // Assign x position to StartPos
        length = GetComponent<SpriteRenderer>().bounds.size.x; // Get x length of the sprite (essentially width)
    }

    void Update()
    {
        float temp = (cam.transform.position.x * (1 - parallaxEffect)); 
        float dist = (cam.transform.position.x * parallaxEffect);

        transform.position = new Vector3(startpos + dist, transform.position.y, transform.position.z);

        if (temp > startpos + length) startpos += length;
        else if (temp < startpos - length) startpos -= length;
    }
}