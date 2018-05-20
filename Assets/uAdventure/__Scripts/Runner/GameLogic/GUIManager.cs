using UnityEngine;
using System.Collections;
using uAdventure.Core;
using System;

namespace uAdventure.Runner
{
    public class GUIManager : MonoBehaviour
    {

        private static GUIManager instance;
        public GameObject Bubble_Prefab, Think_Prefab, Yell_Prefab, Config_Menu_Ref;
        GameObject bubble;
        private bool get_talker = false;
        private string talkerToFind, lastText;
        private GUIProvider guiprovider;
        private AdventureData data;
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

        void Awake()
        {
            instance = this;
        }

        void Start()
        {
            guiprovider = new GUIProvider(Game.Instance.GameState.Data);
        }

        void Update()
        {
            if (get_talker)
            {
                if (GameObject.Find(talkerToFind) != null)
                {
                    get_talker = false;
                    Talk(lastText, talkerToFind);
                }
            }
        }

        public void setCursor(string cursor)
        {
            if (cursor != current_cursor)
            {
                if (!locked)
                {
                    Cursor.SetCursor(guiprovider.getCursor(cursor), new Vector2(0f, 0f), CursorMode.Auto);
                }
                current_cursor = cursor;
            }
        }

        public void showHand(bool show)
        {
            if (show)
                setCursor("over");
            else
                setCursor("default");
        }

        public void Talk(string text, string talkerName = null)
        {
            lastText = text;
            GameObject talkerObject = null;
            if (talkerName == null || talkerName == Player.IDENTIFIER)
            {
                text = text.Replace("[]", "[" + Player.IDENTIFIER + "]");
                NPC player = Game.Instance.GameState.Player;
                BubbleData bubble;

                if (Game.Instance.GameState.IsFirstPerson)
                {
                    bubble = generateBubble(player, text);
                }
                else
                {
                    talkerObject = getTalker(talkerName);
                    if (talkerObject == null)
                        return;
                    bubble = generateBubble(player, text, talkerObject);
                }

                GUIManager.Instance.ShowBubble(bubble);
            }
            else
            {
                talkerObject = getTalker(talkerName);
                if (talkerObject == null)
                    return;

                NPC cha = Game.Instance.GameState.getCharacter(talkerName);
                BubbleData bubble = generateBubble(cha, text, talkerObject);
                GUIManager.Instance.ShowBubble(bubble);
            }
            if (talkerObject)
            {
                var bubbleTalker = talkerObject.GetComponent<CharacterMB>();
                if (bubbleTalker)
                    bubbleTalker.Play("speak");
            }
        }

        public void ShowBubble(BubbleData data)
        {
            data.origin = sceneVector2guiVector(data.origin);
            data.destiny = sceneVector2guiVector(data.destiny);

            //correctBoundaries (data);

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
                bubble = GameObject.Instantiate(Bubble_Prefab);

            bubble.GetComponent<Bubble>().Data = data;
            bubble.transform.SetParent(this.transform);
            bubble.transform.localRotation = Quaternion.Euler(0, 0, 0);
            bubble.transform.localPosition = new Vector3(bubble.transform.localPosition.x, bubble.transform.localPosition.y, 0);
        }

        public void destroyBubbles()
        {
            if (bubble != null)
            {
                var bubbleMB = this.bubble.GetComponent<Bubble>();
                if (bubbleMB.Data.Talker)
                {
                    var characterMB = bubbleMB.Data.Talker.GetComponent<CharacterMB>();
                    if (characterMB)
                        characterMB.Play("stand");
                } 
                this.bubble.GetComponent<Bubble>().destroy();
            }
        }

        public BubbleData generateBubble(NPC cha, string text, GameObject talker = null)
        {
            BubbleData bubble = new BubbleData(text, new Vector2(40, 60), new Vector2(40, 45), talker);

            bubble.TextColor = cha.getTextFrontColor();
            bubble.TextOutlineColor = cha.getTextBorderColor();
            bubble.BaseColor = cha.getBubbleBkgColor();
            bubble.OutlineColor = cha.getBubbleBorderColor();

            if (talker != null)
            {
                Vector2 position = talker.transform.position;

                bubble.Origin = position;
                bubble.Destiny = position + new Vector2(0, talker.transform.lossyScale.y * 0.6f);
            }
            else
            {
                bubble.Origin = Camera.main.transform.position;
                bubble.Destiny = Camera.main.transform.position + new Vector3(0, 15, 0);
            }

            return bubble;
        }

        public void lockCursor()
        {
            locked = true;
        }

        public void releaseCursor()
        {
            locked = false;
            var toPut = current_cursor;
            current_cursor = null;
            setCursor(toPut);
        }

        private Vector2 sceneVector2guiVector(Vector2 v)
        {
            // Convert it to ViewPort
            v = Camera.main.WorldToViewportPoint(v);

            // Adapt from the viewport to the Canvas size
            var canvasRect = this.GetComponent<RectTransform>();
            v.x *= canvasRect.sizeDelta.x;
            v.y *= canvasRect.sizeDelta.y;

            return v;
        }

        private void correctBoundaries(BubbleData bubble)
        {
            if (bubble.destiny.x <= 125f) bubble.destiny.x = 125f;
            else if (bubble.destiny.x >= (800f - 125f)) bubble.destiny.x = (800f - 125f);
        }

        private GameObject getTalker(string talker)
        {
            GameObject ret = GameObject.Find(talker);

            if (ret == null)
            {
                talkerToFind = talker;
                get_talker = true;
            }

            return ret;
        }

        public void showConfigMenu()
        {
            this.Config_Menu_Ref.SetActive(!Config_Menu_Ref.activeSelf);
        }

        public void resetAndExit()
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Application.Quit();
        }

        public void exitApplication()
        {
            Application.Quit();
        }
    }
}