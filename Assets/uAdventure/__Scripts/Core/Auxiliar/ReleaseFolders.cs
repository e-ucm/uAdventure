using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.IO;

namespace uAdventure.Core
{
    /**
     * The only purpose of this class is to keep the path of the folders and files
     * which will be in the release in a common place for both engine and editor
     */
    public class ReleaseFolders
    {
        public static string PROJECTS_FOLDER = "../Projects";

        public static string EXPORTS_FOLDER = "../Exports";

        public static string REPORTS_FOLDER = "../Reports";

        public static readonly string WEB_FOLDER = "web";

        public static readonly string WEB_TEMP_FOLDER = "web/temp";

        public static readonly string CONFIG_FILE_PATH_EDITOR = "config_editor.xml";

        public static readonly string CONFIG_FILE_PATH_ENGINE = "config_engine.xml";

        public static readonly string LANGUAGE_DIR_EDITOR = "i18n/editor";

        public static readonly string LANGUAGE_DIR_ENGINE = "i18n/engine";

        public static readonly string IMAGE_LOADING_DIR = "img/loading";

        //  public static HashMap<string, string> languageNames = new HashMap<string, string>();

        /**
         * Language constant for Unknown language
         */
        //public static readonly string LANGUAGE_UNKNOWN = "es_ES";

        public static readonly string LANGUAGE_SPANISH = "es_ES";

        public static readonly string LANGUAGE_ENGLISH = "en_EN";

        public static readonly string LANGUAGE_DEUTSCH = "de_DE";

        public static readonly string LANGUAGE_GALEGO = "gl_ES";

        public static readonly string LANGUAGE_ITALIANO = "it_IT";

        public static readonly string LANGUAGE_PORTUGESE = "pt_PT";

        public static readonly string LANGUAGE_PORTUGESE_BRAZIL = "pt_BR";

        public static readonly string LANGUAGE_ROMANIA = "ru_RU";

        public static readonly string LANGUAGE_RUSSIAN = "ro_RO";

        public static readonly string LANGUAGE_CHINA = "zh_CN";

        /**
         * Language constant for Default language
         */
        public static readonly string LANGUAGE_DEFAULT = LANGUAGE_ENGLISH;

        /**
         * Returns the relative path of a language file for both editor and engine
         * NOTE: To be used only from editor
         */
        public static string getLanguageFilePath4Editor(bool editor, string language)
        {
            if (editor)
            {
                return "Assets/uAdventure/Editor/Resources/" + LANGUAGE_DIR_EDITOR + "/" + language + ".xml";
            }
            else
            {
                return Path.Combine("Assets/uAdventure/Resources/" + LANGUAGE_DIR_ENGINE, language + ".xml");
            }
        }

        /**
         * Returns the relative path of a language file NOTE: To be used only from
         * engine
         */
        public static string getLanguageFilePath4Engine(string language)
        {

            string path = LANGUAGE_DIR_ENGINE + "/";
            path += language + ".xml";
            return path;
        }

        /**
         * Returns the language ({@link #LANGUAGE_ENGLISH} or
         * {@value #LANGUAGE_SPANISH}) associated to the relative path passed as
         * argument. If no language is recognized, or if path is null, the method
         * returns {@value #LANGUAGE_DEFAULT}
         * 
         * @param path
         * @return
         */
        public static string getLanguageFromPath(string path)
        {
            if (path != null && path.EndsWith(".xml"))
            {
                return path.Substring(path.Length - 9, path.Length - 4);
            }
            else
                return LANGUAGE_DEFAULT;

        }

        public static string getAboutFilePath(string s)
        {

            return "about-" + s + ".html";
        }

        public static string getDefaultAboutFilePath()
        {

            return "about-" + LANGUAGE_DEFAULT + ".html";
        }

        public static string getLanguageFilePath(string language)
        {

            return language + ".xml";
        }

        /**
         * @param projects_folder
         *            the pROJECTS_FOLDER to set
         */
        public static void setProjectsPath(string projects_folder)
        {

            PROJECTS_FOLDER = projects_folder;
        }

        /**
         * @param exports_folder
         *            the eXPORTS_FOLDER to set
         */
        public static void setExportsPath(string exports_folder)
        {

            EXPORTS_FOLDER = exports_folder;
        }

        /**
         * @param reports_folder
         *            the rEPORTS_FOLDER to set
         */
        public static void setReportsPath(string reports_folder)
        {

            REPORTS_FOLDER = reports_folder;
        }

        public static string configFileEditorRelativePath()
        {

            return "Assets/uAdventure/Resources/" + CONFIG_FILE_PATH_EDITOR;
        }
        //public static List<string> getLanguages(string where)
        //{
        //    File directory = new File("i18n" + File.separator + where);
        //    List<string> languages = new ArrayList<string>();
        //    for (File file : directory.listFiles())
        //    {
        //        if (file.getName().endsWith("xml"))
        //        {
        //            string identifier = file.getName().substring(0, file.getName().length() - 4);
        //            languages.add(identifier);
        //            Properties prop = new Properties();
        //            try
        //            {
        //                prop.loadFromXML(new FileInputStream(file));
        //                languageNames.put(identifier, (string)prop.get("Language.Name"));
        //            }
        //            catch (InvalidPropertiesFormatException e)
        //            {
        //                e.printStackTrace();
        //            }
        //            catch (FileNotFoundException e)
        //            {
        //                e.printStackTrace();
        //            }
        //            catch (IOException e)
        //            {
        //                e.printStackTrace();
        //            }
        //        }
        //    }
        //    return languages;
        //}

        //public static string getLanguageName(string language)
        //{
        //    return languageNames.get(language);
        //}
    }
}