using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uAdventure.Core
{
    /// <summary>
    /// Klasa zarządzająca ustawieniami aplikacji.
    /// </summary>
    public class Settings : MonoBehaviour
    {

        /// <summary>
        /// Właściwość (C# Property) odpowiadająca za zapamietywanie i odczytywanie
        /// nazwy wybranego języka.
        /// </summary>
        public static string LanguageOption
        {
            get
            {
                if (PlayerPrefs.GetString("Language").Length == 0)
                {
                    if (Application.systemLanguage == SystemLanguage.Polish)
                    {
                        PlayerPrefs.SetString("Language", "Polski");
                        PlayerPrefs.Save();
                        return "Polski";
                    }
                    else
                    {
                        PlayerPrefs.SetString("Language", "English");
                        PlayerPrefs.Save();
                        return "English";
                    }
                }
                else
                {
                    return PlayerPrefs.GetString("Language");
                }
            }

            set
            {
                PlayerPrefs.SetString("Language", value);
                PlayerPrefs.Save();
                Debug.Log("Language is " + value);
            }
        }
    }
}