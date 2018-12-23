/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class InventoryItemDatabaseController : MonoBehaviourSingleton<InventoryItemDatabaseController>
{
	private const int DEFAULT_FREE_ITEM_ID = -1;
	private static int s_freeItemId = DEFAULT_FREE_ITEM_ID;

	private Dictionary<int, InventoryItemSharedData> _itemId_inventoryItemSharedDataRelation;

	[SerializeField] private InventoryItemSharedData[] _inventoryItemSharedData;

	public InventoryItemSharedData FetchSharedData(int itemId)
	{
		return this._itemId_inventoryItemSharedDataRelation[itemId];
	}

	private void InitializeDatabase()
	{
		InventoryItemDatabaseController.s_freeItemId = DEFAULT_FREE_ITEM_ID;

		this._itemId_inventoryItemSharedDataRelation = new Dictionary<int, InventoryItemSharedData>(this._inventoryItemSharedData.Length);

		for (int i = 0; i < this._inventoryItemSharedData.Length; i++)
		{
			// Index ItemSharedData
			this._inventoryItemSharedData[i].Id = ++InventoryItemDatabaseController.s_freeItemId;

			this._itemId_inventoryItemSharedDataRelation.Add(this._inventoryItemSharedData[i].Id, this._inventoryItemSharedData[i]);
		}
	}

	protected override void Awake()
	{
		base.Awake();

		this.InitializeDatabase();
	}

#if UNITY_EDITOR
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryItemDatabaseController))]
[CanEditMultipleObjects]
public class InventoryItemDatabaseControllerEditor : Editor
{
	protected InventoryItemDatabaseController sInventoryItemDatabaseController;

	private void OnEnable()
	{
#pragma warning disable 0219
		this.sInventoryItemDatabaseController = this.target as InventoryItemDatabaseController;
#pragma warning restore 0219
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif