using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManager : MonoBehaviour

{
    public bool winter_OST = true;


    [Header("Winter's Music")]
    [SerializeField] private AudioSource menuMusicWJ;    
    [SerializeField] private AudioSource levelOneMusicWJ;
    [SerializeField] private AudioSource levelTwoMusicWJ;
    [SerializeField] private AudioSource levelThreeMusicWJ;
    [SerializeField] private AudioSource levelFourMusicWJ;
    [SerializeField] private AudioSource levelFiveMusicWJ;

    [Header("Jingles")]
    [SerializeField] private AudioSource levelComplete;
    [SerializeField] private AudioSource gameOver;

    //[Header("Ella's Music")]
    //[SerializeField] private AudioSource menuMusicEC;
    //[SerializeField] private AudioSource levelOneMusicEC;
    //[SerializeField] private AudioSource levelTwoMusicEC;
    //[SerializeField] private AudioSource levelThreeMusicEC;
    //[SerializeField] private AudioSource levelFourMusicEC;
    //[SerializeField] private AudioSource levelFiveMusicEC;

    [Header("SFX")]
    [SerializeField] private AudioSource sFX;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }
    
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}// end of class
// winter james
