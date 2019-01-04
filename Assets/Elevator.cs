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

public class Elevator : MonoBehaviour
{
	private IEnumerator OnTriggerEnter2D(Collider2D other)
	{
		yield return new WaitForSeconds(0.8f);

		SceneController.Instance.LoadPreviousScene();
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(Elevator))]
[CanEditMultipleObjects]
public class ElevatorEditor : Editor
{
#pragma warning disable 0219, 414
	private Elevator _sElevator;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sElevator = this.target as Elevator;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif