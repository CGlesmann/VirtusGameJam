using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class PlayerInventory : MonoBehaviour {

    [Header("Player Inventory Stats")]
	public int keyCards = 1;

    [Header("GUI References")]
    public TextMeshProUGUI keyCount;

    /// <summary>
    /// Updating the GUI elements
    /// </summary>
    private void Update()
    {
        // Updating the KeyCard Count element
        keyCount.text = keyCards.ToString();
    }
}
