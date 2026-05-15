using UnityEngine;
// Script created by Jason 
public class ParallaxController : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    [SerializeField] private float parallaxEffect;

    void Start()
    {
        startPos = transform.position.x;
    }

    void Update()
    {
        transform.position = new Vector3(startPos + (cam.transform.position.x * parallaxEffect),
                                         transform.position.y,
                                         transform.position.z);
    }
}