using System;
using UnityEditor;
using UnityEngine;
using Object = UnityEngine.Object;

namespace CoreEditor
{
    public class DragListener
    {
        public Action<Object[]> onDragCallback;
        public Action onDragRectDrawCallback;
        public string tipText = "Drag Assets Put Here";

        private GUIStyle dragStyle;
        private Vector2 scrollPos;

        public void DrawRect(Rect dragRect)
        {
            if (null == dragStyle)
            {
                dragStyle = new GUIStyle(GUI.skin.box);
                dragStyle.richText = true;
                dragStyle.alignment = TextAnchor.MiddleCenter;
                dragStyle.fontSize = 24;
                dragStyle.fontStyle = FontStyle.Bold;
                dragStyle.normal.textColor = Color.white;
            }

            GUI.backgroundColor = Color.cyan;
            GUI.Box(dragRect, tipText, dragStyle);
            GUI.backgroundColor = Color.white;

            scrollPos = GUILayout.BeginScrollView(scrollPos, GUILayout.Width(dragRect.width),
                GUILayout.Height(dragRect.height));
            onDragRectDrawCallback?.Invoke();
            GUILayout.EndScrollView();

            CheckDragEvent(dragRect);
        }

        private void CheckDragEvent(Rect dragRect)
        {
            int id = GUIUtility.GetControlID(FocusType.Passive);
            if ((Event.current.type == EventType.DragUpdated || Event.current.type == EventType.DragPerform) &&
                dragRect.Contains(Event.current.mousePosition))
            {
                DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
                DragAndDrop.activeControlID = id;

                if (Event.current.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();
                    onDragCallback?.Invoke(DragAndDrop.objectReferences);
                    DragAndDrop.activeControlID = 0;
                }

                Event.current.Use();
            }
        }
    }
}