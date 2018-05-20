using UnityEditor;
using UnityEngine;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ConfigurationMenu : WindowMenuContainer
    {
        private SetLanguageDeutschMenuItem setLanguageDeutsch;
        private SetLanguageEnglishMenuItem setLanguageEnglish;
        private SetLanguageSpanishMenuItem setLanguageSpanish;
        private SetLanguageGalegoMenuItem setLanguageGalego;
        private SetLanguageItalianoMenuItem setLanguageItaliano;
        private SetLanguagePortugeseMenuItem setLanguagePortugese;
        private SetLanguagePortugeseBrazilMenuItem setLanguagePortugeseBrazil;
        private SetLanguageRomaniaMenuItem setLanguageRomania;
        private SetLanguageRussiaMenuItem setLanguageRussia;
        private SetLanguageChinaMenuItem setLanguageChina;

        public ConfigurationMenu()
        {
            SetMenuItems();
        }

        protected override void Callback(object obj)
        {
            if ((obj as SetLanguageDeutschMenuItem) != null)
                setLanguageDeutsch.OnCliked();
            else if ((obj as SetLanguageEnglishMenuItem) != null)
                setLanguageEnglish.OnCliked();
            else if ((obj as SetLanguageSpanishMenuItem) != null)
                setLanguageSpanish.OnCliked();
            else if ((obj as SetLanguageGalegoMenuItem) != null)
                setLanguageGalego.OnCliked();
            else if ((obj as SetLanguageItalianoMenuItem) != null)
                setLanguageItaliano.OnCliked();
            else if ((obj as SetLanguagePortugeseMenuItem) != null)
                setLanguagePortugese.OnCliked();
            else if ((obj as SetLanguagePortugeseBrazilMenuItem) != null)
                setLanguagePortugeseBrazil.OnCliked();
            else if ((obj as SetLanguageRomaniaMenuItem) != null)
                setLanguageRomania.OnCliked();
            else if ((obj as SetLanguageRussiaMenuItem) != null)
                setLanguageRussia.OnCliked();
            else if ((obj as SetLanguageChinaMenuItem) != null)
                setLanguageChina.OnCliked();
        }

        protected override void SetMenuItems()
        {
            menu = new GenericMenu();

            setLanguageDeutsch = new SetLanguageDeutschMenuItem();
            setLanguageEnglish = new SetLanguageEnglishMenuItem();
            setLanguageSpanish = new SetLanguageSpanishMenuItem();
            setLanguageGalego = new SetLanguageGalegoMenuItem();
            setLanguageItaliano = new SetLanguageItalianoMenuItem();
            setLanguagePortugese = new SetLanguagePortugeseMenuItem();
            setLanguagePortugeseBrazil = new SetLanguagePortugeseBrazilMenuItem();
            setLanguageRomania = new SetLanguageRomaniaMenuItem();
            setLanguageRussia = new SetLanguageRussiaMenuItem();
            setLanguageChina = new SetLanguageChinaMenuItem();

            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguageDeutsch.Label), false, Callback, setLanguageDeutsch);
            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguageEnglish.Label), false, Callback, setLanguageEnglish);
            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguageSpanish.Label), false, Callback, setLanguageSpanish);
            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguageGalego.Label), false, Callback, setLanguageGalego);
            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguageItaliano.Label), false, Callback, setLanguageItaliano);
            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguagePortugese.Label), false, Callback, setLanguagePortugese);
            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguagePortugeseBrazil.Label), false, Callback, setLanguagePortugeseBrazil);
            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguageRomania.Label), false, Callback, setLanguageRomania);
            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguageRussia.Label), false, Callback, setLanguageRussia);
            menu.AddItem(new GUIContent(TC.get("MenuConfiguration.Language") + "/" + setLanguageChina.Label), false, Callback, setLanguageChina);
        }
    }
}