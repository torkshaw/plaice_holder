using UnityEngine;

public class UISFX : MonoBehaviour
{
    public static UISFX instance;

    public AudioSource uiSFX;
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
        uiSFX.PlayOneShot(click);
    }
}
