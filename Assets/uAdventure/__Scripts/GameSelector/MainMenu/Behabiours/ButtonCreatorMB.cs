using UnityEngine;
using System.Collections;


using uAdventure.Runner;

namespace uAdventure.GameSelector
{
    public class ButtonCreatorMB : MonoBehaviour
    {

        public GameObject buttonPrefab;

        string[] directories;

        protected void Start()
        {
            directories = System.IO.Directory.GetDirectories(Game.Instance.ResourceManager.getCurrentDirectory()
                + System.IO.Path.DirectorySeparatorChar + "Games" + System.IO.Path.DirectorySeparatorChar);
        }

        string[] rendered_directories;
        protected void Update()
        {
            if (rendered_directories != directories)
            {
                rendered_directories = directories;

                GameObject tmp;
                foreach (string game in directories)
                {
                    tmp = GameObject.Instantiate(buttonPrefab);
                    tmp.transform.parent = this.transform;
                    tmp.transform.localScale = new Vector3(1, 1, 1);
                    tmp.GetComponent<GameButtonMB>().Path = game;
                }
            }
        }
    }
}