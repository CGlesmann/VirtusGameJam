using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitStats
{
    [Header("Combat Variables")]
    public float unitHealth;
    public float unitMaxHealth;

    public float unitDamage;

    [Header("GameObject reference")]
    public GameObject gObj; 

    /// <summary>
    /// Default Constructor, Sets the Default health to 100 and damage to 10
    /// </summary>
    public UnitStats(GameObject o, float h = 100f, float d = 10f)
    {
        // Setting Health
        unitHealth = h;
        unitMaxHealth = unitHealth;

        // Setting Damage
        unitDamage = d;

        // Setting the Reference
        gObj = o;
    }

    /// <summary>
    /// Takes Damage and checks for death
    /// </summary>
    /// <param name="damage"></param>
    public void TakeDamage(float damage)
    {
        // Reporting the amount of damage unit has taken
        Debug.Log(gObj.name + " Takes " + damage.ToString("F0") + " damage");

        // Assigning the Damage
        unitHealth -= damage;

        // Checking for death
        if (unitHealth <= 0f)
        {
            // Destroying the GameObject
            GameObject.Destroy(gObj);
        }
    }
}
