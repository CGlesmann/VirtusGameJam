/* Created by Luna.Ticode */

using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

#if UNITY_EDITOR
using UnityEditor;
#endif

using TMPro;

public class SceneOperator : MonoBehaviour
{
	[SerializeField] private UnityEventFloat _onLoadingSceneAsync;
	public UnityEventFloat _OnLoadingSceneAsync { get { return this._onLoadingSceneAsync; } }

	public void LoadSceneAsync(int sceneBuildIndex, LoadSceneMode loadSceneMode = LoadSceneMode.Single)
	{
		SceneController.Instance.LoadSceneAsync(sceneBuildIndex, loadSceneMode, this._onLoadingSceneAsync);
	}
	
	#region Next
	public void LoadNextScene(LoadSceneMode loadSceneMode) { SceneController.Instance.LoadNextScene(loadSceneMode); }
	public void LoadNextScene() { this.LoadNextScene(LoadSceneMode.Single); }

	//! Async
	public void LoadNextSceneAsync(LoadSceneMode loadSceneMode) { SceneController.Instance.LoadNextSceneAsync(loadSceneMode); }
	public void LoadNextSceneAsync() { this.LoadNextSceneAsync(LoadSceneMode.Single); }
	#endregion
	
	#region Previous
	public void LoadPreviousScene(LoadSceneMode loadSceneMode) { SceneController.Instance.LoadPreviousScene(loadSceneMode); }
	public void LoadPreviousScene() { SceneController.Instance.LoadPreviousScene(); }

	//! Async
	public void LoadPreviousSceneAsync(LoadSceneMode loadSceneMode) { SceneController.Instance.LoadPreviousSceneAsync(loadSceneMode); }
	public void LoadPreviousSceneAsync() { SceneController.Instance.LoadPreviousSceneAsync(); }
	#endregion

	public void Exit() { SceneController.Instance.Exit(); }

#if UNITY_EDITOR
	//protected override void OnDrawGizmos()
	//{
	//}
#endif
}

#if UNITY_EDITOR
[CustomEditor(typeof(SceneOperator))]
[CanEditMultipleObjects]
public class SceneOperatorEditor : Editor
{
#pragma warning disable 0219, 414
	private SceneOperator _sSceneOperator;
#pragma warning restore 0219, 414

	private void OnEnable()
	{
		this._sSceneOperator = this.target as SceneOperator;
	}

	public override void OnInspectorGUI()
	{
		this.DrawDefaultInspector();
	}
}
#endif