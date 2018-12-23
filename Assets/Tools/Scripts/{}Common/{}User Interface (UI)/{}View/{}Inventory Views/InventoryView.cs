/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventoryView : View
{
	public enum ClearMode { Partial, Full, Destruction }

	[SerializeField] private InventoryItemTooltipView _itemTooltipView;
	public InventoryItemTooltipView _ItemTooltipView { get { return this._itemTooltipView; } }

	[SerializeField] private InventoryItemSlotView _itemSlotViewPrefab;
	public InventoryItemSlotView _ItemSlotViewPrefab { get { return this._itemSlotViewPrefab; } }

	[SerializeField] private Transform _itemSlotViewsContainer;
	public Transform _ItemSlotViewsContainer { get { return this._itemSlotViewsContainer; } }

	private Inventory _displayedInventory;

	public void Display(Inventory inventory)
	{
		this._displayedInventory = inventory;

		this.InitializeLayout();
		this.UpdateLayout();
	}

	private List<InventoryItemSlotView> _itemSlotViews = new List<InventoryItemSlotView>(16);

	public void InitializeLayout()
	{
		if (this._displayedInventory == null)
			return;

		this.Clear(ClearMode.Full);

		for (int i = 0; i < this._displayedInventory.Capacity_; i++)
		{
			InventoryItemSlotView itemSlotView = i < this._itemSlotViews.Count ? this._itemSlotViews[i] : null;

			if (itemSlotView == null)
			{
				itemSlotView = Object.Instantiate(this._itemSlotViewPrefab, this._itemSlotViewsContainer);
				itemSlotView.ParentView = this;

				this._itemSlotViews.Add(itemSlotView);
				this._itemSlotViews[i] = itemSlotView;
				
				this._displayedInventory.AddItemDisplay(itemSlotView, i);
			}
			
			itemSlotView.Open();
		}
	}

	public void UpdateLayout()
	{
		if (this._displayedInventory == null)
			return;

		for (int i = 0; i < this._displayedInventory.Capacity_; i++)
		{
			this._itemSlotViews[i]._ItemView.UpdateLayout();
		}
	}

	public void Clear(ClearMode clearMode)
	{
		if (this._itemSlotViews == null)
			return;

		for (int i = 0; i < this._itemSlotViews.Count; i++)
		{
			this._itemSlotViews[i].Clear(clearMode);
		}
	}

#if UNITY_EDITOR
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryView))]
[CanEditMultipleObjects]
public class InventoryViewEditor : Editor
{
	protected InventoryView sInventoryView;

	private void OnEnable()
	{
#pragma warning disable 0219
		this.sInventoryView = this.target as InventoryView;
#pragma warning restore 0219
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif