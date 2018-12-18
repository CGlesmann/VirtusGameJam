using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Door))]
public class DoorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        // Drawing the base Inspector
        base.OnInspectorGUI();

        // Getting the Target Door
        Door door = (Door)target;

        // Drawing the Set Button
        EditorGUILayout.Separator();
        GUILayout.Label("Door Position Options", EditorStyles.boldLabel);

        // Set Open Position
        if (GUILayout.Button("ClosedPosition to Current"))
        {
            door.SetClosedPositionToCurrent();
            EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());
        }

        // Set Closed Position
        if (GUILayout.Button("OpenPosition to Current"))
        {
            door.SetOpenPositionToCurrent();
        }

        // Drawing the Set Button
        EditorGUILayout.Separator();
        GUILayout.Label("Door State Options", EditorStyles.boldLabel);

        // Set To Opened
        if (GUILayout.Button("Set To Open"))
        {
            door.SetDoorToOpened();
        }

        // Set To Closed
        if (GUILayout.Button("Set To Closed"))
        {
            door.SetDoorToClosed();
        }
    }

}
