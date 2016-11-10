using UnityEngine;
using System.Collections;

public class MainWindowWrapper {

    public void showErrorDialog(string title, string message, string content = "")
    {
        Debug.LogError(title + "\n\n" + message + "\n" + content);
    }
}
