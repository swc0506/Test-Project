using Core.UI;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;

namespace CoreEditor.UI
{
    [CustomEditor(typeof(CombineImage), true)]
    [CanEditMultipleObjects]
    public class CombineImageEditor : GraphicEditor
    {
        SerializedProperty m_Texture;
        SerializedProperty m_Sprite;
        SerializedProperty m_PreserveAspect;
        SerializedProperty m_slicingType;

        GUIContent m_SpriteContent;
        GUIContent m_SlicingContent;


        protected override void OnEnable()
        {
            base.OnEnable();

            m_SlicingContent = new GUIContent("Slicing Type");

            m_Texture = serializedObject.FindProperty("m_Texture");
            m_Sprite= serializedObject.FindProperty("m_Sprite");
            m_PreserveAspect = serializedObject.FindProperty("m_PreserveAspect");
            m_slicingType = serializedObject.FindProperty("slicingType");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            SelectImageGUI();
            AppearanceControlsGUI();
            RaycastControlsGUI();
            
            SetShowNativeSize(true,true);

            EditorGUILayout.PropertyField(m_slicingType, m_SlicingContent);

            if (EditorGUILayout.BeginFadeGroup(m_ShowNativeSize.faded))
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(m_PreserveAspect);
                EditorGUI.indentLevel--;
            }

            EditorGUILayout.EndFadeGroup();
            NativeSizeButtonGUI();

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
                        ((CombineImage) target).sprite = null;
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
                        ((CombineImage) target).texture = null;
                    }
                }
            }
        }
        
        /// <summary>
        /// All graphics have a preview.
        /// </summary>
        public override bool HasPreviewGUI()
        {
            return true;
        }

        /// <summary>
        /// Draw the Image preview.
        /// </summary>
        public override void OnPreviewGUI(Rect rect, GUIStyle background)
        {
            CombineImage inst = target as CombineImage;
            Texture tex = inst.mainTexture;
            if (tex == null) return;

            Rect outer = new Rect(0, 0, 1, 1);
            outer.xMin *= inst.rectTransform.rect.width;
            outer.xMax *= inst.rectTransform.rect.width;
            outer.yMin *= inst.rectTransform.rect.height;
            outer.yMax *= inst.rectTransform.rect.height;
            
            Rect uv=new Rect(0,0,1,1);
            SpriteDrawUtility.DrawSprite(tex, rect, outer, uv, inst.canvasRenderer.GetColor());
        }

        /// <summary>
        /// Info String drawn at the bottom of the Preview
        /// </summary>
        public override string GetInfoString()
        {
            Texture tex = ((CombineImage) target).mainTexture;
            int x = (tex != null) ? Mathf.RoundToInt(tex.width) : 0;
            int y = (tex != null) ? Mathf.RoundToInt(tex.height) : 0;
            
            return string.Format("Image Size: {0}x{1}", x, y);
        }
    }
}