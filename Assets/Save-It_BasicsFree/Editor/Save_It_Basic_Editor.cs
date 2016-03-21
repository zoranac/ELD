using UnityEngine;
using System.Collections;
using UnityEditor;

[CustomEditor(typeof(Save_Load_BasicSystem))]
public class Save_It_Basic_Editor : Editor 
{
    public Texture Logo_SAVE_IT;

 

    public override void OnInspectorGUI()
    {
        EditorGUILayout.HelpBox("Warning!"+System.Environment.NewLine+System.Environment.NewLine+"This is only free version of SAVE-IT plugin. Check Assets Store to purchase full version with all stuff.", MessageType.Warning);

        GUILayout.Label(Logo_SAVE_IT);

        EditorGUILayout.HelpBox("Available Stuff In This Free Version:"+System.Environment.NewLine+System.Environment.NewLine+"- Save & Load cube position"
            +System.Environment.NewLine+"- Full solution"
            +System.Environment.NewLine+"- Editable directory path", MessageType.Info);

        DrawDefaultInspector();
    }
    
}
