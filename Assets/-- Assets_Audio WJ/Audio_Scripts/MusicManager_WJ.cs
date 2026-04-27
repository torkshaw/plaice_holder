using UnityEngine;
using UnityEngine.SceneManagement;

using TMPro;

public class MusicManager : MonoBehaviour

{
    public bool ella_OST;
    public AudioSource musicPlayer;
    [Header("Winter's Music")]
    public AudioClip[] musicWinter;



    //[Header("Ella's Music")]
    //public AudioClip[] musicElla;



    private void Awake()
    {        
        musicPlayer = GetComponent<AudioSource>();
        SceneManager.sceneLoaded += OnSceneLoaded;
        musicPlayer.volume = 0;

    }

    public void SoundtrackToggle()
    {
        ella_OST = !ella_OST;
        if (ella_OST == false)
        {
            Debug.Log("winter ost play");
            PlaySceneMusicWinter();            
        }
        else
        {
            Debug.Log("Ella ost play");
            //    PlaySceneMusicElla();
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (ella_OST == false)
        {
            PlaySceneMusicWinter();
        }
        else
        {
            Debug.Log("Ella ost play");
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
