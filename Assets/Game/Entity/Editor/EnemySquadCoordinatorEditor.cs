using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(EntitySquadCoordinator))]

public class EntitySquadCoordinatorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        EntitySquadCoordinator obj = (EntitySquadCoordinator)target;

        if(GUILayout.Button("Spawn Enemy Squad"))
        {
            obj.SpawnEnemySquad();
        }

        if (GUILayout.Button("Spawn Mail Lover Squad"))
        {
            obj.SpawnMailLoverSquad();
        }
    }
}