using UnityEngine;

public class PlayerSFXManager : MonoBehaviour
{
    [SerializeField] private PlayerLifeController lifeController; // reference to the PlayerLifeController script
    //[SerializeField] private PlayerMovementControllerV2 movementController;
    //[SerializeField] private FishingRodController2D fishingController;

    [SerializeField] private AudioSource playerSFXPlayer; //Audio source for SFX

    [Header("SFX List")]
    [SerializeField] private AudioClip enemyDamageSFX;  //
    [SerializeField] private AudioClip waterDamageSFX;  // audio clips for various sfx
    //[SerializeField] private AudioClip jumpSFX;       //
    //[SerializeField] private AudioClip hookedSFX;     //

    private void OnEnable()
    {
        lifeController.DamageTaken += EnemyDamageSFX;       //
        lifeController.RespawnRequested += WaterDamageSFX;  //
        //movementController.OnJump += JumpSFX;             // subscribing to events from scripts and calling functions to play them
        //fishingController.AttachToObject += HookedSFX;    // sfx beyond basic jump and damage will be added after submission
        //fishingController.ReelingStarted += ReelSFX;      //
    }

    private void OnDisable()
    {
        lifeController.DamageTaken -= EnemyDamageSFX;       //
        lifeController.RespawnRequested -= WaterDamageSFX;  //
        //movementController.OnJump -= JumpSFX;             // unsubscribing from events
        //fishingController.AttachToObject -= HookedSFX;    //  
        //fishingController.ReelingStarted -= ReelSFX;      //
    }

    private void EnemyDamageSFX(Collider2D sourceCollider, Vector2 hitDirection)
    {
        playerSFXPlayer.PlayOneShot(enemyDamageSFX);
    }

    private void WaterDamageSFX()
    {
        playerSFXPlayer.PlayOneShot(waterDamageSFX);
    }

    //private void JumpSFX()
    //{
    //    playerSFXPlayer.PlayOneShot(jumpSFX);
    //}

    //private void HookedSFX()
    //{
    //    playerSFXPlayer.PlayOneShot(hookedSFX);
    //}

    //private void ReelSFX()
    //{

    //}

}//end of class
//winter james
