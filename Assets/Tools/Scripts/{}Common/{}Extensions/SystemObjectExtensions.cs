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
	public static Coroutine StartCoroutine(this object @object, IEnumerator routine)
	{
		return GameManager.Instance.StartCoroutine(routine);
	}

	public static void StopCoroutine(this object @object, Coroutine coroutine)
	{
		GameManager.Instance.StopCoroutine(coroutine);
	}

	public static void StopCoroutine(this object @object, IEnumerator routine)
	{
		GameManager.Instance.StopCoroutine(routine);
	}
}