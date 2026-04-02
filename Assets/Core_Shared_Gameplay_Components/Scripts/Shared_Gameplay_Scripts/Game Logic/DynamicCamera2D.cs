using UnityEngine;

// TS
// Much of this script came from an upgraded version of a tutorial script. This script came from 
// my previous project (scrolling shooter). 
// it is WAY more comprehensive than we need for this project but gave us options to play with when
// we were prototypeing. 

public class DynamicCamera2D : MonoBehaviour
{
    public enum CameraMode
    {
        Stationary,
        AutoScroll,
        PlayerFollow
    }

    [Header("General")]
    [SerializeField] private CameraMode mode = CameraMode.AutoScroll;
    [SerializeField, Tooltip("Usually the player. Needed for AutoScroll push + PlayerFollow.")]
    private Transform followTarget;
    [SerializeField] private Vector2 followOffset = Vector2.zero;

    [Header("Auto Scroll (Rail)")]
    [SerializeField, Tooltip("Direction of the rail. e.g. (1,0)=right, (0,1)=up, (1,1)=diagonal.")]
    private Vector2 scrollDirection = Vector2.right;
    [SerializeField, Tooltip("Speed along the rail in units/second.")]
    private float scrollSpeed = 2f;
    [SerializeField, Tooltip("Allow the player to 'push' the camera forward if they get ahead on the rail.")]
    private bool allowPlayerPush = true;
    [SerializeField, Tooltip("How far ahead along the rail the player can get before the camera moves faster.")]
    private float forwardDeadzone = 3f;
    [SerializeField, Tooltip("Maximum forward speed (rail + push). 0 or negative = unlimited.")]
    private float maxForwardSpeed = 8f;
    [SerializeField, Tooltip("If true, once the camera has moved forward, it will not move backwards along the rail.")]
    private bool preventReverseOnRail = true;

    [Header("Player Follow Mode (Deadzone Box)")]
    [SerializeField] private float deadZoneX = 2f;
    [SerializeField] private float deadZoneY = 1f;
    [SerializeField, Tooltip("Vertical smoothing time in seconds. 0 = no smoothing.")]
    private float verticalSmoothTime = 0.15f;
    [SerializeField, Tooltip("Horizontal smoothing time in seconds. 0 = instant.")]
    private float horizontalSmoothTime = 0.1f;

    [Header("Directional Locks in Player Follow")]
    [SerializeField, Tooltip("Disallow moving camera backwards in X once it has moved forward.")]
    private bool lockBackwardsX = false;
    [SerializeField, Tooltip("Disallow moving camera left beyond its starting X (even before it moves forward).")]
    private bool hardLockLeft = false;
    [SerializeField, Tooltip("Disallow moving camera right beyond its starting X.")]
    private bool hardLockRight = false;
    [SerializeField, Tooltip("Disallow moving camera down beyond its starting Y.")]
    private bool hardLockDown = false;
    [SerializeField, Tooltip("Disallow moving camera up beyond its starting Y.")]
    private bool hardLockUp = false;

    // Internal state
    private Vector2 _scrollDirNormalized = Vector2.right;
    private float _railFurthestAlong = 0f;  // furthest distance along rail we've reached
    private float _initialX, _initialY, _initialZ;
    private float _maxXReached;             // for lockBackwardsX
    private float _verticalVelocity;
    private float _horizontalVelocity;

    private void Awake()
    {
        Vector3 p = transform.position;
        _initialX = p.x;
        _initialY = p.y;
        _initialZ = p.z;

        _maxXReached = _initialX;
        RecalculateScrollDirection();
    }

    private void LateUpdate()
    {
        float dt = Time.deltaTime;
        if (dt <= 0f) return;

        Vector3 currentPos = transform.position;
        Vector3 targetPos = currentPos;

        switch (mode)
        {
            case CameraMode.Stationary:
                // Do nothing automatically; stays wherever you last placed it.
                targetPos = currentPos;
                break;

            case CameraMode.AutoScroll:
                targetPos = HandleAutoScroll(currentPos, dt);
                break;

            case CameraMode.PlayerFollow:
                targetPos = HandlePlayerFollow(currentPos, dt);
                break;
        }

        // Lock Z
        targetPos.z = _initialZ;
        transform.position = targetPos;
    }

