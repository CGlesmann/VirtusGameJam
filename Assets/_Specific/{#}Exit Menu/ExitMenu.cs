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

public class ExitMenu : View
{
	private void Update()
	{
		if (Input.GetKeyDown(KeyCode.Escape))
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
[CustomEditor(typeof(ExitMenu))]
[CanEditMultipleObjects]
public class ExitMenuEditor : ViewEditor
{
#pragma warning disable 0219, 414
	private ExitMenu _sExitMenu;
#pragma warning restore 0219, 414

	protected override void OnEnable()
	{
		base.OnEnable();

		this._sExitMenu = this.target as ExitMenu;
	}
}
#endif