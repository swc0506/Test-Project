using Core.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace CoreEditor.UI
{
    [CustomEditor(typeof(CircleImage))]
    public class CircleImageEditor : GraphicEditor
    {
        SerializedProperty m_Texture;
        SerializedProperty m_Sprite;
        SerializedProperty m_FillAmount;
        SerializedProperty m_Fill;
        SerializedProperty m_Thickness;
        SerializedProperty m_Segments;

        protected override void OnEnable()
        {
            base.OnEnable();

            // Note we have precedence for calling rectangle for just rect, even in the Inspector.
            // For example in the Camera component's Viewport Rect.
            // Hence sticking with Rect here to be consistent with corresponding property in the API.

            m_Texture = serializedObject.FindProperty("m_Texture");
            m_Sprite= serializedObject.FindProperty("m_Sprite");
            m_FillAmount = serializedObject.FindProperty("m_FillAmount");
            m_Fill = serializedObject.FindProperty("m_Fill");
            m_Thickness = serializedObject.FindProperty("m_Thickness");
            m_Segments = serializedObject.FindProperty("m_Segments");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();


            SelectImageGUI();
            AppearanceControlsGUI();
            RaycastControlsGUI();
            
            EditorGUILayout.PropertyField(m_FillAmount);
            EditorGUILayout.PropertyField(m_Fill);
            EditorGUILayout.PropertyField(m_Segments);
            if (!m_Fill.boolValue)
            {
                EditorGUILayout.PropertyField(m_Thickness);
            }

            serializedObject.ApplyModifiedProperties();
        }

        private void SelectImageGUI()
        {
            if (!m_Sprite.objectReferenceValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Texture);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_Texture.objectReferenceValue)
                    {
                        ((CircleImage) target).sprite = null;
                    }
                }
            }
            if (!m_Texture.objectReferenceValue)
            {
                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(m_Sprite);
                if (EditorGUI.EndChangeCheck())
                {
                    if (m_Sprite.objectReferenceValue)
                    {
                        ((CircleImage) target).texture = null;
                    }
                }
            }
        }

        public override bool HasPreviewGUI()
        {
            return true;
        }

        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            CircleImage inst = target as CircleImage;
            Texture tex = inst.mainTexture;
            if (tex == null)
                return;

            Rect outer = new Rect(0, 0, 1, 1);
            outer.xMin *= inst.rectTransform.rect.width;
            outer.xMax *= inst.rectTransform.rect.width;
            outer.yMin *= inst.rectTransform.rect.height;
            outer.yMax *= inst.rectTransform.rect.height;
            Rect uv=new Rect(0,0,1,1);

            SpriteDrawUtility.DrawSprite(tex, rect, outer, uv, inst.canvasRenderer.GetColor());
        }
    }
}