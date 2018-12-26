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

public class HealthBarView : View
{
	[SerializeField] private Slider _healthFill;

	private float _fill;
	public float Fill { get { return this._fill; } set { this._fill = value; this._healthFill.value = this._fill; } }

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(HealthBarView))]
[CanEditMultipleObjects]
public class HealthBarViewEditor : ViewEditor
{
#pragma warning disable 0219, 414
	private HealthBarView _sHealthBarView;
#pragma warning restore 0219, 414

	protected override void OnEnable()
	{
		base.OnEnable();

		this._sHealthBarView = this.target as HealthBarView;
	}
}
#endif