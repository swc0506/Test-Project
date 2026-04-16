using UnityEngine;

namespace Core
{
    public class Shaker
    {
        //震动偏移位置
        private Vector3 shakePos;

        //震动偏移角度
        private Vector3 shakeAngle;

        //震动周期
        private float shakeTime;

        //震动次数
        private int shakeCount;


        private Transform target;
        private Vector3 defPos;
        private Vector3 defAngle;
        private int curShakeCount;

        private float remainShakeTime;

        private Vector3 curPos;
        private Vector3 curAngle;

        public void SetShakeTarget(Transform target)
        {
            this.target = target;
            defPos = target.localPosition;
            defAngle = target.localEulerAngles;
            remainShakeTime = 0;
        }

        public void Shake(Vector3 pos, Vector3 angle, float time, int count)
        {
            shakePos = pos;
            shakeAngle = angle;
            shakeTime = time;
            shakeCount = count;

            curShakeCount = 0;
            curPos = pos;
            curAngle = angle;
        }

        public void Stop()
        {
            curShakeCount = shakeCount = 0;
            target.localPosition = defPos;
            target.localEulerAngles = defAngle;
        }

        public void Update(float deltaTime)
        {
            if (null == target || curShakeCount >= shakeCount)
            {
                return;
            }

            remainShakeTime += deltaTime;
            while (remainShakeTime >= shakeTime)
            {
                remainShakeTime -= shakeTime;
                curShakeCount += 1;
                if (curShakeCount >= shakeCount)
                {
                    target.localPosition = defPos;
                    target.localEulerAngles = defAngle;
                    remainShakeTime = 0;
                    return;
                }

                float average = (shakeCount - curShakeCount) / (float)shakeCount;
                curPos = shakePos * average;
                curAngle = shakeAngle * average;
            }

            float offsetValue = Mathf.Sin(2 * remainShakeTime / shakeTime * Mathf.PI);
            Vector3 pos = defPos + curPos * offsetValue;
            Vector3 angle = defAngle + curAngle * offsetValue;
            target.localPosition = pos;
            target.localEulerAngles = angle;
        }
    }
}