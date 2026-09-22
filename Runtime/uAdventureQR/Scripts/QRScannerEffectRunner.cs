using UnityEngine;
using uAdventure.Runner;
using uAdventure.Core;

namespace uAdventure.QR
{
    [CustomEffectRunner(typeof(QRScannerEffect))]
    public class QRScannerEffectRunner : CustomEffectRunner
    {
        private readonly GameObject QRScannerPrefab;
        private QRScanner scanner;
        bool first;
        public QRScannerEffectRunner()
        {
            first = true;
            QRScannerPrefab = Resources.Load<GameObject>("QRScanner");
        }

        public IEffect Effect { get; set; }

        public bool execute()
        {
            if (first)
            {
                first = false;
                scanner = GameObject.Instantiate(QRScannerPrefab).GetComponent<QRScanner>();
                scanner.Effect = this.Effect;
            }

            // if the prompt is closed, the effect is finished
            return scanner && scanner.execute();
        }
    }
}