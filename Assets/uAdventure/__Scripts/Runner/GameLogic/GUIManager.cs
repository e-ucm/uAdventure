using UnityEngine;
using uAdventure.Core;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using AssetPackage;

namespace uAdventure.Runner
{
    public class GUIManager : MonoBehaviour
    {
        public GameObject Bubble_Prefab, Think_Prefab, Yell_Prefab, Config_Menu_Ref;

        private static GUIManager instance;
        private GameObject bubble;
        private bool get_talker = false;
        private string talkerToFind, lastText;
        private GUIProvider guiprovider;
        private bool locked = false;
        private string current_cursor = "";

        public static GUIManager Instance
        {
            get { return instance; }
        }

        public string Last
        {
            get { return lastText; }
        }

        public GUIProvider Provider
        {
            get { return guiprovider; }
        }

        protected void Awake()
        {
            instance = this;
        }

        protected void Start()
        {
            guiprovider = new GUIProvider(Game.Instance.GameState.Data);
        }

        protected void Update()
        {
            if (get_talker && GameObject.Find(talkerToFind) != null)
            {
                get_talker = false;
                Talk(lastText, talkerToFind);
            }
        }

        public void SetCursor(string cursor)
        {
            if (cursor != current_cursor)
            {
                if (!locked)
                {
                    var cursorToSet = guiprovider.getCursor(cursor);
                    if(cursor != null)
                    {
                        Cursor.SetCursor(cursorToSet, new Vector2(0f, 0f), CursorMode.Auto);
                    }
                    else
                    {
                        Debug.Log("Could not set cursor with name: " + cursor);
                    }
                }
                current_cursor = cursor;
            }
        }

        public void ShowHand(bool show)
        {
            SetCursor(show ? "over" : "default");
        }

        public void Talk(string text, int x, int y, Color textColor, Color textBorderColor)
        {
            lastText = text;
            ShowBubble(GenerateBubble(text, x, y, textColor, textBorderColor));
        }

        public void Talk(string text, int x, int y, Color textColor, Color textBorderColor, Color backgroundColor, Color borderColor)
        {
            lastText = text;
            ShowBubble(GenerateBubble(text, x, y, textColor, textBorderColor, backgroundColor, borderColor));
        }

        public void Talk(string text, string talkerName = null)
        {
            lastText = text;
            GameObject talkerObject = null;
            if (talkerName == null || talkerName == Player.IDENTIFIER)
            {
                text = text.Replace("[]", "[" + Player.IDENTIFIER + "]");
                NPC player = Game.Instance.GameState.Player;
                BubbleData bubbleData;

                if (Game.Instance.GameState.IsFirstPerson || PlayerMB.Instance == null)
                {
                    bubbleData = GenerateBubble(player, text);
                }
                else
                {
                    talkerObject = GetTalker(talkerName);
                    if (talkerObject == null)
                    {
                        return;
                    }
                    bubbleData = GenerateBubble(player, text, talkerObject);
                }

                ShowBubble(bubbleData);
            }
            else
            {
                talkerObject = GetTalker(talkerName);
                if (talkerObject == null)
                {
                    return;
                }

                NPC cha = Game.Instance.GameState.GetCharacter(talkerName);
                ShowBubble(GenerateBubble(cha, text, talkerObject));
            }
            if (talkerObject)
            {
                var bubbleTalker = talkerObject.GetComponent<Representable>();
                if (bubbleTalker)
                {
                    bubbleTalker.Play("speak");
                }
            }
        }

        public void ShowBubble(BubbleData data)
        {
            data.origin = SceneVector2GuiVector(data.origin);
            data.destiny = SceneVector2GuiVector(data.destiny);

            if (bubble != null)
            {
                bubble.GetComponent<Bubble>().destroy();
            }
            if (data.Line.Length > 0 && data.Line[0] == '#')
            {
                if (data.Line[1] == 'O')
                {
                    bubble = GameObject.Instantiate(Think_Prefab);
                }
                else if (data.Line[1] == '!')
                {
                    bubble = GameObject.Instantiate(Yell_Prefab);
                }

                data.Line = data.Line.Substring(3, data.Line.Length - 3);
            }
            else
            {
                bubble = GameObject.Instantiate(Bubble_Prefab);
            }

            bubble.GetComponent<Bubble>().Data = data;
            bubble.transform.SetParent(this.transform);
            bubble.transform.localRotation = Quaternion.Euler(0, 0, 0);
            bubble.transform.localPosition = new Vector3(bubble.transform.localPosition.x, bubble.transform.localPosition.y, 0);
        }

        public void DestroyBubbles()
        {
            if (bubble != null)
            {
                var bubbleMB = this.bubble.GetComponent<Bubble>();
                if (bubbleMB.Data.Talker)
                {
                    var talker = bubbleMB.Data.Talker.GetComponent<Representable>();
                    if (talker)
                    {
                        talker.Play("stand");
                    }
                } 
                this.bubble.GetComponent<Bubble>().destroy();
            }
        }

        protected static BubbleData GenerateBubble(string text, int x, int y)
        {
            return GenerateBubble(text, x, y, Color.white, Color.black);
        }

        protected static BubbleData GenerateBubble(string text, int x, int y, Color textColor, Color textOutlineColor)
        {
            return GenerateBubble(text, x, y, false, textColor, textOutlineColor, Color.white, Color.black);
        }

        protected static BubbleData GenerateBubble(string text, int x, int y, Color textColor, Color textOutlineColor, Color baseColor, Color outlineColor)
        {
            return GenerateBubble(text, x, y, true, textColor, textOutlineColor, baseColor, outlineColor);
        }

        protected static BubbleData GenerateBubble(string text, int x, int y, bool showBorder, Color textColor, Color textOutlineColor, Color baseColor, Color outlineColor)
        {
            var destiny = SceneMB.ToWorldSize(new Vector2(x, y));
            var bubbleData = new BubbleData(text, destiny, destiny - new Vector3(0, -15, 0))
            {
                TextColor = textColor,
                TextOutlineColor = textOutlineColor
            };

            if (showBorder)
            {
                bubbleData.BaseColor = baseColor;
                bubbleData.OutlineColor = outlineColor;
            }

            return bubbleData;
        }

        protected static BubbleData GenerateBubble(NPC cha, string text, GameObject talker = null)
        {
            var newBubble = new BubbleData(text, new Vector2(40, 60), new Vector2(40, 45), talker)
            {
                TextColor = cha.getTextFrontColor(),
                TextOutlineColor = cha.getTextBorderColor(),
                BaseColor = cha.getBubbleBkgColor(),
                OutlineColor = cha.getBubbleBorderColor()
            };

            if (talker != null)
            {
                Vector3 position = talker.transform.position;

                newBubble.Origin = position;
                newBubble.Destiny = position + Camera.main.transform.up * talker.transform.lossyScale.y * 0.6f;
            }
            else
            {
                newBubble.Origin = Camera.main.transform.position;
                newBubble.Destiny = Camera.main.transform.position + Camera.main.transform.up * 15;
            }

            return newBubble;
        }

        public void LockCursor()
        {
            locked = true;
        }

        public void ReleaseCursor()
        {
            locked = false;
            var toPut = current_cursor;
            current_cursor = null;
            SetCursor(toPut);
        }

        private Vector2 SceneVector2GuiVector(Vector3 v)
        {
            // Convert it to ViewPort
            v = Camera.main.WorldToViewportPoint(v);

            // Adapt from the viewport to the Canvas size
            var canvasRect = this.GetComponent<RectTransform>();
            v.x *= canvasRect.sizeDelta.x;
            v.y *= canvasRect.sizeDelta.y;

            return v;
        }

        private GameObject GetTalker(string talker)
        {
            GameObject ret = GameObject.Find(talker);

            if (ret == null)
            {
                talkerToFind = talker;
                get_talker = true;
            }

            return ret;
        }

        public void ShowConfigMenu()
        {
            this.Config_Menu_Ref.SetActive(!Config_Menu_Ref.activeSelf);
        }

        public void OnClickLoad()
        {
            Game.Instance.LoadGame();
        }

        public void OnClickSave()
        {
            Game.Instance.SaveGame();
        }

        public void ResetAndExit()
        {
            Game.Instance.Reset();
        }

        public void ExitApplication()
        {
            if (PlayerPrefs.HasKey("LimesurveyToken") && PlayerPrefs.GetString("LimesurveyToken") != "ADMIN" && PlayerPrefs.HasKey("LimesurveyPost"))
            {
                string path = Application.persistentDataPath;

                if (!path.EndsWith("/"))
                {
                    path += "/";
                }

                Dictionary<string, string> headers = new Dictionary<string, string>();

                Net net = new Net(this);

                WWWForm data = new WWWForm();

                TrackerAssetSettings trackersettings = (TrackerAssetSettings) TrackerAsset.Instance.Settings;
                string backupfile = Application.persistentDataPath + System.IO.Path.DirectorySeparatorChar + trackersettings.BackupFile;

                data.AddField("token", PlayerPrefs.GetString("LimesurveyToken"));
                data.AddBinaryData("traces", System.Text.Encoding.UTF8.GetBytes(System.IO.File.ReadAllText(backupfile)));

                //d//ata.headers.Remove ("Content-Type");// = "multipart/form-data";

                net.POST(PlayerPrefs.GetString("LimesurveyHost") + "classes/collector", data, new SavedTracesListener());

                System.IO.File.AppendAllText(path + PlayerPrefs.GetString("LimesurveyToken") + ".csv", System.IO.File.ReadAllText(backupfile));
                PlayerPrefs.SetString("CurrentSurvey", "post");
                SceneManager.LoadScene("_Survey");
            }
            else
                Application.Quit();
        }

        class SavedTracesListener : Net.IRequestListener
        {
            public void Result(string data)
            {
                Debug.Log("------------------------");
                Debug.Log(data);
            }

            public void Error(string error)
            {
                Debug.Log("------------------------");
                Debug.Log(error);
            }
        }
    }
}