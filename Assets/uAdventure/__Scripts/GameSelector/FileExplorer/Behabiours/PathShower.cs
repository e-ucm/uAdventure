using UnityEngine;
using System.Collections;
using System.IO;
using UnityEngine.UI;
using System.Collections.Generic;

using uAdventure.Runner;

namespace uAdventure.GameSelector
{
    public class PathShower : MonoBehaviour
    {

        public Sprite folderimage, gameimage;

        public GameObject itemPrefab;
        private string path;
        public string Path
        {
            get { return path; }
            set
            {
                path = value + System.IO.Path.DirectorySeparatorChar;
                forceupdate = true;
            }
        }

        string[] folders;
        string[] files;
        bool forceupdate = false;

        public Color foldercolor, gamecolor, selectedgamecolor;

        List<GameObject> games;
        List<string> gamepaths;
        GameObject selected;
        Button addbutton;
        // Use this for initialization
        void Start()
        {
            // TODO 
            //Path = ResourceManager.Instance.getStoragePath();

            if (folderimage == null)
                folderimage = Resources.Load("GUI/folder") as Sprite;

            if (gameimage == null)
                gameimage = Resources.Load("GUI/gamepad") as Sprite;

            addbutton = GameObject.Find("AddGame").GetComponent<Button>();

            games = new List<GameObject>();
            gamepaths = new List<string>();
        }

        public void levelUp()
        {
            string[] splitted = path.Split(System.IO.Path.DirectorySeparatorChar);
            string tmp = splitted[0];
            for (int i = 1; i < splitted.Length - 2; i++)
                tmp += System.IO.Path.DirectorySeparatorChar + splitted[i];
            Path = tmp;
        }

        // Update is called once per frame
        void Update()
        {
            if (forceupdate)
            {
                forceupdate = false;
                this.clear();

                folders = Directory.GetDirectories(path);
                files = Directory.GetFiles(path);


                foreach (string folder in folders)
                    addFolder(folder);

                foreach (string game in files)
                    if (game.Contains(".jar"))
                        addGame(game);
            }
        }

        void clear()
        {
            foreach (Transform t in this.transform)
            {
                GameObject.Destroy(t.gameObject);
            }

            addbutton.interactable = false;
            selected = null;
            games = new List<GameObject>();
            gamepaths = new List<string>();
        }

        public void selectGame(int i)
        {
            selected = games[i];

            foreach (GameObject g in games)
            {
                g.GetComponent<Image>().color = gamecolor;
            }

            selected.GetComponent<Image>().color = selectedgamecolor;
            LoaderController.Instance.gamePath = gamepaths[i];
            addbutton.interactable = true;
        }

        Button.ButtonClickedEvent ev;
        void addFolder(string path)
        {
            string[] splitted = path.Split(System.IO.Path.DirectorySeparatorChar);
            Transform folder = addItem(splitted[splitted.Length - 1], folderimage, foldercolor);

            ev = new Button.ButtonClickedEvent();
            ev.AddListener(delegate { Path = path; });

            folder.GetComponent<Button>().onClick = ev;
        }

        void addGame(string path)
        {
            string[] splitted = path.Split(System.IO.Path.DirectorySeparatorChar);
            Transform game = addItem(splitted[splitted.Length - 1], gameimage, gamecolor);

            ev = new Button.ButtonClickedEvent();
            int count = games.Count;
            ev.AddListener(delegate { this.selectGame(count); });

            game.GetComponent<Button>().onClick = ev;

            games.Add(game.gameObject);
            gamepaths.Add(path);
        }

        Transform tmp, panel;
        Transform addItem(string name, Sprite image, Color color)
        {
            tmp = GameObject.Instantiate(itemPrefab).transform;
            tmp.parent = this.transform;
            tmp.localScale = new Vector3(1, 1, 1);

            tmp.GetComponent<Image>().color = color;

            panel = tmp.transform.Find("Panel");
            panel.Find("Miniatura").GetComponent<Image>().sprite = image;
            panel.Find("Titulo").GetComponent<Text>().text = name;

            return tmp;
        }
    }
}