/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

[System.Serializable]
public class Inventory
{
	private const int DEFAULT_SLOT_INDEX = -1;

	private InventoryView _inventoryDisplayController;
	public InventoryView InventoryDisplayController__ { set { this._inventoryDisplayController = value; } }

	private InventoryItemSlot[] _itemSlots;
	private int _itemsQuantity;

	public int Capacity_ { get; private set; }

	public Inventory(int size, int capacity)
	{
		this._itemSlots = new InventoryItemSlot[size];

		this._itemsQuantity = 0;

		if (capacity <= size)
			this.Capacity_ = capacity;
		else
		{
			Debug.LogError(this.GetType().Name + " - Capacity can't be larger than size. Capacity was set to max value possible = size.");
			this.Capacity_ = size;
		}
	}

	public void AddItemDisplay(InventoryItemSlotView inventoryItemDisplayController, int index)
	{
		inventoryItemDisplayController.Clear(InventoryView.ClearMode.Partial);
		this._itemSlots[index].InventoryItemDisplayController__ = inventoryItemDisplayController;
	}

	private bool Add(InventoryItem item)
	{
		if (item.InventoryItemSharedData_._Stackable)
		{
			int firstFreeSlotIndex = Inventory.DEFAULT_SLOT_INDEX;

			for (int i = 0; i < this.Capacity_; i++)
			{
				if (this._itemSlots[i]._IsFree)
				{
					if (firstFreeSlotIndex == Inventory.DEFAULT_SLOT_INDEX)
						firstFreeSlotIndex = i;
				}
				else if (this._itemSlots[i].Item.InventoryItemSharedData_.Id == item.InventoryItemSharedData_.Id)
				{
					this._itemSlots[i].Item._Quantity += item._Quantity;

					return true;
				}
			}

			if (firstFreeSlotIndex != Inventory.DEFAULT_SLOT_INDEX)
			{
				this._itemSlots[firstFreeSlotIndex].Item = item;

				this._itemsQuantity++;
				return true;
			}
		}
		else
		{
			if (this._itemsQuantity == this.Capacity_)
				return false;

			for (int i = 0; i < this.Capacity_; i++)
			{
				if (this._itemSlots[i]._IsFree)
				{
					this._itemSlots[i].Item = item;

					this._itemsQuantity++;
					return true;
				}
			}
		}

		return false;
	}

	public void Add(int itemId, int quantity)
	{
		for (int i = 0; i < quantity; i++)
		{
			this.Add(new InventoryItem(itemId, 1));
		}
	}

	public bool Remove(InventoryItem item)
	{
		if (this._itemsQuantity == 0)
			return false;

		for (int i = 0; i < this.Capacity_; i++)
		{
			if (this._itemSlots[i]._IsFree)
				continue;

			if (this._itemSlots[i].Item == item)
			{
				this._itemSlots[i].Clear(InventoryView.ClearMode.Partial);

				this._itemsQuantity--;
				return true;
			}
		}

		return false;
	}

	public bool Remove(int itemId)
	{
		if (this._itemsQuantity == 0)
			return false;

		for (int i = 0; i < this.Capacity_; i++)
		{
			if (this._itemSlots[i]._IsFree)
				continue;

			if (this._itemSlots[i].Item.InventoryItemSharedData_.Id == itemId)
			{
				this._itemSlots[i].Clear(InventoryView.ClearMode.Partial);

				this._itemsQuantity--;
				return true;
			}
		}

		return false;
	}

	public enum RemoveResultStatus { NegativeQuantity, Success, NotFound }

	public RemoveResultStatus Remove(int itemId, int quantity)
	{
		if (this._itemsQuantity == 0)
			return RemoveResultStatus.NotFound;

		for (int i = 0; i < this.Capacity_; i++)
		{
			if (this._itemSlots[i]._IsFree)
				continue;

			if (this._itemSlots[i].Item.InventoryItemSharedData_.Id == itemId)
			{
				if (this._itemSlots[i].Item._Quantity < quantity)
					return RemoveResultStatus.NegativeQuantity;

				this._itemSlots[i].Item._Quantity -= quantity;

				if (this._itemSlots[i].Item._Quantity == 0)
					this._itemSlots[i].Clear(InventoryView.ClearMode.Partial);

				this._itemsQuantity--;
				return RemoveResultStatus.Success;
			}
		}

		return RemoveResultStatus.NotFound;
	}

	public bool Contains(InventoryItem item)
	{
		if (this._itemsQuantity == 0)
			return false;

		for (int i = 0; i < this.Capacity_; i++)
		{
			if (this._itemSlots[i]._IsFree)
				continue;

			if (this._itemSlots[i].Item == item)
				return true;
		}

		return false;
	}

	public bool Contains(int itemId)
	{
		if (this._itemsQuantity == 0)
			return false;

		for (int i = 0; i < this.Capacity_; i++)
		{
			if (this._itemSlots[i]._IsFree)
				continue;

			if (this._itemSlots[i].Item.InventoryItemSharedData_.Id == itemId)
				return true;
		}

		return false;
	}

	public void Rebuild()
	{
		// Inventory code

		if (this._inventoryDisplayController != null)
		{
			// Update display code
		}
	}
}

[System.Serializable]
public struct InventoryItemSlot
{
	private InventoryItemSlotView _itemSlotView;
	public InventoryItemSlotView InventoryItemDisplayController__ { set { this._itemSlotView = value; } }

	private InventoryItem _item;
	public InventoryItem Item
	{
		get { return this._item; }
		set
		{
			if (value != null)
			{
				this._item = value;
				this._itemSlotView._ItemView.Open();
				this._itemSlotView._ItemView.Display(this._item);
				this._item.OnQuantityChange += this._itemSlotView._ItemView.UpdateLayout;
			}
#if UNITY_EDITOR
			else
			{
				Debug.LogWarning("Trying to assign null. Use `Clear()` instead.");
			}
#endif
		}
	}

	public bool _IsFree { get { return this._item == null; } }

	public void Clear(InventoryView.ClearMode clearMode)
	{
		this._item = null;
		this._itemSlotView.Clear(clearMode);
	}
}