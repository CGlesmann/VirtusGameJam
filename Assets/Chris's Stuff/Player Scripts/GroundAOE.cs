using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundAOE : MonoBehaviour
{
    [Header("Collision Variables")]
    public LayerMask enemyLayer;
    public float radius = 1f;
    public Vector3 aoeOffset;
    public bool checking = false;

    [Header("Ability Damage Stats")]
    public float damage = 15f;
    public float knockBackStr = 1f;

    [Header("Aibility Stall")]
    public float idleTime;

    private List<GameObject> hits;
    private float timer = 0f;

    private void Awake()
    {
        hits = new List<GameObject>();

        GetComponent<Animator>().SetBool("Idle", true);
        timer = idleTime;
    }

    private void Update()
    {
        timer -= Time.deltaTime;

        if (timer <= 0f)
        {
            GetComponent<Animator>().SetBool("Idle", false);
        } else {
            if (checking)
            {
                CheckForCollision();
            }
        }
    }

    public void StartChecking()
    {
        checking = true;
    }

    public void CheckForCollision()
    {
        // Getting an Array of all hit targets
        RaycastHit2D[] targets = Physics2D.CircleCastAll(transform.position + aoeOffset, radius, Vector2.left, 0f, enemyLayer);
        foreach(RaycastHit2D target in targets)
        {
            // Running Through Each target and applying damage
            Enemy enemy = target.collider.gameObject.GetComponent<Enemy>();
            if (!hits.Contains(enemy.gameObject))
            {
                enemy.sTracker.TakeDamage(enemy.gameObject, damage);

                // Getting the KnockBack Direction
                Vector2 dir = new Vector2((enemy.gameObject.transform.position.x - transform.position.x), (enemy.gameObject.transform.position.y - transform.position.y));
                dir /= dir.magnitude;

                // Applying the knockback
                enemy.ApplyKnockBack(knockBackStr, dir);

                // Adding the Target to the list
                hits.Add(enemy.gameObject);
            }
        }
    }

    public void DestroyUlt()
    {
        GameObject.Destroy(gameObject);
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position + aoeOffset, radius);
    }

}
