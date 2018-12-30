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

[CreateAssetMenu(fileName = "String Data", menuName = "Data/Primitives/String Data")]
public class StringData : ScriptableObject
{
	[TextArea]
	[SerializeField] private string _data;
	public string _Data { get { return this._data; } }

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(StringData))]
[CanEditMultipleObjects]
public class StringDataEditor : Editor
{
#pragma warning disable 0219, 414
	private StringData _sStringData;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sStringData = this.target as StringData;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif