#if MAP
using Core.Map;
using UnityEditor;
using UnityEngine;

namespace CoreEditor.Map
{
    [CustomEditor(typeof(GridPainter), true)]
    public class GridPainterEditor : Editor
    {
        GridPainter instance;

        private void OnEnable()
        {
            instance = target as GridPainter;
        }

        public override void OnInspectorGUI()
        {
            EditorGUI.BeginChangeCheck();
            serializedObject.Update();
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mapLength"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("mapWidth"));
            EditorGUILayout.PropertyField(serializedObject.FindProperty("gridSize"));

            serializedObject.ApplyModifiedProperties();

            if (EditorGUI.EndChangeCheck())
            {
                instance.BuildGrid();
                SceneView.RepaintAll();
            }

            if (instance.transform.hasChanged)
            {
                instance.UpdatePosition();
                SceneView.RepaintAll();
            }
        }
    }
}
#endif