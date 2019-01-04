using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
    [Header("Bullet Variables")]
    public LayerMask playerLayer;

    [SerializeField] private Vector3 bulletDirection;
    [SerializeField] private float bulletSpeed;
    [SerializeField] private float acceptanceRange;
    [SerializeField] private float damage;

    private bool setUp = false;

    public void SetUp(float spd, float dmg, Vector3 dir)
    {
        bulletSpeed = spd;
        bulletDirection = dir;
        damage = dmg;

        setUp = true;
    }

    private void Update()
    {
        if (setUp)
        {
            transform.position += bulletDirection.normalized * bulletSpeed * Time.deltaTime;

            RaycastHit2D col = Physics2D.BoxCast(transform.position, Vector2.one * acceptanceRange, 0f, Vector2.zero, 0f, playerLayer);
            if (col)
            {
                Player p = col.collider.gameObject.GetComponent<Player>();
                p.manager.playerStats.TakeDamage(p.gameObject, damage);

                GameObject.Destroy(gameObject);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(transform.position, Vector3.one * acceptanceRange);
    }
}
