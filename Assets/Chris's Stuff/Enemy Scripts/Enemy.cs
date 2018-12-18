using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MovementController))]
public class Enemy : MonoBehaviour
{
    // Private Enums
    private enum PhaseStyle { Top, Bottom, Left, Right, Behind, Front };

    [Header("Enemy Stats Reference")]
    public UnitStats stats;
    public float attackRange = 0.5f;
    public float attackCooldown = 0.5f;

    private float attackTimer = 0f;

    [Header("Movement Variables")]
    public LayerMask wallLayer;
    public LayerMask playerLayer;

    [Header("Wander Variables")]
    public float wanderSpeed = 4f;
    public bool canWander = true;
    public Vector3 wanderRange;

    [SerializeField] private Vector3 wanderOrigin;
    [SerializeField] private Vector3 wanderPoint = Vector3.zero;
    [SerializeField] private float acceptanceRange;
    [SerializeField] private float wanderDelay = 1.5f;

    private float wanderDelayTimer = 0f;

    [Header("Chasing Variables")]
    public float chaseSpeed = 6f;
    public Vector3 sightRange;
    public Vector3 chaseRange;

    public bool isChasing = false;

    private MovementController controller;
    private GameObject target = null;

    private void Awake()
    {
        controller = GetComponent<MovementController>();

        // Creating a new reference to UnitStats
        stats = new UnitStats(2f, 1f);
    }

    private void Update()
    {
        if (!isChasing)
        {
            // Try getting a target
            target = GetTarget();
            if (target != null)
            {
                isChasing = true;
                return;
            }

            // Else wander around
            WanderState();
        }
        else
        {
            // Checking to make sure its still in range
            if (!InRange())
            {
                isChasing = false;
                target = null;
            }

            if (target == null)
            {
                isChasing = false;
                return;
            }

            float dist = (target.transform.position - transform.position).magnitude;
            if (dist <= attackRange)
            {
                if (attackTimer <= 0f)
                {
                    Player player = this.target.GetComponent<Player>();
                    if (player != null && !player.GetComponent<UnitMovement>().isDashing)
                    {
                        player.stats.TakeDamage(player.gameObject, stats.unitDamage);
                        attackTimer = attackCooldown;

                        if (player == null)
                        {
                            target = null;
                            isChasing = false;
                        } 
                    }
                }
            }

            
        }

        if (attackTimer > 0f)
            attackTimer -= Time.deltaTime;
    }

    private void FixedUpdate()
    {
        if (isChasing)
        {
            // Get the movement Vector
            float dist = (target.transform.position - transform.position).magnitude;
            if (dist > attackRange)
            {
                Vector2 dir = (target.transform.position - transform.position) / dist;

                controller.Move(dir * chaseSpeed * Time.deltaTime);
            }
        }
    }

    private void WanderState()
    {
        // Getting a point to wander to if none is chosen
        if (wanderDelayTimer > 0f)
        {
            wanderDelayTimer -= Time.deltaTime;
            return;
        }

        // Assigning a new Wander Point
        if (wanderPoint == Vector3.zero)
        {
            float x = Random.Range(wanderOrigin.x - (wanderRange.x / 2), wanderOrigin.x + (wanderRange.x / 2));
            float y = Random.Range(wanderOrigin.y - (wanderRange.y / 2), wanderOrigin.y + (wanderRange.y / 2));
            wanderPoint = new Vector3(x, y, 0f);

            RaycastHit2D h = Physics2D.BoxCast(wanderPoint, Vector2.one, 0f, Vector3.left, 0f, wallLayer);
            if (h)
            {
                Vector3 p = h.point;
                if (Vector3.Distance(wanderPoint, p) > acceptanceRange)
                {
                    wanderPoint = Vector3.zero;
                    return;
                }
            }
        }

        // Checking to make sure enemy can reatch wanderPoint
        float dist = (wanderPoint - transform.position).magnitude;
        Vector3 dir = (wanderPoint - transform.position) / dist;
        RaycastHit2D hit = Physics2D.Raycast(transform.position, dir, dist, wallLayer);

        if (hit) {
            // Check to Make sure we are in wanderZone
            if (InWanderZone())
            {
                wanderPoint = Vector3.zero;
                return;
            }

            if (!InWanderZone())
            {
                wanderOrigin = transform.position;
                wanderPoint = Vector3.zero;

                return;
            }
        }

        if (dist != 0f)
        {
            controller.Move(dir * wanderSpeed * Time.deltaTime);
        }

        // Checking the Distance to the wander point
        if (Vector3.Distance(transform.position, wanderPoint) <= acceptanceRange)
        {
            wanderPoint = Vector3.zero;
            wanderDelayTimer = wanderDelay;
        }
    }

