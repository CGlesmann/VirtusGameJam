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
        if (Input.GetMouseButtonDown(0))
            TryMeleeAttack();
        if (Input.GetMouseButtonDown(1))
            TryRangeAttack();

        // Updating the GUI
        UpdatePlayerGUIElements();

        // Updating the Player Sprite
        UpdatePlayerSprite();

        // Updating the Attacking Timer
        UpdateAttackTimers();
    }

    private void UpdatePlayerSprite()
    {
        Vector3 movement = new Vector3(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), 0f);

        anim.SetFloat("Horizontal", movement.x);
        anim.SetFloat("Vertical", movement.y);
        anim.SetFloat("Magnitude", movement.magnitude);

        
        if (Input.GetKey(KeyCode.W))
        {
            anim.SetBool("IsMoving", true);
            anim.SetBool("UpMove", true);
            anim.SetBool("DownMove", false);
            anim.SetBool("LeftMove", false);
            anim.SetBool("RightMove", false);
            return;
        }

        if (Input.GetKey(KeyCode.A))
        {
            anim.SetBool("IsMoving", true);
            anim.SetBool("UpMove", false);
            anim.SetBool("DownMove", false);
            anim.SetBool("LeftMove", true);
            anim.SetBool("RightMove", false);
            return;
        }

        if (Input.GetKey(KeyCode.S))
        {
            anim.SetBool("IsMoving", true);
            anim.SetBool("UpMove", false);
            anim.SetBool("DownMove", true);
            anim.SetBool("LeftMove", false);
            anim.SetBool("RightMove", false);
            return;
        }

        if (Input.GetKey(KeyCode.D))
        {
            anim.SetBool("IsMoving", true);
            anim.SetBool("UpMove", false);
            anim.SetBool("DownMove", false);
            anim.SetBool("LeftMove", false);
            anim.SetBool("RightMove", true);
            return;
        }

        anim.SetBool("IsMoving", false);
        anim.SetBool("UpMove", false);
        anim.SetBool("DownMove", false);
        anim.SetBool("LeftMove", false);
        anim.SetBool("RightMove", false);

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
