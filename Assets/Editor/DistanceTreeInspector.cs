using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(RenderTreeHandler))]
// ^ This is the script we are making a custom editor for.
public class DistanceTreeInspector : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();
        //This draws the default screen.  You don't need this if you want
        //to start from scratch, but I use this when I'm just adding a button or
        //some small addition and don't feel like recreating the whole inspector.
        if (GUILayout.Button("Save Config"))
        {
            (target as RenderTreeHandler).UpdateLinearTree();
        }
    }
}