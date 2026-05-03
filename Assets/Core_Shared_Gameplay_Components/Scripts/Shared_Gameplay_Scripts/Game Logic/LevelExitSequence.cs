using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelExitSequence : MonoBehaviour
{

    [SerializeField] private string levelTransitionScene; // what scene to play next
    [SerializeField] private GameObject exitSprite;
    [SerializeField] private GameObject exitCollider;
    [SerializeField] private ParticleSystem exitFX;

    private bool exitUnlocked = false; 
    private bool isTransitioning = false;

    private void Start() // start with these elements turned off
    {
        if (exitSprite != null)
        {
            exitSprite.SetActive(false);
        }

        if (exitCollider != null)
        {
            exitCollider.SetActive(false);
        }

    } // end start

    public void PlaySequence(int fishCaught, int fishRequired)
    {
        Debug.Log($"Exit sequence started: {fishCaught} / {fishRequired}"); // print outcome

        if (exitSprite != null) // guard against missing object
        {
            exitSprite.SetActive(true);
        }

        if (exitCollider != null) // guard against missing object
        {
            exitCollider.SetActive(true);
        }

        if (exitFX != null) // guard against missing object
        { 
            exitFX.Play();
        }

        exitUnlocked = true;

    } // end playsequence


    private void OnTriggerEnter2D(Collider2D other) // when player gets into the collieder
    {
        if (!exitUnlocked) // do nothing till exit unlocked
        {
            return;
        }

        if (isTransitioning) // do nothing if we're already swithcing scenes
        {
            return;
        }

        if (!other.CompareTag("Player")) // if the object does NOT have the player tag drop out
        {
            return;
        }

        // otherwise - do the load function
        LoadNextLevel();

    } // end OnTriggerEnter2D

    private void LoadNextLevel() // load the scene referenced in the object
    {
        if (string.IsNullOrWhiteSpace(levelTransitionScene)) // guard agasint missing scene
        {
            Debug.LogWarning("LevelExitSequence: No level transition scene assigned.");
            return;
        }

        isTransitioning = true;
        SceneManager.LoadScene(levelTransitionScene); // load the new scene

    } // end LoadNextLevel


}// end class