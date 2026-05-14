using UnityEngine;
using TMPro;

public class FishingMinigame : MonoBehaviour
{
    //this is where you drag in the fish and top/bottom pivot sprites
    [SerializeField] Transform topPivot;
    [SerializeField] Transform bottomPivot;
    [SerializeField] Transform fish;


    //fish mechanics
    float fishPosition;
    float fishDestination;
    float fishTimer;
    [SerializeField] float timerMultiplicator = 3f;
    float fishSpeed;
    [SerializeField] float smoothMotion = 1f;


    //hook mechanics
    [SerializeField] Transform hook;
    float hookPosition;
    [SerializeField] float hookSize = 0.1f;
    [SerializeField] float hookPower = 0.5f;
    float hookProgress;
    float hookPullVelocity;
    [SerializeField] float hookPullPower = 0.01f;
    [SerializeField] float hookGravityPower = 0.005f;
    [SerializeField] float hookProgressDegradationPower = 0.1f;
    bool pause = false;
    [SerializeField] float failTimer = 10f;

    //progress mechanics
    [SerializeField] Transform progressBarContainer;

    //achievement mechanics
    [SerializeField] TMP_Text achievementText;
    [SerializeField] GameObject AchievementPrefab5FishCaught;
    [SerializeField] GameObject AchievementPrefab10FishCaught;
    [SerializeField] GameObject AchievementPrefab15FishCaught;
    private void Update()
    {
        if(pause)
        {
            return;
        }
        Fish();
        Hook();
        ProgressCheck();
        
    }

    void ProgressCheck()
    {
        Vector3 ls = progressBarContainer.localScale;
        ls.y = hookProgress;
        progressBarContainer.localScale = ls;

        float min = hookPosition - hookSize / 2;
        float max = hookPosition + hookSize / 2;

        if (min < fishPosition && fishPosition < max)
        {
            hookProgress += hookPower * Time.deltaTime;
        }
        else
        {
            hookProgress -= hookProgressDegradationPower * Time.deltaTime;
            failTimer -= Time.deltaTime;

            if(failTimer < 0f)
            {
                Lose();
            }
        }

        if (hookProgress >= 1f)
        {
            Win();
        }

        hookProgress = Mathf.Clamp(hookProgress, 0f, 1f);
    }

            void Hook()
            {
                if (Input.GetMouseButton(0))
                {
                    hookPullVelocity += hookPullPower * Time.deltaTime;
                }

                hookPullVelocity -= hookGravityPower * Time.deltaTime;

                hookPosition += hookPullVelocity;

                if(hookPosition - hookSize / 2 <= 0f && hookPullVelocity < 0f)
                {
                    hookPullVelocity = 0f;
                }
                if(hookPosition + hookSize / 2 >= 1f && hookPullVelocity > 0f)
                {
                    hookPullVelocity = 0f;
                }
                hookPosition = Mathf.Clamp(hookPosition, hookSize / 2, 1 - (hookSize / 2));

                hook.position = Vector3.Lerp(bottomPivot.position, topPivot.position, hookPosition);
            }
    void Fish()
    {
        fishTimer -= Time.deltaTime;
        //check if there is time left in the timer
        if (fishTimer < 0f)
        {
            //if theres not, create a new random amount of time
            fishTimer = UnityEngine.Random.value * timerMultiplicator;

            fishDestination = UnityEngine.Random.value;
        }
        //move the current position smoothly to the destination
        fishPosition = Mathf.SmoothDamp(fishPosition, fishDestination, ref fishSpeed, smoothMotion);
        fish.position = Vector3.Lerp(bottomPivot.position, topPivot.position, fishPosition);
    }

    void Win()
    {
        pause = true;

        Debug.Log("YOU WIN");

        int fishCaught = PlayerPrefs.GetInt("FishCaught", 0);
        fishCaught++;

        PlayerPrefs.SetInt("FishCaught", fishCaught);

        // Achievements
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

        if (FishingLevelManager.Instance != null)
        {
            FishingLevelManager.Instance.NotifyMinigameWon();
        }
        //PlayerPrefs.DeleteAll();
    }
    void Lose()
    {
        pause = true;
        Debug.Log("YOU LOSE");

        if (FishingLevelManager.Instance != null)
        {
            FishingLevelManager.Instance.NotifyMinigameLost();
        }
    }

    void ShowAchievement5Fish(string message)
    {
        GameObject popup = Instantiate(AchievementPrefab5FishCaught);

        Destroy(popup, 3f);
        

        Invoke(nameof(HideAchievement), 3f);
    }

    void ShowAchievement10Fish(string message)
    {
        GameObject popup = Instantiate(AchievementPrefab10FishCaught);

        Destroy(popup, 3f);


        Invoke(nameof(HideAchievement), 3f);
    }

    void ShowAchievement15Fish(string message)
    {
        GameObject popup = Instantiate(AchievementPrefab15FishCaught);

        Destroy(popup, 3f);


        Invoke(nameof(HideAchievement), 3f);
    }

    void HideAchievement()
    {
        achievementText.gameObject.SetActive(false);
    }
}
