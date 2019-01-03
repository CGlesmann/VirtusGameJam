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

public class AudioPlaybackEngine : MonoBehaviour
{
	[SerializeField] private AudioPlayer.AudioType _audioType;

	[SerializeField] private AudioClip _audioClip;

	[Range(0f, 1f)]
	[SerializeField] private float _volume = 1f;

	public void Play()
	{
		AudioPlayer.Instance.Play(this._audioClip, this._audioType, this._volume);
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(AudioPlaybackEngine))]
[CanEditMultipleObjects]
public class AudioPlaybackEngineEditor : Editor
{
#pragma warning disable 0219, 414
	private AudioPlaybackEngine _sAudioPlaybackEngine;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sAudioPlaybackEngine = this.target as AudioPlaybackEngine;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif