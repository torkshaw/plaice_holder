using UnityEngine;
using TMPro; // and we're using TMP for all teh text, so this needs to be in here too
using UnityEngine.UI; // cos we're using buttons on this - and this is the unity namespace for this
using UnityEngine.SceneManagement; // this also needs to reload the scene so we need this in here too

public class GameUIController : MonoBehaviour
{

    // define serialised fields for the UI elements to drag them in

    [SerializeField] private TMP_Text livesText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject player; // WJ 19/03 - player game object
    [SerializeField] private Button playAgainButton;
    [SerializeField] private PlayerLifeController lifeController; // refence to the lifecontroller, o the script that will call the gameover and respawn envents



    private void Awake()
    {

        gameOverPanel.SetActive(false); // hid panel on awake


    } // end awake

    private void Start()
    {


        // subscribing to events from the LifeController
        lifeController.GameOverRequested += HandleGameOver; // look at the PlayerLifeController script (we assigned this above)
                                                            // when the "gameoverrequested" event fires (is Invoked) also run HandleGameOver
                                                            // the += here means "add this to the thing on the left".
                                                            // so; "add HandleGameOver to the list of things to do when "GameOverRequested" fires.
                                                            // so here we're essentially making this script a listener for the GameOverRequested, and describing what to do when we hear it
                                                            // this goes in 'start' because we want to create this connection/listening from when all the objects have been created

        lifeController.RespawnRequested += UpdateLivesUI; // same thing here but with that respawn funciton. this happens in lifeController after the player dies, and lives are reduced.
        lifeController.DamageTaken += HandleDamageTaken;  // samething here but we're listening for DamageTaken notifier and running HandleDamageTaken

        UpdateLivesUI(); // we also need to call the lives update funciton on start so that we show the right number of lives from the get-go


    } // end start

    public void RestartGame() // little function to call the scenemanager and reload. it is PUBLIC because we're going to want the button to be able to access it for OnCLick
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // capture the scene number we're in and then reload that one when button is pressed
        Time.timeScale = 1f; // restart time/gameplay

    } // end restartgame

    private void HandleGameOver()
    {
        player.SetActive(false); // WJ 19/03 - set player game object inactive
        gameOverPanel.SetActive(true); // show game over panel
        Time.timeScale = 0f; // pause gameplay

    } // end handlegameover


    private void UpdateLivesUI() // update on screen UI
    {

        livesText.text = "Lives: " + lifeController.CurrentLives; // turn livesText into "Lives: <the current value from lifecontroller's CurrentLives>"

    }

    private void HandleDamageTaken(Collider2D sourceCollider, Vector2 hitDirection) // little helper to ensure we can still use the DamageTaken event even without its data
                                                                                    // dmage taken usually requires some additional data about what damaged me, so we allow for it here
    {
        UpdateLivesUI(); // then run the update lives method from above
    }



} // end class