    // -----------------------------
    // AUTO-SCROLL (RAIL) LOGIC
    // -----------------------------
    private Vector3 HandleAutoScroll(Vector3 currentPos, float dt)
    {
        if (_scrollDirNormalized.sqrMagnitude < 0.0001f)
        {
            RecalculateScrollDirection();
        }

        Vector2 camPos2D = currentPos;
        Vector2 dir = _scrollDirNormalized;

        // Baseline rail movement
        Vector2 desiredPos2D = camPos2D + dir * scrollSpeed * dt;

        // Player push along the rail (like your old script but generalized)
        if (allowPlayerPush && followTarget != null)
        {
            Vector2 playerPos2D = (Vector2)followTarget.position + followOffset;
            Vector2 camToPlayer = playerPos2D - camPos2D;

            // How far along the rail axis is the player, relative to the camera?
            float distAlongRail = Vector2.Dot(camToPlayer, dir);

            if (distAlongRail > forwardDeadzone)
            {
                float extra = distAlongRail - forwardDeadzone;
                desiredPos2D += dir * extra;
            }
        }

        // Optional: clamp forward speed (rail-space only)
        if (maxForwardSpeed > 0f)
        {
            Vector2 delta = desiredPos2D - camPos2D;
            float forwardAmount = Vector2.Dot(delta, dir);

            if (forwardAmount > 0f)
            {
                float maxForwardDelta = maxForwardSpeed * dt;
                if (forwardAmount > maxForwardDelta)
                {
                    // Remove the excess forward component
                    float excess = forwardAmount - maxForwardDelta;
                    delta -= dir * excess;
                    desiredPos2D = camPos2D + delta;
                }
            }
        }

        // Optional: prevent backwards along the rail
        if (preventReverseOnRail)
        {
            float currentAlong = Vector2.Dot(camPos2D, dir);
            float desiredAlong = Vector2.Dot(desiredPos2D, dir);

            if (desiredAlong < _railFurthestAlong)
            {
                // Clamp back to furthest
                float correction = _railFurthestAlong - desiredAlong;
                desiredPos2D += dir * correction;
            }
            else
            {
                _railFurthestAlong = desiredAlong;
            }
        }

        return new Vector3(desiredPos2D.x, desiredPos2D.y, currentPos.z);
    }

    // -----------------------------
    // PLAYER-FOLLOW (DEADZONE BOX)
    // -----------------------------
    private Vector3 HandlePlayerFollow(Vector3 currentPos, float dt)
    {
        if (followTarget == null)
        {
            // No target: do nothing
            return currentPos;
        }

        Vector3 desired = currentPos;
        Vector3 targetWorld = followTarget.position + (Vector3)followOffset;

        float dx = targetWorld.x - currentPos.x;
        float dy = targetWorld.y - currentPos.y;

        // Horizontal deadzone
        if (Mathf.Abs(dx) > deadZoneX)
        {
            float moveX = dx - Mathf.Sign(dx) * deadZoneX;

            if (horizontalSmoothTime > 0f)
            {
                float targetX = currentPos.x + moveX;
                float newX = Mathf.SmoothDamp(
                    currentPos.x,
                    targetX,
                    ref _horizontalVelocity,
                    horizontalSmoothTime
                );
                desired.x = newX;
            }
            else
            {
                desired.x += moveX;
            }
        }

        // Vertical deadzone
        if (Mathf.Abs(dy) > deadZoneY)
        {
            float moveY = dy - Mathf.Sign(dy) * deadZoneY;

            if (verticalSmoothTime > 0f)
            {
                float targetY = currentPos.y + moveY;
                float newY = Mathf.SmoothDamp(
                    currentPos.y,
                    targetY,
                    ref _verticalVelocity,
                    verticalSmoothTime
                );
                desired.y = newY;
            }
            else
            {
                desired.y += moveY;
            }
        }

        // Directional locks (X)
        if (hardLockLeft && desired.x < _initialX)
            desired.x = _initialX;
        if (hardLockRight && desired.x > _initialX)
            desired.x = _initialX;

        // Mario-style "no going back" in follow mode
        if (lockBackwardsX)
        {
            if (desired.x < _maxXReached)
            {
                desired.x = _maxXReached;
            }
            else
            {
                _maxXReached = desired.x;
            }
        }

        // Directional locks (Y)
        if (hardLockDown && desired.y < _initialY)
            desired.y = _initialY;
        if (hardLockUp && desired.y > _initialY)
            desired.y = _initialY;

        return desired;
    }

