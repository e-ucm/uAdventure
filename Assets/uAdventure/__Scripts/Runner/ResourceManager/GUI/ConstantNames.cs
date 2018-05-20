using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Runner
{
    public class ConstantNames
    {
        public struct Lang
        {
            public string[] Actions;
        }

        static Dictionary<string, Lang> languages;

        static public Dictionary<string, Lang> L
        {
            get
            {
                if (languages == null)
                {
                    languages = new Dictionary<string, Lang>();

                    //################# ENGLISH #################
                    Lang english = new Lang();
                    english.Actions = new string[] { "Examine", "Grab", "Give to", "Use With", "Use", "Custom", "Custom Interact", "Talk to", "Drag to" };
                    languages.Add("EN", english);

                    //################# SPANISH #################

                    Lang spanish = new Lang();
                    spanish.Actions = new string[] { "Examinar", "Agarrar", "Dar a", "Usar con", "Usar", "Personalizada", "Interacción Personalizada", "Hablar con", "Arrastrar a" };

                    languages.Add("ES", spanish);

                }

                return languages;
            }
        }
    }
}