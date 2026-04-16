#if I18N
using UnityEditor;
using UnityEngine;

namespace CoreEditor.I18N
{
    internal class I18NSettingsObject : SettingsObject
    {
        public string il8nPath;
        public bool useTextLocalized;

        public static I18NSettingsObject Get()
        {
            return Load<I18NSettingsObject>("Il8N");
        }

        private void Initial()
        {
            il8nPath = null;
            useTextLocalized = true;
        }
    }
}
#endif