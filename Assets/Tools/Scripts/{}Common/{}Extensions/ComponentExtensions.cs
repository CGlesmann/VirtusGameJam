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

public static class ComponentExtensions
{
	public static void ActivateGameObject(this Component component)
	{
		component.gameObject.SetActive(true);
	}

	public static void DeactivateGameObject(this Component component)
	{
		component.gameObject.SetActive(false);
	}
}