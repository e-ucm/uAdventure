using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Xml;

public class eFrame {
	private Texture2D image;
	public Texture2D Image {
		get { return image; }
		set { image = value; }
	}

	private int duration = 500;
	public int Duration {
		get { return duration; }
		set { duration = value; }
	}
}

public class eAnim : Resource {
	public List<eFrame> frames;
	public XmlDocument xmld;
	private ResourceManager.LoadingType type;
	string path;

	public eAnim(string path, ResourceManager.LoadingType type){
		frames = new List<eFrame> ();
        this.type = type;

		Regex pattern = new Regex("[óñ]");
		this.path = pattern.Replace(path, "+¦");

		string[] splitted = path.Split ('.');
		if (splitted [splitted.Length - 1] == "eaa") {
			string eaaText = "";

			switch (ResourceManager.Instance.getLoadingType ()) {
			case ResourceManager.LoadingType.SYSTEM_IO:
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
				eaaText = System.IO.File.ReadAllText (path);
#endif
				break;
			case ResourceManager.LoadingType.RESOURCES_LOAD:
                if (path.Contains(".eaa")) ;
                    path = path.Substring(0, path.Length - 4);

				TextAsset ta = Resources.Load (path) as TextAsset;
				if(ta!=null)
					eaaText = ta.text;
				break;
			}
			parseEea (eaaText);
		} else
			createOldMethod ();


	}

	private void parseEea(string eaaText){
		xmld = new XmlDocument ();

		xmld.LoadXml (eaaText);

		eFrame tmp;
		XmlNode animation = xmld.SelectSingleNode ("/animation");
		foreach (XmlElement node in animation.ChildNodes) {
			if (node.Name == "frame") {
				tmp = new eFrame ();
				tmp.Duration = int.Parse (node.GetAttribute ("time"));
			
				string ruta = node.GetAttribute ("uri");

				tmp.Image = ResourceManager.Instance.getImage(ruta);

				frames.Add (tmp);
			} else if (node.Name == "transition") {
				if(frames.Count>0)
					frames [frames.Count - 1].Duration += int.Parse(node.GetAttribute ("time"));
			}
		}
	}


	private static string[] extensions = {".png",".jpg",".jpeg"};
	private void createOldMethod(){
		xmld = new XmlDocument ();
		Texture2DHolder auxHolder;
		eFrame tmp;
		int num = 1;
		string ruta = "";

		switch (type) {
		case ResourceManager.LoadingType.RESOURCES_LOAD:
			ruta = path + "_" + intToStr (num);
			auxHolder = new Texture2DHolder (ruta,this.type);

			while(auxHolder.Loaded()){
				tmp = new eFrame ();
				tmp.Duration = 500;
				tmp.Image = auxHolder.Texture;
				frames.Add(tmp);

				num++;
				ruta = path + "_" + intToStr (num);
				auxHolder = new Texture2DHolder(ruta, this.type);
			}
			break;
		case ResourceManager.LoadingType.SYSTEM_IO:
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
			ruta = path + "_" + intToStr (num);
			string working_extension = "";
			
			foreach (string extension in extensions) {
				auxHolder = new Texture2DHolder (ruta, this.type);
				if (System.IO.File.Exists (ruta + extension)) {
					working_extension = extension;
					break;
				}
			}

			ruta = ruta + working_extension;
			while (System.IO.File.Exists (ruta)) {
				tmp = new eFrame ();
				tmp.Duration = 500;
				tmp.Image = new Texture2DHolder (ruta, this.type).Texture;
				frames.Add (tmp);
				num++;
				ruta = path + "_" + intToStr (num) + working_extension;
			}
#endif
                break;
		}

	}

	private static string intToStr(int number){
		if (number < 10)
			return "0" + number;
		else
			return number.ToString ();
	}

	public bool Loaded(){
		return this.frames.Count > 0;
	}
}
