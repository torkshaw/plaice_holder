using UnityEngine;
using UnityEngine.SceneManagement;


public class MusicManager : MonoBehaviour

{
    public static MusicManager instance; //instance for singleton scripting
    public bool ella_OST; //bool to toggle soudntrack versions
    public AudioSource musicPlayer; // audio source component to play game music

    [Header("Winter's Music")]// header shown in unity inspector
    public AudioClip[] musicWinter; // array storing winter's soundtracks



    [Header("Ella's Music")]// header shown in unity inspector
    public AudioClip[] musicElla;// array storing ella's soundtracks



    void Awake()
    {
        
            if (instance != null)   //
            {                       //
                Destroy(gameObject);//
                return;             // singleton scripting to only allow one instance of audiomanager
            }                       //
                                    //
            instance = this;        //

            DontDestroyOnLoad(gameObject); //allows music manager to persist between scenes
        

        musicPlayer = GetComponent<AudioSource>(); //fetches audiosource component attached to object
        musicPlayer.volume = 0.7f; // initialises audiosource volume (used for testing and mixing)
        SceneManager.sceneLoaded += OnSceneLoaded;      // subscribe to unity scene manager's sceneLoaded event and call OnSceneLoaded function each scene load

    }//end awake

    public void SoundtrackToggle() // function to toggle soundtrack version
    {
        ella_OST = !ella_OST; // checks the value of the soundtrack version bool
        if (ella_OST == false)
        {
            Debug.Log("winter ost play"); // outputs to debug log
            PlaySceneMusicWinter();       //play winter's soundtrack     
        }
        else
        {
            Debug.Log("Ella ost play"); // outputs to debug log
            PlaySceneMusicElla(); //play ella's soundtrack
        }
    }//end soundtrack toggle

    void OnSceneLoaded(Scene scene, LoadSceneMode mode) // function called when scene is loaded
    {
        if (ella_OST == false) // checks bool value
        {
            PlaySceneMusicWinter(); //play winter's soundtrack
        }
        else
        {            
            PlaySceneMusicElla(); //play ella's soundtrack
        }
        

    }

    void PlaySceneMusicWinter() // function to play winter's soundtrack
    {

        int sceneIndex = SceneManager.GetActiveScene().buildIndex; // fetches the currently active scene's build index
        if (sceneIndex < musicWinter.Length) //checks if a music track exists for the current scene index
        {
            musicPlayer.clip = musicWinter[sceneIndex]; //assigns clip to audiosource
            musicPlayer.Play(); //plays audio source
            
        }
        else
        {
            Debug.Log("No music assigned for scene"); //outputs to debug log if no music clip exists for current scene
        }
    }//end PlaySceneMusicWinter

    void PlaySceneMusicElla() // function to play ella's soundtrack
    {

        int sceneIndex = SceneManager.GetActiveScene().buildIndex; // fetches the currently active scene's build index
        if (sceneIndex < musicElla.Length) //checks if a music track exists for the current scene index
        {
            musicPlayer.clip = musicElla[sceneIndex];//assigns clip to audiosource
            musicPlayer.Play();//plays audio source
        }
        else
        {
            Debug.Log("No music assigned for scene"); //outputs to debug log if no music clip exists for current scene
        }
    }// end PlaySceneMusicElla

}// end of class
// winter james
