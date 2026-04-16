#if MAP
using System.IO;
using System.Text;
using Core;
using Core.Map;
using UnityEditor;
using UnityEngine;
using FileUtils = Core.FileUtils;
using Object = UnityEngine.Object;

namespace CoreEditor.Map
{
    public static class EditWindow
    {
        public static GridPainter focusBox;
    }

    public abstract class EditWindow<T> : EditorWindow where T : GridPainter
    {
        private const string MASK_EXTENSION_NAME = "json";
        protected string editType;

        private string filePath;
        protected T painter;
        private Vector2 drawPos;

        private SceneView sceneView;
        private Vector3 prevMouseWorldPos;
        protected bool isRetreat;
        private Vector2 mouseDownPos;

        protected virtual void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        protected virtual void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUI;
        }

        private void OnFocus()
        {
            SetFocusBox();
        }

        private void SetFocusBox()
        {
            EditWindow.focusBox?.SetBoxActive(false);
            EditWindow.focusBox = painter;
            EditWindow.focusBox?.SetBoxActive(true);
        }

        protected virtual void OnDestroy()
        {
            ClearPainter();
        }

        private void OnGUI()
        {
            drawPos = EditorGUILayout.BeginScrollView(drawPos);

            DrawNewOrOpen();
            if (null != painter)
            {
                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                EditorGUILayout.LabelField(string.Format("Current {0}:{1}", editType, painter.gameObject.name),
                    EditorStyles.boldLabel);
                DrawSetAttribute();
                EditorGUILayout.EndVertical();

                GUILayout.Space(5);

                OnDrawEdit();
            }

            OnDrawGUI();

            EditorGUILayout.EndScrollView();
        }

        private void DrawNewOrOpen()
        {
            GUILayoutOption normalHeight = GUILayout.MinHeight(40);

            GUILayout.Space(6);
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("New " + editType, normalHeight))
            {
                ClearPainter();
                filePath = EditorUtility.SaveFilePanelInProject("Save Path", editType, string.Empty,
                    string.Empty);
                if (!string.IsNullOrEmpty(filePath))
                {
                    filePath = filePath + "." + MASK_EXTENSION_NAME;
                    CreatePainter();
                }
            }

            if (GUILayout.Button("Open " + editType, normalHeight))
            {
                ClearPainter();
                string selectPath = EditorUtility.OpenFilePanel("Select Path", null, MASK_EXTENSION_NAME);
                if (!string.IsNullOrEmpty(selectPath))
                {
                    filePath = selectPath;
                    CreatePainter();
                    string jsonText = File.ReadAllText(filePath, Encoding.UTF8);
                    painter.SetGridsConfig(jsonText);
                    SceneView.RepaintAll();
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        private void DrawSetAttribute()
        {
            EditorGUI.BeginChangeCheck();
            GUILayoutOption width = GUILayout.Width(90);
            painter.transform.position =
                EditorGUILayout.Vector3Field("Position:", painter.transform.position);
            if (EditorGUI.EndChangeCheck())
            {
                painter.UpdatePosition();
                SceneView.RepaintAll();
            }

            EditorGUI.BeginChangeCheck();
            GUILayout.BeginHorizontal();
            {
                painter.MapLength = EditorGUILayout.DelayedIntField("MapLength:", painter.MapLength);
                EditorGUILayout.LabelField("Grid Count:" + painter.RowCount, width);
                GUILayout.EndHorizontal();
            }
            GUILayout.BeginHorizontal();
            {
                painter.MapWidth = EditorGUILayout.DelayedIntField("MapWidth:", painter.MapWidth);
                EditorGUILayout.LabelField("Grid Count:" + painter.ColumnCount, width);
                GUILayout.EndHorizontal();
            }
            painter.GridSize = EditorGUILayout.DelayedFloatField("GridSize:", painter.GridSize);
            if (EditorGUI.EndChangeCheck())
            {
                painter.BuildGrid();
                SceneView.RepaintAll();
            }

            if (GUILayout.Button("Save " + editType, GUILayout.MinHeight(40)))
            {
                string jsonText = JsonUtils.ToJson(painter.GenerateGridsData());
                FileUtils.CreateFile(filePath, jsonText);
                AssetDatabase.Refresh();
            }
        }

        protected abstract void OnDrawEdit();

        protected abstract void OnDrawGUI();

        private void ClearPainter()
        {
            if (null != painter)
            {
                Object.DestroyImmediate(painter.gameObject);

                if (EditWindow.focusBox == painter)
                {
                    EditWindow.focusBox = null;
                }

                painter = null;
            }
        }

        private void CreatePainter()
        {
            string maskName = Path.GetFileNameWithoutExtension(filePath);
            GameObject go = new GameObject(maskName);
            go.transform.position = Vector3.zero;
            painter = go.AddComponent<T>();
            painter.SetBoxActive(true);
            SetFocusBox();

            sceneView = SceneView.lastActiveSceneView;
            Vector3 pos = painter.transform.position;
            Quaternion rotate = Quaternion.Euler(90, 0, 0);
            sceneView.LookAt(pos, rotate);
            Selection.activeObject = null;
        }

        private void OnSceneGUI(SceneView sceneView)
        {
            if (null == painter)
            {
                return;
            }

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Keyboard));
            if (Event.current.type == EventType.KeyDown)
            {
                if (!isRetreat)
                {
                    isRetreat = Event.current.keyCode == KeyCode.LeftShift;
                }
            }
            else if (Event.current.type == EventType.KeyUp)
            {
                isRetreat = false;
            }

            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));
            if (Event.current.type == EventType.MouseDrag)
            {
                if (Event.current.button == 0)
                {
                    if (Selection.activeObject == null &&
                        GetSceneMouseInWorldPosition(Event.current.mousePosition, out Vector3 pos))
                    {
                        OnEditGrid(pos);
                        if (prevMouseWorldPos != Vector3.zero)
                        {
                            Vector3 deltaPos = pos - prevMouseWorldPos;
                            Vector3 deltaNor = deltaPos.normalized;
                            float lerpCount = (int) deltaPos.magnitude / painter.GridSize;
                            for (int i = 0; i < lerpCount; i++)
                            {
                                Vector3 lerpPos = prevMouseWorldPos + deltaNor * (i + 1) * painter.GridSize;
                                OnEditGrid(lerpPos);
                            }
                        }

                        prevMouseWorldPos = pos;
                        SceneView.RepaintAll();
                    }
                }
            }
            else if (Event.current.type == EventType.MouseDown)
            {
                if (Event.current.button == 0)
                {
                    mouseDownPos = Event.current.mousePosition;
                    prevMouseWorldPos = Vector3.zero;
                }
                else if (Event.current.button == 1)
                {
                    Selection.activeObject = null;
                    UnityEditor.Tools.current = Tool.Move;
                }
            }
            else if (Event.current.type == EventType.MouseUp)
            {
                if (Event.current.mousePosition == mouseDownPos)
                {
                    if (GetSceneMouseInWorldPosition(Event.current.mousePosition, out Vector3 pos))
                    {
                        OnClickGrid(pos);
                    }
                }
            }
        }

        private bool GetSceneMouseInWorldPosition(Vector3 mousePos, out Vector3 worldPos)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(mousePos);
            RaycastHit[] hits = Physics.RaycastAll(ray.origin, ray.direction, Mathf.Infinity);
            foreach (var hit in hits)
            {
                if (hit.transform.gameObject == painter.gameObject)
                {
                    worldPos = hit.point;
                    return true;
                }
            }

            worldPos = Vector3.zero;
            return false;
        }

        protected abstract void OnEditGrid(Vector3 pos);

        protected abstract void OnClickGrid(Vector3 pos);
    }
}
#endif