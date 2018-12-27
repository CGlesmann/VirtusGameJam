using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Player : MonoBehaviour {

    // Private Enums
    private enum AttackStyle { Horizontal, Vertical };

    [Header("Stats Reference")]
    public LevelManager manager;
    public UnitState pState = new UnitState();

    [Header("GUI References")]
    public HealthBarView playerHealthBar;

    [Header("Player Combat Variables")]
    public LayerMask enemyLayer;
    public LayerMask wallLayer;

    [Header("Player Melee Variables")]
    public bool checkingForNextAttack = false;
    public bool attackDetermined = false;
    public bool attackSuccessful = false;
    public float meleeAttackCooldown;
    public Vector3 attackAreaOffset;
    public Vector3 attackArea;

    [SerializeField] private float length = 2.5f;
    [SerializeField] private float width = 1f;

    [SerializeField] private float srWidthAspect = 1f;
    [SerializeField] private float srHeightAspect = 1f;

    private float meleeAttackTimer = 0f;

    [Header("Player Range Variables")]
    public float rangeAttackCooldown;
    public GameObject rangePrefab;

    private float rangeAttackTimer = 0f;

    [Header("Input Variables")]
    public KeyCode attackKey;

    [Header("Animation Variables")]
    [SerializeField] private bool moving = false;
    [SerializeField] private bool attacking = false;
    [SerializeField] private int attackCount = 0;

    private UnitMovement playerMovement;
    private Animator anim;

    /// <summary>
    /// Getting References
    /// </summary>
    private void Awake()
    {
        playerMovement = GetComponent<UnitMovement>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Checking for Attack Input
        if (Input.GetMouseButtonDown(0) && !attacking)
            MeleeAttack();

        if (Input.GetMouseButtonDown(1))
            TryRangeAttack();

        // Updating the GUI
        UpdatePlayerGUIElements();

        // Getting Attack Input
        if (checkingForNextAttack)
            if (Input.GetMouseButtonDown(0))
            {
                Debug.Log("Next Attack Successful");
                attackSuccessful = true;
            }

        // Updating the Player Sprite
        if (pState.StateClear())
            UpdatePlayerSprite();

        // Updating the Attacking Timer
        UpdateAttackTimers();

        // Updating the Player State
        pState.UpdateState();
    }

    private void UpdatePlayerSprite()
    {
        Vector2 movement = Vector2.zero;
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A))
            x -= 1f;
        if (Input.GetKey(KeyCode.D))
            x += 1f;
        if (Input.GetKey(KeyCode.W))
            y += 1f;
        if (Input.GetKey(KeyCode.S))
            y -= 1f;

        movement = new Vector2(x, y);

        if (movement != Vector2.zero && !attacking)
        {
            anim.SetFloat("Horizontal", movement.x);
            anim.SetFloat("Vertical", movement.y);
            anim.SetBool("Moving", true);

            attackDetermined = false;
            attackSuccessful = false;

            return;
        } else
        {
            anim.SetBool("Moving", false);
        }

        return;
    }

    private void UpdateAttackTimers()
    {
        // Updating the Melee Attack Timer
        if (meleeAttackTimer > 0f)
            meleeAttackTimer -= Time.deltaTime;

        // Updating the Range Attack Timer
        if (rangeAttackTimer > 0f)
            rangeAttackTimer -= Time.deltaTime;
    }

    private void AdjustAttackArea()
    {
        // Adjusting the AttackAreaOffset based on velocity
        Vector3 lastVelocity = GetComponent<UnitMovement>().lastVelocity;
        if (lastVelocity.x != 0f)
        {
            attackArea = new Vector3(length, width, 0f);
            attackAreaOffset = new Vector3(((length / 2) * Mathf.Sign(lastVelocity.x)) + ((srWidthAspect) * Mathf.Sign(lastVelocity.x)), 0f, 0f);
        }
        else
        {
            attackArea = new Vector3(width, length, 0f);
            attackAreaOffset = new Vector3(0f, ((length / 2) * Mathf.Sign(lastVelocity.y)) + ((srHeightAspect) * Mathf.Sign(lastVelocity.y)), 0f);
        }
        
    }

    private void TryMeleeAttack()
    {
        if (meleeAttackTimer <= 0f)
        {
            // Read just the Attack Area based on movement
            AdjustAttackArea();

            // Searching for a target
            Vector3 lastVelocity = GetComponent<UnitMovement>().lastVelocity;
            Vector3 attackVector = new Vector3(((length / 2) * Mathf.Sign(lastVelocity.x)) + ((transform.localScale.x / 2) * Mathf.Sign(lastVelocity.x)),
                                               ((width / 2) * Mathf.Sign(lastVelocity.y)) + ((transform.localScale.y / 2) * Mathf.Sign(lastVelocity.y)),
                                               0f);

            float a = Vector3.Angle(transform.position, transform.position + attackVector);
            RaycastHit2D hit = Physics2D.BoxCast(transform.position + attackAreaOffset, attackArea, a, Vector2.left, 0f, enemyLayer);
            if (hit)
            {
                Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();

                if (enemy == null)
                {
                    Debug.Log("Hit an Object on enemyLayer but couldn't find Enemy Reference");
                    return;
                }

                // Dealing the Damage to the Unit
                enemy.sTracker.TakeDamage(enemy.gameObject, manager.playerStats.unitDamage);
                enemy.StartCoroutine("HurtFlash");

                // Starting the Timer
                meleeAttackTimer = meleeAttackCooldown;
            }
        }
    }

    public void MeleeAttack()
    {
        checkingForNextAttack = true;
        attacking = true;
        anim.SetBool("Attacking", attacking);
        attackCount++;

        pState.canMove = false;
    }

    public void StartCheckingForNextAttack()
    {
        checkingForNextAttack = true;
        attackSuccessful = false;
        attackDetermined = false;
    }

    public void CheckForNextAttack()
    {
        if (!attackDetermined)
        {
            if (attackSuccessful)
            {
                if (attackCount <= 3)
                {
                    attackCount++;
                    attackSuccessful = false;
                    attackDetermined = true;
                    return;
                }
                else
                    StopAttacking();
            }
            else
                StopAttacking();
        }

        attackDetermined = true;
        return;
    }

    public void StopAttacking()
    {
        checkingForNextAttack = false;
        attackDetermined = false;
        attacking = false;
        anim.SetBool("Attacking", attacking);
        attackCount = 0;

        pState.canMove = true;
    }

    private void TryRangeAttack()
    {
        if (rangeAttackTimer <= 0f)
        {
            // Getting the Direction of the Bullet
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = new Vector2((mousePos - transform.position).x, (mousePos - transform.position).y);
            float a = Mathf.Atan2((mousePos - transform.position).y, (mousePos - transform.position).x) * Mathf.Rad2Deg;

            // Creating a range attack prefab
            GameObject newSpear = Instantiate(rangePrefab);
            newSpear.transform.position = transform.position;
            newSpear.transform.rotation = Quaternion.Euler(0f, 0f, a);
            newSpear.GetComponent<PlayerBullet>().SetBullet(dir / dir.magnitude);

            rangeAttackTimer = rangeAttackCooldown;
        }
    }

    public void ApplyKnockBack(float power, Vector2 dir)
    {
        // Setting the Player State
        pState.StunUnit(power);
        StartCoroutine("KnockBack", dir);
    }

    IEnumerator KnockBack(Vector2 dir)
    {
        // KnockBack Variables
        int reps = 10;
        float time = pState.stunTimer;
        float pow = 20f;
        float delay = (time / reps);

        for(int i = 0; i < reps; i++)
        {
            Vector3 vel = new Vector3((dir * pow).x, (dir * pow).y);
            GetComponent<MovementController>().Move(vel * Time.deltaTime);

            yield return new WaitForSeconds(delay);
        }
    }

    IEnumerator HurtFlash()
    {
        // Getting the SpriteRenderer
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Color baseColor = sr.color;
        Color flashColor = Color.white;
        int flashReps = 6;
        float delay = 0.05f;

        for (int i = 0; i < flashReps; i++)
        {
            sr.color = (i % 2 == 0) ? flashColor : baseColor;
            yield return new WaitForSeconds(delay);
        }
    }

    /// <summary>
    /// Updates all GUI References
    /// </summary>
    private void UpdatePlayerGUIElements()
    {
        // Updating the HealthBar
        playerHealthBar.Fill = (manager.playerStats.unitHealth / manager.playerStats.unitMaxHealth);
    }

    /// <summary>
    /// Drawing the Attack Area
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Drawing the Attack Zone
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position + attackAreaOffset, attackArea);
    }

}
