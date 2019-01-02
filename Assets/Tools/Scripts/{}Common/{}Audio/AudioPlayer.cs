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
public class AudioPlayer : MonoBehaviourSingleton<AudioPlayer>
{
	public enum AudioType { Music, SFX }

	[SerializeField] private AudioSource _musicAudioSource;
	[SerializeField] private AudioSource _sfxAudioSource;

	private bool _musicMuted;
	public bool MusicMuted_
	{
		get { return this._musicMuted; }
		private set
		{
			this._musicMuted = value;

			PlayerPrefs.SetInt("MusicMuted", this._musicMuted ? 1 : 0);
		}
	}

	private bool _sfxMuted;
	public bool SFXMuted_
	{
		get { return this._sfxMuted; }
		private set
		{
			this._sfxMuted = value;

			PlayerPrefs.SetInt("SFXMuted", this._sfxMuted ? 1 : 0);
		}
	}

	public void Mute(AudioType audioType)
	{
		switch (audioType)
		{
			case AudioType.Music:

				this.MusicMuted_ = true;

				this._musicAudioSource.mute = this.MusicMuted_;

				break;
			case AudioType.SFX:

				this.SFXMuted_ = true;

				this._sfxAudioSource.mute = this.SFXMuted_;

				break;
		}
	}

	public void UnMute(AudioType audioType)
	{
		switch (audioType)
		{
			case AudioType.Music:

				this.MusicMuted_ = false;

				this._musicAudioSource.mute = this.MusicMuted_;

				break;
			case AudioType.SFX:

				this.SFXMuted_ = false;

				this._sfxAudioSource.mute = this.SFXMuted_;

				break;
		}
	}

	public void ToggleMute(AudioType audioType)
	{
		bool muted = true;

		switch (audioType)
		{
			case AudioType.Music:

				muted = this.MusicMuted_;

				break;
			case AudioType.SFX:

				muted = this.SFXMuted_;

				break;
		}

		if (muted)
		{
			this.UnMute(audioType);
		}
		else
		{
			this.Mute(audioType);
		}
	}

	public void MuteMusic() { this.Mute(AudioType.Music); }
	public void UnMuteMusic() { this.UnMute(AudioType.Music); }

	public void MuteSFX() { this.Mute(AudioType.SFX); }
	public void UnMuteSFX() { this.UnMute(AudioType.SFX); }

	public void Play(AudioClip audioClip, AudioType audioType)
	{
		switch (audioType)
		{
			case AudioType.Music:

				this.PlayFading(this._musicAudioSource, 20f, audioClip, 2f);

				break;
			case AudioType.SFX:

				this._sfxAudioSource.PlayOneShot(audioClip);

				break;
		}
	}
	public void PlayMusic(AudioClip audioClip) { this.Play(audioClip, AudioType.Music); }
	public void PlaySFX(AudioClip audioClip) { this.Play(audioClip, AudioType.SFX); }

	public void Play(AudioClip audioClip, AudioType audioType, float volumeScale)
	{
		switch (audioType)
		{
			case AudioType.Music:

				this.PlayFading(this._musicAudioSource, 20f, audioClip, 2f, volumeScale);

				break;
			case AudioType.SFX:

				this._sfxAudioSource.PlayOneShot(audioClip, volumeScale);

				break;
		}
	}

	private IEnumerator PlayFadingProcess(AudioSource audioSource, float fadeTime, AudioClip audioClip, float delayTime, float volumeScale)
	{
		fadeTime /= 2f;

		float progress = 1f - volumeScale;

		if (audioSource.isPlaying)
		{
			float initialVolumeScale = audioSource.volume;

			while (progress <= fadeTime)
			{
				float calculatedProgress = (progress / fadeTime);

				audioSource.volume = Mathf.Lerp(initialVolumeScale, 0f, calculatedProgress);

				progress += Time.unscaledDeltaTime;

				yield return null;
			}

			yield return new WaitForSecondsRealtime(delayTime);
		}

		audioSource.Stop();
		audioSource.clip = audioClip;
		audioSource.Play();

		progress = 0f;
		while (progress <= fadeTime)
		{
			float calculatedProgress = (progress / fadeTime);

			audioSource.volume = Mathf.Lerp(0f, volumeScale, calculatedProgress);

			progress += Time.unscaledDeltaTime;

			yield return null;
		}

		audioSource.volume = volumeScale;
	}

	private Coroutine _playFadingProcess;

	private void PlayFading(AudioSource audioSource, float fadeTime, AudioClip audioClip, float delayTime, float volumeScale)
	{
		if (this._playFadingProcess != null)
			audioSource.StopCoroutine(this._playFadingProcess);

		this._playFadingProcess = audioSource.StartCoroutine(this.PlayFadingProcess(audioSource, fadeTime, audioClip, delayTime, volumeScale));
	}

	private void PlayFading(AudioSource audioSource, float fadeTime, AudioClip audioClip, float delayTime)
	{
		this.PlayFading(audioSource, fadeTime, audioClip, delayTime, audioSource.volume);
	}

	private void PlayFading(AudioSource audioSource, float fadeTime, AudioClip audioClip)
	{
		this.PlayFading(audioSource, fadeTime, audioClip, 0f, audioSource.volume);
	}

	private IEnumerator PlayAudioSelectionProcess(AudioSelection audioSelection, AudioType audioType)
	{
		while (true)
		{
			this.Play(audioSelection.Next(), audioType);

			yield return new WaitForSecondsRealtime(audioSelection._SelectedAudioClip.length + 1f);
		}
	}

	public void PlayAudioSelection(AudioSelection audioSelection, AudioType audioType)
	{
		this.StartCoroutine(this.PlayAudioSelectionProcess(audioSelection, audioType));
	}

	private AudioSelectionPlaybackEngine _musicPlaybackEngine;

	protected override void Awake()
	{
		base.Awake();

		this._musicPlaybackEngine = this.GetComponent<AudioSelectionPlaybackEngine>();

		if (PlayerPrefs.GetInt("MusicMuted") == 1)
			this.MuteMusic();
		else
			this.UnMuteMusic();

		if (PlayerPrefs.GetInt("SFXMuted") == 1)
			this.MuteSFX();
		else
			this.UnMuteSFX();
	}

	private void Start()
	{
		this._musicPlaybackEngine.PlayLoop();
	}

#if UNITY_EDITOR
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(AudioPlayer))]
[CanEditMultipleObjects]
public class AudioPlayerEditor : Editor
{
#pragma warning disable 0219, 414
	private AudioPlayer _sAudioPlayer;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sAudioPlayer = this.target as AudioPlayer;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif