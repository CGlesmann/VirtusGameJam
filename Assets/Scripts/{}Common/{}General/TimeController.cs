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

public static class TimeController
{
	public static IEnumerator TimeFreezeProcess(float timeScale, int frames)
	{
		Time.timeScale = timeScale;

		for (int i = 0; i < frames; i++)
		{
			yield return null;
		}

		Time.timeScale = 1;
	}

	public static IEnumerator TimeFreezeProcess(float timeScale, float durationInSeconds)
	{
		Time.timeScale = timeScale;

		yield return new WaitForSecondsRealtime(durationInSeconds);

		Time.timeScale = 1;
	}

#if UNITY_EDITOR
#endif
}

//#if UNITY_EDITOR
//[CustomEditor(typeof(TimeController))]
//[CanEditMultipleObjects]
//public class TimeControllerEditor : Editor
//{
//#pragma warning disable 0219, 414
//	private TimeController _sTimeController;
//#pragma warning restore 0219, 414

//	private void OnEnable()
//	{
//		this._sTimeController = this.target as TimeController;
//	}

//	public override void OnInspectorGUI()
//	{
//		this.DrawDefaultInspector();
//	}
//}
//#endif