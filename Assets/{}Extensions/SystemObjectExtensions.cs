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

public static class SystemObjectExtensions
{
	public static void StartCoroutine(this object @object, IEnumerator routine)
	{
		GameManager.Instance.StartCoroutine(routine);
	}
}