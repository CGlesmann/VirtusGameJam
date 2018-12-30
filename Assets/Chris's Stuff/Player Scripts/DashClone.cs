using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DashClone : MonoBehaviour
{
    [Header("Dash Clone Vars")]
    public float life = 0.075f;

    public Animator anim;

    private void Awake()
    {
        if (anim == null)
            anim = GetComponent<Animator>();
    }

    private void Update()
    {
        // Updating the Life Counter on the shadow
        life -= Time.deltaTime;

        // Checking for death
        if (life <= 0f)
        {
            GameObject.Destroy(gameObject);
        }
    }

}
