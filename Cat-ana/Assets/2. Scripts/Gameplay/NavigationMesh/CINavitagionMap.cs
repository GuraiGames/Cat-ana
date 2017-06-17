using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR

using UnityEditor;

[CustomEditor(typeof(NavigationMap))]
public class CENavigationMap : Editor
{
    float pos_x, pos_y, pos_z = 0;

    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        NavigationMap myScript = (NavigationMap)target;

        EditorGUILayout.Separator();
        EditorGUILayout.LabelField("NAVEGATION MESH CONTROL");

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("x: ");
        pos_x = EditorGUILayout.FloatField(pos_x);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("y: ");
        pos_y = EditorGUILayout.FloatField(pos_y);
        GUILayout.EndHorizontal();

        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("z: ");
        pos_z = EditorGUILayout.FloatField(pos_z);
        GUILayout.EndHorizontal();

        if (GUILayout.Button("Add point"))
        {
            if (myScript.nav_map_point == null || myScript.nav_map_points_parent == null)
            {
                Debug.LogError("Nav map point or nav mav point parent not set to an object on the inspector");
                return;
            }

            GameObject point = Instantiate(myScript.nav_map_point);
            point.transform.position = new Vector3(pos_x, pos_y, pos_z);
            point.transform.parent = myScript.nav_map_points_parent.transform;
            point.name = "Point [" + pos_x + ", " + pos_z + "]";
        }
    }
}

#endif
