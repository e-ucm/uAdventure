using UnityEngine;
using System.Collections;

public class LoaderController {

    static LoaderController instance;
    public static LoaderController Instance{
        get {
            if (instance == null)
                instance = new LoaderController ();

			return instance;
        }
    }

    private LoaderController(){
    }

    public string gamePath = "";
}
