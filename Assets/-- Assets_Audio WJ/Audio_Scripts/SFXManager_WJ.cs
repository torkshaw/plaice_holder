using UnityEngine;

public class PlayerSFXManager : MonoBehaviour
{
    public AudioSource playerSFXPlayer;
    [SerializeField] private AudioClip damageSplash;
    [SerializeField] private AudioClip damageEnemy;
    [SerializeField] private PlayerLifeController lifeController;

    private void Awake()
    {
        playerSFXPlayer = GetComponent<AudioSource>();        
    }

    private void OnEnable()
    {
        lifeController.DamageTaken += EnemyDamageSFX;
        lifeController.RespawnRequested += WaterDamageSFX;
    }

    void EnemyDamageSFX(Collider2D sourceCollider, Vector2 hitDirection)
    {
        Debug.Log("Enemy Damage SFX Play");
    }

    void WaterDamageSFX()
    {
        Debug.Log("Sploosh Damage SFX Play");
        playerSFXPlayer.PlayOneShot(damageSplash);
    }

}
