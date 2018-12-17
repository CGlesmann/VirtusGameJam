using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MovementController))]
public class UnitMovement : MonoBehaviour
{
    public float moveSpeed = 6;
    Vector3 velocity;

    MovementController controller;

    void Start()
    {
        controller = GetComponent<MovementController>();
    }

    void FixedUpdate()
    {

        Vector2 input = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        velocity.x = input.x * moveSpeed;
        velocity.y = input.y * moveSpeed;
        controller.Move(velocity * Time.deltaTime);
    }
}