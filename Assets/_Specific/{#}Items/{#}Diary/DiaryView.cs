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

[RequireComponent(typeof(AudioSelectionPlaybackEngine))]
public class DiaryView : View
{
	[SerializeField] private TextMeshProUGUI _titleTextField;
	[SerializeField] private TextMeshProUGUI _textTextField;

	private AudioSelectionPlaybackEngine _playbackEngine;

	public void Display(DiaryData diaryData)
	{
		this._titleTextField.text = diaryData._Title;
		this._textTextField.text = diaryData._Text;

		if (Random.value <= 1f)
		{
			this._playbackEngine.Play();
		}
	}

	public override void Hide()
	{
		base.Hide();

		this._playbackEngine.Stop();
	}

	protected override void Awake()
	{
		base.Awake();

		this._playbackEngine = this.GetComponent<AudioSelectionPlaybackEngine>();
	}

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(DiaryView))]
[CanEditMultipleObjects]
public class DiaryViewEditor : ViewEditor
{
#pragma warning disable 0219, 414
	private DiaryView _sDiaryView;
#pragma warning restore 0219, 414

	protected override void OnEnable()
	{
		base.OnEnable();

		this._sDiaryView = this.target as DiaryView;
	}
}
#endif