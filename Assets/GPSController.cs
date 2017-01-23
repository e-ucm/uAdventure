using UnityEngine;
using System.Collections;

public class GPSController : MonoBehaviour {

    public Texture2D connectedSimbol;
    public Texture2D connectingSimbol;
    public Texture2D disconnectedSimbol;
    public float blinkingTime;
    public float iconWidth, iconHeight;

    private float time;

    void Update()
    {

        time += Time.deltaTime;

        if (time > blinkingTime)
            time = time - Mathf.Floor(time/blinkingTime) * blinkingTime;
    }

    void OnGUI()
    {
        var paintSimbol = disconnectedSimbol;

        switch (Input.location.status) { 
            default:
            case LocationServiceStatus.Failed:
            case LocationServiceStatus.Stopped:
                paintSimbol = (time > blinkingTime / 2f) ? connectedSimbol : connectingSimbol;
                break;
            case LocationServiceStatus.Initializing:
                paintSimbol = (time > blinkingTime / 2f) ? connectedSimbol : connectingSimbol;
                break;
            case LocationServiceStatus.Running:
                paintSimbol = (Input.location.lastData.timestamp == 0) ? ((time > blinkingTime / 2f) ? connectedSimbol : connectingSimbol) : connectedSimbol;
                break;
        }

        if(Event.current.type == EventType.Repaint)
            GUI.DrawTexture(new Rect(Screen.width - iconWidth - 5, 5, iconWidth, iconHeight), paintSimbol);
    }


}
