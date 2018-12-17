using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Cinemachine;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

[System.Serializable]
public class Juice
{
	[SerializeField] private ParticleSystem _visualEffectPrefab;
	[SerializeField] private AudioSelection _soundEffectSelection;

	//! Time
	[SerializeField] private bool _modifyTimeScale = false;
	[SerializeField] private float _targetTimeScale = 0.01f;

	[SerializeField] private bool _timeUseFrames = false;
	[SerializeField] private int _targetTimeScaleDurationInFrames = 3;
	[SerializeField] private float _targetTimeScaleDurationInSeconds = 0.02f;

	[SerializeField] private bool _lerpTimeScale;
	[SerializeField] private float _lerpSpeed = 6.5f;

	//! Camera
	//[SerializeField] private bool _addCameraShake = false;

	[SerializeField] private CinemachineImpulseSource _impulseSource;

	//! Flash
	[SerializeField] private Image _flashImage;

	[SerializeField] private Gradient _flashGradient;

	[SerializeField] private bool _flashUseFrames = false;
	[SerializeField] private int _flashFrames = 3;

	[Range(0.01f, 100f)]
	[SerializeField] private float _flashSpeed = 2f;

	[SerializeField] private bool _flashUseCustomAlphaCurve = true;
	[SerializeField] private AnimationCurve _flashAnimationCurve;

	private IEnumerator ScreenFlashProcess()
	{
		float progress = 0f;
		Color color;

		if (this._flashUseFrames)
		{
			for (int i = 0; i < this._flashFrames; i++)
			{
				color = this._flashGradient.Evaluate(progress);

				if (this._flashUseCustomAlphaCurve)
					color.a = this._flashAnimationCurve.Evaluate(progress);

				this._flashImage.color = color;

				progress = i / (float)this._flashFrames;

				yield return null;
			}
		}
		else
		{
			while (progress < 1f)
			{
				color = this._flashGradient.Evaluate(progress);

				if (this._flashUseCustomAlphaCurve)
					color.a = this._flashAnimationCurve.Evaluate(progress);

				this._flashImage.color = color;

				progress += Time.unscaledDeltaTime * this._flashSpeed;

				yield return null;
			}
		}

		color = this._flashGradient.Evaluate(1f);

		if (this._flashUseCustomAlphaCurve)
			color.a = 0f;

		this._flashImage.color = color;
	}

	public ParticleSystem MakeItRain(Transform targetPoint)
	{
		ParticleSystem particleSystem = null;

		if (this._visualEffectPrefab != null)
		{
			particleSystem = Object.Instantiate(this._visualEffectPrefab, targetPoint.position, Quaternion.identity);
		}

		if (this._soundEffectSelection != null)
		{
			AudioPlayer.Instance.PlayOneShot(this._soundEffectSelection.GetRandom(), AudioPlayer.AudioType.SFX);
		}

		if (this._modifyTimeScale)
		{
			if (this._timeUseFrames)
			{
				this.StartCoroutine(TimeController.TimeFreezeProcess(this._targetTimeScale, this._targetTimeScaleDurationInFrames));
			}
			else
			{
				this.StartCoroutine(TimeController.TimeFreezeProcess(this._targetTimeScale, this._targetTimeScaleDurationInSeconds));
			}
		}

		if (this._impulseSource != null)
		{
			this._impulseSource.GenerateImpulse();
		}

		if (this._flashImage != null)
		{
			this.StartCoroutine(this.ScreenFlashProcess());
		}

		return particleSystem;
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
			EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_soundEffectSelection"), true);

			EditorGUILayout.Space();

			EditorGUILayout.LabelField("Time", EditorStyles.boldLabel);

			SerializedProperty modifyTimeScaleProp = serializedProperty.FindPropertyRelative("_modifyTimeScale");
			EditorGUILayout.PropertyField(modifyTimeScaleProp, true);

			if (modifyTimeScaleProp.boolValue)
			{
				EditorGUI.indentLevel++;

				EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_targetTimeScale"), true);

				SerializedProperty useFramesProp = serializedProperty.FindPropertyRelative("_timeUseFrames");
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

			//SerializedProperty addCameraShakeProp = serializedProperty.FindPropertyRelative("_addCameraShake");
			//EditorGUILayout.PropertyField(addCameraShakeProp, true);

			//if (addCameraShakeProp.boolValue)
			//{
			//EditorGUI.indentLevel++;

			EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_impulseSource"), true);

			//EditorGUI.indentLevel--;
			//}

			EditorGUILayout.LabelField("Screen Flash", EditorStyles.boldLabel);

			SerializedProperty flashImageProp = serializedProperty.FindPropertyRelative("_flashImage");
			EditorGUILayout.PropertyField(flashImageProp, true);

			if (flashImageProp.objectReferenceValue != null)
			{
				EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_flashGradient"), true);

				SerializedProperty flashUseFramesProp = serializedProperty.FindPropertyRelative("_flashUseFrames");
				EditorGUILayout.PropertyField(flashUseFramesProp, true);

				EditorGUI.indentLevel++;

				if (flashUseFramesProp.boolValue)
					EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_flashFrames"), true);
				else
					EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_flashSpeed"), true);

				EditorGUI.indentLevel--;

				SerializedProperty flashUseCustomAlphaProp = serializedProperty.FindPropertyRelative("_flashUseCustomAlphaCurve");
				EditorGUILayout.PropertyField(flashUseCustomAlphaProp, true);

				if (flashUseCustomAlphaProp.boolValue)
				{
					EditorGUI.indentLevel++;

					EditorGUILayout.PropertyField(serializedProperty.FindPropertyRelative("_flashAnimationCurve"), true);

					EditorGUI.indentLevel--;
				}
			}
		}

		serializedProperty.serializedObject.ApplyModifiedProperties();
	}
}
#endif