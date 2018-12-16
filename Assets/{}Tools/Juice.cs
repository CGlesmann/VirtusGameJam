using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

[System.Serializable]
public class Juice
{
	[SerializeField] private ParticleSystem _visualEffectPrefab;
	[SerializeField] private AudioClip _soundEffect;

	//! Time
	[SerializeField] private bool _modifyTimeScale = false;
	[SerializeField] private float _targetTimeScale = 0.01f;

	[SerializeField] private bool _useFrames = false;
	[SerializeField] private int _targetTimeScaleDurationInFrames = 3;
	[SerializeField] private float _targetTimeScaleDurationInSeconds = 0.02f;

	[SerializeField] private bool _lerpTimeScale;
	[SerializeField] private float _lerpSpeed = 6.5f;

	//! Camera
	[SerializeField] private bool _addCameraShake = false;

	public void MakeItRain(Transform targetPoint)
	{
		if (this._visualEffectPrefab != null)
		{
			Object.Instantiate(this._visualEffectPrefab, targetPoint.position, Quaternion.identity);
		}

		if (this._soundEffect != null)
		{
			AudioPlayer.Instance.PlayOneShot(this._soundEffect, AudioPlayer.AudioType.SFX);
		}

		if (this._modifyTimeScale)
		{
			if (this._useFrames)
			{
				this.StartCoroutine(TimeController.TimeFreezeProcess(this._targetTimeScale, this._targetTimeScaleDurationInFrames));
			}
			else
			{
				this.StartCoroutine(TimeController.TimeFreezeProcess(this._targetTimeScale, this._targetTimeScaleDurationInSeconds));
			}
		}
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}

	[HideInInspector]
	[SerializeField] private bool _foldoutEO;
#endif
}

#if UNITY_EDITOR
[CustomPropertyDrawer(typeof(Juice))]
[CanEditMultipleObjects]
public class JuicePropertyDrawer : PropertyDrawer
{
#pragma warning disable 0219, 414
	private Juice _sJuice;
#pragma warning restore 0219, 414

	public override float GetPropertyHeight(SerializedProperty serializedProperty, GUIContent label)
	{
		return 0f;
	}

	public override void OnGUI(Rect rect, SerializedProperty serializedProperty, GUIContent label)
	{
		SerializedProperty foldoutEOProp = serializedProperty.FindPropertyRelative("_foldoutEO");

		foldoutEOProp.boolValue = EditorGUILayout.Foldout(foldoutEOProp.boolValue, "Juice");

		if (foldoutEOProp.boolValue)
		{
			EditorGUILayout.LabelField("Particles", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_visualEffectPrefab"), true);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Audio", EditorStyles.boldLabel);
			EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_soundEffect"), true);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Time", EditorStyles.boldLabel);

			SerializedProperty modifyTimeScaleProp = serializedProperty.FindPropertyRelative("_modifyTimeScale");
			EditorGUILayout.PropertyField(modifyTimeScaleProp, true);

			if (modifyTimeScaleProp.boolValue)
			{
				EditorGUI.indentLevel++;

				EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_targetTimeScale"), true);

				SerializedProperty useFramesProp = serializedProperty.FindPropertyRelative("_useFrames");
				EditorGUILayout.PropertyField(useFramesProp, true);

				EditorGUI.indentLevel++;

				if (useFramesProp.boolValue)
				{
					EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_targetTimeScaleDurationInFrames"), true);
				}
				else
				{
					EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_targetTimeScaleDurationInSeconds"), true);
				}

				EditorGUI.indentLevel--;

				SerializedProperty lerpTimeScaleProp = serializedProperty.FindPropertyRelative("_lerpTimeScale");
				EditorGUILayout.PropertyField(lerpTimeScaleProp, true);

				if (lerpTimeScaleProp.boolValue)
				{
					EditorGUI.indentLevel++;

					EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_lerpSpeed"), true);

					EditorGUI.indentLevel--;
				}

				EditorGUI.indentLevel--;
			}

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Camera Shake", EditorStyles.boldLabel);

			SerializedProperty addCameraShakeProp = serializedProperty.FindPropertyRelative("_addCameraShake");
			EditorGUILayout.PropertyField(addCameraShakeProp, true);

			if (addCameraShakeProp.boolValue)
			{
				EditorGUI.indentLevel++;
				


				EditorGUI.indentLevel--;
			}
		}

		serializedProperty.serializedObject.ApplyModifiedProperties();
	}
}
#endif