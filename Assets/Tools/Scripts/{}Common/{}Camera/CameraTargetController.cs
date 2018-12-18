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

public class CameraTargetController : MonoBehaviour
{
	public Transform Target;

	public bool LockX;
	public bool LockY;
	public bool LockZ;

	private void Update()
	{
		this.transform.position =
			new Vector3(
				this.LockX ? this.transform.position.x : this.Target.position.x,
				this.LockY ? this.transform.position.y : this.Target.position.y,
				this.LockZ ? this.transform.position.z : this.Target.position.z
			);
	}

#if UNITY_EDITOR
#endif
}


#if UNITY_EDITOR
[CustomEditor(typeof(CameraTargetController))]
[CanEditMultipleObjects]
public class CameraTargetControllerEditor : Editor
{
#pragma warning disable 0219, 414
	private CameraTargetController _sCameraTargetController;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sCameraTargetController = this.target as CameraTargetController;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif