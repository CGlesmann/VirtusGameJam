using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class MutatedScientist : MonoBehaviour
{
    // Private Enums
    private enum BossStage { Stage1, Stage2, Stage3 };
    private int stageEvents;

    [Header("MiniBoss Stats")]
    public LayerMask playerLayer;
    public UnitStats mbStats;
    public UnitState uState;
    public GameObject player;
    [SerializeField] private BossStage stage;
    [SerializeField] private int eventNum = 0;
    [SerializeField] private bool doingMove = false;

    [Header("Battle Start Variables")]
    public bool battleStarted = false;
    public Vector2 slamPosition;

    [Header("Jump Ability Variables")]
    [SerializeField] private GameObject shadow;
    [SerializeField] private Vector2 shadowOffset;
    [SerializeField] private Vector2 shadowScaleStart;
    [SerializeField] private Vector2 shadowScaleEnd;
    [SerializeField] private bool inAir; // Tracks when jump should be maintained
    [SerializeField] private bool isJumping; // Tracks when jumping up

    [SerializeField] private float slamDamage;
    [SerializeField] private float slamRadius;
    [SerializeField] private float slamKnockback;

    [Header("Melee Routine Variables")]
    [SerializeField] private bool meleeMode = false;
    [SerializeField] private float meleeSpeed = 8f;
    [SerializeField] private Vector2 acceptanceRange = Vector2.one;

    [Header("Dashing Variables")]
    [SerializeField] private bool charging = false;
    [SerializeField] private GameObject dashClone;
    [SerializeField] private float chargeTime;
    [SerializeField] private float chargeSpeed;
    [SerializeField] private float chargeDamage;

    [SerializeField] private float m_attackCooldown;
    private float m_attackTimer = 0f;

    [Header("Special Case Variables")]
    [SerializeField] private Vector2 awayRadius;

    [Header("Starting Routine")]
    public UnityEvent[] startEvents;

    [Header("Phase 1")]
    public UnityEvent[] phase1Events;

    private Enemy enemy;
    private MovementController mController;
    private UnitMovement mControl;
    private Animator anim;

    private void Awake()
    {
        enemy = GetComponent<Enemy>();
        mController = GetComponent<MovementController>();
        anim = GetComponent<Animator>();
        mControl = GetComponent<UnitMovement>();
        uState = new UnitState();

        if (player == null)
            player = GameObject.Find("Player");

        shadow.SetActive(false);

        m_attackTimer = m_attackCooldown;

        stageEvents = phase1Events.Length;
    }

    private void Update()
    {
        if (battleStarted)
        {
            if (doingMove)
                return;

            eventNum++;
            if (eventNum == stageEvents)
                eventNum = 0;
                
            phase1Events[eventNum].Invoke();
            doingMove = true;

            return;
        }
    }

    private void ResetAbilityAnimations()
    {
        anim.SetBool("Jumping", false);
        anim.SetBool("Slamming", false);
        anim.SetBool("Swinging", false);
        anim.SetBool("Moving", false);
    }

	[SerializeField] private UnityEvent _onMutationStart;

    public void StartMutation()
    {
        anim.SetBool("Mutating", true);

		this._onMutationStart.Invoke();
    }

    /// <summary>
    /// Call this to function to start miniboss battle
    /// </summary>
    public void StartBattle()
    {
        // Resetting Triggers
        ResetAbilityAnimations();

        //Invoking the First Ability
        battleStarted = true;
        phase1Events[0].Invoke();
        doingMove = true;
        return;
    }
    
    #region Jump Functions
    public void JumpUp()
    {
        if (!isJumping && !inAir)
        {
            isJumping = true;
            StartCoroutine("JumpingUp");
        } else
            Debug.Log("Trying to jump but cant: " + isJumping.ToString() + " | " + inAir.ToString());
    }

    public float GettingJumpingPosition()
    {
        return ((Camera.main.gameObject.transform.position.y + Camera.main.orthographicSize / 2f) + (transform.localScale.y * 2));
    }

    IEnumerator JumpingUp()
    {
        float yGoal = GettingJumpingPosition();
        float time = 0.15f;
        int reps = 20;
        float inc = (yGoal - transform.position.y) / reps;
        float delay = (time / reps);

        shadow.SetActive(true);
        shadow.transform.position = transform.position + new Vector3(shadowOffset.x, shadowOffset.y);
        shadow.transform.localScale = shadowScaleStart;

        Vector2 shadowScaleInc = (shadowScaleEnd - shadowScaleStart) / reps;

        anim.SetBool("Jumping", true);
        for (int i = 0; i < reps; i++)
        {
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y + inc, transform.position.z);
            transform.position = newPos;

            shadow.transform.localScale += new Vector3(shadowScaleInc.x, shadowScaleInc.y);

            yield return new WaitForSeconds(delay);
        }

        GetComponent<SpriteRenderer>().enabled = false;

        isJumping = false;
        inAir = true;
        StartCoroutine("FloatingInAir");
    }

    IEnumerator FloatingInAir()
    {
        // Declaring Tracking Variables
        float time = 1.5f;
        int reps = 200;
        float delay = time / reps;
        Vector2 floatPos;

        // Repeatedly getting the float position
        for(int i = 0; i < reps; i++)
        {
            floatPos = new Vector2(player.transform.position.x, GettingJumpingPosition());
            transform.position = floatPos;
            shadow.transform.position = player.transform.position + new Vector3(shadowOffset.x, shadowOffset.y);
            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(0.75f);
        StartCoroutine("SlamDown");
    }

    IEnumerator SlamDown()
    {
        float yGoal = shadow.transform.position.y - shadowOffset.y;
        float time = 0.075f;
        int reps = 10;
        float inc = (yGoal - transform.position.y) / reps;
        float delay = (time / reps);

        shadow.transform.localScale = shadowScaleEnd;
        Vector2 shadowScaleInc = (shadowScaleStart - shadowScaleEnd) / reps;
        GetComponent<SpriteRenderer>().enabled = true;

        for (int i = 0; i < reps; i++)
        {
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y + inc, transform.position.z);
            transform.position = newPos;

            shadow.transform.localScale += new Vector3(shadowScaleInc.x, shadowScaleInc.y);

            if ((i / reps) <= 0.25f)
                anim.SetBool("Slamming", true);

            yield return new WaitForSecondsRealtime(0.001f);
        }

        // Checking for a hit
        RaycastHit2D slamHit = Physics2D.CircleCast(shadow.transform.position, slamRadius, Vector2.left, 0f, playerLayer);
        if (slamHit)
        {
            Player p = player.GetComponent<Player>();
            p.manager.playerStats.TakeDamage(player, slamDamage);

            Vector2 dir = new Vector2((player.transform.position - transform.position).x, (player.transform.position - transform.position).y);
            p.ApplyKnockBack(0.05f, dir / dir.magnitude);
        }

        shadow.SetActive(false);
        inAir = false;

        doingMove = false;
    }
    #endregion

    #region Melee Functions
    public void StartMeleeRoutine(float dur)
    {
        anim.SetBool("Moving", true);
        StartCoroutine("MeleeRoutine", dur);
    }

    private IEnumerator MeleeRoutine(float dur)
    {
        // Declaring Tracker Variables
        float timer = dur;
        float speed = meleeSpeed;

        // Repping the Moving Script
        while (timer > 0f)
        {
            // Decrementing the Timer
            timer -= Time.deltaTime;

            // Checking to see if player is in range
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, acceptanceRange, 0f, Vector2.zero, 0f, playerLayer);
            if (hit)
            {
                //Trying Melee Attacks
                if (m_attackTimer <= 0f)
                {
                    anim.SetBool("Swinging", true);
                    yield return new WaitForSeconds(0.75f);
                } else
                {
                    m_attackTimer -= Time.deltaTime;
                }

            } else {
                anim.SetBool("Moving", true);

                // Moving towards the player
                Vector3 pPos = player.transform.position;
                Vector3 moveVector = new Vector3((pPos.x - transform.position.x), (pPos.y - transform.position.y)).normalized;
                moveVector /= moveVector.magnitude;

                mController.Move(moveVector * speed * Time.deltaTime);

                if ((enemy.sTracker.health / mbStats.unitMaxHealth) <= 0.5f)
                {
                    Debug.Log("Checking for another charge");
                    RaycastHit2D away = Physics2D.BoxCast(transform.position, awayRadius, 0f, Vector2.left, 0f, playerLayer);
                    if (!away)
                    {
                        Debug.Log("Chase Charge");
                        ChargePlayer();
                        yield break;
                    }
                }
            }
            
            yield return new WaitForSeconds(0.01f);
        }

        anim.SetBool("Moving", false);
        doingMove = false;
    }

    private void MeleeAttack()
    {
        RaycastHit2D hit = Physics2D.BoxCast(transform.position, acceptanceRange * 1.1f, 0f, Vector2.left, 0f, playerLayer);
        if (hit)
        {
            // Getting the Player Reference
            Player p = player.GetComponent<Player>();

            // Applying the KnockBack
            Vector3 dir = player.transform.position - transform.position;
            dir /= dir.magnitude;

            // Applying the Damage
            p.manager.playerStats.TakeDamage(player, mbStats.unitDamage);

            // Setting the Attack Timer
            m_attackTimer = m_attackCooldown;
        }

        anim.SetBool("Swinging", false);
    }
    #endregion

    #region Idle Functions
    public void Idle(float dir)
    {
        StartCoroutine("StandIdle", dir);

        // Resetting Triggers
        ResetAbilityAnimations();
    }

    private IEnumerator StandIdle(float dir)
    {
        yield return new WaitForSeconds(dir);
        doingMove = false;
    }
    #endregion

    #region Charge Function
    public void ChargePlayer()
    {
        if (!charging)
        {
            // Starting the Charging Corroutine
            StartCoroutine("ChargeShake", 1.5f);

            // Resetting Triggers
            ResetAbilityAnimations();
        }
    }

    IEnumerator ChargeShake(float dur)
    {
        Vector3 startPos = transform.position;
        float timer = dur;

        while (timer > 0f)
        {
            timer -= Time.deltaTime;
            transform.localPosition = startPos + (Random.insideUnitSphere.normalized * 0.1f);

            yield return new WaitForSeconds(0.001f);
        }

        transform.position = startPos;
        StartCoroutine("Charge");
    }

    IEnumerator Charge()
    {
        charging = true;
        Vector2 dir = (player.transform.position - transform.position).normalized;
        bool pHit = false;

        float length = 0.25f;
        int reps = 10;
        float delay = (length / reps);

        float inc = this.chargeSpeed / reps;

        for (int i = 0; i < reps; i++)
        {
            // Moving
            mController.Move(dir * inc);

            // Creating Dash Effect
            GameObject newClone = Instantiate(dashClone);
            newClone.transform.position = transform.position;

            // Checking for a collision
            if (!pHit)
            {
                RaycastHit2D hit = Physics2D.BoxCast(transform.position, Vector2.one * slamRadius, 0f, Vector2.left, 0f, playerLayer);
                if (hit)
                {
                    // Damage the Player
                    Player p = player.GetComponent<Player>();
                    p.manager.playerStats.TakeDamage(player, chargeDamage);

                    // Stunning the Player
                    p.pState.StunUnit(1.5f);

                    // Marking the Hit
                    pHit = true;
                }
            }

            yield return new WaitForSeconds(delay);
        }

        charging = false;
        doingMove = false;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        // Drawing the Slam Radius
        Gizmos.color = Color.blue;
        //Gizmos.DrawWireSphere(shadow.transform.position, slamRadius);
        //Gizmos.DrawWireCube(transform.position, acceptanceRange);
        Gizmos.DrawWireCube(transform.position, awayRadius);
    }

}