    private void RecalculateScrollDirection()
    {
        if (scrollDirection.sqrMagnitude < 0.0001f)
        {
            scrollDirection = Vector2.right;
        }
        _scrollDirNormalized = scrollDirection.normalized;

        // Initialize rail furthest distance
        Vector2 camPos2D = transform.position;
        _railFurthestAlong = Vector2.Dot(camPos2D, _scrollDirNormalized);
    }


    // -----------------------------
    // RESPAWN / RESET SUPPORT
    // -----------------------------
    public void ResetToPosition(Vector3 worldPosition)
    {
        // Snap camera immediately
        transform.position = new Vector3(
            worldPosition.x,
            worldPosition.y,
            _initialZ
        );

        // Reset internal forward locks
        _maxXReached = worldPosition.x;

        // Reset rail progress so "no reverse" doesn't block us
        _railFurthestAlong = Vector2.Dot(
            new Vector2(worldPosition.x, worldPosition.y),
            _scrollDirNormalized
        );

        // Reset smoothing velocities to avoid post-respawn drift
        _horizontalVelocity = 0f;
        _verticalVelocity = 0f;
    }


    // -----------------------------
    // PUBLIC API FOR OTHER SCRIPTS
    // -----------------------------

    public void SetMode(CameraMode newMode)
    {
        mode = newMode;
    }

    public void ConfigureAutoScroll(Vector2 direction, float speed, float newForwardDeadzone, bool allowPush = true)
    {
        scrollDirection = direction;
        scrollSpeed = speed;
        forwardDeadzone = newForwardDeadzone;
        allowPlayerPush = allowPush;
        RecalculateScrollDirection();
    }

    public void ConfigurePlayerFollow(float dzX, float dzY, bool noBackwardsX)
    {
        deadZoneX = dzX;
        deadZoneY = dzY;
        lockBackwardsX = noBackwardsX;
    }

    public void SetFollowTarget(Transform t)
    {
        followTarget = t;
    }

    // -----------------------------
    // GIZMOS
    // -----------------------------
    private void OnDrawGizmosSelected()
    {
        // Draw follow deadzone (PlayerFollow mode)
        if (mode == CameraMode.PlayerFollow)
        {
            Gizmos.color = Color.cyan;
            Vector3 p = Application.isPlaying ? transform.position : transform.position;
            Vector3 size = new Vector3(deadZoneX * 2f, deadZoneY * 2f, 0f);
            Gizmos.DrawWireCube(p, size);
        }

        // Draw rail direction + forward deadzone (AutoScroll)
        if (mode == CameraMode.AutoScroll)
        {
            Gizmos.color = Color.yellow;
            Vector3 p = transform.position;
            Vector2 dir = (_scrollDirNormalized.sqrMagnitude > 0.0001f) ? _scrollDirNormalized : scrollDirection.normalized;
            if (dir.sqrMagnitude < 0.0001f) dir = Vector2.right;

            // Direction arrow
            Vector3 end = p + (Vector3)(dir * 3f);
            Gizmos.DrawLine(p, end);
            Gizmos.DrawSphere(end, 0.1f);

            // Forward deadzone marker
            Vector3 deadzonePoint = p + (Vector3)(dir * forwardDeadzone);
            Gizmos.DrawWireSphere(deadzonePoint, 0.1f);
        }
    }
}
