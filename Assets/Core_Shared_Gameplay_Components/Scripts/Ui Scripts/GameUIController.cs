using UnityEngine;
using TMPro; // and we're using TMP for all teh text, so this needs to be in here too
using UnityEngine.UI; // cos we're using buttons on this - and this is the unity namespace for this
using UnityEngine.SceneManagement; // this also needs to reload the scene so we need this in here too

public class GameUIController : MonoBehaviour
{

    // define serialised fields for the UI elements to drag them in

    [SerializeField] private TMP_Text livesText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private GameObject pauseMenu; // WJ - pause menu game object
    [SerializeField] private GameObject player; // WJ - player game object    
    [SerializeField] private Button playAgainButton;
    [SerializeField] private PlayerLifeController lifeController; // refence to the lifecontroller, o the script that will call the gameover and respawn envents
    [SerializeField] private string mainMenu = "MainMenu"; // WJ - string for navigation to main menu
    private bool gamePaused = false; // WJ - bool to check if game is paused
    private string[] coffeeCode; // WJ - string to store cheat code
    private int coffeeCodeIndex; // WJ - int to store cheat code length
    private bool coffeeCodeActive = false; // WJ - bool to check if cheat code active



    private void Awake()
    {

        gameOverPanel.SetActive(false); // hid panel on awake
        Time.timeScale = 1f; // WJ 26/03 - ensure game timescale is normal at load


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

        coffeeCode = new string[] { "c", "o", "f", "f", "e", "e" };// WJ - assign new string containing cheat code sequence
        coffeeCodeIndex = 0;// WJ - initialise cheat code index

    } // end start

    private void Update()
    {
        
        if (Input.GetKeyDown(KeyCode.Escape))// WJ - call pause function on ESC press
        {            
            PauseGame();
        }// WJ - end if

        if (pauseMenu.activeSelf == true)// WJ - enable cheat entry while pause menu is active
        {
            CheatEntry();
        }// WJ - end if

        if (pauseMenu.activeSelf == false && coffeeCodeActive == true) // WJ - double game speed if game is unpaused and cheat code has been entered
        {                                                              
            Time.timeScale = 2f;                                       
        }// WJ - end if

    }// end update


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

    public void ReturnToMenu() // function for returning to main menu
    {
        Time.timeScale = 1f; // WJ - reset game speed
        SceneManager.LoadSceneAsync(mainMenu); // WJ -load main menu, async to prevent load stuttering
    } // WJ - end returntomenu

    public void PauseGame()// WJ - Function to pause game
    {
        gamePaused = !gamePaused; // WJ - flips gamepaused bool state

        if (gamePaused == true && gameOverPanel.activeSelf == false) // WJ - checks that the game should be paused and the game over screen is inactive
        {
            pauseMenu.SetActive(true); // WJ - display pause menu
            Time.timeScale = 0f; // WJ - pause game           
        }
        else
        {
            pauseMenu.SetActive(false); // WJ - deactivate pause menu
            Time.timeScale = 1f; // WJ - resume game
        }

    }//WJ - end PauseGame

    private void CheatEntry()// WJ - Function to check for cheat code entry
    {
        if (Input.anyKeyDown) // WJ - check key input
        {

            if (Input.GetKeyDown(coffeeCode[coffeeCodeIndex])) // WJ - increment index if input matches string
            {
                coffeeCodeIndex++;
            }

            else // WJ - reset index if input does not match string
            {
                coffeeCodeIndex = 0;
            }

        }// WJ - end if

        if (coffeeCodeIndex == coffeeCode.Length) // WJ - check if index = length of cheat code string
        {
            
            if (coffeeCodeActive == false) // WJ - activate cheat code if inactive and reset index
            {
                coffeeCodeActive = true;
                Debug.Log("zip zoom");
                coffeeCodeIndex = 0;

            }// WJ - end if active = false

            else // WJ - set cheat code inactive if active and reset index
            {
                Debug.Log("crash :(");
                coffeeCodeActive = false;
                coffeeCodeIndex = 0;
            }// WJ - end else

        } //WJ - end if index = length

    }// WJ - end CheatEntry

} // end class
