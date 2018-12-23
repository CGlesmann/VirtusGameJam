using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Door : MonoBehaviour {

    // Door State Enum
    public enum DoorState { Locked, Unlocked }; // Tracks whether or not the door is locked
    public enum DoorMoveStyle { Horizontal, Vertical }; // Tracks how the door opens

    [Header("Door Variable")]
    public KeyCode interactKey = KeyCode.E;
    public DoorState dState;
    public DoorMoveStyle dStyle;

    [SerializeField]
    public Vector3 openPosition;
    [SerializeField]
    public Vector3 closedPosition;

    public bool autoClose = true; // Controls the auto close function

    private bool isOpen = false;
    private bool isMoving = false; // Tracks whether or not the door is opening or closing
    private bool playerPassed = false; // Tracks whether the player has passed through the door for autoClose

    [Header("Door Collision Variables")]
    public LayerMask playerLayer;
    public Vector3 interactionRadius;

    private SpriteRenderer sr;
    private Player player;

    /// <summary>
    /// Getting Private References
    /// </summary>
    private void Awake()
    {
        player = GameObject.Find("Player").GetComponent<Player>();
        if (player == null)
            Debug.Log("Door couldn't find the player");

        sr = GetComponent<SpriteRenderer>();
        if (sr == null)
            Debug.Log("Door doesn't have a sprite renderer");
    }

    /// <summary>
    /// Checks for Door Interactions
    /// </summary>
    private void Update()
    {
        if (!isMoving)
        {
            // Checks for interactions
            if (Input.GetKeyDown(interactKey))
            {
                if (PlayerInRange())
                {
                    if (isOpen)
                        if (!PlayerNotInDoor())
                            return;

                    if (dState == DoorState.Locked)
                    {
                    }

                    StartCoroutine("ToggleDoor");
                    isMoving = true;

                    return;
                }
            }

            // Checking for auto Close
            if (isOpen & autoClose)
            {
                // If Player Hasn't passed through, track for player passing
                if (!playerPassed)
                {
                    // Waiting for player to Pass through the door
                    if (!PlayerNotInDoor())
                        playerPassed = true;

                    return;
                }

                // Otherwise after player passes through door, close it
                if (PlayerNotInDoor())
                {
                    StartCoroutine("ToggleDoor");
                    isMoving = true;

                    return;
                }
            }
        }
    }

    /// <summary>
    /// Opening and Closing the Door
    /// </summary>
    /// <returns></returns>
    IEnumerator ToggleDoor()
    {
        // Setting the ANimation Variables
        int reps = 20;
        float delay = 0.01f;
        float incAmount = 0f;

        // Getting the incAmount and storing it
        if (!isOpen)
        {
            switch (dStyle)
            {
                case DoorMoveStyle.Vertical:
                    incAmount = (openPosition.y - closedPosition.y) / reps;
                    break;
                case DoorMoveStyle.Horizontal:
                    incAmount = (openPosition.x - closedPosition.x) / reps;
                    break;
            }
        } else {
            switch (dStyle)
            {
                case DoorMoveStyle.Vertical:
                    incAmount = (closedPosition.y - openPosition.y) / reps;
                    break;
                case DoorMoveStyle.Horizontal:
                    incAmount = (closedPosition.x - openPosition.x) / reps;
                    break;
            }
        }

        // Incrementing the door sprite
        for(int i = 0; i < reps; i++)
        {
            switch (dStyle)
            {
                case DoorMoveStyle.Vertical:
                    transform.position += new Vector3(0f, incAmount, 0f);
                    break;
                case DoorMoveStyle.Horizontal:
                    transform.position += new Vector3(incAmount, 0f, 0f);
                    break;
            }

            yield return new WaitForSeconds(delay);
        }

        // Flipping the Open Sign and stopping movement
        isOpen = !isOpen;
        isMoving = false;

        playerPassed = false;
    }

    /// <summary>
    /// Casts a BoxCast and returns if a hit was made
    /// </summary>
    /// <returns></returns>
    public bool PlayerInRange()
    {
        // Cast a collisionBox on the player Layer
        RaycastHit2D hit = Physics2D.BoxCast(closedPosition, interactionRadius, 0f, Vector2.left, 0f, playerLayer);
        return (hit); 
    }

    /// <summary>
    /// Returns whether or not the player is in the doors range
    /// </summary>
    /// <returns></returns>
    public bool PlayerNotInDoor()
    {
        // Casting the CollisionBox in the doors range
        RaycastHit2D hit = Physics2D.BoxCast(closedPosition, transform.localScale * 0.95f, 0f, Vector2.left, 0f, playerLayer);
        return (!hit);
    }

    /// <summary>
    /// Gets the current position of the door and stores it in openPosition
    /// </summary>
    public void SetOpenPositionToCurrent()
    {
        openPosition = transform.position;
        return;
    }

    /// <summary>
    /// Gets the current position of the door and stores it in closedPosition
    /// </summary>
    public void SetClosedPositionToCurrent()
    {
        closedPosition = transform.position;
        return;
    }

    /// <summary>
    /// Setting the Door to a closed state
    /// </summary>
    public void SetDoorToClosed()
    {
        // Setting the door to be closed
        isOpen = false;
        transform.position = closedPosition;
    }

    /// <summary>
    /// Setting the Door to an open state
    /// </summary>
    public void SetDoorToOpened()
    {
        // Setting the door to be closed
        isOpen = true;
        transform.position = openPosition;
    }

    /// <summary>
    /// Drawing the collisionChecks
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Setting the Interaction Gizmo Color
        Gizmos.color = Color.green;
        // Drawing the Interaction Radius
        Gizmos.DrawCube(closedPosition, interactionRadius);

        // Setting the Door Range Gizmo Color
        Gizmos.color = Color.red;
        // Drawing the Interaction Radius
        Gizmos.DrawCube(closedPosition, transform.localScale * 0.95f);
    }

}
