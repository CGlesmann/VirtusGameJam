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

public class InventoryItemTooltipView : View
{
	[SerializeField] private TextMeshProUGUI _descriptionTextField;

	[SerializeField] private Vector2 _offset;

	public void Display(InventoryItemSharedData inventoryItemSharedData, InventoryItemSlotView inventoryItemSlotView)
	{
		this._descriptionTextField.text = inventoryItemSharedData._Description;
		
		this.rectTransform.anchoredPosition = inventoryItemSlotView._RectTransform.anchoredPosition + this._offset;
	}

#if UNITY_EDITOR
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryItemTooltipView))]
[CanEditMultipleObjects]
public class InventoryItemTooltipViewEditor : ViewEditor
{
#pragma warning disable 0219, 414
	private InventoryItemTooltipView _sInventoryItemTooltipView;
#pragma warning restore 0219, 414

	protected override void OnEnable()
	{
		base.OnEnable();

		this._sInventoryItemTooltipView = this.target as InventoryItemTooltipView;
	}
}
#endif