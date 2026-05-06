using UnityEngine;

public class UISFX : MonoBehaviour
{
    public static UISFX instance;

    public AudioSource source;
    public AudioClip click;

    void Awake()
    {
        if (instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void PlayClick()
    {
        source.PlayOneShot(click);
    }
}
