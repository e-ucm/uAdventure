using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using uAdventure.Runner;
using System;
using uAdventure.Core;
using dZine4D.Misc.QR;

namespace uAdventure.QR
{
    public class QRPrompt : MonoBehaviour, CustomEffectRunner
    {
        public QRReader reader;
        public Effect Effect {
            get { return QRPromptEffect; }
            set { QRPromptEffect = value as QRPromptEffect; }
        }
        public QRPromptEffect QRPromptEffect { get; set; }

        public Text text;
        private bool force_wait = true;

        void Start()
        {
            text.text = QRPromptEffect.PromptMessage;
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

            bool blackList = QRPromptEffect.SelectionType == QRPromptEffect.ListType.BlackList;
            bool contained = QRPromptEffect.ValidIds.Contains(content);

            if ((blackList && !contained) || (!blackList && contained))
            {
                qr = Game.Instance.GameState.FindElement<QR>(content);
                if(qr != null && ConditionChecker.check(qr.Conditions))
                {// Si existe y además cumple las condiciones

                    // Mostramos el contenido y el resto de efectos
                    var effects = new Effects();
                    if(qr.Content != "")
                    {
                        effects.add(new SpeakPlayerEffect(qr.Content));
                    }
                    foreach(var effect in qr.Effects.getEffects()) effects.add(effect);

                    effectHolder = new EffectHolder(effects);
                    this.transform.GetChild(0).gameObject.SetActive(false);
                }
            }
        }

        public void OnClosePrompt()
        {
            if(effectHolder == null)
            {
                force_wait = false;
                finished = true;
            }
        }

        public bool execute()
        {
            if (effectHolder != null)
            {
                force_wait = effectHolder.execute();
                if (!force_wait)
                    finished = true;
                //DestroyImmediate(this.gameObject);
            }

            return force_wait;
        }
    }
}