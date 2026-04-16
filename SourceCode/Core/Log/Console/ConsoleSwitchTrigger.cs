using System;
using System.Collections.Generic;
using UnityEngine;

namespace Core.Log
{
    /// <summary>
    /// log控制台开关触发器器
    /// </summary>
    public interface ISwitchTrigger
    {
        /// <summary>
        /// 是否触发
        /// </summary>
        /// <returns></returns>
        bool Trigger();
    }


    /// <summary>
    /// 台默认开关触发器器 目前支持
    /// 1.按`键
    /// 2.手势画半径为屏幕1/3的2个圆
    /// </summary>
    internal class DefaultSwitchTrigger : ISwitchTrigger
    {
        private const float INTERVAL = 0.4f;
        private const int NEED_NUM = 5;

        private float prevDownTime;
        private int validNum;


        public bool Trigger()
        {
            if (!Application.isMobilePlatform)
            {
                if (Input.GetKeyDown(KeyCode.BackQuote))
                {
                    return true;
                }
            }

            return IsGestureDone();
        }

        private bool IsGestureDone()
        {
            if (Input.GetMouseButtonDown(0))
            {
                float currTime = Time.unscaledTime;
                if (prevDownTime == 0)
                {
                    prevDownTime = currTime;
                }

                if (RecordValidNum(currTime))
                {
                    if (validNum >= NEED_NUM)
                    {
                        ResetValidNum();
                        return true;
                    }
                }
                else
                {
                    ResetValidNum();
                }
            }

            return false;
        }

        private bool RecordValidNum(float currTime)
        {
            if (currTime - prevDownTime < INTERVAL)
            {
                Vector3 touchPos = Input.mousePosition;
                Vector2 size = new Vector2(Screen.width * 0.5f, Screen.height * 0.5f);
                if (touchPos.x > size.x && touchPos.y > size.y)
                {
                    validNum++;
                    prevDownTime = currTime;
                    return true;
                }
            }

            return false;
        }

        private void ResetValidNum()
        {
            prevDownTime = 0;
            validNum = 0;
        }
    }
}