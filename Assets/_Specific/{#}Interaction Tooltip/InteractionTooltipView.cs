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

public class InteractionTooltipView : View
{
	[SerializeField] private Transform _target;

	[SerializeField] private Vector2 _offset = new Vector3(0f, 5f);

	private void FixedUpdate()
	{
		Vector2 newAnchoredPosition = Camera.main.WorldToScreenPoint(this._target.position);

		this.rectTransform.anchoredPosition = newAnchoredPosition + this._offset;
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(InteractionTooltipView))]
[CanEditMultipleObjects]
public class InteractionTooltipViewEditor : ViewEditor
{
#pragma warning disable 0219, 414
	private InteractionTooltipView _sInteractionTooltipView;
#pragma warning restore 0219, 414

	protected override void OnEnable()
	{
		base.OnEnable();

		this._sInteractionTooltipView = this.target as InteractionTooltipView;
	}
}
#endif