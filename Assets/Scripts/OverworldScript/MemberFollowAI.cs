using System.Collections.Generic;
using System.Collections;
using UnityEngine;


public class MemberFollowAI : MonoBehaviour
{

    [SerializeField] private float holdExitDelay; // Seconds to idle after jump off
    private float holdExitTimer = 0f;
    private bool wasHoldingPlayer = false;
    private bool isHoldingPlayer = false;
    private bool wasOnTopTriggered = false;

    [SerializeField] private Transform player;
    [SerializeField] private float holdDetectionHeight = 1.2f;
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private Transform followTarget;
    [SerializeField] private int speed;
    [SerializeField] private float maxTeleportDistance = 5f;
    [SerializeField] private float teleportDelay = 3f;

    private float followDist;
    private Animator anim;
    private SpriteRenderer sprite;
    private Vector3 scale;

    private float teleportTimer = 0f;
    private bool isCountingDown = false;

    private const string IS_WALK_PARAM = "IsWalk";
    private const string HOLDING_PLAYER = "Hold";

    private Rigidbody rb;
    private void Awake()
    {
        scale = transform.localScale;
        rb = GetComponent<Rigidbody>();

        followTarget = GameObject.FindFirstObjectByType<PlayerController>().transform;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        anim = gameObject.GetComponent<Animator>();
        sprite = gameObject.GetComponent<SpriteRenderer>();

        followTarget = GameObject.FindFirstObjectByType<PlayerController>().transform;
    }

    // private IEnumerator RotateCharacter(float targetYRotation)
    // {
    //     float duration = 0.2f; // Adjust rotation speed
    //     float elapsedTime = 0;
    //     Quaternion startRotation = transform.rotation;
    //     Quaternion targetRotation = Quaternion.Euler(0, targetYRotation, 0);

    //     while (elapsedTime < duration)
    //     {
    //         transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / duration);
    //         elapsedTime += Time.deltaTime;
    //         yield return null;
    //     }

    //     transform.rotation = targetRotation;
    // }


    // Update is called once per frame
    void FixedUpdate()
    {
        HandleFollower();
        FollowerTeleport();
        CheckHoldPlayer();
    }

    private void FollowerTeleport()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, followTarget.position);

        // Start countdown if too far
        if (distanceToPlayer > maxTeleportDistance)
        {
            if (!isCountingDown)
            {
                isCountingDown = true;
                teleportTimer = teleportDelay;
            }
        }
        else
        {
            // Reset countdown if close again
            isCountingDown = false;
            teleportTimer = 0f;
        }

        // Count down and teleport
        if (isCountingDown)
        {
            teleportTimer -= Time.deltaTime;
            if (teleportTimer <= 0f)
            {
                TeleportToPlayer();
                isCountingDown = false;
            }
        }
    }

    private void HandleFollower()
    {
        float distanceToPlayer = Vector3.Distance(transform.position, followTarget.position);

        if (followTarget.position.y - transform.position.y > 2f)
        {
            // If the player is 3 units higher, stop the follower from walking
            anim.SetBool(IS_WALK_PARAM, false);
            return; // Exit early and do not proceed with the walking logic
        }

        // Only pause after player has jumped OFF (not when jumped nearby or before they were on top)
        if (holdExitTimer > 0f)
        {
            holdExitTimer -= Time.deltaTime;
            anim.SetBool(IS_WALK_PARAM, false);
            return;
        }

        if (distanceToPlayer > maxTeleportDistance)
        {
            anim.SetBool(IS_WALK_PARAM, false);
            return;
        }

        // Walk logic
        if (Vector3.Distance(transform.position, followTarget.position) > followDist + 1)
        {
            anim.SetBool(IS_WALK_PARAM, true);
            float step = speed * Time.deltaTime;
            Vector3 targetPosition = new Vector3(followTarget.position.x, transform.position.y, followTarget.position.z);
            transform.position = Vector3.MoveTowards(transform.position, targetPosition, step);

            if (followTarget.position.x - transform.position.x < 0)
            {
                sprite.transform.localScale = new Vector3(scale.x, scale.y, scale.z);
            }
            else
            {
                sprite.transform.localScale = new Vector3(-scale.x, scale.y, scale.z);
            }
        }
        else
        {
            anim.SetBool(IS_WALK_PARAM, false);
        }
    }


    private void CheckHoldPlayer()
    {
        Vector3 detectionOrigin = transform.position + Vector3.up * holdDetectionHeight;
        Collider[] hits = Physics.OverlapSphere(detectionOrigin, 0.3f, playerLayer);

        isHoldingPlayer = false;

        foreach (var hit in hits)
        {
            if (hit.CompareTag("Player"))
            {
                isHoldingPlayer = true;
                break;
            }
        }

        anim.SetBool(HOLDING_PLAYER, isHoldingPlayer);

        // Detect jump-off AFTER being on top
        if (wasHoldingPlayer && !isHoldingPlayer && wasOnTopTriggered)
        {
            holdExitTimer = holdExitDelay;
            wasOnTopTriggered = false;
        }

        // Mark when player is on top for the first time
        if (isHoldingPlayer)
        {
            wasOnTopTriggered = true;
        }

        wasHoldingPlayer = isHoldingPlayer;

        if (isHoldingPlayer)
        {
            rb.isKinematic = true; // Enable kinematic when holding player
        }
        else
        {
            rb.isKinematic = false; // Disable kinematic when not holding player
        }
    }


    public void SetFollowDistance(float _followDistance)
    {
        followDist = _followDistance;
    }

    private void TeleportToPlayer()
    {
        transform.position = followTarget.position;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Vector3 detectionOrigin = transform.position + Vector3.up * holdDetectionHeight;
        Gizmos.DrawWireSphere(detectionOrigin, 0.3f);
    }

}
