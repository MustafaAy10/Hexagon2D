using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Hexagon2D.UI.PanelSystem;

[CustomEditor(typeof(ScorePanel))]
public class ScorePanelEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if(GUILayout.Button("PlayerPrefs Delete All"))
        {
            PlayerPrefs.DeleteAll();
        }
    }
}
