using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using RAGE.Analytics;
using RAGE.Analytics.Formats;
using UnityEngine.EventSystems;
using System.Linq;
using System;

namespace uAdventure.Runner
{
    public class SceneMB : MonoBehaviour, IRunnerChapterTarget, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IConfirmWantsDrag
    {
        public static readonly Vector2 ScenePivot = new Vector2(0, 1f);
        public static readonly Vector2 DefaultPivot = Vector2.one / 2f;
        public const float VideoSceneRatio = 4f / 3f;
        public const float PixelsToWorld = 0.1f;
        public const float PixelsSceneHeight = 600f;
        public const float WorldSceneHeight = PixelsSceneHeight * PixelsToWorld;

        public GameObject Exit_Prefab, ActiveArea_Prefab, Character_Prefab, Object_Prefab, Atrezzo_Prefab, Player_Prefab;
        private Transform Exits, ActiveAreas, Characters, Objects, Atrezzos, Background, Foreground;
        private bool interactuable = false;
        private MovieState movieplayer = MovieState.NOT_MOVIE;
        private bool ready = false;

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

        public Vector2 PixelsSize
        {
            get { return WorldSize / PixelsToWorld; }
        }

        public Vector2 WorldSize
        {
            get { return Background.localScale; }
        }

        public float Scale
        {
            get { return PixelsSize.y / PixelsSceneHeight; }
        }

        public TrajectoryHandler Trajectory
        {
            get { return trajectoryHandler; }
        }

        public bool IsReady { get { return ready; } }

        private ElementReference player_context;
        private TrajectoryHandler trajectoryHandler;
        private Dictionary<ElementReference, ElementReference> contexts = new Dictionary<ElementReference, ElementReference>();

        public List<float> sorting_layers = new List<float>();

        // Use this for initialization
        void Start()
        {
            this.gameObject.name = sd.getId();
            RenderScene();
        }

        bool fading = false;
        float total_time = 0f, current_time = 0f;
        float resistance = 5000f;

        private void Update()
        {
            if(!dragging && endDragSpeed != Vector2.zero)
            {
                MoveCamera(endDragSpeed * -Time.deltaTime);
                var lastSpeed = endDragSpeed;
                endDragSpeed -= (endDragSpeed.normalized * resistance) * Time.deltaTime;
                if (lastSpeed.sqrMagnitude < endDragSpeed.sqrMagnitude)
                    endDragSpeed = Vector2.zero;
            }
        }

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
                if (movie.isError())
                {
                    movieplayer = MovieState.NOT_MOVIE;
                    Debug.Log(movie.Error);
                }
                else if (movie.Loaded())
                {
                    Debug.Log("Seted movie, ready to play.");
                    movie.Play();
                    interactuable = true;
                    movieplayer = MovieState.PLAYING;
                }
            }
            else if (movieplayer == MovieState.PLAYING)
            {
                ready = true;
                if (!movie.isPlaying())
                {
                    movie.Stop();
                    Interacted();
                }
            }
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
                GameObject.DestroyImmediate(this.gameObject);
        }

        private void loadParents()
        {
            this.Background = this.transform.Find("Background");
            this.Foreground = this.transform.Find("Foreground");
            this.Exits = this.transform.Find("Exits");
            this.ActiveAreas = this.transform.Find("ActiveAreas");
            this.Characters = this.transform.Find("Characters");
            this.Objects = this.transform.Find("Objects");
            this.Atrezzos = this.transform.Find("Atrezzos");
        }

        private eAnim slides;
        private int current_slide;
        //private SlidesceneResource current_resource;
        

        public Vector3 ToWorldSize(Vector2 size)
        {
            var worldSize = Vector3.one;
            worldSize.x = size.x * PixelsToWorld;
            worldSize.y = size.y * PixelsToWorld;
            return worldSize;
        }

        public Vector3 ToWorldPosition(Vector2 position, Vector2 size, Vector2 pivot, float depth = 0f)
        {
            return ToWorldPosition(position, size, pivot, DefaultPivot, depth);
        }

        public Vector3 ToWorldPosition(Vector2 position, Vector2 size, Vector2 pivot, Vector2 objectPivot, float depth = 0f)
        {
            // WorldPosition = (Position + (Size . (ObjectPivot - Pivot))) * ToWorldRatio; ( "." means component-wise product)
            Vector3 worldPosition = position;
            // Reverse the Y
            worldPosition.y = PixelsSize.y - worldPosition.y;

            if(pivot != objectPivot)
            {
                // Apply the pivot translation
                var traslation = (Vector3)(objectPivot - pivot);
                traslation.Scale(size);
                worldPosition += traslation;
            }

            worldPosition = ToWorldSize(worldPosition);
            worldPosition.z = depth;

            return worldPosition;
        }

        public void RenderScene()
        {
            this.transform.position = new Vector3(0, 0, 0);
            loadParents();
            switch (sd.getType())
            {
                case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
                    InventoryManager.Instance.Show = false;
                    movie = Game.Instance.ResourceManager.getVideo(((Videoscene)sd).getResources()[0].getAssetPath(Videoscene.RESOURCE_TYPE_VIDEO));
                    movieplayer = MovieState.LOADING;
                    SetBackground(movie.Movie);
                    break;
                case GeneralScene.GeneralSceneSceneType.SCENE:
                    InventoryManager.Instance.Show = true;
                    Scene rsd = (Scene)sd;
                    Texture2D backgroundTexture = null;
                    foreach (ResourcesUni sr in rsd.getResources())
                    {
                        if (ConditionChecker.check(sr.getConditions()))
                        {
                            backgroundTexture = Game.Instance.ResourceManager.getImage(sr.getAssetPath(Scene.RESOURCE_TYPE_BACKGROUND));
                            SetBackground(backgroundTexture);

                            if (sr.getAssetPath(Scene.RESOURCE_TYPE_FOREGROUND) != "")
                            {
                                Texture2D foregroundTexture = Game.Instance.ResourceManager.getImage(sr.getAssetPath(Scene.RESOURCE_TYPE_FOREGROUND));

                                Foreground.GetComponent<Renderer>().material.SetTexture("_MainTex", backgroundTexture);
                                Foreground.GetComponent<Renderer>().material.SetTexture("_AlphaTex", foregroundTexture);

                                Foreground.localScale = Background.localScale;
                                var foreGroundPos = Background.localPosition;
                                foreGroundPos.z = 1;
                                Foreground.localPosition = foreGroundPos;
                            }

                            break;
                        }
                    }

                    //###################### CHARACTERS ######################
                    deleteChilds(Characters);
                    foreach (ElementReference context in rsd.getCharacterReferences())
                        if (!Game.Instance.GameState.getRemovedElements().Contains(context.getTargetId()))
                            instanceElement<NPC>(context);

                    //###################### OBJECTS ######################
                    deleteChilds(Objects);
                    foreach (ElementReference context in rsd.getItemReferences())
                        if(!Game.Instance.GameState.getRemovedElements().Contains(context.getTargetId()))
                            instanceElement<Item>(context);

                    //###################### ATREZZOS ######################
                    deleteChilds(Atrezzos);
                    foreach (ElementReference context in rsd.getAtrezzoReferences())
                        if (!Game.Instance.GameState.getRemovedElements().Contains(context.getTargetId()))
                            instanceElement<Atrezzo>(context);

                    //###################### ACTIVEAREAS ######################
                    deleteChilds(ActiveAreas);

                    foreach (ActiveArea ad in rsd.getActiveAreas())
                        if (!Game.Instance.GameState.getRemovedElements().Contains(ad.getId()))
                            if (ConditionChecker.check(ad.getConditions()))
                                instanceRectangle<ActiveArea>(ad);

                    //###################### EXITS ######################
                    deleteChilds(Exits);

                    foreach (Exit exit in rsd.getExits())
                        if (exit.isHasNotEffects() || ConditionChecker.check(exit.getConditions()))
                            instanceRectangle<Exit>(exit);


                    if (!Game.Instance.GameState.IsFirstPerson)
                    {
                        player_context = Game.Instance.GameState.PlayerContext;

                        //###################### BARRIERS ######################
                        var barriers = rsd.getBarriers().FindAll(b => ConditionChecker.check(b.getConditions())).ToArray();

                        var trajectory = rsd.getTrajectory();
                        if (trajectory == null)
                        {
                            barriers = barriers.ToList().ConvertAll(b => {
                                Barrier r = b.Clone() as Barrier;
                                r.setValues(r.getX(), 0, r.getWidth(), backgroundTexture.height);
                                return r;
                            }).ToArray();

                            trajectory = new Trajectory();
                            var width = backgroundTexture ? (int)backgroundTexture.width : Screen.width;
                            trajectory.addNode("leftSide", 0, player_context.getY(), player_context.getScale());
                            trajectory.addNode("rightSide", width, player_context.getY(), player_context.getScale());
                            trajectory.addSide("leftSide", "rightSide", width);
                        }

                        trajectoryHandler = new TrajectoryHandler(TrajectoryHandler.CreateBlockedTrajectory(trajectory, barriers));
                        /*GameObject.Destroy(this.transform.FindChild ("Player"));*/

                        PlayerMB player = GameObject.Instantiate(Player_Prefab).GetComponent<PlayerMB>();
                        player.transform.parent = Characters;
                        player.Element = Game.Instance.GameState.Player;
                        player.Context = player_context;
                        // Force the start
                        player.SendMessage("Start");
                        ready = true;
                    }
                    else
                    {
                        ready = true;
                    }
                    
                    Camera.main.GetComponent<PlayerFollower>().SettleInstant();

                    break;
                case GeneralScene.GeneralSceneSceneType.SLIDESCENE:
                    InventoryManager.Instance.Show = false;
                    Slidescene ssd = (Slidescene)sd;
                    current_slide = 0;
                    foreach (ResourcesUni r in ssd.getResources())
                    {
                        if (ConditionChecker.check(r.getConditions()))
                        {
                            this.slides = Game.Instance.ResourceManager.getAnimation(r.getAssetPath(Slidescene.RESOURCE_TYPE_SLIDES));
                            SetSlide(0);
                            ready = true;
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
                new_context.setLayer(context.getLayer());
                new_context.setInfluenceArea(context.getInfluenceArea());
                new_context.setConditions(context.getConditions());
                contexts.Add(context, new_context);
            }

            context = contexts[context];

            GameObject base_prefab;
            Transform parent;
            Element element;

            if (typeof(T) == typeof(Atrezzo))
            {
                base_prefab = Atrezzo_Prefab;
                parent = Atrezzos;
                element = Game.Instance.GameState.getAtrezzo(context.getTargetId());
            }
            else if (typeof(T) == typeof(NPC))
            {
                base_prefab = Character_Prefab;
                parent = Characters;
                element = Game.Instance.GameState.getCharacter(context.getTargetId());
            }
            else if (typeof(T) == typeof(Item))
            {
                base_prefab = Object_Prefab;
                parent = Objects;
                element = Game.Instance.GameState.getObject(context.getTargetId());
            }
            else
            {
                return;
            }

            GameObject ret = Instantiate(base_prefab, parent);
            ret.GetComponent<Representable>().Context = context;
            ret.GetComponent<Representable>().Element = element;
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

            GameObject ret = GameObject.Instantiate(base_prefab, parent);
            Transform trans = ret.GetComponent<Transform>();
            if (parent == ActiveAreas)
                ret.GetComponent<ActiveAreaMB>().Element = (ActiveArea)context;
            else if (parent == Exits)
                ret.GetComponent<ExitMB>().Element = (Exit)context;

            Vector2 tmpPos = new Vector2(context.getX(), context.getY());
            Vector2 tmpSize = new Vector2(context.getWidth(), context.getHeight());

            // ActiveArea pivot starts from top-left corner (Y = 1, X = 0)
            trans.localPosition = ToWorldPosition(tmpPos, tmpSize, new Vector2(0, 1f), 0);
            trans.localScale = ToWorldSize(tmpSize);
        }

        public bool canBeInteracted()
        {
            return interactuable;
        }

        public void setInteractuable(bool state)
        {
            this.interactuable = state;
        }

        private void SetBackground(Texture texture)
        {
            if (Background)
            {
                var size = new Vector2(texture.width, texture.height);
                Background.GetComponent<Renderer>().material.mainTexture = texture;
                Background.localScale = ToWorldSize(size);
                Background.localPosition = ToWorldPosition(Vector2.zero, size, ScenePivot, 20);
                transform.localScale = (Vector3) (Vector2.one * (PixelsSceneHeight / size.y)) + new Vector3(0, 0, 1);
            }

        }

        private void SetSlide(int i)
        {
            if (slides != null && i < slides.frames.Count)
            {
                this.current_slide = i;
                SetBackground(slides.frames[current_slide].Image);
            }
        }

        private bool ProgressSlide()
        {
            var canProgress = current_slide + 1 < this.slides.frames.Count;

            if (canProgress)
            {
                SetSlide(current_slide + 1);
            }

            return canProgress;
        }

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            InteractuableResult res = InteractuableResult.IGNORES;
            
            switch (sceneData.getType())
            {
                case GeneralScene.GeneralSceneSceneType.SCENE:
                    if (!Game.Instance.GameState.IsFirstPerson)
                    {
                        RaycastHit hitInfo;
                        Background.GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(pointerData.position), out hitInfo, float.MaxValue);
                        var texPos = PixelsSize;
                        var texCoord = hitInfo.textureCoord;
                        texCoord.y = 1 - texCoord.y;
                        texPos.Scale(texCoord);
                        var accesible = TrajectoryHandler.GetAccessibleTrajectory(PlayerMB.Instance.getPosition(), trajectoryHandler);
                        PlayerMB.Instance.Move(accesible.closestPoint(texPos));
                    }
                    break;
                case GeneralScene.GeneralSceneSceneType.SLIDESCENE:
                    if (ProgressSlide()) res = InteractuableResult.REQUIRES_MORE_INTERACTION;
                    else res = FinishCutscene((Cutscene)sceneData);
                    break;
                case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
                    var videoscene = (Videoscene)sceneData;
                    if (movieplayer == MovieState.NOT_MOVIE
                        || movieplayer == MovieState.STOPPED
                        || (movieplayer == MovieState.PLAYING && !movie.isPlaying())
                        || videoscene.isCanSkip())
                    {
                        movie.Stop();
                        movieplayer = MovieState.STOPPED;
                        if (movieplayer == MovieState.PLAYING)
                            Tracker.T.accessible.Skipped(sceneData.getId(), AccessibleTracker.Accessible.Cutscene);
                        res = FinishCutscene(videoscene);
                    }
                    break;
            }

            return res;
        }

        private InteractuableResult FinishCutscene(Cutscene cutscene)
        {
            InteractuableResult res = InteractuableResult.DOES_SOMETHING;
            TriggerSceneEffect triggerScene = null;

            switch ((cutscene).getNext())
            {
                case Slidescene.GOBACK:
                    var previousTarget = Game.Instance.GameState.PreviousChapterTarget;
                    if (previousTarget == null)
                    {
                        var possibleTargets = Game.Instance.GameState.GetObjects<IChapterTarget>();
                        previousTarget = possibleTargets.FirstOrDefault(t => t.getId() != this.sceneData.getId());
                    }
                    if(previousTarget != null)
                    {
                        triggerScene = new TriggerSceneEffect(previousTarget.getId(), int.MinValue, int.MinValue);
                    }
                    break;
                case Slidescene.NEWSCENE:
                    triggerScene = new TriggerSceneEffect(cutscene.getTargetId(), int.MinValue, int.MinValue);
                    break;
                case Slidescene.ENDCHAPTER:
                    break;
            }

            Effects e = new Effects()
            {
                triggerScene
            };
            var cutsceneEffects = ((Cutscene)sceneData).getEffects();
            if (cutsceneEffects != null)
            {
                e.AddRange(cutsceneEffects);
            }

            Game.Instance.Execute(new EffectHolder(e));

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

        MovieHolder movie;
        

        public void OnPointerClick(PointerEventData eventData)
        {
            MenuMB.Instance.hide();
            Interacted(eventData);
        }

        bool dragging = false;
        Vector2 endDragSpeed = Vector2.zero;
        public void OnBeginDrag(PointerEventData eventData)
        {
            if(PlayerMB.Instance == null)
            {
                eventData.Use();
                dragging = true;
            }
        }

        

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                dragging = false;
                eventData.Use();
                endDragSpeed = eventData.delta / Time.deltaTime;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragging)
            {
                eventData.Use();
                MoveCamera(-eventData.delta);
            }
        }

        private void MoveCamera(Vector2 delta)
        {
            var cameraPos = Camera.main.transform.position;
            var screenCameraPosition = Camera.main.WorldToScreenPoint(cameraPos);
            screenCameraPosition.x += delta.x;
            screenCameraPosition.y += delta.y;
            var newPos = Camera.main.ScreenToWorldPoint(screenCameraPosition);
            Camera.main.transform.position = new Vector3(newPos.x, newPos.y, cameraPos.z);
        }

        public void OnConfirmWantsDrag(PointerEventData data)
        {
            if (sceneData is Scene)
                data.Use();
        }
    }
}