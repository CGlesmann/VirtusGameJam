/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

[RequireComponent(typeof(Image))]
public class InventoryItemSlotView : View
{
	public InventoryView ParentView { get; set; }

	[SerializeField] private InventoryItemView _itemView;
	public InventoryItemView _ItemView { get { return this._itemView; } }

	public void Clear(InventoryView.ClearMode clearMode)
	{
		switch (clearMode)
		{
			case InventoryView.ClearMode.Partial:

				this._itemView.Close();

				break;
			case InventoryView.ClearMode.Full:

				this.Close();

				break;
			case InventoryView.ClearMode.Destruction:

				Object.Destroy(this.gameObject);

				break;
		}
	}

#if UNITY_EDITOR
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryItemSlotView))]
[CanEditMultipleObjects]
public class InventoryItemSlotViewEditor : Editor
{
	protected InventoryItemSlotView sInventoryItemSlotView;

	private void OnEnable()
	{
#pragma warning disable 0219
		this.sInventoryItemSlotView = this.target as InventoryItemSlotView;
#pragma warning restore 0219
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif