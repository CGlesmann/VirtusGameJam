/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

public class Item : MonoBehaviour
{
	[SerializeField] private InventoryItemSharedData _inventoryItemSharedData;

	[SerializeField] private UnityEvent _onInventoryItemPointerDown;

	public void PickUp()
	{
		if (GameManager.Instance.PlayerInventory_.Add(new InventoryItem(this._inventoryItemSharedData.Id, this._onInventoryItemPointerDown.Invoke)))
		{
			Object.Destroy(this.gameObject);
		}
	}

	public void CheckForInput()
	{
		if (Input.GetKeyDown(KeyCode.E))
		{
			this.PickUp();
		}
	}

	[SerializeField] private View _interactionTooltip;

	public void ShowRelatedView()
	{
		this._interactionTooltip.Show();
	}

	public void HideRelatedView()
	{
		this._interactionTooltip.Hide();
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Item))]
[CanEditMultipleObjects]
public class ItemEditor : Editor
{
#pragma warning disable 0219, 414
	private Item _sItem;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sItem = this.target as Item;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif