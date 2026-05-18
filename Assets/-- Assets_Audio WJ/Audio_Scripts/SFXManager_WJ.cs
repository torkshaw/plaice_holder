using UnityEngine;

public class PlayerSFXManager : MonoBehaviour
{
    [SerializeField] private PlayerLifeController lifeController;
    //[SerializeField] private PlayerMovementControllerV2 movementController;
    //[SerializeField] private FishingRodController2D fishingController;
    public AudioSource playerSFXPlayer;

    [Header("SFX List")]
    [SerializeField] private AudioClip enemyDamageSFX;
    [SerializeField] private AudioClip waterDamageSFX;
    //[SerializeField] private AudioClip jumpSFX;
    //[SerializeField] private AudioClip hookedSFX;
    private void OnEnable()
    {
        lifeController.DamageTaken += EnemyDamageSFX;
        lifeController.RespawnRequested += WaterDamageSFX;
        //movementController.OnJump += JumpSFX;
        //fishingController.AttachToObject += HookedSFX;
        //fishingController.ReelingStarted += ReelSFX;
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

private void EnemyDamageSFX(Collider2D sourceCollider, Vector2 hitDirection)
    {
        playerSFXPlayer.PlayOneShot(enemyDamageSFX);
    }

    private void WaterDamageSFX()
    {
        playerSFXPlayer.PlayOneShot(waterDamageSFX);
    }

}//end of class
//winter james
