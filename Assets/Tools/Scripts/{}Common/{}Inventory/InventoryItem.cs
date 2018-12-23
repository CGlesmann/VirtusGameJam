/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

using Action = System.Action;

[System.Serializable]
public class InventoryItem
{
	public InventoryItemSharedData InventoryItemSharedData_ { get; private set; }

	public event Action OnQuantityChange;

	private int _quantity;
	public int _Quantity
	{
		get { return this._quantity; }
		set
		{
			this._quantity = value;
			if (this.OnQuantityChange != null)
				this.OnQuantityChange.Invoke();
		}
	}

	public InventoryItem(int id, int quantity)
	{
		this.InventoryItemSharedData_ = InventoryItemDatabaseController.Instance.FetchSharedData(id);
		this._Quantity = quantity;
	}
}