using UnityEditor.Rendering;
using UnityEngine;

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
        if (FishingLevelManager.Instance != null)
        {
            FishingLevelManager.Instance.NotifyMinigameWon();
        }
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
}
