using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour {

    [Header("Player Inventory Vars")]
    [HideInInspector] public PlayerInventory pInven;

    /// <summary>
    /// Getting References
    /// </summary>
    private void Awake()
    {
        // Getting the PlayerInventory Component
        pInven = GetComponent<PlayerInventory>();
    }

}
