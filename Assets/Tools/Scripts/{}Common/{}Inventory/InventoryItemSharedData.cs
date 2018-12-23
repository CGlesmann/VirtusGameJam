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

[System.Serializable]
[CreateAssetMenu(fileName = "name Inventory Item Shared Data", menuName = "Inventory/Item Shared Data", order = 1)]
public class InventoryItemSharedData : ScriptableObject
{
	public int Id { get; set; }

	[SerializeField] private bool _stackable;
	public bool _Stackable { get { return this._stackable; } }

	[SerializeField] private Sprite _icon;
	public Sprite _Icon { get { return this._icon; } }

	[TextArea]
	[SerializeField] private string _description;
	public string _Description { get { return this._description; } }
}

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryItemSharedData))]
[CanEditMultipleObjects]
public class InventoryItemSharedDataEditor : Editor
{
#pragma warning disable 0219, 414
	private InventoryItemSharedData _sInventoryItemSharedData;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sInventoryItemSharedData = this.target as InventoryItemSharedData;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif