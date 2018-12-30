﻿using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MovementController))]
public class UnitMovement : MonoBehaviour
{
    [Header("Movement Variables")]
    public float baseMoveSpeed = 6f;
    private Vector3 velocity;

    private Player player;
    private MovementController controller;

    [Header("Dashing Variables")]
    public GameObject dashClone;
    public bool isDashing = false;
    public float dashSpeed = 12f;
    public float dashTime = 0.1f; // In Seconds
    public float dashCoolDown = 0.75f; // In Seconds

    private float dashCool = 0f;
    private float dashTimer = 0f;

    [Header("Misc Variables")]
    public Vector3 lastVelocity = Vector3.zero;

    private Animator anim;

    void Awake()
    {
        controller = GetComponent<MovementController>();
        player = GetComponent<Player>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.LeftShift) && lastVelocity != Vector3.zero && player.pState.StateClear())
        {
            if (!isDashing)
            {
                anim.SetBool("Dashing", true);

                StartCoroutine("Dashing");
                isDashing = true;
            }
        }

        // Creating the Dash Clones
        if (isDashing)
        {
            GameObject newClone = Instantiate(dashClone);
            newClone.transform.position = transform.position;

            newClone.GetComponent<DashClone>().anim.SetFloat("Horizontal", anim.GetFloat("Horizontal"));
            newClone.GetComponent<DashClone>().anim.SetFloat("Vertical", anim.GetFloat("Vertical"));
        }
    }

    private void FixedUpdate()
    {
        if (!isDashing && player.pState.StateClear())
        {
            Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

            float speed = baseMoveSpeed;

            velocity.x = input.x * speed;
            velocity.y = input.y * speed;

            Vector3 toMove = velocity * Time.deltaTime;
            controller.Move(toMove);

            if (toMove != Vector3.zero)
            {
                lastVelocity = toMove;
            }
        }
    }

    IEnumerator Dashing()
    {
        Vector2 dir = lastVelocity;

        float power = 60f;
        float length = 0.15f;
        int reps = 10;
        float delay = (length / reps);
        
        float inc = power / reps;

        for(int i = 0; i < reps; i++)
        {
            controller.Move(velocity * inc * Time.deltaTime);
            yield return new WaitForSeconds(delay);
        }

        isDashing = false;
        anim.SetBool("Dashing", false);
    }

}