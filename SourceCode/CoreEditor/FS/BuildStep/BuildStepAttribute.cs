using System;
using UnityEditor;

namespace CoreEditor.FS
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
    public class BuildStepAttribute : Attribute
    {
        public string Name;

        public string Describe;
    }
}
