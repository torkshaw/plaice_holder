using UnityEngine;

// this is what we use to fire the minigame but also to track wins/losses (TS)

public class FishingLevelManager : MonoBehaviour
{
    public static FishingLevelManager Instance { get; private set; }

    [Header("Minigame Setup")]
    [SerializeField] private FishingMinigame fishingMinigamePrefab; // this is the prefab of the minigame. we do a new one for each time we fish.
    [SerializeField] private Vector3 minigameSpawnOffset = new Vector3(2f, 1.5f, 0f); // where FROM the spawn location we want to fire up the game

    [Header("Spawn Location Object")]
    [SerializeField] private Transform playerTransform; // what object do we want o spawwn next to (respecting the offset above)

    [Header("Game Stats (for this scene/level)")]
    [SerializeField] private int fishCaughtThisLevel;
    [SerializeField] private bool isMinigameActive;

    private HookableFish2D currentFish;
    private FishingRodController2D currentRod;
    private FishingMinigame activeMinigameInstance; // the instance of the prefab we've fired up for this

    public bool IsMinigameActive => isMinigameActive; // this is public so the rod script can stop doing controls
    public int FishCaughtThisLevel => fishCaughtThisLevel; // this is public so we can use it to drive level mechanics

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Debug.LogWarning("More than one FishingLevelManager found. Destroying duplicate."); // safety check in case there's a scene loading problem
            Destroy(gameObject);
            return;
        }

        Instance = this;
    } // end Awake

    public void StartFishingEncounter(HookableFish2D fish, FishingRodController2D rod) // this is what fires up the minigame
    {
        if (isMinigameActive) // if its running already, dont
        {
            return;
        }

        if (fish == null) // if there is no fish attached, dont
        {
            return;
        }

        if (fishingMinigamePrefab == null) // if the prefab didnt get assigned to the field then report it and drop out
        {
            Debug.LogWarning("FishingLevelManager : No FishingMinigame prefab assigned.");
            return;
        }

        // assign the varibales for the current minigame session
        currentFish = fish;
        currentRod = rod;
        isMinigameActive = true;

        Vector3 spawnPosition = GetMinigameSpawnPosition(); // grab the calculated spawn position
        activeMinigameInstance = Instantiate(fishingMinigamePrefab, spawnPosition, Quaternion.identity); // spawn the prefab

    } // end StartFishingEncounter

    private Vector3 GetMinigameSpawnPosition() // this is where the minigame qwill popup.
                                               // if no object has been specified we're trying for the player (+ offset), or the rodpoint (+ offset)
    {
        if (playerTransform != null)
        {
            return playerTransform.position + minigameSpawnOffset;
        }

        if (currentRod != null && currentRod.RodPoint != null)
        {
            return currentRod.RodPoint.position + minigameSpawnOffset;
        }

        return transform.position + minigameSpawnOffset;
    } // end GetMinigameSpawnPosition


    // these two are just tiny methods that the FishinMinigame can call (and it does) on win or loss.
    // they're carved out as two different methods so taht the minigame script doesn't have to pass data through - it can just call either and they provide the necessary bool
    // for the CompleteCurrentFishingEncounter via the type of call, rather than asking the minigame to provide an answer.
    // this is mostly to stay nice and compatible with how the minigame already works.

    public void NotifyMinigameWon()
    {
        CompleteCurrentFishingEncounter(true);
    } // end NotifyMinigameWon

    public void NotifyMinigameLost()
    {
        CompleteCurrentFishingEncounter(false);
    } // end NotifyMinigameLost

    private void CompleteCurrentFishingEncounter(bool playerWon) // listening for the outcomes from Kai's minigame script
    {
        if (!isMinigameActive)
        {
            return;
        }

        if (playerWon) // when it fires a win
        {
            fishCaughtThisLevel++; // increase the fish caught for the level

            if (currentFish != null)
            {
                currentFish.NotifyMinigameWon(); // run NotifyMinigameWon 
            }

            Debug.Log($"Fish caught. Total fish caught this level: {fishCaughtThisLevel}");
        }
        else
        {
            if (currentFish != null)
            {
                currentFish.NotifyMinigameLost(); // if lost then run the escape
            }

            Debug.Log("Fish escaped.");
        }

        if (activeMinigameInstance != null) // destroy this instance of the minigame prefab now the game is over
        {
            Destroy(activeMinigameInstance.gameObject);
            activeMinigameInstance = null;
        }

        // reset all our minigame variables to nil
        currentFish = null; 
        currentRod = null;
        isMinigameActive = false;
    } // end CompleteCurrentFishingEncounter

} // end class