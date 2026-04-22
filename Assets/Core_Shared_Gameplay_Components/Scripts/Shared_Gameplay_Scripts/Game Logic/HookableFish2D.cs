using UnityEngine;


// this script is the FISH version of the hookable object. it identifues the object its on as a fish and handles part of the minigame flow (TS).
// this has the same 'onhooked' and 'onunhooked' method format as the other hookable objects, so the rod can stay generic

public class HookableFish2D : MonoBehaviour, IHookableTarget2D // inhereit from IHookableTarget2D
{
    [Header("Hook Setup")]
    [SerializeField] private Transform attachPoint; // attach point, just like other obects

    [Header("State")]
    [SerializeField] private bool fishCanBeHooked = true; 

    private FishingRodController2D currentRod;
    private bool isMinigameRunning;
    private bool isCaught;


    // just like with th eoplatforms we need to announce all its various moving parts
    public Transform AttachPoint => attachPoint;

    public bool IsCaught => isCaught;
    public bool IsMinigameRunning => isMinigameRunning; // this is unique to fish, since they kick off the minigame when attached

    private void Awake()
    {
        if (attachPoint == null)
        {
            attachPoint = transform;
        }
    }

    public void OnHooked(FishingRodController2D rod)
    {
        if (!fishCanBeHooked)
        {
            return;
        }

        if (isCaught)
        {
            return;
        }

        if (isMinigameRunning)
        {
            return;
        }

        currentRod = rod;
        isMinigameRunning = true; 

        if (FishingLevelManager.Instance != null)
        {
            FishingLevelManager.Instance.StartFishingEncounter(this, rod); // start the minigame and pass through the details of the where/what is hooked
        }
        else
        {
            Debug.LogWarning($"No FishingLevelManager found in scene. Fish '{name}' cannot start minigame.");
            isMinigameRunning = false;
        }
    } // end OnHooked

    public void OnUnhooked(FishingRodController2D rod)
    {
        if (currentRod == rod)
        {
            currentRod = null;
        }
    } // end OnUnhooked

    public void NotifyMinigameWon()
    {
        isCaught = true;
        isMinigameRunning = false;

        if (currentRod != null)
        {
            currentRod.Detach();
            currentRod = null;
        }

        gameObject.SetActive(false);
    } // end NotifyMinigameWon

    public void NotifyMinigameLost()
    {
        isMinigameRunning = false;

        if (currentRod != null)
        {
            currentRod.Detach();
            currentRod = null;
        }
    } // end NotifyMinigameLost
}
