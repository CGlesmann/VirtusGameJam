using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitState
{
    public bool stunned = false, canMove = true;
    public float stunTimer = 0f;

    public void ResetState()
    {
        stunned = false;
        canMove = true;

        stunTimer = 0f;
    }

    public void UpdateState()
    {
        // Updating Stun
        if (stunTimer > 0f)
        {
            stunTimer -= Time.deltaTime;
            if (stunTimer <= 0f)
            {
                stunned = false;
            }
        }
    }

    public void StunUnit(float stunPower = 1f)
    {
        stunned = false;
        stunTimer = stunPower;
    }

    public bool StateClear()
    {
        if (!stunned && canMove)
            return true;

        return false;
    }
}
