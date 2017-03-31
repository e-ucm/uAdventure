using UnityEngine;
using uAdventure.Runner;
using uAdventure.Core;

namespace uAdventure.QR
{
    [CustomEffectRunner(typeof(QRPromptEffect))]
    public class QRPromptEffectRunner : CustomEffectRunner
    {
        private GameObject QRPromptPrefab;
        private QRPrompt prompt;
        bool first;
        public QRPromptEffectRunner()
        {
            first = true;
            QRPromptPrefab = Resources.Load<GameObject>("QRPrompt");
        }

        public Effect Effect { get; set; }

        public bool execute()
        {
            if (first)
            {
                first = false;
                prompt = GameObject.Instantiate(QRPromptPrefab).GetComponent<QRPrompt>();
                prompt.Effect = this.Effect;
            }

            // if the prompt is closed, the effect is finished
            return prompt && prompt.execute();
        }
    }
}