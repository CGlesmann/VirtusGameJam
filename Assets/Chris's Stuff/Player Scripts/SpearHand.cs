using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpearHand : MonoBehaviour
{
    [Header("SpearHand Variables")]
    public GameObject spearPrefab;
    public Vector3 spearSpawnOffset;

    [Header("SpearHand State Variables")]
    [SerializeField] private GameObject currentSpear;
    public bool isAiming;

    private Animator anim;

    private void Awake()
    {
        anim = transform.parent.gameObject.GetComponent<Animator>();

        currentSpear = null;
        isAiming = false;
    }

    private void Update()
    {
        if (isAiming)
        {
            // Rotating the Arm Based on mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = new Vector2((mousePos - transform.position).x, (mousePos - transform.position).y);
            float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            // Updating the Spear
            currentSpear.transform.position = transform.position + spearSpawnOffset;
            currentSpear.transform.rotation = Quaternion.Euler(0f, 0f, a);

            // Updating the Animator
            /*
            anim.SetFloat("Horizontal", dir.x);
            anim.SetFloat("Vertical", dir.y);
            */

            if (Input.GetMouseButtonUp(1))
            {
                Vector3 newDir = new Vector3(dir.x, dir.y, 0f);
                currentSpear.GetComponent<PlayerBullet>().SetBullet(newDir);

                StopAiming();
            }
        }
    }

    public void CreateSpear()
    {
        if (currentSpear == null)
        {
            // Spawning A Spear in the hand
            currentSpear = Instantiate(spearPrefab);
            currentSpear.transform.position = transform.position + spearSpawnOffset;

            // Starting aiming the spear
            StartAiming();

            return;
        }
    }

    public void StartAiming()
    {
        isAiming = true;
        return;
    }

    public void StopAiming()
    {
        // Stop the Aiming
        isAiming = false;
        currentSpear = null;

        // Tell Player to throw spear
        transform.parent.gameObject.GetComponent<Player>().ThrowSpear();
        return;
    }

    private void OnDrawGizmosSelected()
    {
        // Drawing the SpearSpawn Point
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position + spearSpawnOffset, 0.025f);
    }
}
