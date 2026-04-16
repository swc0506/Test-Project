using System;
using System.IO;
using UnityEditor;
using UnityEngine;
using FileUtils = Core.FileUtils;
using Object = UnityEngine.Object;

namespace CoreEditor
{
    public class GameObjectUtils
    {
        public static bool CreatePrefab(Object fbx, string outPath, Action<GameObject> prePostAction = null)
        {
            if (null == fbx)
            {
                return false;
            }

            GameObject go = (GameObject)GameObject.Instantiate(fbx);
            prePostAction?.Invoke(go);
            FileUtils.CreateDirectory(Path.GetDirectoryName(outPath));
            GameObject prefab = PrefabUtility.SaveAsPrefabAsset(go, outPath);
            EditorUtility.SetDirty(prefab);
            GameObject.DestroyImmediate(go);
            return true;
        }

        public static bool CreatePrefab(string fbxPath, string outPath, Action<GameObject> prePostAction = null)
        {
            Object fbx = AssetDatabase.LoadAssetAtPath<Object>(fbxPath);
            if (null == fbx)
            {
                Debug.LogWarningFormat("fbx is null:{0}", fbxPath);
                return false;
            }

            return CreatePrefab(fbx, outPath, prePostAction);
        }

        public static Material CreateMaterial(string shader, string outPath)
        {
            Material material = new Material(Shader.Find(shader));
            FileUtils.CreateDirectory(Path.GetDirectoryName(outPath));
            AssetDatabase.CreateAsset(material, outPath);
            EditorUtility.SetDirty(material);
            return material;
        }

        public static AnimationClip CreateAnimationClip(string modelPath)
        {
            AnimationClip modelClip = AssetDatabase.LoadAssetAtPath<AnimationClip>(modelPath);
            if (null != modelClip)
            {
                string outPath = Path.GetDirectoryName(modelPath) + "/" + modelClip.name + ".anim";
                AnimationClip clip = new AnimationClip();
                clip.name = modelClip.name;
                clip.wrapMode = WrapMode.Loop;
                EditorCurveBinding[] bindingDatas = AnimationUtility.GetCurveBindings(modelClip);
                foreach (var binding in bindingDatas)
                {
                    AnimationUtility.SetEditorCurve(clip, binding, AnimationUtility.GetEditorCurve(modelClip, binding));
                }

                EditorUtility.SetDirty(clip);
                AssetDatabase.CreateAsset(clip, outPath);
                return clip;
            }

            return null;
        }

        public static void SetLayer(GameObject go, string name)
        {
            int layer = LayerMask.NameToLayer(name);
            if (go.layer != layer)
            {
                var trans = go.GetComponentsInChildren<Transform>(true);
                foreach (var item in trans)
                {
                    item.gameObject.layer = layer;
                }
            }
        }

        public static bool MatchBoxColliderBound(GameObject go, BoxCollider boxCollider)
        {
            Renderer[] renders = go.GetComponentsInChildren<Renderer>();
            if (renders.Length == 0)
            {
                boxCollider.center = Vector3.one * 0.5f;
                boxCollider.size = Vector3.one;
                return false;
            }

            Bounds bounds = renders[0].bounds;
            for (int i = 1; i < renders.Length; i++)
            {
                bounds.Encapsulate(renders[i].bounds);
            }

            boxCollider.center = bounds.center - go.transform.position;
            Vector3 scale = go.transform.localScale;
            Vector3 size = bounds.size;
            if (size != Vector3.zero)
            {
                size.x /= scale.x;
                size.y /= scale.y;
                size.z /= scale.z;
            }

            boxCollider.size = size;
            return true;
        }
        
        public static Vector3 GetSize(GameObject go)
        {
            Renderer[] renders = go.GetComponentsInChildren<Renderer>();
            if (renders.Length == 0)
            {
                return Vector3.one;
            }

            Bounds bounds = renders[0].bounds;
            for (int i = 1; i < renders.Length; i++)
            {
                bounds.Encapsulate(renders[i].bounds);
            }

            return bounds.size;
        }
    }
}