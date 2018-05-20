using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Xml;
using UnityEngine.SceneManagement;

using uAdventure.Runner;

namespace uAdventure.GameSelector
{
    public class GameButtonMB : MonoBehaviour
    {

        string path, imagepath, gamename;

        public string Path
        {
            get { return path; }
            set
            {
                string[] tmp = value.Split(System.IO.Path.DirectorySeparatorChar);
                gamename = tmp[tmp.Length - 1];
                path = value + System.IO.Path.DirectorySeparatorChar;
                imagepath = path + System.IO.Path.DirectorySeparatorChar + "gui" + System.IO.Path.DirectorySeparatorChar;
            }
        }

        Image image;
        Text text;
        // Use this for initialization
        void Start()
        {
            Transform panel = this.transform.Find("Panel");
            image = panel.Find("Miniatura").GetComponent<Image>();
            text = panel.Find("Titulo").GetComponent<Text>();

            Texture2D tx;
            if (System.IO.File.Exists(imagepath + "standalone_game_icon.png"))
                tx = Game.Instance.ResourceManager.getImage(imagepath + "standalone_game_icon.png");
            else
                tx = Game.Instance.ResourceManager.getImage(imagepath + "Icono-Motor-128x128.png");

            image.sprite = Sprite.Create(tx, new Rect(0, 0, tx.width, tx.height), new Vector2(0.5f, 0.5f));

            XmlDocument doc = new XmlDocument();
            doc.Load(path + "descriptor.xml");

            text.text = doc.SelectSingleNode("/game-descriptor/title").InnerText;

            Button.ButtonClickedEvent ev = new Button.ButtonClickedEvent();
            ev.AddListener(delegate { startGame(); });

            this.GetComponent<Button>().onClick = ev;
        }

        // Update is called once per frame
        void Update()
        {

        }

        void startGame()
        {
            Game.GameToLoad = gamename;
            SceneManager.LoadScene("_Scene1");
        }
    }
}