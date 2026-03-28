using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicManager : MonoBehaviour

{
    public bool winter_OST = true;
    public AudioSource musicPlayer;

    [Header("Winter's Music")]
    public AudioClip[] musicWinter;


    //[Header("Ella's Music")]
    //public AudioClip[] musicElla;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        musicPlayer = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        musicPlayer.volume = 0;
    }

    public void SoundtrackToggle()
    {
        winter_OST = !winter_OST;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (winter_OST == true)
        {
            PlaySceneMusicWinter();
        }
        else
        {
        //    PlaySceneMusicElla();
        }
        

    }

    void PlaySceneMusicWinter()
    {

        int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        if (sceneIndex < musicWinter.Length)
        {
            musicPlayer.clip = musicWinter[sceneIndex];
            musicPlayer.Play();
        }
        else
        {
            Debug.Log("No music assigned for scene");
        }
    }

    //void PlaySceneMusicElla()
    //{

        //int sceneIndex = SceneManager.GetActiveScene().buildIndex;
        //if (sceneIndex < musicElla.Length)
        //{
       //     musicPlayer.clip = musicElla[sceneIndex];
        //    musicPlayer.Play();
       // }
     //  else
       // {
       //     Debug.Log("No music assigned for scene");
       // }
   // }
}// end of class
// winter james
