using UnityEditor;
using UnityEngine;

namespace Editor
{
    [CustomEditor(typeof(Chessboard_88))]
    public class Lesson88_Editor : UnityEditor.Editor
    {
        public override void OnInspectorGUI()
        {
            // 调用默认的InspectorGUI
            DrawDefaultInspector();
            
            //获取目标脚本
            Chessboard_88 chessboard = target as Chessboard_88;
            
            if (GUILayout.Button("更新棋盘纹理"))
            {
                chessboard.UpdateTexture();
            }
            
        }
    }
}
