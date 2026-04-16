using UnityEditor;
using UnityEngine;

namespace CoreEditor
{
    public abstract class SettingsObject : ScriptableObject
    {
        private const string SETTINGS_PATH = "Assets/Core/Editor Resources/Settings";

        public static T Load<T>(string name) where T : SettingsObject
        {
            string path = string.Format("{0}/{1}.asset", SETTINGS_PATH, name);
            T obj = AssetDatabase.LoadAssetAtPath<T>(path);
            if (null == obj)
            {
                obj = CreateInstance<T>();
                obj.OnInitial();
                AssetUtils.CreateAsset(obj, path);
            }

            return obj;
        }

        protected virtual void OnInitial()
        {
        }

        public void Save()
        {
            EditorUtility.SetDirty(this);
            AssetDatabase.SaveAssets();
        }
    }
}