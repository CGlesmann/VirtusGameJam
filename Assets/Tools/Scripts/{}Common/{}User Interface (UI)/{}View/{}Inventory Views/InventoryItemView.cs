/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

public class InventoryItemView : View, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
	[SerializeField] InventoryItemSlotView _parentSlotView;
	private InventoryView _parentInventoryView { get { return this._parentSlotView.ParentView; } }

	[SerializeField] private Image _itemIconImageField;
	public Image _ItemIconImageField { get { return this._itemIconImageField; } }

	[SerializeField] private TextMeshProUGUI _itemQuantityTextField;
	public TextMeshProUGUI _ItemQuantityTextField { get { return this._itemQuantityTextField; } }

	public InventoryItem DisplayedItem_ { get; private set; }

	public void Display(InventoryItem item)
	{
		this.DisplayedItem_ = item;

		this.InitializeLayout();
		this.UpdateLayout();
	}

	public virtual void InitializeLayout()
	{
		if (this.DisplayedItem_ == null)
			return;

		if (this.DisplayedItem_.InventoryItemSharedData_._Stackable)
			this._itemQuantityTextField.ActivateGameObject();
		else
			this._itemQuantityTextField.DeactivateGameObject();

		this._itemIconImageField.sprite = this.DisplayedItem_.InventoryItemSharedData_._Icon;
	}

	public virtual void UpdateLayout()
	{
		if (this.DisplayedItem_ == null)
			return;

		if (this.DisplayedItem_.InventoryItemSharedData_._Stackable)
			this._itemQuantityTextField.text = this.DisplayedItem_._Quantity.ToString();
	}

	public void Clear()
	{
		this.DisplayedItem_ = null;
	}

	public virtual void OnPointerEnter(PointerEventData eventData)
	{
		if (this.DisplayedItem_ == null)
			return;

		this._parentInventoryView._ItemTooltipView.Show();
		this._parentInventoryView._ItemTooltipView.Display(this.DisplayedItem_.InventoryItemSharedData_, this._parentSlotView);

		this.DisplayedItem_.OnPointerEnter?.Invoke();
	}

	public virtual void OnPointerExit(PointerEventData eventData)
	{
		if (this.DisplayedItem_ == null)
			return;

		this._parentInventoryView._ItemTooltipView.Hide();

		this.DisplayedItem_.OnPointerExit?.Invoke();
	}

	public virtual void OnPointerDown(PointerEventData eventData)
	{
		if (this.DisplayedItem_ == null)
			return;

		this.DisplayedItem_.OnPointerDown?.Invoke();
	}

	public virtual void OnPointerUp(PointerEventData eventData)
	{
		if (this.DisplayedItem_ == null)
			return;

		this.DisplayedItem_.OnPointerUp?.Invoke();
	}

#if UNITY_EDITOR
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(InventoryItemView))]
[CanEditMultipleObjects]
public class InventoryItemViewEditor : Editor
{
#pragma warning disable 0219, 414
	private InventoryItemView _sInventoryItemView;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sInventoryItemView = this.target as InventoryItemView;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif