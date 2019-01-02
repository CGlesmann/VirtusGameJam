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

public class MonoBehaviourSingleton<T> : MonoBehaviour
	where T : MonoBehaviourSingleton<T>
{
	public static T Instance { get; private set; }

	protected virtual void Awake()
	{
		if (Instance == null)
		{
			Instance = this as T;

			Transform target = Instance.transform;

			while (target.parent != null)
				target = target.parent;

			Object.DontDestroyOnLoad(target.gameObject);
		}
		else if (Instance != this)
		{
			if (this.transform.parent != null && this.transform.parent.childCount == 1)
				Object.Destroy(this.transform.parent.gameObject);
			else
				Object.Destroy(this.gameObject);
		}
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}