/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.Events;
using System;

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

	public Action OnPointerEnter { get; set; }
	public Action OnPointerExit  { get; set; }
	public Action OnPointerDown  { get; set; }
	public Action OnPointerUp    { get; set; }

	public InventoryItem(int id, int quantity = 1)
	{
		this.InventoryItemSharedData_ = InventoryItemDatabaseController.Instance.FetchSharedData(id);
		this._Quantity = quantity;
	}

	public InventoryItem(int id, Action onPointerDown, int quantity = 1) :
		this(id, quantity)
	{
		this.OnPointerDown = onPointerDown;
	}

	public InventoryItem(int id, Action onPointerDown, Action onPointerUp, int quantity = 1) : 
		this(id, onPointerDown, quantity)
	{
		this.OnPointerUp = onPointerUp;
	}

	public InventoryItem(int id, Action onPointerEnter, Action onPointerExit, Action onPointerDown, Action onPointerUp, int quantity = 1) : 
		this(id, onPointerDown, onPointerUp, quantity)
	{
		this.OnPointerEnter = onPointerEnter;
		this.OnPointerExit = onPointerExit;
	}
}