using UnityEditor;

namespace CoreEditor
{
    [CustomEditor(typeof(SettingsObject), true)]
    public class SettingsObjectEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            EditorGUI.BeginDisabledGroup(true);
            base.OnInspectorGUI();
            EditorGUI.EndDisabledGroup();
        }
    }
}