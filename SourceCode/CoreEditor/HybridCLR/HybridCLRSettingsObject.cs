#if HYBRIDCLR
using System.Collections.Generic;

namespace CoreEditor.HybridCLR
{
    public class HybridCLRSettingsObject : SettingsObject
    {
        public bool addCodeProjectsToSolution = true;
        public bool autoCompileCodeProjects = true;

        public List<string> projects;

        protected override void OnInitial()
        {
            projects = new List<string>();
        }

        public static HybridCLRSettingsObject Get()
        {
            return Load<HybridCLRSettingsObject>("HybridCLR");
        }
    }
}

#endif