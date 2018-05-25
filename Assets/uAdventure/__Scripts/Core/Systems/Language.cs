using UnityEngine;
using System.Collections.Generic;
using System;

/// <summary>
/// Klasa powiązana z systemem językowym aplikacji.
/// </summary>
using System.Text.RegularExpressions;

namespace uAdventure.Core
{
    public class Language : MonoBehaviour
    {

        public static bool Initialized { get; private set; }

        #region Zmienne publiczne

        // Lista nazw dostępnych języków
        public static List<string> LanguageNames;

        #endregion

        #region Zmienne prywatne

        // Słownik etykiet i powiązanych z nimi tekstów.
        private static Dictionary<string, string> LanguageTexts;

        #endregion

        #region Metody publiczne 

        /// <summary>
        /// Metoda wywoływana w celu odświeżenia list tłumaczeń możliwych do wybrania 
        /// w przypadku komponentów LanguageHelper
        /// </summary>
        public static void Reload()
        {
            LanguageNames.Clear();
            LanguageTexts.Clear();
            var languagesFile = Resources.Load<TextAsset>("languages");
            string serializedLangbase = languagesFile ? languagesFile.text.Replace(Environment.NewLine, "") : "";

            Settings.LanguageOption = "English";
            try
            {
                LangbaseRoot LangTexts = LangbaseRoot.LoadFromText(serializedLangbase);
                for (int i = 0; i < LangTexts.Labels.Label.Count; i++)
                {
                    for (int j = 0; j < LangTexts.Labels.Label[i].Text.Count; j++)
                    {
                        //Debug.Log ("ID = " + LangTexts.Labels.Label [i].Id);
                        if (LangTexts.Labels.Label[i].Text[j].Lang.Equals(Settings.LanguageOption)
                            && LangTexts.Labels.Label[i].Text[j].TextValue.Length > 0)
                        {
                            string textToAdd = LangTexts.Labels.Label[i].Text[j].TextValue
                                .Replace("\n", "")
                                .Replace("  ", "")
                                .Replace("\\br", "<br/>");

                            LanguageTexts.Add(
                                LangTexts.Labels.Label[i].Id,
                                textToAdd
                            );
                        }
                    }
                }
            }
            catch
            {
                Debug.LogError("Critical error: language XML is broken. Ensure that file is a valid XML and there is no duplicates.");
            }
        }

        /// <summary>
        /// Metoda zwracająca tłumaczenie tekstu przekazanego w parametrze
        /// </summary>
        public static string GetText(string id, bool withOutNewLine = false)
        {
            if (LanguageTexts.ContainsKey(id))
            {
                if (withOutNewLine)
                {
                    return LanguageTexts[id];
                }
                else
                {
                    return LanguageTexts[id].Replace("\\n", Environment.NewLine);
                }
            }
            else
            {
                return "ERROR_LABEL";
            }
        }

        public static void Initialize()
        {
            if (Initialized)
                return;

            LanguageNames = new List<string>();
            LanguageTexts = new Dictionary<string, string>();
            Reload();

            Initialized = true;
        }

        #endregion
    }
}