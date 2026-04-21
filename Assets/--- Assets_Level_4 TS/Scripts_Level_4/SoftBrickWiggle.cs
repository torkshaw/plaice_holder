using UnityEngine;

/// <summary>
/// Small, cheap "soft brick" wobble:
/// - Off by default unless startEnabled is true
/// - Optionally only runs while the object is inside the camera view
///   (with a configurable viewport margin)
/// - Uses localPosition so it plays nicely with moving parents
/// </summary>
public class SoftBrickWiggle : MonoBehaviour
{
    [Header("Wiggle Shape")]
    [SerializeField, Tooltip("How far to move from the original local position (X/Y) in world units.")]
    private Vector2 amplitude = new Vector2(0.05f, 0.05f);

    [SerializeField, Tooltip("How fast the wiggle cycles (higher = faster).")]
    private float frequency = 1.5f;

    [SerializeField, Tooltip("Base phase offset for this brick's wiggle.")]
    private float phaseOffset = 0f;

    [SerializeField, Tooltip("Randomise phase per brick so they don't all move in sync.")]
    private bool randomisePhase = true;

    [Header("Activation")]
    [SerializeField, Tooltip("If true, this brick starts wiggling immediately (subject to camera culling).")]
    private bool startEnabled = true;

    [SerializeField, Tooltip("If true, wiggle only runs while the brick is visible (plus a small viewport margin).")]
    private bool cullByCamera = true;

    [SerializeField, Tooltip("Extra viewport margin outside [0,1] where wiggle still runs. " +
                             "E.g. 0.1 = wiggle while between -0.1 and 1.1 in X/Y.")]
    private float viewportMargin = 0.1f;

    private Camera mainCam;
    private Vector3 baseLocalPosition;
    private bool wiggleEnabled;

    private void Awake()
    {
        baseLocalPosition = transform.localPosition;
        mainCam = Camera.main;

        if (randomisePhase)
        {
            phaseOffset += Random.Range(0f, Mathf.PI * 2f);
        }

        wiggleEnabled = startEnabled;
    }

    private void LateUpdate()
    {
        // If wiggle is fully disabled, snap back to base and do nothing.
        if (!wiggleEnabled)
        {
            transform.localPosition = baseLocalPosition;
            return;
        }

        // Optional camera-based culling
        if (cullByCamera && mainCam != null)
        {
            Vector3 vp = mainCam.WorldToViewportPoint(transform.position);

            // vp.z < 0 means "behind the camera"
            if (vp.z < 0f ||
                vp.x < -viewportMargin || vp.x > 1f + viewportMargin ||
                vp.y < -viewportMargin || vp.y > 1f + viewportMargin)
            {
                // Off-screen: reset to base and skip wiggle this frame
                transform.localPosition = baseLocalPosition;
                return;
            }
        }

        // Actually do the wiggle
        float t = Time.time * frequency + phaseOffset;

        float xOffset = Mathf.Sin(t) * amplitude.x;
        float yOffset = Mathf.Cos(t * 1.1f) * amplitude.y;  // 1.1 just so X/Y aren't perfectly synced

        transform.localPosition = baseLocalPosition + new Vector3(xOffset, yOffset, 0f);
    }

    // ------------------------------------------------
    // Public controls (optional, for use by other scripts)
    // ------------------------------------------------

    public void EnableWiggle()
    {
        wiggleEnabled = true;
    }

    public void DisableWiggle()
    {
        wiggleEnabled = false;
        transform.localPosition = baseLocalPosition;
    }
}
