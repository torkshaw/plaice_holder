using UnityEngine;

public class PlayerWallDetector : MonoBehaviour
{
    public bool isTouchingLeftWall { get; private set; }
    public bool isTouchingRightWall { get; private set; }
    public bool isTouchingWall => isTouchingLeftWall || isTouchingRightWall;

    [SerializeField] private Transform leftWallCheckPoint;
    [SerializeField] private Transform rightWallCheckPoint;
    [SerializeField] private Vector2 wallCheckSize = new Vector2(0.1f, 0.6f); // Tall skinny box
    [SerializeField] private LayerMask wallLayerMask;


    private void FixedUpdate()
    {
        PerformWallCheck();
    }

    private void PerformWallCheck()
    {
        if (leftWallCheckPoint == null || rightWallCheckPoint == null)
        {
            Debug.LogError("WallCheckPoints not assigned!");
            return;
        }

      

        Collider2D leftHit = Physics2D.OverlapBox(
            leftWallCheckPoint.position, wallCheckSize, 0f, wallLayerMask
            );

        Collider2D rightHit = Physics2D.OverlapBox(
            rightWallCheckPoint.position, wallCheckSize, 0f, wallLayerMask
            );

        isTouchingLeftWall = leftHit != null;
        isTouchingRightWall = rightHit != null;
    }

    private void OnDrawGizmosSelected()
    {
        if (leftWallCheckPoint == null || rightWallCheckPoint == null) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireCube(leftWallCheckPoint.position, wallCheckSize);
        Gizmos.DrawWireCube(rightWallCheckPoint.position, wallCheckSize);
    }

}
