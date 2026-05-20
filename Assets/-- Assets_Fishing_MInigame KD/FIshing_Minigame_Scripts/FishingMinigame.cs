using UnityEngine;
using TMPro;

public class FishingMinigame : MonoBehaviour
{
    //this is where you drag in the fish and top/bottom pivot sprites
    [SerializeField] Transform topPivot;
    [SerializeField] Transform bottomPivot;
    [SerializeField] Transform fish;


    //fish mechanics
    // Current fish position between pivots
    float fishPosition;

    // Random target destination for the fish
    float fishDestination;

    // Timer controlling when fish changes direction
    float fishTimer;

    // Multiplier affecting fish movement timing randomness
    [SerializeField] float timerMultiplicator = 3f;

    // SmoothDamp velocity reference
    float fishSpeed;

    // Controls how smooth the fish movement is
    [SerializeField] float smoothMotion = 1f;


    //hook mechanics
    // Hook transform reference
    [SerializeField] Transform hook;

    // Current hook position
    float hookPosition;

    // Size of the hook catch area
    [SerializeField] float hookSize = 0.2f;

    // Speed that progress increases when fish is inside hook
    [SerializeField] float hookPower = 0.5f;

    // Current catch progress
    float hookProgress;

    // Current hook movement velocity
    float hookPullVelocity;

    // Force applied when player clicks
    [SerializeField] float hookPullPower = 0.01f;

    // Gravity force pulling hook downward
    [SerializeField] float hookGravityPower = 0.005f;

    // Speed progress decreases when fish leaves hook area
    [SerializeField] float hookProgressDegradationPower = 0.1f;

    // Pauses the minigame after win/loss
    bool pause = false;

    // Time player can fail before losing
    [SerializeField] float failTimer = 10f;

    // Stores default hook size for resetting after god mode
    float normalHookSize;

    //progress mechanics
    // Reference to progress bar UI object
    [SerializeField] Transform progressBarContainer;

    //achievement mechanics

    // Achievement text UI reference
    [SerializeField] TMP_Text achievementText;

    // Achievement popup prefabs
    [SerializeField] GameObject AchievementPrefab5FishCaught;
    [SerializeField] GameObject AchievementPrefab10FishCaught;
    [SerializeField] GameObject AchievementPrefab15FishCaught;

    void Start()
    {
        // Save the normal hook size at the start
        normalHookSize = hookSize;
    }

    private void Update()
    {
        // Stop updating if game is paused
        if (pause)
        {
            return;
        }

        // God mode makes hook fill almost entire fishing area
        if (PlayerLifeController.godMode)
        {
            hookSize = 1f;
        }
        else
        {
            // Reset hook size if god mode disabled
            hookSize = normalHookSize;
        }
        // Run core minigame systems
        Fish();
        Hook();
        ProgressCheck();
        


    }

    // Handles catch progress and win/loss logic
    void ProgressCheck()
    {
        // Scale progress bar based on hook progress
        Vector3 ls = progressBarContainer.localScale;
        ls.y = hookProgress;
        progressBarContainer.localScale = ls;

        // Calculate hook area limits
        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;


        // If fish is inside hook area
        if (min < fishPosition && fishPosition < max)
        {
            // Increase catch progress
            hookProgress += hookPower * Time.deltaTime;
        }
        else
        {
            // Decrease catch progress
            hookProgress -= hookProgressDegradationPower * Time.deltaTime;

            // Reduce fail timer
            failTimer -= Time.deltaTime;

            // Lose if timer runs out
            if (failTimer < 0f)
            {
                Lose();
            }
        }

        // Win if progress reaches full
        if (hookProgress >= 1f)
        {
            Win();
        }

        // Clamp progress between 0 and 1
        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
    }

    // Handles hook movement
    void Hook()
            {
        // Left mouse button pulls hook upward
        if (Input.GetMouseButton(0))
                {
                    hookPullVelocity += hookPullPower * Time.deltaTime;
                }

        // Gravity constantly pulls hook downward
        hookPullVelocity -= hookGravityPower * Time.deltaTime;

        // Move hook position
        hookPosition += hookPullVelocity;

        // Stop hook leaving bottom boundary
        if (hookPosition - hookSize / 2 <= 0f && hookPullVelocity < 0f)
                {
                    hookPullVelocity = 0f;
                }

        // Stop hook leaving top boundary
        if (hookPosition + hookSize / 2 >= 1f && hookPullVelocity > 0f)
                {
                    hookPullVelocity = 0f;
                }
        // Clamp hook position inside valid area
        hookPosition = Mathf.Clamp(hookPosition, hookSize / 2, 1 - (hookSize / 2));

        // Move hook object visually between pivots
        hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
            }

    // Handles fish movement
    void Fish()
    {
        // Reduce fish movement timer
        fishTimer -= Time.deltaTime;
        // If timer reaches 0
        if (fishTimer < 0f)
        {
            // Generate new random timer
            fishTimer = UnityEngine.Random.value * timerMultiplicator;

            // Generate new random destination
            fishDestination = UnityEngine.Random.value;
        }
        // Smoothly move fish toward destination
        fishPosition = Mathf.SmoothDamp(fishPosition, fishDestination, ref fishSpeed, smoothMotion);

        // Move fish visually between pivots
        fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPosition);
    }

    // Called when player wins the minigame
    void Win()
    {
        pause = true;

        Debug.Log("YOU WIN");

        // Load current saved fish count
        int fishCaught = PlayerPrefs.GetInt("FishCaught", 0);

        // Increase fish count
        fishCaught++;

        // Save updated fish count
        PlayerPrefs.SetInt("FishCaught", fishCaught);

        // Achievement unlocks
        if (fishCaught == 5)
        {
            ShowAchievement5Fish("Achievement Unlocked: Catch 5 Fish");
        }

        if (fishCaught == 10)
        {
            ShowAchievement10Fish("Achievement Unlocked: Catch 10 Fish");
        }

        if (fishCaught == 15)
        {
            ShowAchievement15Fish("Achievement Unlocked: Catch 15 Fish");
        }

        // Notify level manager that player won
        if (FishingLevelManager.Instance != null)
        {
            FishingLevelManager.Instance.NotifyMinigameWon();
        }
        //deletes the saved fish count for testing purposes, comment out when not needed
        //PlayerPrefs.DeleteAll();
    }

    // Called when player loses the minigame
    void Lose()
    {
        pause = true;
        Debug.Log("YOU LOSE");

        // Notify level manager that player lost
        if (FishingLevelManager.Instance != null)
        {
            FishingLevelManager.Instance.NotifyMinigameLost();
        }
    }


    // Spawn achievement popup for 5 fish
    void ShowAchievement5Fish(string message)
    {
        GameObject popup = Instantiate(AchievementPrefab5FishCaught);

        Destroy(popup, 3f);
        

        Invoke(nameof(HideAchievement), 3f);
    }

    // Spawn achievement popup for 10 fish
    void ShowAchievement10Fish(string message)
    {
        GameObject popup = Instantiate(AchievementPrefab10FishCaught);

        Destroy(popup, 3f);


        Invoke(nameof(HideAchievement), 3f);
    }

    // Spawn achievement popup for 15 fish
    void ShowAchievement15Fish(string message)
    {
        GameObject popup = Instantiate(AchievementPrefab15FishCaught);

        Destroy(popup, 3f);


        Invoke(nameof(HideAchievement), 3f);
    }


    // Hides achievement text UI
    void HideAchievement()
    {
        achievementText.gameObject.SetActive(false);
    }
}