    private bool InWanderZone()
    {
        Vector3 topLeft = new Vector3(wanderOrigin.x - (wanderRange.x / 2), wanderOrigin.y - (wanderRange.y / 2), 0f);
        Vector3 botRight = new Vector3(wanderOrigin.x + (wanderRange.x / 2), wanderOrigin.y + (wanderRange.y / 2), 0f);

        return (transform.position.x >= topLeft.x && transform.position.x <= botRight.x && transform.position.y >= botRight.y && transform.position.y <= topLeft.y);
    }

    private void Phase(PhaseStyle style)
    {
        // Get the Point to phase to
        Vector3 phasePoint = Vector3.zero;
        GameObject player = GameObject.Find("Player");
        Vector3 phaseVelocity;

        switch (style)
        {
            case PhaseStyle.Front:
                phaseVelocity = player.GetComponent<UnitMovement>().lastVelocity;
                phasePoint = player.transform.position + -phaseVelocity;
                phasePoint = new Vector3(phasePoint.x + ((phaseVelocity.x != 0) ? (Mathf.Sign(phaseVelocity.x) * 1.5f) : 0f),
                                         phasePoint.y + ((phaseVelocity.y != 0) ? (Mathf.Sign(phaseVelocity.y) * 1.5f) : 0f),
                                         0f);
                break;

            case PhaseStyle.Behind:
                phaseVelocity = player.GetComponent<UnitMovement>().lastVelocity;
                phasePoint = player.transform.position + phaseVelocity;
                phasePoint = new Vector3(phasePoint.x + ((phaseVelocity.x != 0) ? (Mathf.Sign(-phaseVelocity.x) * 1.5f) : 0f),
                                         phasePoint.y + ((phaseVelocity.y != 0) ? (Mathf.Sign(-phaseVelocity.y) * 1.5f) : 0f),
                                         0f);
                break;

            case PhaseStyle.Top:
                phasePoint = player.transform.position + Vector3.up;
                break;
            case PhaseStyle.Bottom:
                phasePoint = player.transform.position + Vector3.down;
                break;
            case PhaseStyle.Left:
                phasePoint = player.transform.position + Vector3.left;
                break;
            case PhaseStyle.Right:
                phasePoint = player.transform.position + Vector3.right;
                break;
        }

        RaycastHit2D hit = Physics2D.BoxCast(phasePoint, transform.localScale, 0f, Vector3.left, 0f, wallLayer);
        if (hit)
            return;

        transform.position = phasePoint;
    }

    private GameObject GetTarget()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, (!isChasing) ? sightRange : chaseRange, 0f, Vector2.zero, 0f, playerLayer);
        if (hit)
        {
            GameObject player = hit.collider.gameObject;
            float dist = (player.transform.position - transform.position).magnitude;
            Vector3 dir = (player.transform.position - transform.position) / dist;
            RaycastHit2D wall = Physics2D.Raycast(transform.position, dir, dist, wallLayer);

            if (!wall)
            {
                return player;
            }
        }

        return null;
    }

    private bool InRange()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, (!isChasing) ? sightRange : chaseRange, 0f, Vector2.zero, 0f, playerLayer);
        if (hit)
        {
            GameObject player = hit.collider.gameObject;
            float dist = (player.transform.position - transform.position).magnitude;
            Vector3 dir = (player.transform.position - transform.position) / dist;
            RaycastHit2D wall = Physics2D.Raycast(transform.position, dir, dist, wallLayer);

            if (!wall)
            {
                return true;
            }
        }

        return false;
    }

    IEnumerator HurtFlash()
    {
        // Getting the SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color baseColor = sr.color;
        Color flashColor = Color.black;
        int flashReps = 6;
        float delay = 0.05f;

        for (int i = 0; i < flashReps; i++)
        {
            sr.color = (i % 2 == 0) ? flashColor : baseColor;
            yield return new WaitForSeconds(delay);
        }
    }

    private void OnDrawGizmos()
    {
        // Drawing the Range
        Gizmos.color = (isChasing)?Color.red:Color.black;
        Gizmos.DrawWireCube(transform.position, (isChasing) ? chaseRange : sightRange);

        // Drawing the Wander Zone
        Gizmos.color = new Color(255f, 255f, 0f, 0.25f);
        Gizmos.DrawCube(wanderOrigin, wanderRange);

        if (wanderPoint != Vector3.zero)
        {
            Gizmos.color = Color.black;
            Gizmos.DrawCube(wanderPoint, Vector3.one * 2f);
        }
    }
}
