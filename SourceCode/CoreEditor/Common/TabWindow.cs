using System;
using UnityEditor;
using UnityEngine;

namespace CoreEditor
{
    public abstract class BaseWindow
    {
        private TabWindow context;

        protected T GetContext<T>() where T : TabWindow
        {
            return context as T;
        }

        public abstract string Title { get; }

        public virtual void Initial(TabWindow context)
        {
            this.context = context;
        }

        public virtual void Enable()
        {
        }

        public virtual void Disable()
        {
        }

        public virtual void DrawGUI()
        {
        }
    }

    public abstract class TabWindow : EditorWindow
    {
        protected BaseWindow[] windows;
        private int selectTabIndex;
        private GUIContent[] toolbarGUIContents;
        private int toolbarGUIIndex;

        private Vector2 scrollPos;

        protected static void OpenWindow<T>(string title) where T : TabWindow
        {
            T editor = GetWindow<T>(title);
            editor.minSize = new Vector2(400, 300);
        }

        protected abstract string SelectTabIndexKey { get; }

        protected abstract void OnInitial();

        protected virtual void OnEnable()
        {
            OnInitial();
            selectTabIndex = -1;
            int length = windows.Length;
            toolbarGUIContents = new GUIContent[length];
            for (int i = 0; i < length; i++)
            {
                toolbarGUIContents[i] = new GUIContent(windows[i].Title);
            }

            toolbarGUIIndex = EditorPrefs.GetInt(SelectTabIndexKey, 0);
            SelectTab(toolbarGUIIndex);
        }

        protected virtual void OnLostFocus()
        {
            if (selectTabIndex >= 0 && selectTabIndex < windows.Length)
            {
                windows[selectTabIndex].Disable();
            }
        }

        private void SelectTab(int tabIndex)
        {
            if (tabIndex < 0 || tabIndex >= windows.Length || selectTabIndex == tabIndex)
            {
                return;
            }
            if (selectTabIndex >= 0 && selectTabIndex < windows.Length)
            {
                windows[selectTabIndex].Disable();
            }

            selectTabIndex = tabIndex;
            EditorPrefs.SetInt(SelectTabIndexKey, selectTabIndex);
            windows[selectTabIndex].Initial(this);
            windows[selectTabIndex].Enable();
            OnSelectTabChanged();
        }

        protected virtual void OnSelectTabChanged()
        {
        }

        private void OnGUI()
        {
            EditorGUI.BeginChangeCheck();
            toolbarGUIIndex = GUILayout.Toolbar(toolbarGUIIndex, toolbarGUIContents, GUILayout.MinHeight(26));
            if (EditorGUI.EndChangeCheck())
            {
                SelectTab(toolbarGUIIndex);
            }

            OnDrawGUI();
            GUILayout.Space(5);
            if (selectTabIndex >= 0 && selectTabIndex < windows.Length)
            {
                scrollPos = EditorGUILayout.BeginScrollView(scrollPos);
                windows[selectTabIndex].DrawGUI();
                EditorGUILayout.EndScrollView();
            }
        }

        protected virtual void OnDrawGUI()
        {
        }
    }
}