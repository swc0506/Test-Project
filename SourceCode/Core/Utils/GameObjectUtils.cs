using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core
{
    public static class GameObjectUtils
    {
        private static readonly List<Transform> childTrans = new List<Transform>();

        public static void SetActive(GameObject go, bool active)
        {
            if (null != go && go.activeSelf != active)
            {
                go.SetActive(active);
            }
        }

        public static void SetActive(Transform trans, bool active)
        {
            if (null != trans)
            {
                SetActive(trans.gameObject, active);
            }
        }


        public static void SetActiveByScale(GameObject go, bool active)
        {
            Vector3 scale = active ? Vector3.one : Vector3.zero;
            if (null != go && go.transform.localScale != scale)
            {
                go.transform.localScale = scale;
            }
        }

        public static void SetActiveByScale(Transform trans, bool active)
        {
            if (null != trans)
            {
                SetActiveByScale(trans.gameObject, active);
            }
        }


        public static void SetParent(Transform trans, Transform parent, bool resetLocation)
        {
            if (null != trans && trans.parent != parent)
            {
                trans.SetParent(parent);
                if (resetLocation)
                {
                    trans.localScale = Vector3.one;
                    trans.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
                }
            }
        }

        public static void SetParent(Transform trans, Transform parent)
        {
            SetParent(trans, parent, true);
        }

        public static void SetParent(GameObject go, Transform parent, bool resetLocation)
        {
            if (null != go)
            {
                SetParent(go.transform, parent, resetLocation);
            }
        }

        public static void SetParent(GameObject go, Transform parent)
        {
            SetParent(go, parent, true);
        }

        public static void SetScale(GameObject go, float scale)
        {
            Vector3 scaleVec = Vector3.one * scale;
            if (null != go && go.transform.localScale != scaleVec)
            {
                go.transform.localScale = scaleVec;
            }
        }

        public static void SetScale(Transform trans, float scale)
        {
            if (null != trans)
            {
                SetScale(trans.gameObject, scale);
            }
        }


        public static void SetLocation(Transform trans, Vector3 pos, Quaternion rot)
        {
            if (null != trans)
            {
                trans.SetPositionAndRotation(pos, rot);
            }
        }

        public static void SetLocation(Transform trans, Vector3 pos, Vector3 angle)
        {
            SetLocation(trans, pos, Quaternion.Euler(angle));
        }

        public static void SetLocation(GameObject go, Vector3 pos, Quaternion rot)
        {
            if (null != go)
            {
                SetLocation(go.transform,pos,rot);
            }
        }
        
        public static void SetLocation(GameObject go, Vector3 pos, Vector3 angle)
        {
            if (null != go)
            {
                SetLocation(go.transform, pos, angle);
            }
        }
        

        public static void SetLocalLocation(Transform trans, Vector3 pos, Quaternion rot)
        {
            if (null != trans)
            {
                trans.SetLocalPositionAndRotation(pos,rot);
            }
        }
        
        public static void SetLocalLocation(Transform trans, Vector3 pos, Vector3 angle)
        {
            SetLocation(trans, pos, Quaternion.Euler(angle));
        }
   
        public static void SetLocalLocation(GameObject go, Vector3 pos, Quaternion rot)
        {
            if (null != go)
            {
                SetLocalLocation(go.transform, pos, rot);
            }
        }
        
        public static void SetLocalLocation(GameObject go, Vector3 pos, Vector3 angle)
        {
            if (null != go)
            {
                SetLocalLocation(go.transform, pos, angle);
            }
        }


        public static void SetLayer(GameObject go, string name)
        {
            if (null != go)
            {
                int layer = LayerMask.NameToLayer(name);
                if (go.layer != layer)
                {
                    childTrans.Clear();
                    go.GetComponentsInChildren<Transform>(true, childTrans);
                    foreach (var item in childTrans)
                    {
                        item.gameObject.layer = layer;
                    }
                }
            }
        }

        public static void SetLayer(Transform trans, string name)
        {
            if (null != trans)
            {
                SetLayer(trans.gameObject, name);
            }
        }

        public static void SetLayer(GameObject go, int layer)
        {
            if (null != go)
            {
                if (go.layer != layer)
                {
                    childTrans.Clear();
                    go.GetComponentsInChildren<Transform>(true, childTrans);
                    foreach (var item in childTrans)
                    {
                        item.gameObject.layer = layer;
                    }
                }
            }
        }

        public static void SetLayer(Transform trans, int layer)
        {
            if (null != trans)
            {
                SetLayer(trans.gameObject, layer);
            }
        }


        public static Component GetComponent(string goName, Type type)
        {
            if (!string.IsNullOrEmpty(goName) && null != type)
            {
                GameObject go = GameObject.Find(goName);
                if (null != go)
                {
                    return go.GetComponent(type);
                }
            }

            return null;
        }

        public static T GetComponent<T>(string goName) where T : Component
        {
            return GetComponent(goName, typeof(T)) as T;
        }
    }
}