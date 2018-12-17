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

public class GameManager : MonoBehaviourSingleton<GameManager>
{
#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(GameManager))]
[CanEditMultipleObjects]
public class GameManagerEditor : Editor
{
#pragma warning disable 0219, 414
	private GameManager _sGameManager;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sGameManager = this.target as GameManager;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif