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
	[SerializeField] private AudioPlayer.AudioType _audioType;

	[SerializeField] private AudioSelection _audioSelection;
	[SerializeField] private Vector2 _silenceTimeRange = new Vector2(15f, 45f);

    private IEnumerator PlayProcess()
	{
		while (true)
		{
			AudioClip audioClip = this._audioSelection.GetRandom();
			
			AudioPlayer.Instance.PlayOneShot(audioClip, this._audioType);

			yield return new WaitForSecondsRealtime(audioClip.length + Random.Range(this._silenceTimeRange.x, this._silenceTimeRange.y));
		}
	}

	public void Stop()
	{
		this.StopAllCoroutines();
	}

	public void Play()
	{
		this.Stop();

		this.StartCoroutine(this.PlayProcess());
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