using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
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
    public AudioClip[] meleeSwingSFX;

    [Header("Player Melee Variables")]
    public int meleeAttack = 0;
    public bool swinging = false; // Tracks when to check for collisions
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
    public bool startingThrow = false;
    public float rangeAttackCooldown;
    public GameObject rangePrefab;
	[SerializeField] private AudioClip _rangeAttackSFX;

    private float rangeAttackTimer = 0f;

    [Header("Ultimate Ability Vars")]
    public GameObject ultimatePrefab;
    public float ultimateCooldown;

    private float ultimateTimer = 0f;

    [Header("Input Variables")]
    public KeyCode attackKey;

    [Header("Animation Variables")]
    [SerializeField] private bool moving = false;
    [SerializeField] private bool attacking = false;
    [SerializeField] private int attackCount = 0;

    [Header("Spearhand Variables")]
    public Transform spearHand;
    public Vector3 handOffset;
    public Vector3 handRotation;

    private UnitMovement playerMovement;
    private Animator anim;

    private List<GameObject> hits;
    private RaycastHit2D[] enemies;

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
        {
            if (!startingThrow)
            {
                //TryRangeAttack();
                anim.SetBool("ThrowStart", true);
                startingThrow = true;
                pState.canMove = false;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space) && !anim.GetBool("Ultimate"))
            if (ultimateTimer <= 0f)
            {
                anim.SetBool("Ultimate", true);
                pState.canMove = false;
            }

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

        // Updating ultimate timer
        if (ultimateTimer > 0f)
        {
            ultimateTimer -= Time.deltaTime;
        }

        // Updating attacking
        if (swinging)
        {
            TryMeleeAttack();
        }
    }

    private void UpdatePlayerSprite()
    {
        Vector2 movement = Vector2.zero;
        float x = 0f;
        float y = 0f;

        if (Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow))
            x -= 1f;
        if (Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow))
            x += 1f;
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.UpArrow))
            y += 1f;
        if (Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.DownArrow))
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

    public void RestrainPlayer()
    {
        pState.canMove = false;
    }

    public void FreePlayer()
    {
        pState.canMove = true;
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

    public void AdjustAttackArea()
    {
        // Adjusting the AttackAreaOffset based on velocity
        Vector3 lastVelocity = GetComponent<UnitMovement>().lastVelocity;
        if (lastVelocity.x != 0f)
        {
            attackArea = new Vector3(length, width, 0f);
            attackAreaOffset = new Vector3(((length / 4) * Mathf.Sign(lastVelocity.x)) + ((srWidthAspect) * Mathf.Sign(lastVelocity.x)), 0f, 0f);
        }
        else
        {
            attackArea = new Vector3(width, length, 0f);
            attackAreaOffset = new Vector3(0f, ((length / 4) * Mathf.Sign(lastVelocity.y)) + ((srHeightAspect) * Mathf.Sign(lastVelocity.y)), 0f);
        }
        
    }

    public void StartSwinging()
    {
        if (swinging == false)
        {
            hits = new List<GameObject>();
            swinging = true;
            enemies = null;

            if (attackCount < 3)
                AudioPlayer.Instance.PlaySFX(meleeSwingSFX[attackCount - 1]);
        }
    }

    public void CreateSpear()
    {
        spearHand.gameObject.GetComponent<SpearHand>().CreateSpear();
        return;
    }

    public void ThrowSpear()
    {
        anim.SetBool("ThrowStart", false);
        anim.SetBool("Throwing", true);

		AudioPlayer.Instance.PlaySFX(this._rangeAttackSFX);

		return;
    }

    public void ResetThrowState()
    {
        anim.SetBool("Throwing", false);
        startingThrow = false;
        pState.canMove = true;
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
            enemies = Physics2D.BoxCastAll(transform.position + attackAreaOffset, attackArea, a, Vector2.left, 0f, enemyLayer);
         
            foreach(RaycastHit2D hit in enemies)
            {
                if (!hits.Contains(hit.collider.gameObject))
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

                    // Applying a very brief stun
                    enemy.eState.StunUnit(0.075f);

                    // Adding the GameObject to the hits
                    hits.Add(hit.collider.gameObject);
                }
            }
        }
    }

    public void StopSwinging()
    {
        if (swinging == true)
        {
            swinging = false;
            enemies = null;
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

    public void CreateUltimate()
    {
        if (ultimateTimer <= 0f)
        {
            GameObject ability = Instantiate(ultimatePrefab);
            ability.transform.position = transform.position;

            ultimateTimer = ultimateCooldown;;
        }
    }

    public void ResetUltTrigger()
    {
        anim.SetBool("Ultimate", false);
        pState.canMove = true;
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
        int reps = 20;
        float time = pState.stunTimer;
        float pow = 500f;
        float delay = (time / reps);
        float inc = (pow / reps);

        for(int i = 0; i < reps; i++)
        {
            Vector3 vel = new Vector3((dir * inc).x, (dir * inc).y);
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
