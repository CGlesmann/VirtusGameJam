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

[CreateAssetMenu(fileName = "Level 1 Diary Data 1", menuName = "Data/Diary Data")]
public class DiaryData : ScriptableObject
{
	[SerializeField] private string _title;
	public string _Title { get { return this._title; } }

	[TextArea(40, int.MaxValue)]
	[SerializeField] private string _text;
	public string _Text { get { return this._text; } }

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(DiaryData))]
[CanEditMultipleObjects]
public class DiaryDataEditor : Editor
{
#pragma warning disable 0219, 414
	private DiaryData _sDiaryData;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sDiaryData = this.target as DiaryData;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif