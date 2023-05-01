using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(CameraRig))]
public class CameraRigEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        CameraRig obj = (CameraRig)target;

        if (GUILayout.Button("Set to Cinematic Angle"))
        {
            obj.SetToCinematicAngle();
        }

        if (GUILayout.Button("Set to Gameplay Angle"))
        {
            obj.SetToGameplayAngle();
        }

        if (GUILayout.Button("Fade to Black"))
        {
            obj.FadeToBlack();
        }

        if (GUILayout.Button("Fade from Black"))
        {
            obj.FadeFromBlack();
        }
    }
}