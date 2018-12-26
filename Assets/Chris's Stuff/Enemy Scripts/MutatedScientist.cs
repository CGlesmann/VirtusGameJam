using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEditor;

public class MutatedScientist : MonoBehaviour
{
    // Private Enums
    private enum BossStage { Stage1, Stage2, Stage3 };

    [Header("MiniBoss Stats")]
    public LayerMask playerLayer;
    public UnitStats mbStats;
    public UnitState uState;
    public GameObject player;
    [SerializeField] private BossStage stage;

    [Header("Battle Start Variables")]
    public bool battleStarted = false;
    public Vector2 slamPosition;

    [Header("Jump Ability Variables")]
    // Jumping Variables
    [SerializeField] private GameObject shadow;
    [SerializeField] private Vector2 shadowOffset;
    [SerializeField] private Vector2 shadowScaleStart;
    [SerializeField] private Vector2 shadowScaleEnd;
    [SerializeField] private bool inAir; // Tracks when jump should be maintained
    [SerializeField] private bool isJumping; // Tracks when jumping up

    [SerializeField] private float slamDamage;
    [SerializeField] private Vector2 slamRadius;
    [SerializeField] private float slamKnockback;

    [Header("Starting Routine")]
    public UnityEvent[] startEvents;

    [Header("Phase 1")]
    public UnityEvent[] phase1Events;

    private UnitMovement mControl;

    private void Awake()
    {
        mControl = GetComponent<UnitMovement>();
        uState = new UnitState();

        if (player == null)
            player = GameObject.Find("Player");

        shadow.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
            if (!inAir && !isJumping)
                JumpUp();
    }

    #region Start Functions
    /// <summary>
    /// Call this to function to start miniboss battle
    /// </summary>
    public void StartBattle()
    {
        StartCoroutine("StartBattleRoutine");
        return;
    }

    /// <summary>
    /// Runs through the StartRoutine List
    /// </summary>
    /// <returns></returns>
    IEnumerator StartBattleRoutine()
    {
        // Declaring the tracker variables
        float delay = 2f; // In Seconds

        // Running Through Each Event
        for(int i = 0; i < startEvents.Length; i++)
        {
            startEvents[i].Invoke();
            yield return new WaitForSeconds(delay);
        }

        battleStarted = true;
    }
    #endregion
    
    #region Ability Functions
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

        for (int i = 0; i < reps; i++)
        {
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y + inc, transform.position.z);
            transform.position = newPos;

            shadow.transform.localScale += new Vector3(shadowScaleInc.x, shadowScaleInc.y);

            yield return new WaitForSeconds(delay);
        }

        yield return new WaitForSeconds(1f);

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

        Debug.Log(delay);

        shadow.transform.localScale = shadowScaleEnd;
        Vector2 shadowScaleInc = (shadowScaleStart - shadowScaleEnd) / reps;

        for (int i = 0; i < reps; i++)
        {
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y + inc, transform.position.z);
            transform.position = newPos;

            shadow.transform.localScale += new Vector3(shadowScaleInc.x, shadowScaleInc.y);

            yield return new WaitForSecondsRealtime(0.001f);
        }

        // Checking for a hit
        RaycastHit2D slamHit = Physics2D.CapsuleCast(shadow.transform.position, slamRadius, CapsuleDirection2D.Horizontal, 0f, Vector2.left, 0f, playerLayer);
        if (slamHit)
        {
            Player p = player.GetComponent<Player>();
            p.manager.playerStats.TakeDamage(player, slamDamage);

            Vector2 dir = new Vector2((player.transform.position - transform.position).x, (player.transform.position - transform.position).y);
            p.ApplyKnockBack(0.05f, dir / dir.magnitude);
        }

        shadow.SetActive(false);
        inAir = false;
    }
    #endregion

    private void OnDrawGizmosSelected()
    {
        // Drawing the Slam Radius
        Gizmos.color = Color.blue;
        //Gizmos.DrawWireCube(shadow.transform.position, new Vector3(slamRadius.x, slamRadius.y));
    }

}

[CustomEditor(typeof(MutatedScientist))]
public class BossInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        MutatedScientist s = (MutatedScientist)target;

        if (GUILayout.Button("Jump Up Position"))
        {
            Debug.Log(s.GettingJumpingPosition());
        }
    }
}