using UnityEngine;
using uAdventure.Core;
using System.Collections;
using UnityEngine.SceneManagement;
using System.Linq;
using UniRx;

namespace uAdventure.Runner
{
    public class GUIManager : MonoBehaviour
    {
        public GameObject Bubble_Prefab, Think_Prefab, Yell_Prefab, Config_Menu_Ref;
        public GameObject SaveButton, LoadButton, ResetButton;

        private static GUIManager instance;
        private GameObject bubble;
        private bool get_talker = false;
        private string talkerToFind;
        private ConversationLine line;
        private GUIProvider guiprovider;
        private bool locked = false;
        private string current_cursor = "";
        private bool started = false;


        public static GUIManager Instance
        {
            get { return instance; }
        }

        public ConversationLine Line
        {
            get { return line; }
        }

        public GUIProvider Provider
        {
            get { return guiprovider; }
        }

        protected void Awake()
        {
            if(instance != null)
            {
                DestroyImmediate(this.gameObject);
                return;
            }

            DontDestroyOnLoad(this.gameObject);
            instance = this;
        }

        protected void Start()
        {
            var gs = Game.Instance.GameState;
            if (gs != null)
            {
                started = true;
                guiprovider = new GUIProvider(gs.Data);

                SaveButton.SetActive(gs.Data.isShowSaveLoad());
                LoadButton.SetActive(gs.Data.isShowSaveLoad());
                ResetButton.SetActive(gs.Data.isShowReset());

                if (Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer)
                {
                    this.GetComponent<UnityEngine.UI.CanvasScaler>().referenceResolution = new Vector2(600, 400);
                }

                /*Game.Instance.OnShowText += (finished, line, text, x, y, textColor, textOutlineColor, baseColor, outlineColor, id) =>
                {
                    if (!finished)
                    {
                        if (line != null)
                        {
                            Talk(line);
                        }
                        else if (textColor == Game.NoColor && textOutlineColor == Game.NoColor)
                        {
                            Talk(text, x, y, Color.white, Color.black);
                        }
                        else if(baseColor == Game.NoColor && outlineColor == Game.NoColor)
                        {
                            Talk(text, x, y, textColor, textOutlineColor);
                        }
                        else
                        {
                            Talk(text, x, y, textColor, textOutlineColor, baseColor, outlineColor);
                        }
                    }
                };*/

            }
        }

        protected void Update()
        {
            if (!started)
            {
                Start();
            }

            if (get_talker && GameObject.Find(talkerToFind) != null)
            {
                get_talker = false;
                Talk(line.getText(), talkerToFind);
            }
        }

        public void SetCursor(string cursor)
        {
            if (!started)
            {
                Start();
            }

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

        public InteractuableResult InteractWithDialogue()
        {
            if(bubble != null)
            {
                return bubble.GetComponent<Bubble>().Interacted();
            }
            else
            {
                return InteractuableResult.IGNORES;
            }
        }

        public void Talk(string text, int x, int y, Color textColor, Color textBorderColor)
        {
            line = new ConversationLine("", text);
            ShowBubble(GenerateBubble(text, x, y, textColor, textBorderColor));
        }

        public void Talk(string text, int x, int y, Color textColor, Color textBorderColor, Color backgroundColor, Color borderColor)
        {
            line = new ConversationLine("", text);
            ShowBubble(GenerateBubble(text, x, y, textColor, textBorderColor, backgroundColor, borderColor));
        }
        public void Talk(ConversationLine line)
        {
            var resources = line.getResources().Checked().FirstOrDefault();
            if (resources != null)
            {
                var image = resources.existAsset("image") ? Game.Instance.ResourceManager.getImage(resources.getAssetPath("image")) : null;
                var audio = resources.existAsset("audio") ? Game.Instance.ResourceManager.getAudio(resources.getAssetPath("audio")) : null;
                var tts = resources.existAsset("tts") ? resources.getAssetPath("tts") == "yes" : false;
                Talk(line.getText(), image, audio, tts, line.getName());
            }
            else
            {
                Talk(line.getText(), null, null, false, line.getName());
            }
            this.line = line;
        }

        public void Talk(string text, string talkerName = null)
        {
            Talk(text, null, null, false, talkerName);
        }

        public void Talk(string text, Texture2D image, AudioClip audio, bool synthetizeVoice, string talkerName = null)
        {
            line = new ConversationLine("", text);
            GameObject talkerObject = null;
            BubbleData bubbleData;
            if (talkerName == null || talkerName == Player.IDENTIFIER)
            {
                text = text.Replace("[]", "[" + Player.IDENTIFIER + "]");
                NPC player = Game.Instance.GameState.Player;

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
                bubbleData.Image = image;
                bubbleData.TTS = synthetizeVoice;
                bubbleData.Audio = audio;

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
                bubbleData = GenerateBubble(cha, text, talkerObject);
                bubbleData.Image = image;
                bubbleData.TTS = synthetizeVoice;
                bubbleData.Audio = audio;
                ShowBubble(bubbleData);
            }
            if (talkerObject)
            {
                var talkerRepresentable = talkerObject.GetComponent<Representable>();
                if (talkerRepresentable)
                {
                    talkerRepresentable.Play("speak");
                }
            }
        }

        public void ShowBubble(BubbleData data)
        {
            data.origin = SceneVector2GuiVector(data.origin);
            data.destiny = SceneVector2GuiVector(data.destiny);

            if (bubble != null)
            {
                DestroyBubbles();
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
                this.bubble.GetComponent<Bubble>().Destroy();
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
                newBubble.Talker = talker;
            }
            else
            {
                newBubble.Origin = Camera.main.transform.position;
                newBubble.Destiny = Camera.main.transform.position + Camera.main.transform.up * 15;
                newBubble.Talker = talker;
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
            var nextState = !Config_Menu_Ref.activeSelf;
            Time.timeScale = nextState ? 0 : 1;
            this.Config_Menu_Ref.SetActive(nextState);
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
            StartCoroutine(Game.Instance.Restart());
        }
        public void ClearData()
        {
            Game.Instance.ClearAndRestart();
        }

        public void ExitApplication()
        {
            ShowConfigMenu();
            Game.Instance.Quit();
        }
    }
}