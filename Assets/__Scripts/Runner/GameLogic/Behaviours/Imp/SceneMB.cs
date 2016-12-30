using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using uAdventure.RageTracker;
using System;

namespace uAdventure.Runner
{
    public class SceneMB : MonoBehaviour, IRunnerChapterTarget
    {

        public GameObject Exit_Prefab, ActiveArea_Prefab, Character_Prefab, Object_Prefab, Atrezzo_Prefab, Player_Prefab;
        private Transform Exits, ActiveAreas, Characters, Objects, Atrezzos;
        private bool interactuable = false;

        private GeneralScene sd;
        public GeneralScene sceneData
        {
            get { return sd; }
            set { sd = value; }
        }

        public object Data
        {
            get { return sceneData; }
            set { sceneData = (GeneralScene)value; }
        }

        private ElementReference player_context;
        private TrajectoryHandler trajectory;
        private Dictionary<ElementReference, ElementReference> contexts = new Dictionary<ElementReference, ElementReference>();

        // Use this for initialization
        void Start()
        {
            this.gameObject.name = sd.getId();
            RenderScene();
        }

        bool fading = false;
        float total_time = 0f, current_time = 0f;

        void FixedUpdate()
        {
            if (fading)
            {
                current_time -= Time.deltaTime;
                float alpha = current_time / total_time;
                colorChilds(new Color(1, 1, 1, alpha));
                if (alpha <= 0)
                {
                    GameObject.Destroy(this.gameObject);
                }
            }

            if (movieplayer == MovieState.LOADING)
            {
                if (movie.Loaded())
                {
                    setMovie();
                    Debug.Log("Seted movie, ready to play.");
                    movie.Play();
                    interactuable = true;
                    movieplayer = MovieState.PLAYING;
                }
            }
            else if (movieplayer == MovieState.PLAYING && !movie.isPlaying())
            {
                movie.Stop();
                Interacted();
            }
#if UNITY_WEBGL
        if(movie!=null)
            movie.Movie.Update();
#endif
        }

        public void Destroy(float time = 0)
        {
            if (time != 0)
            {
                total_time = time;
                current_time = time;
                fading = true;
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 10);
            }
            else
                GameObject.Destroy(this.gameObject);
        }

        private void loadParents()
        {
            this.Exits = this.transform.FindChild("Exits");
            this.ActiveAreas = this.transform.FindChild("ActiveAreas");
            this.Characters = this.transform.FindChild("Characters");
            this.Objects = this.transform.FindChild("Objects");
            this.Atrezzos = this.transform.FindChild("Atrezzos");
        }

        private eAnim slides;
        private int current_slide;
        //private SlidesceneResource current_resource;

        public void RenderScene()
        {
            switch (sd.getType())
            {
                case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
                    movie = ResourceManager.Instance.getVideo(((Videoscene)sd).getResources()[0].getAssetPath(Videoscene.RESOURCE_TYPE_VIDEO));
                    movieplayer = MovieState.LOADING;
                    this.transform.FindChild("Background").localPosition = new Vector3(40, 30, 20);
                    break;
                case GeneralScene.GeneralSceneSceneType.SCENE:

                    loadParents();

                    Scene rsd = (Scene)sd;
                    foreach (ResourcesUni sr in rsd.getResources())
                    {
                        if (ConditionChecker.check(sr.getConditions()))
                        {
                            Texture2D tmp = ResourceManager.Instance.getImage(sr.getAssetPath(Scene.RESOURCE_TYPE_BACKGROUND));

                            Transform background = this.transform.FindChild("Background");
                            background.GetComponent<Renderer>().material.mainTexture = tmp;
                            float scale = (tmp.width / (tmp.height / 600f)) / 800f;

                            this.transform.position = new Vector3(40, 30, 20);
                            background.localPosition = new Vector3(((80 * scale) - 80) / 2f, 0, 20);
                            background.transform.localScale = new Vector3(scale * 80, 60, 1);
                            break;
                        }
                    }

                    //###################### CHARACTERS ######################
                    deleteChilds(Characters);
                    foreach (ElementReference context in rsd.getCharacterReferences())
                        instanceElement<NPC>(context);

                    //###################### OBJECTS ######################
                    deleteChilds(Objects);
                    foreach (ElementReference context in rsd.getItemReferences())
                        instanceElement<Item>(context);

                    //###################### ATREZZOS ######################
                    deleteChilds(Atrezzos);
                    foreach (ElementReference context in rsd.getAtrezzoReferences())
                        instanceElement<Atrezzo>(context);

                    //###################### ACTIVEAREAS ######################
                    deleteChilds(ActiveAreas);

                    foreach (ActiveArea ad in rsd.getActiveAreas())
                        if (ConditionChecker.check(ad.getConditions()))
                            instanceRectangle<ActiveArea>(ad);

                    //###################### EXITS ######################
                    deleteChilds(Exits);

                    foreach (Exit exit in rsd.getExits())
                        if (exit.isHasNotEffects() || ConditionChecker.check(exit.getConditions()))
                            instanceRectangle<Exit>(exit);


                    if (!Game.Instance.GameState.isFirstPerson())
                    {
                        trajectory = new TrajectoryHandler(rsd.getTrajectory());
                        if (player_context == null)
                        {
                            //Vector2 pos = LineHandler.nodeToVector2 (lines [lines.Count-1].end);
                            Trajectory.Node pos = trajectory.getLastNode();
                            player_context = new ElementReference("Player", pos.getX(), pos.getY(), rsd.getPlayerLayer());
                            player_context.setScale(pos.getScale());
                        }
                        /*GameObject.Destroy(this.transform.FindChild ("Player"));*/

                        var player = GameObject.Instantiate(Player_Prefab).GetComponent<PlayerMB>();
                        player.transform.parent = Characters;
                        player.Element = Game.Instance.GameState.getPlayer();
                        player.Context = player_context;
                    }

                    break;
                case GeneralScene.GeneralSceneSceneType.SLIDESCENE:
                    Slidescene ssd = (Slidescene)sd;
                    foreach (ResourcesUni r in ssd.getResources())
                    {
                        if (ConditionChecker.check(r.getConditions()))
                        {
                            this.slides = ResourceManager.Instance.getAnimation(r.getAssetPath(Slidescene.RESOURCE_TYPE_SLIDES));
                            this.transform.FindChild("Background").GetComponent<Renderer>().material.mainTexture = this.slides.frames[0].Image;
                            this.transform.position = new Vector3(40, 30, 20);
                            break;
                        }
                    }
                    break;
            }
        }

        private void deleteChilds(Transform parent)
        {
            if (parent != null)
                foreach (Transform child in parent)
                {
                    GameObject.Destroy(child.gameObject);
                }
        }

        private void instanceElement<T>(ElementReference context) where T : Element
        {
            if (!ConditionChecker.check(context.getConditions()))
                return;

            if (!contexts.ContainsKey(context))
            {
                ElementReference new_context = new ElementReference(context.getTargetId(), context.getX(), context.getY());
                new_context.setScale(context.getScale());
                contexts.Add(context, new_context);
            }

            context = contexts[context];

            GameObject base_prefab;
            Transform parent;
            Element element;
            switch (typeof(T).ToString())
            {
                case "Atrezzo":
                    base_prefab = Atrezzo_Prefab;
                    parent = Atrezzos;
                    element = Game.Instance.GameState.getAtrezzo(context.getTargetId());
                    break;
                case "NPC":
                    base_prefab = Character_Prefab;
                    parent = Characters;
                    element = Game.Instance.GameState.getCharacter(context.getTargetId());
                    break;
                case "Item":
                    base_prefab = Object_Prefab;
                    parent = Objects;
                    element = Game.Instance.GameState.getObject(context.getTargetId());
                    break;
                default:
                    return;
            }

            GameObject ret = GameObject.Instantiate(base_prefab);
            Transform trans = ret.GetComponent<Transform>();
            ret.GetComponent<Representable>().Context = context;
            ret.GetComponent<Representable>().Element = element;
            trans.SetParent(parent);
        }

        private void instanceRectangle<T>(Rectangle context) where T : Rectangle
        {
            if (context == null)
                return;

            GameObject base_prefab;
            Transform parent;

            if (typeof(T) == typeof(ActiveArea))
            {
                base_prefab = ActiveArea_Prefab;
                parent = ActiveAreas;
            }
            else if(typeof(T) == typeof(Exit))
            {
                base_prefab = Exit_Prefab;
                parent = Exits;
            }else
            {
                return;
            }

            GameObject ret = GameObject.Instantiate(base_prefab);
            Transform trans = ret.GetComponent<Transform>();
            if (parent == ActiveAreas)
                ret.GetComponent<ActiveAreaMB>().aaData = (ActiveArea)context;
            else if (parent == Exits)
                ret.GetComponent<ExitMB>().exitData = (Exit)context;

            trans.localScale = new Vector3(context.getWidth() / 10f, context.getHeight() / 10f, 1);
            Vector2 tmppos = new Vector2(context.getX(), context.getY()) / 10 + (new Vector2(trans.localScale.x, trans.localScale.y)) / 2;

            trans.localPosition = new Vector2(tmppos.x, 60 - tmppos.y);
            trans.SetParent(ActiveAreas);
        }

        public bool canBeInteracted()
        {
            return interactuable;
        }

        public void setInteractuable(bool state)
        {
            this.interactuable = state;
        }

        public InteractuableResult Interacted(RaycastHit hit = default(RaycastHit))
        {
            Effects e;

            InteractuableResult res = InteractuableResult.IGNORES;

            switch (sceneData.getType())
            {
                case GeneralScene.GeneralSceneSceneType.SCENE:
                    if (!Game.Instance.GameState.isFirstPerson())
                    {
                        PlayerMB.Instance.move(trajectory.route(PlayerMB.Instance.getPosition(), trajectory.closestPoint(Camera.main.ScreenToWorldPoint(Input.mousePosition))));
                    }
                    break;
                case GeneralScene.GeneralSceneSceneType.SLIDESCENE:
                    if (current_slide + 1 < this.slides.frames.Count)
                    {
                        current_slide++;
                        this.transform.FindChild("Background").GetComponent<Renderer>().material.mainTexture = this.slides.frames[current_slide].Image;
                        res = InteractuableResult.REQUIRES_MORE_INTERACTION;
                    }
                    else
                    {
                        switch (((Slidescene)sceneData).getNext())
                        {
                            case Slidescene.NEWSCENE:
                                Game.Instance.renderScene(((Slidescene)sceneData).getTargetId());
                                break;
                            case Slidescene.ENDCHAPTER:
                                //Game.Instance(); 
                                break;
                        }

                        e = ((Slidescene)sceneData).getEffects();
                        if (e != null && Game.Instance.Execute(new EffectHolder(e)))
                            res = InteractuableResult.DOES_SOMETHING;

                    }
                    break;
                case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
                    switch (((Videoscene)sceneData).getNext())
                    {
                        case Slidescene.NEWSCENE:
                            Game.Instance.renderScene(((Videoscene)sceneData).getTargetId());
                            break;
                        case Slidescene.ENDCHAPTER:
                            break;
                    }

                    e = ((Videoscene)sceneData).getEffects();
                    if (e != null && Game.Instance.Execute(new EffectHolder(e)))
                        res = InteractuableResult.DOES_SOMETHING;

                    if (movie.isPlaying())
                    {
                        movie.Stop();
                        Tracker.T.accessible.Skipped(sceneData.getId(), AccessibleTracker.Accessible.Cutscene);
                    }

                    break;
            }

            return res;
        }

        private void colorChilds(Color color)
        {
            foreach (Transform t1 in transform)
            {
                if (t1.name != "Exits")
                {
                    if (t1.GetComponent<Renderer>() != null)
                        t1.GetComponent<Renderer>().material.color = color;

                    foreach (Transform t2 in t1)
                    {
                        if (t2.GetComponent<Renderer>() != null)
                            t2.GetComponent<Renderer>().material.color = color;
                    }
                }
            }
        }

        // VIDEOSCENE FUNCTIONS
        private enum MovieState { NOT_MOVIE, LOADING, PLAYING, STOPPED };
        private MovieState movieplayer;

        MovieHolder movie;

        public void setMovie()
        {
            //this.transform.FindChild ("Background").GetComponent<Renderer>().material.mainTexture =;

            this.transform.FindChild("Background").GetComponent<MeshRenderer>().material = new Material(Shader.Find("Diffuse"));
            this.transform.FindChild("Background").GetComponent<MeshRenderer>().material.mainTexture = movie.Movie;
            //sound.clip = movie.audioClip;
        }
    }
}