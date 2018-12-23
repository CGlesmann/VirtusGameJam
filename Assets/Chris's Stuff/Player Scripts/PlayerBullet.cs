using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{
    [Header("Player Bullet Variables")]
    public LayerMask enemyLayer;
    public LayerMask wallLayer;
    public float bulletSpeed;
    public Vector3 bulletDir;

    public Player player;

    private bool set = false;

    [SerializeField] private float colRange;

    /// <summary>
    /// Updates the Bullets Movement
    /// </summary>
    public void FixedUpdate()
    {
        // Checking to make sure the bullet is set
        if (set)
        {
            // Moving the Bullet
            transform.position += bulletDir * bulletSpeed * Time.deltaTime;

            // Checking for a wall collision
            if (CheckForWallCollision())
                GameObject.Destroy(gameObject);

            if (CheckForEnemyCollision())
                GameObject.Destroy(gameObject);
        }
    }

    /// <summary>
    /// Making sure there is no collision with a wall
    /// </summary>
    /// <returns></returns>
    private bool CheckForWallCollision()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, bulletDir, 0.25f, wallLayer);
        return (hit);
    }

    /// <summary>
    /// Checking for an enemy collision
    /// </summary>
    /// <returns></returns>
    private bool CheckForEnemyCollision()
    {
        // Checking for a hit
        RaycastHit2D hit = Physics2D.Raycast(transform.position, bulletDir, 0.25f, enemyLayer);
        if (hit)
        {
            // Getting the Enemy
            Enemy enemy = hit.collider.gameObject.GetComponent<Enemy>();
            if (enemy == null)
                return true;

            // Damaging the Enemy
            enemy.sTracker.TakeDamage(enemy.gameObject, 5f);
        }

        return (hit);
    }

    /// <summary>
    /// Getting a direction and speed and setting them in Instance
    /// </summary>
    /// <param name="shootDirection"></param>
    /// <param name="bSpeed"></param>
    public void SetBullet(Vector3 shootDirection, float bSpeed = 15f)
    {
        bulletDir = shootDirection;
        bulletSpeed = bSpeed;

        set = true;
    }

}
