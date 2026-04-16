#if I18N
using System;
using Core.I18n;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace CoreEditor.I18N
{
    [CustomEditor(typeof(TextLocalized))]
    public class TextLocalizedEditor : Editor
    {
        private TextLocalized instance;
        private LanguagePackage currPkg;

        private Vector2 inputScrollPos;
        private SerializedProperty proText;
        private SerializedProperty proKey;

        private Vector2 matchScrollPos;

        private string inputText;

        private void Awake()
        {
            instance = (TextLocalized) target;
        }

        protected void OnEnable()
        {
            instance = (TextLocalized) target;
            currPkg = LanguagePackageHelper.LoadCurrent();

            proText = serializedObject.FindProperty("text");
            proKey = serializedObject.FindProperty("key");
            if (null == proText.objectReferenceValue)
            {
                proText.objectReferenceValue = instance.GetComponent<Text>();
                serializedObject.ApplyModifiedProperties();
            }

            string key = proKey.stringValue;
            if (!string.IsNullOrEmpty(key))
            {
                string text = GetText(key);
                SetTextInfo(key, text);
            }
        }

        private string GetText(string key)
        {
            foreach (var item in currPkg.texts)
            {
                if (item.key == key)
                {
                    return item.value;
                }
            }

            return null;
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            if (null == currPkg)
            {
                EditorGUILayout.HelpBox("Language Package Is Null,Please Config In [I18n Window]",
                    MessageType.Info);
                return;
            }

            EditorGUILayout.TextField("Key:", proKey.stringValue);
            GUILayout.Space(1);
            EditorGUILayout.LabelField("Text:");
            GUILayout.Space(2);
            GUILayout.BeginHorizontal();
            {
                EditorGUI.BeginChangeCheck();
                GUILayoutOption layoutOption = GUILayout.MaxHeight(80);
                inputScrollPos = EditorGUILayout.BeginScrollView(inputScrollPos, layoutOption);
                inputText = GUILayout.TextArea(inputText, layoutOption);
                EditorGUILayout.EndScrollView();
                if (EditorGUI.EndChangeCheck())
                {
                    ((Text) proText.objectReferenceValue).text = inputText;
                    ((Text) proText.objectReferenceValue).SetAllDirty();
                }

                EditorGUI.BeginDisabledGroup(string.IsNullOrEmpty(inputText));
                {
                    if (GUILayout.Button("+", GUILayout.Width(20), GUILayout.ExpandHeight(true)))
                    {
                        CreateNewText(inputText);
                    }

                    EditorGUI.EndDisabledGroup();
                }
                GUILayout.EndHorizontal();
            }

            if (!string.IsNullOrEmpty(inputText))
            {
                DrawSearchTipText(inputText);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void CreateNewText(string text)
        {
            string key = FindKey(text);
            if (string.IsNullOrEmpty(key))
            {
                key = GuidTo16String();
                TextInfo info = new TextInfo(key, text);
                currPkg.texts.Add(info);
                LanguagePackageHelper.Save(currPkg);
            }

            SetTextInfo(key, text);
        }

        private string FindKey(string value)
        {
            foreach (var item in currPkg.texts)
            {
                if (item.value == value)
                {
                    return item.key;
                }
            }

            return null;
        }

        private void SetTextInfo(string key, string text)
        {
            proKey.stringValue = key;
            ((Text) proText.objectReferenceValue).text = text;

            inputText = string.Empty;
            GUI.FocusControl(string.Empty);
        }

        private void DrawSearchTipText(string text)
        {
            matchScrollPos = GUILayout.BeginScrollView(matchScrollPos);
            for (int i = 0; i < currPkg.texts.Count; i++)
            {
                TextInfo textInfo = currPkg.texts[i];
                if (!textInfo.value.Contains(text))
                {
                    continue;
                }

                if (GUILayout.Button(textInfo.value, GUILayout.ExpandWidth(true), GUILayout.Height(20)))
                {
                    SetTextInfo(textInfo.key, textInfo.value);
                }
            }

            GUILayout.EndScrollView();
        }

        private string GuidTo16String()
        {
            long i = 1;
            foreach (byte b in Guid.NewGuid().ToByteArray())
            {
                i *= ((int) b + 1);
            }

            return string.Format("{0:x}", i - DateTime.Now.Ticks);
        }
    }
}
#endif