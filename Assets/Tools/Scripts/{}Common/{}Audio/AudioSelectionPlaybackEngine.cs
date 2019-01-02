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

public class AudioSelectionPlaybackEngine : MonoBehaviour
{
	public enum PlaybackType { Sequential, Random }

	[SerializeField] private AudioPlayer.AudioType _audioType;
	[SerializeField] private PlaybackType _playbackType;

	[SerializeField] private AudioSelection _audioSelection;
	[SerializeField] private Vector2 _silenceTimeRange = new Vector2(15f, 45f);

	[Range(0f, 1f)]
	[SerializeField] private float _volume = 1f;

	private bool _isPlaying;

	private IEnumerator PlayingProcess(float time)
	{
		this._isPlaying = true;

		yield return new WaitForSeconds(time);

		this._isPlaying = false;
	}

	private Coroutine _playingProcess;

	private void StartPlaying(float time)
	{
		if (this._playingProcess != null)
		{
			this.StopCoroutine(this._playingProcess);
		}

		this._playingProcess = this.StartCoroutine(this.PlayingProcess(time));
	}

	/// <summary>
	/// 
	/// </summary>
	/// <returns>AudioClip length.</returns>
	private float Play(PlaybackType playbackType)
	{
		AudioClip audioClip = this._audioSelection._SelectedAudioClip;

		switch (playbackType)
		{
			case PlaybackType.Sequential:

				audioClip = this._audioSelection.Next();

				break;
			case PlaybackType.Random:

				audioClip = this._audioSelection.GetRandom();

				break;
		}

		AudioPlayer.Instance.Play(audioClip, this._audioType, this._volume);

		this.StartPlaying(audioClip.length);

		return audioClip.length;
	}

	public void Play()
	{
		if (this._audioSelection == null || this._isPlaying)
			return;

		this.Play(this._playbackType);
	}

	private IEnumerator PlayProcess()
	{
		while (true)
		{
			yield return new WaitForSecondsRealtime(this.Play(this._playbackType) + Random.Range(this._silenceTimeRange.x, this._silenceTimeRange.y));
		}
	}

	public void Stop()
	{
		AudioPlayer.Instance.StopAllCoroutines();

		this._isPlaying = false;
	}

	public void PlayLoop()
	{
		if (this._audioSelection == null || this._isPlaying)
			return;

		this.Stop();

		AudioPlayer.Instance.StartCoroutine(this.PlayProcess());
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(AudioSelectionPlaybackEngine))]
[CanEditMultipleObjects]
public class AudioSelectionPlaybackEngineEditor : Editor
{
#pragma warning disable 0219, 414
	private AudioSelectionPlaybackEngine _sAudioSelectionPlaybackEngine;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sAudioSelectionPlaybackEngine = this.target as AudioSelectionPlaybackEngine;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif