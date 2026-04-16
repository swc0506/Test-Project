using System;
using UnityEngine;

namespace Core
{
    public class MonoEventProxy : MonoSingleton<MonoEventProxy>
    {
        private Action<float> updateEvent;

        public event Action<float> UpdateEvent
        {
            add { updateEvent += value; }
            remove { updateEvent -= value; }
        }


        private Action lateUpdateEvent;

        public event Action LateUpdateEvent
        {
            add { lateUpdateEvent += value; }
            remove { lateUpdateEvent -= value; }
        }

        private Action<float> fixedUpdateEvent;

        public event Action<float> FixedUpdateEvent
        {
            add { fixedUpdateEvent += value; }
            remove { fixedUpdateEvent -= value; }
        }


        private Action screenSizeChangeEvent;

        public event Action ScreenSizeChangeEvent
        {
            add { screenSizeChangeEvent += value; }
            remove { screenSizeChangeEvent -= value; }
        }


        private Action applicationQuitEvent;

        public event Action ApplicationQuitEvent
        {
            add { applicationQuitEvent += value; }
            remove { applicationQuitEvent -= value; }
        }

        private const int CHECK_SCREEN_INTERVAL = 2;
        private ScreenOrientation prevScreenOrientation;
        private int prevScreenWidth;
        private int prevScreenHeight;
        
        private int elapseFrame;
        private bool screenChanged;

        protected override void OnInitial()
        {
            base.OnInitial();
            gameObject.hideFlags = HideFlags.HideInHierarchy;
            prevScreenOrientation = Screen.orientation;
            prevScreenWidth = Screen.width;
            prevScreenHeight = Screen.height;
        }

        protected override void OnDispose()
        {
            base.OnDispose();
            updateEvent = null;
            lateUpdateEvent = null;
            screenSizeChangeEvent = null;
            applicationQuitEvent = null;
        }

        private void Update()
        {
            updateEvent?.Invoke(Time.deltaTime);
        }

        private void LateUpdate()
        {
            lateUpdateEvent?.Invoke();
        }

        private void FixedUpdate()
        {
            fixedUpdateEvent?.Invoke(Time.fixedDeltaTime);
           
            if (++elapseFrame == CHECK_SCREEN_INTERVAL)
            {
                elapseFrame = 0;
                if (CheckScreenSizeChange())
                {
                    screenChanged = true;
                }
            }

            if (screenChanged && elapseFrame == 0)
            {
                screenChanged = false;
                screenSizeChangeEvent?.Invoke();
            }
        }

        private bool CheckScreenSizeChange()
        {
            if (prevScreenOrientation != Screen.orientation)
            {
                prevScreenOrientation = Screen.orientation;
                return true;
            }

            if (prevScreenWidth != Screen.width || prevScreenHeight != Screen.height)
            {
                prevScreenWidth = Screen.width;
                prevScreenHeight = Screen.height;
                return true;
            }

            return false;
        }
        
        private void OnApplicationQuit()
        {
            applicationQuitEvent?.Invoke();
        }
    }
}