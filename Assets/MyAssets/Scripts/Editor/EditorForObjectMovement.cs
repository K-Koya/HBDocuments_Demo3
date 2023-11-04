using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using GluonGui;
using static ObjectMovement;

[CustomEditor(typeof(ObjectMovement))]
public class EditorForObjectMovement : Editor
{
    protected virtual void OnSceneGUI()
    {
        ObjectMovement script = target as ObjectMovement;

        List<TripInfo> tripInfos = script.TripInfos;

        foreach (TripInfo ti in tripInfos)
        {
            EditorGUI.BeginChangeCheck();
            Vector3 point = Handles.PositionHandle(ti.Point, Quaternion.identity);
            bool isPosChanges = EditorGUI.EndChangeCheck();

            Handles.CubeHandleCap(0, point, Quaternion.identity, 0.2f, EventType.Repaint);

            if (isPosChanges)
            {
                ti.Point = point;
            }
        }
    }
}
