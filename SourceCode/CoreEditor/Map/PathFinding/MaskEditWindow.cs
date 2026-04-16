#if MAP
using UnityEngine;
using UnityEditor;
using Core.Map;
using UnityEditorInternal;

namespace CoreEditor.Map
{
    public class MaskEditWindow : EditWindow<MaskGridPainter>
    {
        private int selectEditType;
        private LayerMask obstacleLayerMask = 1;

        private GUIContent[] editContents = new GUIContent[2]
        {
            new GUIContent("Manual Edit Obstacle"),
            new GUIContent("Automatic Scan Obstacle")
        };

        [MenuItem("Tools/Map/Mask Window", false, 5082)]
        public static void ShowWindow()
        {
            MaskEditWindow editWindow = GetWindow<MaskEditWindow>("Mask Window");
            editWindow.minSize = new Vector2(400, 300);
        }

        protected override void OnEnable()
        {
            editType = "Mask";
            base.OnEnable();
        }

        protected override void OnDrawEdit()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            GUILayoutOption minHeight = GUILayout.MinHeight(35);

            selectEditType = GUILayout.Toolbar(selectEditType, editContents, minHeight);
            GUILayout.Space(10);
            if (selectEditType == 0)
            {
                EditorGUILayout.HelpBox("Add Obstacle:Drag Left Mouse Button", MessageType.Info);
                EditorGUILayout.HelpBox("Remove Obstacle:Drag Left Mouse Button & Press Keycode LeftShift",
                    MessageType.Info);
            }
            else if (selectEditType == 1)
            {
                obstacleLayerMask = EditorGUILayout.MaskField("Set Obstacle LayerMask",
                    InternalEditorUtility.LayerMaskToConcatenatedLayersMask(obstacleLayerMask),
                    InternalEditorUtility.layers);
                GUILayout.Space(6);
                if (GUILayout.Button("Start Scan", minHeight))
                {
                    ScanObstacle();
                }
            }
            EditorGUILayout.EndVertical();
            
            GUILayout.Space(10);
            EditorGUILayout.BeginHorizontal();
            {
                GUILayoutOption width = GUILayout.Width(position.width * 0.5f);
                if (GUILayout.Button("Set All Grids To Obstacle", width, minHeight))
                {
                    SetAllObstacle();
                }

                if (GUILayout.Button("Clear All Obstacle", width, minHeight))
                {
                    ClearAllObstacle();
                }

                EditorGUILayout.EndHorizontal();
            }
        }

        protected override void OnDrawGUI()
        {
        }

        protected override void OnEditGrid(Vector3 pos)
        {
            MaskNode node = painter.MaskGrid.GetNode(pos);
            if (null != node)
            {
                node.SetWalkable(isRetreat);
            }
        }

        protected override void OnClickGrid(Vector3 pos)
        {
        }

        private void ScanObstacle()
        {
            for (int i = 0; i < painter.MaskGrid.Row; i++)
            {
                for (int j = 0; j < painter.MaskGrid.Column; j++)
                {
                    MaskNode maskNode = painter.MaskGrid.GetNode(i, j);
                    if (null != maskNode)
                    {
                        bool walkable = !Physics.CheckSphere(maskNode.Pos, painter.GridSize * 0.45f,
                            obstacleLayerMask, QueryTriggerInteraction.Ignore);
                        maskNode.SetWalkable(walkable);
                    }
                }
            }

            SceneView.RepaintAll();
        }

        private void SetAllObstacle()
        {
            for (int i = 0; i < painter.MaskGrid.Row; i++)
            {
                for (int j = 0; j < painter.MaskGrid.Column; j++)
                {
                    painter.MaskGrid.GetNode(i, j).SetWalkable(false);
                }
            }

            SceneView.RepaintAll();
        }

        private void ClearAllObstacle()
        {
            for (int i = 0; i < painter.MaskGrid.Row; i++)
            {
                for (int j = 0; j < painter.MaskGrid.Column; j++)
                {
                    painter.MaskGrid.GetNode(i, j).SetWalkable(true);
                }
            }

            SceneView.RepaintAll();
        }
    }
}
#endif