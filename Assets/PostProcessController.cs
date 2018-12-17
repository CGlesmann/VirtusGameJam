/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.Rendering.PostProcessing;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

[RequireComponent(typeof(PostProcessVolume))]
public class PostProcessController : MonoBehaviourSingleton<PostProcessController>
{
	public static PostProcessVolume GlobalPostProcessVolume_ { get; private set; }

	protected override void Awake()
	{
		base.Awake();

		if (PostProcessController.GlobalPostProcessVolume_ == null)
		{
			PostProcessController.GlobalPostProcessVolume_ = this.GetComponent<PostProcessVolume>();

			if (!PostProcessController.GlobalPostProcessVolume_.isGlobal)
				Debug.LogError("Post Process Volume isn't global.");
		}
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(PostProcessController))]
[CanEditMultipleObjects]
public class PostProcessControllerEditor : Editor
{
#pragma warning disable 0219, 414
	private PostProcessController _sPostProcessController;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sPostProcessController = this.target as PostProcessController;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif