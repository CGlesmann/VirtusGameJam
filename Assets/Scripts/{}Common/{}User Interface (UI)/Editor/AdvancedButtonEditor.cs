using UnityEditor;
using UnityEditor.UI;


#if UNITY_EDITOR
[CustomEditor(typeof(AdvancedButton))]
[CanEditMultipleObjects]
public class AdvancedButtonEditor : SelectableEditor
{
#pragma warning disable 0219, 414
	private AdvancedButton _sAdvancedButton;
#pragma warning restore 0219, 414

	protected override void OnEnable()
	{
		base.OnEnable();

		this._sAdvancedButton = this.target as AdvancedButton;
	}

	public override void OnInspectorGUI()
	{
		base.OnInspectorGUI();

		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_onPointerEnter"), true);
		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_onPointerExit"), true);
		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_onPointerDown"), true);
		EditorGUILayout.PropertyField(this.serializedObject.FindProperty("_onPointerUp"), true);

		this.serializedObject.ApplyModifiedProperties();
	}
}
#endif