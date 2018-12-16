/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using System.IO;
using System;

using Random = UnityEngine.Random;
using Object = UnityEngine.Object;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

[CreateAssetMenu(fileName = "name Audio Selection", menuName = "Audio/Audio Selection", order = 81)]
public class AudioSelection : ScriptableObject
{
	[SerializeField] private AudioClip[] _audioClips;

	private int _selectedAudioClipIndex = 0;

	public AudioClip _SelectedAudioClip { get { return this._audioClips[this._selectedAudioClipIndex]; } }

	public AudioClip Previous(bool loop = true)
	{
		this._selectedAudioClipIndex = loop ?
			((--this._selectedAudioClipIndex % this._audioClips.Length) + this._audioClips.Length) % this._audioClips.Length :
			Mathf.Clamp(--this._selectedAudioClipIndex, 0, this._audioClips.Length - 1);

		return this._audioClips[this._selectedAudioClipIndex];
	}

	public AudioClip Next(bool loop = true)
	{
		this._selectedAudioClipIndex = loop ?
			((++this._selectedAudioClipIndex % this._audioClips.Length) + this._audioClips.Length) % this._audioClips.Length :
			Mathf.Clamp(++this._selectedAudioClipIndex, 0, this._audioClips.Length - 1);

		return this._audioClips[this._selectedAudioClipIndex];
	}

	public AudioClip GetRandom()
	{
		return this._audioClips[Random.Range(0, this._audioClips.Length)];
	}

	public void Reset()
	{
		this._selectedAudioClipIndex = 0;
	}

	private void OnEnable()
	{
		this.Reset();
	}

#if UNITY_EDITOR
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(AudioSelection))]
[CanEditMultipleObjects]
public class AudioSelectionEditor : Editor
{
#pragma warning disable 0219, 414
	private AudioSelection _sAudioSelection;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sAudioSelection = this.target as AudioSelection;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif