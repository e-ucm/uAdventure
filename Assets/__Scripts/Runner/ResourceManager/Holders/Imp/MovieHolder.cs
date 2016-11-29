using UnityEngine;
using System.Collections;

namespace uAdventure.Runner
{
    public class MovieHolder
    {
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
        public MovieTexture Movie;
#else
    public WebGLMovieTexture Movie;
#endif

        bool loaded = false;
        string path;

        // ##################################################
        // ################## CONSTRUCTORS ##################
        // ##################################################

        public MovieHolder(string path, ResourceManager.LoadingType type)
        {
            loaded = true;
            this.path = path;
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
            switch (type)
            {
                case ResourceManager.LoadingType.RESOURCES_LOAD:
                    LoadFromResources(path);
                    break;
                case ResourceManager.LoadingType.SYSTEM_IO:
                    Game.Instance.StartCoroutine(LoadFromSystem(path));
                    break;
            }
#else
        LoadFromWebGL(path);
#endif
        }

        public bool Loaded()
        {
            return loaded;
        }

        // #####################################################
        // ################## CONTROL METHODS ##################
        // #####################################################

        System.DateTime started_playing;
        public bool isPlaying()
        {
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
            return Movie.isPlaying;
#else
        return true;
#endif
        }

        public void Play()
        {
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
            Movie.Play();
#else
        Debug.Log("playing");
        Movie.Play();
#endif
        }

        public void Stop()
        {
#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
            Movie.Stop();
#else
        Movie.Pause();
        Movie.Seek(0f);
#endif
        }

        // #####################################################
        // ################## LOADING METHODS ##################
        // #####################################################

#if !(UNITY_WEBPLAYER || UNITY_WEBGL)
        private IEnumerator LoadFromSystem(string uri)
        {
            string url_prefix = "file:///";
            string videoname = uri;
            string[] splitted = videoname.Split('.');
            string dir = "", fullname = splitted[0];

            for (int i = 1; i < splitted.Length - 1; i++)
                fullname += "." + splitted[i];

            if (System.IO.File.Exists(Game.Instance.getSelectedGame() + fullname + ".ogv"))
                dir = url_prefix + Game.Instance.getSelectedGame() + fullname + ".ogv";
            else
                dir = url_prefix + Game.Instance.getSelectedGame() + videoname;

            Debug.Log(dir);

            WWW www = new WWW(dir);

            yield return www;
            if (www.error != null)
            {
                Debug.Log("Error: Can't laod movie! - " + www.error);
                yield break;

            }
            else
            {
                MovieTexture video = www.movie as MovieTexture;
                Debug.Log("Movie loaded");
                Debug.Log(www.movie);
                loaded = true;
                Movie = video;
            }
        }

        private MovieTexture LoadFromResources(string uri)
        {
            string videoname = uri;
            string[] splitted = videoname.Split('.');
            string fullname = splitted[0];

            for (int i = 1; i < splitted.Length - 1; i++)
                fullname += "." + splitted[i];

            fullname = Game.Instance.getGameName() + "/" + fullname;
            Movie = Resources.Load(fullname) as MovieTexture;

            if (Movie == null)
            {
                loaded = false;
                Debug.Log("No se pudo cargar: " + this.path);
            }
            else
                loaded = true;

            return Movie;
        }

#else

    private WebGLMovieTexture LoadFromWebGL(string uri)
    {
        string videoname = uri;
        string[] splitted = videoname.Split('/');

        splitted = splitted[splitted.Length-1].Split('.');
        string fullname = splitted[0];

        for (int i = 1; i < splitted.Length - 1; i++)
            fullname += "." + splitted[i];

        fullname = "StreamingAssets/" + fullname + ".ogv";

        Debug.Log(fullname);
        
        Movie = new WebGLMovieTexture(fullname);
        loaded = true;

        return Movie;
    }

#endif

    }
}