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

public class JuiceEmmiterTest : MonoBehaviour
{
	[SerializeField] private Juice _juice;

	public void Exec()
	{
		ParticleSystem particleSystem = this._juice.MakeItRain(this.transform);

		//Destroy(particleSystem.gameObject, particleSystem.main.duration);
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(JuiceEmmiterTest))]
[CanEditMultipleObjects]
public class JuiceEmmiterTestEditor : Editor
{
#pragma warning disable 0219, 414
	private JuiceEmmiterTest _sJuiceEmmiterTest;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sJuiceEmmiterTest = this.target as JuiceEmmiterTest;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif