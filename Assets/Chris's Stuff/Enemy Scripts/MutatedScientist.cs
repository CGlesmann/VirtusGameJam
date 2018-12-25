using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MutatedScientist : MonoBehaviour
{
    [Header("MiniBoss Stats")]
    public UnitStats mbStats;
    public UnitState uState;

    [Header("Battle Start Variables")]
    public bool battleStarted = false;
    public Vector2 slamPosition;

    [Header("Battle Ability Variables")]
    // Jumping Variables
    [SerializeField] private bool isJumping;

    [Header("Test")]
    public UnityEvent[] events;

    private UnitMovement mControl;

    private void Awake()
    {
        mControl = GetComponent<UnitMovement>();
        uState = new UnitState();
    }

    #region Start Functions
    public void StartBattle()
    {
        //StartCoroutine("StartBattleRoutine");
        return;
    }

    #endregion

    #region Ability Functions
    IEnumerator JumpingUp()
    {
        float yGoal = Camera.main.rect.yMax;
        float time = 0.75f;
        int reps = 20;
        float inc = (yGoal - transform.position.y) / reps;
        float delay = (time / reps);

        Debug.Log("yGoal: " + yGoal);

        for (int i = 0; i < reps; i++)
        {
            Vector3 newPos = new Vector3(transform.position.x, transform.position.y + inc, transform.position.z);
            transform.position = newPos;

            yield return new WaitForSeconds(delay);
        }

        isJumping = true;
    }
    #endregion
}
