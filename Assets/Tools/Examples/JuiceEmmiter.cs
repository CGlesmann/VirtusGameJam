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

public class JuiceEmmiter : MonoBehaviour
{
	[SerializeField] private Juice _juice;

	public void Emmit()
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
[CustomEditor(typeof(JuiceEmmiter))]
[CanEditMultipleObjects]
public class JuiceEmmiterEditor : Editor
{
#pragma warning disable 0219, 414
	private JuiceEmmiter _sJuiceEmmiter;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sJuiceEmmiter = this.target as JuiceEmmiter;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif