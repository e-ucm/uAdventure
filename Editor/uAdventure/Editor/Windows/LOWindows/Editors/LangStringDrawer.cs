using IMS.MD.v1p2;
using IMS.MD.v1p3p2;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core.Metadata;
using UnityEditor;
using UnityEngine;

namespace uAdventure.Editor
{
    [CustomPropertyDrawer(typeof(LangString))]
    [CustomPropertyDrawer(typeof(LangStringType))]
    [CustomPropertyDrawer(typeof(langType))]
    public class LangStringDrawer : PropertyDrawer
    {
        private Dictionary<string, string> languages =
            new Dictionary<string, string>()
            {
            {"de","alemán"},
            {"en","inglés"},
            {"es","español"},
            {"fr","francés"},
            {"it","italiano"},
            {"aa","afar"},
            {"ab","abjasio(o abjasiano)"},
            {"ae","avéstico"},
            {"af","afrikáans"},
            {"ak","akano"},
            {"am","amhárico"},
            {"an","aragonés"},
            {"ar","árabe"},
            {"as","asamés"},
            {"av","avar(o ávaro)"},
            {"ay","aimara"},
            {"az","azerí"},
            {"ba","baskir"},
            {"be","bielorruso"},
            {"bg","búlgaro"},
            {"bh","bhoyapurí"},
            {"bi","bislama"},
            {"bm","bambara"},
            {"bn","bengalí"},
            {"bo","tibetano"},
            {"br","bretón"},
            {"bs","bosnio"},
            {"ca","catalán"},
            {"ce","checheno"},
            {"ch","chamorro"},
            {"co","corso"},
            {"cr","cree"},
            {"cs","checo"},
            {"cu","eslavo eclesiástico antiguo"},
            {"cv","chuvasio"},
            {"cy","galés"},
            {"da","danés"},
            {"dv","maldivo(o dhivehi)"},
            {"dz","dzongkha"},
            {"ee","ewé"},
            {"el","griego(moderno)"},
            {"eo","esperanto"},
            {"et","estonio"},
            {"eu","euskera"},
            {"fa","persa"},
            {"ff","fula"},
            {"fi","finés(o finlandés)"},
            {"fj","fiyiano(o fiyi)"},
            {"fo","feroés"},
            {"fy","frisón(o frisio)"},
            {"ga","irlandés(o gaélico)"},
            {"gd","gaélico escocés"},
            {"gl","gallego"},
            {"gn","guaraní"},
            {"gu","guyaratí(o gujaratí)"},
            {"gv","manés(gaélico manés o de Isla de Man)"},
            {"ha","hausa"},
            {"he","hebreo"},
            {"hi","hindi(o hindú)"},
            {"ho","hiri motu"},
            {"hr","croata"},
            {"ht","haitiano"},
            {"hu","húngaro"},
            {"hy","armenio"},
            {"hz","herero"},
            {"ia","interlingua"},
            {"id","indonesio"},
            {"ie","occidental"},
            {"ig","igbo"},
            {"ii","yi de Sichuán"},
            {"ik","iñupiaq"},
            {"io","ido"},
            {"is","islandés"},
            {"iu","inuktitut(o inuit)"},
            {"ja","japonés"},
            {"jv","javanés"},
            {"ka","georgiano"},
            {"kg","kongo(o kikongo)"},
            {"ki","kikuyu"},
            {"kj","kuanyama"},
            {"kk","kazajo(o kazajio)"},
            {"kl","groenlandés(o kalaallisut)"},
            {"km","camboyano(o jemer)"},
            {"kn","canarés"},
            {"ko","coreano"},
            {"kr","kanuri"},
            {"ks","cachemiro(o cachemir)"},
            {"ku","kurdo"},
            {"kv","komi"},
            {"kw","córnico"},
            {"ky","kirguís"},
            {"la","latín"},
            {"lb","luxemburgués"},
            {"lg","luganda"},
            {"li","limburgués"},
            {"ln","lingala"},
            {"lo","lao"},
            {"lt","lituano"},
            {"lu","luba-katanga(o chiluba)"},
            {"lv","letón"},
            {"mg","malgache(o malagasy)"},
            {"mh","marshalés"},
            {"mi","maorí"},
            {"mk","macedonio"},
            {"ml","malayalam"},
            {"mn","mongol"},
            {"mr","maratí"},
            {"ms","malayo"},
            {"mt","maltés"},
            {"my","birmano"},
            {"na","nauruano"},
            {"nb","noruego bokmål"},
            {"nd","ndebele del norte"},
            {"ne","nepalí"},
            {"ng","ndonga"},
            {"nl","neerlandés(u holandés)"},
            {"nn","nynorsk"},
            {"no","noruego"},
            {"nr","ndebele del sur"},
            {"nv","navajo"},
            {"ny","chichewa"},
            {"oc","occitano"},
            {"oj","ojibwa"},
            {"om","oromo"},
            {"or","oriya"},
            {"os","osético(u osetio, u oseta)"},
            {"pa","panyabí(o penyabi)"},
            {"pi","pali"},
            {"pl","polaco"},
            {"ps","pastú(o pastún, o pashto)"},
            {"pt","portugués"},
            {"qu","quechua"},
            {"rm","romanche"},
            {"rn","kirundi"},
            {"ro","rumano"},
            {"ru","ruso"},
            {"rw","ruandés(o kiñaruanda)"},
            {"sa","sánscrito"},
            {"sc","sardo"},
            {"sd","sindhi"},
            {"se","sami septentrional"},
            {"sg","sango"},
            {"si","cingalés"},
            {"sk","eslovaco"},
            {"sl","esloveno"},
            {"sm","samoano"},
            {"sn","shona"},
            {"so","somalí"},
            {"sq","albanés"},
            {"sr","serbio"},
            {"ss","suazi(o swati, o siSwati)"},
            {"st","sesotho"},
            {"su","sundanés(o sondanés)"},
            {"sv","sueco"},
            {"sw","suajili"},
            {"ta","tamil"},
            {"te","télugu"},
            {"tg","tayiko"},
            {"th","tailandés"},
            {"ti","tigriña"},
            {"tk","turcomano"},
            {"tl","tagalo"},
            {"tn","setsuana"},
            {"to","tongano"},
            {"tr","turco"},
            {"ts","tsonga"},
            {"tt","tártaro"},
            {"tw","twi"},
            {"ty","tahitiano"},
            {"ug","uigur"},
            {"uk","ucraniano"},
            {"ur","urdu"},
            {"uz","uzbeko"},
            {"ve","venda"},
            {"vi","vietnamita"},
            {"vo","volapük"},
            {"wa","valón"},
            {"wo","wolof"},
            {"xh","xhosa"},
            {"yi","yídish(o yidis, o yiddish)"},
            {"yo","yoruba"},
            {"za","chuan(o chuang, o zhuang)"},
            {"zh","chino"},
            {"zu","zulú"}
            };

        private List<string> keys;
        private string[] values;

        // Draw the property inside the given rect
        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var langFieldSize = 80;
            var buttonSize = 20;
            var strings = property.FindPropertyRelative("string") ?? property.FindPropertyRelative("langstring");
            var eachHeight = position.height / (strings.arraySize);

            // Using BeginProperty / EndProperty on the parent property means that
            // prefab override logic works on the entire property.
            EditorGUI.BeginProperty(position, label, property);

            // Draw label
            position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

            // Don't make child fields be indented
            var indent = EditorGUI.indentLevel;
            EditorGUI.indentLevel = 0;

            // Calculate rects
            var textField = new Rect(position.x, position.y, position.width - langFieldSize - buttonSize, eachHeight);
            var langField = new Rect(textField.x + textField.width, position.y, langFieldSize, eachHeight);
            var button = new Rect(langField.x + langField.width, position.y, buttonSize, eachHeight);

            for (int i = 0; i < strings.arraySize; i++)
            {
                var line = strings.GetArrayElementAtIndex(i);
                var text = line.FindPropertyRelative("value") ?? line.FindPropertyRelative("Value");
                var lang = line.FindPropertyRelative("language") ?? line.FindPropertyRelative("lang");
                EditorGUI.PropertyField(textField, text, GUIContent.none);
                var selectedLang = lang.stringValue;
                var newSelected = EditorGUI.Popup(langField, Mathf.Max(0, keys.IndexOf(selectedLang)), values);
                lang.stringValue = keys[newSelected];

                if (GUI.Button(button, i == 0 ? "+" : "-"))
                {
                    if(property.GetPropertyFieldType() == typeof(LangString))
                    {

                        if (i == 0)
                        {
                            var langString = property.GetPropertyObject<LangString>();
                            var l = langString.String.ToList();
                            l.Add(new LangString.LanguageString());
                            langString.String = l.ToArray();
                        }
                        else
                        {
                            var langString = property.GetPropertyObject<LangString>();
                            var l = langString.String.ToList();
                            l.RemoveAt(i);
                            langString.String = l.ToArray();
                            i--;
                        }
                    } 
                    else if (property.GetPropertyFieldType() == typeof(LangStringType))
                    {

                        if (property.GetPropertyFieldType() == typeof(LangStringType))
                        {

                            if (i == 0)
                            {
                                var langString = property.GetPropertyObject<LangStringType>();
                                var l = langString.@string.ToList();
                                l.Add(new LanguageStringType());
                                langString.@string = l.ToArray();
                            }
                            else
                            {
                                var langString = property.GetPropertyObject<LangStringType>();
                                var l = langString.@string.ToList();
                                l.RemoveAt(i);
                                langString.@string = l.ToArray();
                                i--;
                            }
                        }
                    }
                    else if (property.GetPropertyFieldType() == typeof(langType))
                    {

                        if (property.GetPropertyFieldType() == typeof(langType))
                        {

                            if (i == 0)
                            {
                                var langString = property.GetPropertyObject<langType>();
                                var l = langString.langstring.ToList();
                                l.Add(new langstringType());
                                langString.langstring = l.ToArray();
                            }
                            else
                            {
                                var langString = property.GetPropertyObject<langType>();
                                var l = langString.langstring.ToList();
                                l.RemoveAt(i);
                                langString.langstring = l.ToArray();
                                i--;
                            }
                        }
                    }
                    property.serializedObject.Update();
                }

                langField.y += eachHeight;
                textField.y += eachHeight;
                button.y += eachHeight;
            }

            // Set indent back to what it was
            EditorGUI.indentLevel = indent;

            EditorGUI.EndProperty();
        }

        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if (keys == null)
            {
                keys = languages.Keys.ToList();
                values = languages.Values.ToArray();
            }

            var height = base.GetPropertyHeight(property, label);
            var size = 1;

            if (property.GetPropertyFieldType() == typeof(LangString))
            {
                var langString = property.GetPropertyObject<LangString>();

                if (langString.String == null || langString.String.Length == 0)
                {
                    langString.String = new LangString.LanguageString[1] { new LangString.LanguageString { Language = "en" } };
                    property.serializedObject.Update();
                }
                size = langString.String.Length;
            }
            else if (property.GetPropertyFieldType() == typeof(LangStringType))
            {
                var langString = property.GetPropertyObject<LangStringType>();

                if (langString.@string == null || langString.@string.Length == 0)
                {
                    langString.@string = new LanguageStringType[1] { new LanguageStringType { language = "en" } };
                    property.serializedObject.Update();
                }
                size = langString.@string.Length;
            }
            else if (property.GetPropertyFieldType() == typeof(langType))
            {
                var langString = property.GetPropertyObject<langType>();

                if (langString.langstring == null || langString.langstring.Length == 0)
                {
                    langString.langstring = new langstringType[1] { new langstringType { lang = "en" } };
                    property.serializedObject.Update();
                }
                size = langString.langstring.Length;
            }

            return height * Mathf.Max(1, size);
        }

    }
}

