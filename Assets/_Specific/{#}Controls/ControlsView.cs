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

public class ControlsView : View
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.F1))
		{
			this.ToggleVisibility();
		}
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(ControlsView))]
[CanEditMultipleObjects]
public class ControlsViewEditor : ViewEditor
{
#pragma warning disable 0219, 414
	private ControlsView _sControlsView;
#pragma warning restore 0219, 414

	protected override void OnEnable()
	{
		base.OnEnable();

		this._sControlsView = this.target as ControlsView;
	}
}
#endif