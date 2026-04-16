#if HYBRIDCLR
using UnityEditor;

namespace CoreEditor.HybridCLR
{
    [CustomEditor(typeof(HybridCLRSettingsObject), true)]
    public class HybridCLRSettingsObjectEditor : Editor
    {
        private SerializedObject so;

        private void OnEnable()
        {
            so = new SerializedObject(this);
        }

        public override void OnInspectorGUI()
        {
            base.OnInspectorGUI();
            so.Update();
            so.ApplyModifiedProperties();
        }
    }
}
#endif