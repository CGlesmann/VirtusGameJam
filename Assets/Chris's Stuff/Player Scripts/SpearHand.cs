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

    private void Update()
    {
        if (isAiming)
        {
            // Rotating the Arm Based on mouse position
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            Vector2 dir = new Vector2((mousePos - transform.position).x, (mousePos - transform.position).y);
            float a = Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;

            transform.rotation = Quaternion.Euler(0f, 0f, a);

            // Updating the Spear
            currentSpear.transform.position = transform.position + spearSpawnOffset;
            currentSpear.transform.rotation = transform.rotation;
        }
    }

    public void CreateSpear()
    {
        // Spawning A Spear in the hand
        currentSpear = Instantiate(spearPrefab);
        currentSpear.transform.position = transform.position + spearSpawnOffset;

        return;
    }

    public void StartAiming()
    {
        isAiming = true;
        return;
    }

    public void StopAiming()
    {
        isAiming = false;
        return;
    }

    private void OnDrawGizmosSelected()
    {
        // Drawing the SpearSpawn Point
        Gizmos.color = Color.black;
        Gizmos.DrawSphere(transform.position + spearSpawnOffset, 0.025f);
    }

}
