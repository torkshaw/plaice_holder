using UnityEngine;
using UnityEngine.SceneManagement;

public class SFXManager : MonoBehaviour
{
    public AudioSource menuSFXPlayer;    
    public AudioClip menuButtonClick;

    private void Awake()
    {
        menuSFXPlayer = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        
    }

    public void MenuButtonSFX()
    {
        menuSFXPlayer = GetComponent<AudioSource>();
        menuSFXPlayer.clip = menuButtonClick;
        menuSFXPlayer.Play();
    }


}
