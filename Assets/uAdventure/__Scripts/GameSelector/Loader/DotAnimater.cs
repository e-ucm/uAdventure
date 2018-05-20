using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace uAdventure.GameSelector
{
    public class DotAnimater : MonoBehaviour
    {
        string[] strings = { "...", " ..", ". .", ".. " };

        Text dots;

        void Start()
        {
            dots = this.GetComponent<Text>();
        }

        float time_since_last_change = 0;
        int pos = 0;
        void Update()
        {

            time_since_last_change += Time.deltaTime;

            if (time_since_last_change > 0.5f)
            {
                pos = (pos + 1) % 4;
                time_since_last_change = 0;
                dots.text = strings[pos];
            }
        }
    }
}