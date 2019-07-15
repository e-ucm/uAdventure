using UnityEngine;
using UnityEngine.UI;
using uAdventure.Runner;
using uAdventure.Core;
using dZine4D.Misc.QR;

namespace uAdventure.QR
{
    public class QRScanner : MonoBehaviour, CustomEffectRunner
    {
        public QRReader reader;
        public IEffect Effect {
            get { return QrScannerEffect; }
            set { QrScannerEffect = value as QRScannerEffect; }
        }
        public QRScannerEffect QrScannerEffect { get; set; }

        public Text text;
        private bool force_wait = true;

        void Start()
        {
            text.text = QrScannerEffect.ScannerMessage;
            reader.OnQrCodeDetected.AddListener(OnQRCode);
        }

        bool finished = false;
        void Update()
        {
            if (finished)
            {
                DestroyImmediate(this.gameObject);
            }
        }

        private QR qr;
        private EffectHolder effectHolder;
        public void OnQRCode(string content)
        {
            if (qr != null)
                return;

            bool blackList = QrScannerEffect.SelectionType == QRScannerEffect.ListType.BlackList;
            bool contained = QrScannerEffect.ValidIds.Contains(content);

            if ((blackList && !contained) || (!blackList && contained))
            {
                qr = Game.Instance.GameState.FindElement<QR>(content);
                if(qr != null && ConditionChecker.check(qr.Conditions))
                {// Si existe y además cumple las condiciones

                    // Mostramos el contenido y el resto de efectos
                    var effects = new Effects();
                    if(qr.Content != "")
                    {
                        effects.Add(new SpeakPlayerEffect(qr.Content));
                    }
                    foreach(var effect in qr.Effects.getEffects())
                    {
                        effects.Add(effect);
                    }

                    effectHolder = new EffectHolder(effects);
                    this.transform.GetChild(0).gameObject.SetActive(false);
                    Game.Instance.PulseOnTime(effectHolder.effects[0], 0);
                }
            }
        }

        public void OnClosePrompt()
        {
            if(effectHolder == null)
            {
                force_wait = false;
                finished = true;
                Game.Instance.PulseOnTime(null, 0);
            }
        }

        public bool execute()
        {
            if (effectHolder != null)
            {
                force_wait = effectHolder.execute();
                if (!force_wait)
                {
                    finished = true;
                }
            }

            return force_wait;
        }
    }
}