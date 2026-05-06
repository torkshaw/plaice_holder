using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public static SFXManager instance;
    public AudioSource menuSFXPlayer;    
    public AudioClip menuButtonClick;

    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(gameObject);
            return;
        }

        else
        {
            instance = this;
        }
        DontDestroyOnLoad(gameObject);

        menuSFXPlayer = GetComponent<AudioSource>(); 
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    public void MenuButtonSFX()
    {
        menuSFXPlayer.clip = menuButtonClick;
        menuSFXPlayer.Play();
    }


}
