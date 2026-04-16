using System;
using UnityEngine;

namespace Core
{
    public enum OrbitType
    {
        //直线
        Straight,

        //抛物线
        Parabola,
    }

    public class MovementOrbit
    {
        private Vector3 fromPos;
        private Vector3 toPos;
        private Vector3 deltaPos;
        private Vector3 curPos;
        private Vector3 velocityStart;
        public OrbitType orbitType;
        public float gravity = -9.8f;
        public float height = 1;

        private float totalTime;

        public Vector3 FromPos
        {
            get { return fromPos; }
        }

        public Vector3 ToPos
        {
            get { return toPos; }
        }

        public Vector3 DeltaPos
        {
            get { return deltaPos; }
        }

        public void SetPosition(Vector3 fromPos, Vector3 toPos)
        {
            this.fromPos = fromPos;
            this.toPos = toPos;
            deltaPos = toPos - fromPos;
            curPos = fromPos;

            if (orbitType == OrbitType.Parabola)
            {
                CalcParabola();
            }
        }

        private void CalcParabola()
        {
            float topY = fromPos.y > toPos.y ? fromPos.y : toPos.y;
            topY += height;
            float d1 = topY - fromPos.y;
            float d2 = topY - toPos.y;
            float g2 = 2 / -gravity;
            float t1 = (float)Math.Sqrt(g2 * d1);
            float t2 = (float)Math.Sqrt(g2 * d2);
            totalTime = t1 + t2;
            float vx = deltaPos.x / totalTime;
            float vy = -gravity * t1;
            float vz = deltaPos.z / totalTime;
            velocityStart.Set(vx, vy, vz);
        }

        public Vector3 GetPoint(float t)
        {
            if (t <= 0)
            {
                curPos = fromPos;
            }
            else if (t >= 1)
            {
                curPos = toPos;
            }
            else
            {
                if (orbitType == OrbitType.Straight)
                {
                    curPos = fromPos + deltaPos * t;
                }
                else
                {
                    float time = t * totalTime;
                    curPos = fromPos + velocityStart * time;

                    float dy = 0.5f * gravity * time * time;
                    curPos.y = fromPos.y + velocityStart.y * time + dy;
                }
            }

            return curPos;
        }
    }
}