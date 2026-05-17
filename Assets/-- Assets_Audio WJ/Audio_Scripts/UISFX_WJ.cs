using UnityEngine;

public class UISFX : MonoBehaviour
{
    public static UISFX instance; // instance for singleton scripting

    public AudioSource uisfx; // audio source to play audio
    public AudioClip click; // audio clip holding the sound effect

    void Awake()
    {
        if (instance != null)   //
        {                       //
            Destroy(gameObject);//
            return;             // singleton scripting
        }                       //
                                //
        instance = this;        //
        DontDestroyOnLoad(gameObject);//allow object to persist between scenes
    }

    public void PlayClick() // fucntion to play the menu buttoin click
    {
        uisfx.PlayOneShot(click); // plays the click sound effect
    }
}
