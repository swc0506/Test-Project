using System;
using Core;
using LitJson;
using UnityEditor;
using UnityEngine;
using UnityEngine.Networking;

namespace CoreEditor.Tools
{
    public class JsonViewerWindow : EditorWindow
    {
        string inputStr = @"{""testObject"":{""message"":""hello"",""code"":200,""obj"":{},""array"":[0,1]}}";

        private Vector2 scrollPos;
        private int buttonHeight;
        private string url;
        private UnityWebRequest webRequest;

        [MenuItem("Tools/Json Window", false, 5010)]
        public static void ShowWindow()
        {
            JsonViewerWindow editor = CreateInstance<JsonViewerWindow>();
            editor.titleContent = new GUIContent("Json Window");
            editor.minSize = new Vector2(400, 300);
            editor.Show();
        }

        private void OnEnable()
        {
            buttonHeight = 60;
            url = string.Empty;
        }

        private void OnGUI()
        {
            int areaHeight = (int) position.height - buttonHeight - 50;
            GUILayoutOption heightOption = GUILayout.MaxHeight(areaHeight);
            scrollPos = EditorGUILayout.BeginScrollView(scrollPos, heightOption);
            inputStr = GUILayout.TextArea(inputStr, heightOption);
            EditorGUILayout.EndScrollView();
            EditorGUILayout.Separator();
            url = EditorGUILayout.TextField("Get Json Url:", url);
            EditorGUILayout.Separator();

            bool disable = string.IsNullOrEmpty(inputStr) && string.IsNullOrEmpty(url);
            EditorGUI.BeginDisabledGroup(disable);
            if (GUILayout.Button("Format Json", GUILayout.Height(buttonHeight)))
            {
                if (!string.IsNullOrEmpty(url))
                {
                    webRequest?.Abort();
                    webRequest = UnityWebRequest.Get(url);
                    webRequest.SendWebRequest();
                }
                else
                {
                    FormatJson(inputStr);
                }
            }

            EditorGUI.EndDisabledGroup();

            CheckWebRequest();
        }

        private void CheckWebRequest()
        {
            if (null != webRequest && webRequest.isDone)
            {
                if (webRequest.isNetworkError || webRequest.isHttpError)
                {
                    base.ShowNotification(
                        new GUIContent(string.Format("WebRequest Was An Error:{0}", webRequest.error)));
                }
                else
                {
                    inputStr = webRequest.downloadHandler.text;
                    FormatJson(inputStr);
                }

                webRequest = null;
            }
        }

        private void FormatJson(string text)
        {
            try
            {
                text = JsonUtils.ToJson(JsonUtils.ToObject(text), true);
                if (!string.IsNullOrEmpty(text))
                {
                    GUI.FocusControl(null);
                    inputStr = text;
                }
            }
            catch (Exception e)
            {
                base.ShowNotification(new GUIContent("There Was An Error While Parsing Json:" + e.Message));
            }
        }
    }
}