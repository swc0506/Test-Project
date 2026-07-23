using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace LogicLayer
{
    public class RenderBehaviour : MonoBehaviour
    {
        public LogicObject LogicObject { get; protected set; }

        public virtual void OnCreate()
        {
        }

        public virtual void OnRelease()
        {
        }
    }
}