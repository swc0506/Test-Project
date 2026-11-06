using UnityEngine;
using System.Collections;

namespace UnityEngine.UI
{
    public struct Range
    {
        public Range(float min, float max)
        {
            Min = min;
            Max = max;
        }

        public float Min;
        public float Max;
    }

    public struct Range2
    {
        public Range2(Vector2 min, Vector2 max)
        {
            Min = min;
            Max = max;
        }

        Vector2 Min;
        Vector2 Max;
    }

    public class GeUIEffect : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}
