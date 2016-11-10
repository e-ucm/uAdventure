using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ConstantNames
{
    public struct Lang{
        public string[] Actions;
    }

    static Dictionary<string,Lang> languages;

    static public Dictionary<string,Lang> L{
        get {
            if (languages == null){
                languages = new Dictionary<string, Lang> ();

            //################# ENGLISH #################
            Lang english = new Lang();
            english.Actions = new string[] { "Examine", "Grab", "Give to", "Use With", "Use", "Custom", "Custom Interact", "Talk to" };
            languages.Add("EN",english);

            //################# SPANISH #################

            Lang spanish = new Lang();
            spanish.Actions = new string[] { "Examinar", "Agarrar", "Dar a", "Usar con", "Usar", "Personalizada", "Interacción Personalizada", "Hablar con" };

            languages.Add("ES",spanish);

            }

            return languages;
        }
    }
}