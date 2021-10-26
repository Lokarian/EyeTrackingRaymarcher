using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(Transform))]
public class WorldPosViewer : Editor
{

    public override void OnInspectorGUI()
    {
        var castTarget = target as Transform;
        DrawDefaultInspector();
        EditorGUILayout.BeginHorizontal ();
        EditorGUILayout.Vector3Field("World Pos", castTarget.position);
        //this will display the target's world pos.
        EditorGUILayout.EndHorizontal ();
    }
}