using System;
using UnityEngine;

namespace Core.Log
{
    public partial class Commander
    {
        private void RegisterUnityType()
        {
            parser.AddContainsFallback(ContainsType);
            parser.AddParseFallback(ParseType);
            parser.RegisterParse(typeof(Vector2), ParseVector2);
            parser.RegisterParse(typeof(Vector3), ParseVector3);
            parser.RegisterParse(typeof(Vector4), ParseVector4);
            parser.RegisterParse(typeof(Quaternion), ParseQuaternion);
            parser.RegisterParse(typeof(Color), ParseColor);
            parser.RegisterParse(typeof(Color32), ParseColor32);
            parser.RegisterParse(typeof(Rect), ParseRect);
            parser.RegisterParse(typeof(RectOffset), ParseRectOffset);
            parser.RegisterParse(typeof(Bounds), ParseBounds);
            parser.RegisterParse(typeof(GameObject), ParseGameObject);
        }

        private bool ContainsType(Type type)
        {
            return typeof(Component).IsAssignableFrom(type);
        }

        private bool ParseType(string input, Type type, out object output)
        {
            if (typeof(Component).IsAssignableFrom(type))
            {
                GameObject gameObject = input == "null" ? null : GameObject.Find(input);
                output = gameObject ? gameObject.GetComponent(type) : null;
                return true;
            }

            output = null;
            return false;
        }

        private float[] ParseFloatArray(string input)
        {
            int count = 0;
            if (splitter.TrySplitParameters(input, out var args))
            {
                count = args.Count;
            }

            float[] values = new float[count];
            for (int i = 0; i < count; i++)
            {
                if (!parser.TryParse(args[i], typeof(float), out var val))
                {
                    return null;
                }

                values[i] = (float) val;
            }

            return values;
        }

        private bool ParseVector2(string input, out object output)
        {
            float[] values = ParseFloatArray(input);
            if (null == values)
            {
                output = null;
                return false;
            }

            Vector2 res = Vector2.zero;
            for (int i = 0; i < values.Length; i++)
            {
                res[i] = values[i];
            }

            output = res;
            return true;
        }

        private bool ParseVector3(string input, out object output)
        {
            float[] values = ParseFloatArray(input);
            if (null == values)
            {
                output = null;
                return false;
            }

            Vector3 res = Vector3.zero;
            for (int i = 0; i < values.Length; i++)
            {
                res[i] = values[i];
            }

            output = res;
            return true;
        }

        private bool ParseVector4(string input, out object output)
        {
            float[] values = ParseFloatArray(input);
            if (null == values)
            {
                output = null;
                return false;
            }

            Vector4 res = Vector4.zero;
            for (int i = 0; i < values.Length; i++)
            {
                res[i] = values[i];
            }

            output = res;
            return true;
        }

        private bool ParseQuaternion(string input, out object output)
        {
            float[] values = ParseFloatArray(input);
            if (null == values)
            {
                output = null;
                return false;
            }

            Quaternion res = Quaternion.identity;
            for (int i = 0; i < values.Length; i++)
            {
                res[i] = values[i];
            }

            output = res;
            return true;
        }

        private bool ParseColor(string input, out object output)
        {
            float[] values = ParseFloatArray(input);
            if (null == values)
            {
                output = null;
                return false;
            }

            Color res = Color.black;
            for (int i = 0; i < values.Length; i++)
            {
                res[i] = values[i];
            }

            output = res;
            return true;
        }

        private bool ParseColor32(string input, out object output)
        {
            float[] values = ParseFloatArray(input);
            if (null == values)
            {
                output = null;
                return false;
            }

            Color32 res = Color.black;
            res.r = values.Length > 0 ? (byte) Mathf.RoundToInt(values[0]) : res.r;
            res.g = values.Length > 2 ? (byte) Mathf.RoundToInt(values[1]) : res.g;
            res.b = values.Length > 2 ? (byte) Mathf.RoundToInt(values[2]) : res.b;
            res.a = values.Length > 3 ? (byte) Mathf.RoundToInt(values[3]) : res.a;
            output = res;
            return true;
        }

        private bool ParseRect(string input, out object output)
        {
            float[] values = ParseFloatArray(input);
            if (null == values)
            {
                output = null;
                return false;
            }

            Rect res = Rect.zero;
            res.x = values.Length > 0 ? values[0] : res.x;
            res.y = values.Length > 1 ? values[1] : res.y;
            res.width = values.Length > 2 ? values[2] : res.width;
            res.height = values.Length > 3 ? values[3] : res.height;
            output = res;
            return true;
        }

        private bool ParseRectOffset(string input, out object output)
        {
            float[] values = ParseFloatArray(input);
            if (null == values)
            {
                output = null;
                return false;
            }

            RectOffset res = new RectOffset();
            res.left = values.Length > 0 ? Mathf.RoundToInt(values[0]) : 0;
            res.right = values.Length > 1 ? Mathf.RoundToInt(values[1]) : 0;
            res.top = values.Length > 2 ? Mathf.RoundToInt(values[2]) : 0;
            res.bottom = values.Length > 3 ? Mathf.RoundToInt(values[3]) : 0;
            output = res;
            return true;
        }

        private bool ParseBounds(string input, out object output)
        {
            float[] values = ParseFloatArray(input);
            if (null == values)
            {
                output = null;
                return false;
            }

            Vector3 center = Vector3.zero;
            for (int i = 0; i < values.Length && i < 3; i++)
            {
                center[i] = values[i];
            }

            Vector3 size = Vector3.zero;
            for (int i = 3; i < values.Length && i < 6; i++)
            {
                size[i - 3] = values[i];
            }

            Bounds res = new Bounds(center, size);
            output = res;
            return true;
        }

        private bool ParseGameObject(string input, out object output)
        {
            GameObject gameObject = input == "null" ? null : GameObject.Find(input);
            output = gameObject;
            return true;
        }
    }
}