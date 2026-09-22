using UnityEngine;

namespace uAdventure.Simva
{
    [CreateAssetMenu(fileName = "SimvaPluginSettings", menuName = "uAdventure/Simva/Simva Plugin Settings")]
    public class SimvaPluginSettings : ScriptableObject
    {
        public bool SaveAuthUntilCompleted = true;
        public bool RunGameIfSimvaIsNotConfigured = true;
        public bool ContinueOnQuit = true;
        public bool EnableLoginDemoButton = true;
        public string LanguageByDefault;
        public bool SaveDisclaimerAccepted = false;
        public bool BasicScormXAPIDataManagementByGame = false;
        public bool EnableDebugLogging = false;
    }
}
