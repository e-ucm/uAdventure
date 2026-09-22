using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Threading;

using uAdventure.Runner;

namespace uAdventure.GameSelector
{
    public class LoaderMB : MonoBehaviour
    {



        //Slider slider;
        Thread unzipthread;

        // Use this for initialization
        void Start()
        {
            //Debug.Log (LoaderController.Instance.gamePath);
            //slider = this.GetComponent<Slider> ();
            /*unzipthread = new Thread (unZip);
            unzipthread.Start();*/
            this.StartCoroutine(unZip());
        }

        // Update is called once per frame
        void Update()
        {
            //slider.value = ZipUtil.Progress;

            /*if (ResourceManager.Instance.extracted)
            {
                SceneManager.LoadScene("_MenuScene");
            }*/
        }


        IEnumerator unZip()
        {
            //ResourceManager.Instance.extractFile(LoaderController.Instance.gamePath);
            yield return null;
        }
    }
}