using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using RAGE.Analytics;
using UnityEngine.EventSystems;
using System.Linq;
using System;
using AssetPackage;

namespace uAdventure.Runner
{
    [RequireComponent(typeof(TransitionManager))]
    public class SceneMB : MonoBehaviour, IRunnerChapterTarget, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IConfirmWantsDrag
    {
        private class HeightLayerComparer : IComparer<ScenePositioner>
        {
            public Dictionary<ScenePositioner, float> Heights { get; set; }
            public int Compare(ScenePositioner x, ScenePositioner y)
            {
                var order = (int)(Heights[x] - Heights[y]);
                if (Mathf.Approximately(order, 0))
                {
                    order = x.Context.getLayer() - y.Context.getLayer();
                }
                return order;
            }
        }

        private enum MovieState { NOT_MOVIE, LOADING, PLAYING, STOPPED };

        // Constants
        public static readonly Vector2 ScenePivot = new Vector2(0, 1f);
        public static readonly Vector2 DefaultPivot = Vector2.one / 2f;
        public static readonly float VideoSceneRatio = 4f / 3f;
        public static readonly float PixelsToWorld = 0.1f;
        public static readonly float PixelsSceneHeight = 600f;
        public static readonly float WorldSceneHeight = PixelsSceneHeight * PixelsToWorld;

        // Properties
        [SerializeField]
        protected GameObject exitPrefab, activeAreaPrefab, characterPrefab, objectPrefab, atrezzoPrefab, playerPrefab;
        private Transform exitsHolder, activeAreasHolder, referencesHolder, background, foreground;
        private bool interactuable;
        private bool ready;
        private bool firstRender = true;
        private bool dragging;
        private Vector2 endDragSpeed;
        private Dictionary<ScenePositioner, float> heights;
        private SortedList<ScenePositioner, float> finalOrder;

        // Movements
        private float minZ;
        private float maxZ;
        private TrajectoryHandler trajectoryHandler;

        // Transitions
        private TransitionManager transitionManager;
        private readonly float resistance = 5000f;


        // Slides Properties
        private eAnim slides;
        private int currentSlide;

        // Movie Properties
        private MovieHolder movie;
        private MovieState movieplayer = MovieState.NOT_MOVIE;


        public GeneralScene SceneData { get; set; }

        public object Data
        {
            get { return SceneData; }
            set { SceneData = (GeneralScene)value; }
        }

        public Vector2 PixelsSize
        {
            get { return WorldSize / PixelsToWorld; }
        }

        public Vector2 WorldSize
        {
            get { return background.localScale; }
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

        protected void Awake()
        {
            var comparer = new HeightLayerComparer();
            heights = new Dictionary<ScenePositioner, float>();
            finalOrder = new SortedList<ScenePositioner, float>(comparer);
            comparer.Heights = heights;
        }

        // Use this for initialization
        protected void Start()
        {
            this.gameObject.name = SceneData.getId();
            this.transitionManager = GetComponent<TransitionManager>();

            RenderScene();
        }

        protected void Update()
        {
            if(!dragging && endDragSpeed != Vector2.zero)
            {
                MoveCamera(endDragSpeed * -Time.deltaTime);
                var lastSpeed = endDragSpeed;
                endDragSpeed -= (endDragSpeed.normalized * resistance) * Time.deltaTime;
                if (lastSpeed.sqrMagnitude < endDragSpeed.sqrMagnitude)
                {
                    endDragSpeed = Vector2.zero;
                }
            }
        }

        protected void LateUpdate()
        {
            if (!referencesHolder || referencesHolder.childCount == 0)
            {
                return;
            }

            // First we categorize the childs
            // (This is not necesary if the childs wont change)
            var staticChilds = new List<ScenePositioner>();
            heights.Clear();
            finalOrder.Clear();

            for (int i = 0; i < referencesHolder.childCount; i++)
            {
                var representable = referencesHolder.GetChild(i).GetComponent<ScenePositioner>();
                // Not layered element
                if (representable.Context.getLayer() < 0)
                {
                    heights[representable] = representable.Context.getY();
                    finalOrder[representable] = representable.Context.getY();
                }
                else
                {
                    staticChilds.Add(representable);
                }
            }

            // Then we sort the static childs by layer
            staticChilds.Sort((r1, r2) => r1.Context.getLayer() - r2.Context.getLayer());

            // Then we stabilish a layer Y:
            //  Higher layers should appear on top of lower layers.
            //  Lower Y elements should also appear on top of higher Y elements.
            //  Since the first rule is more important, to fix this, we simulate the Y coordinate for elements
            //  that have higher Y coordinate, but lower layer. The Y used is the minimum Y for the lower layer elements.
            var maxY = staticChilds.Any() ? staticChilds.First().Position.y : 0;
            foreach(var child in staticChilds)
            {
                // Since we're iterating backwards we carry the minimum to the higher layers
                maxY = Mathf.Max(maxY, child.Position.y);
                // Finally we insert the elements: The layered elements use the simulated Y, whereas the dynamic ones use their real Y.
                heights[child] = maxY;
                finalOrder[child] = maxY;
            }

            // And then we apply the Z coordinate
            if (finalOrder.Any())
            {
                var zStep = (maxZ - minZ) / finalOrder.Count;
                var count = 0;
                foreach (var kv in finalOrder)
                {
                    kv.Key.Z = minZ + zStep * count;
                    count++;
                }
            }
        }

        protected void FixedUpdate()
        {
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

        public void Destroy(float time, System.Action onDestroy)
        {
            GameObject.DestroyImmediate(this.gameObject);
            onDestroy();
        }

        private void LoadParents()
        {
            this.background = this.transform.Find("Background");
            if (background)
            {
                this.transitionManager.UseMaterial(background.GetComponent<MeshRenderer>().material);
            }
            this.foreground = this.transform.Find("Foreground");
            this.exitsHolder = this.transform.Find("Exits");
            this.activeAreasHolder = this.transform.Find("ActiveAreas");
            this.referencesHolder = this.transform.Find("References");
        }

        private void LoadZBoundaries()
        {
            // Space for references goes from background to the references holder
            minZ = 1; 
            maxZ = -1; 
        }
        
        public static Vector3 ToWorldSize(Vector2 size)
        {
            var worldSize = Vector3.one;
            worldSize.x = size.x * PixelsToWorld;
            worldSize.y = size.y * PixelsToWorld;
            return worldSize;
        }

        public Vector3 ToWorldPosition(Vector2 position, Vector2 size, Vector2 pivot)
        {
            return ToWorldPosition(position, size, pivot, 0f);
        }

        public Vector3 ToWorldPosition(Vector2 position, Vector2 size, Vector2 pivot, float depth)
        {
            return ToWorldPosition(position, size, pivot, DefaultPivot, depth);
        }

        public Vector3 ToWorldPosition(Vector2 position, Vector2 size, Vector2 pivot, Vector2 objectPivot)
        {
            return ToWorldPosition(position, size, pivot, objectPivot, 0f);
        }

        public Vector3 ToWorldPosition(Vector2 position, Vector2 size, Vector2 pivot, Vector2 objectPivot, float depth)
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
            ready = false;
            this.transform.position = new Vector3(0, 0, 0);
            LoadParents();
            switch (SceneData.getType())
            {
                default:
                    Debug.LogError("Wrong scene type: " + SceneData.GetType());
                    ready = true;
                    break;
                case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
                    InventoryManager.Instance.Show = false;
                    movie = Game.Instance.ResourceManager.getVideo(((Videoscene)SceneData).getResources()[0].getAssetPath(Videoscene.RESOURCE_TYPE_VIDEO));
                    movieplayer = MovieState.LOADING;
                    SetBackground(movie.Movie);
                    break;
                case GeneralScene.GeneralSceneSceneType.SCENE:
                    Scene scene = (Scene)SceneData;
                    InventoryManager.Instance.Show = !SceneData.HideInventory;
                    Texture2D backgroundTexture = null;
                    foreach (ResourcesUni sr in scene.getResources())
                    {
                        if (ConditionChecker.check(sr.getConditions()))
                        {
                            backgroundTexture = Game.Instance.ResourceManager.getImage(sr.getAssetPath(Scene.RESOURCE_TYPE_BACKGROUND));
                            SetBackground(backgroundTexture);

                            var foregroundPath = sr.getAssetPath(Scene.RESOURCE_TYPE_FOREGROUND);
                            if (!string.IsNullOrEmpty(foregroundPath))
                            {
                                Texture2D foregroundTexture = Game.Instance.ResourceManager.getImage(foregroundPath);

                                foreground.GetComponent<Renderer>().material.SetTexture("_MainTex", backgroundTexture);
                                foreground.GetComponent<Renderer>().material.SetTexture("_AlphaTex", foregroundTexture);

                                foreground.localScale = background.localScale;
                                var foreGroundPos = background.localPosition;
                                foreGroundPos.z = 1;
                                foreground.localPosition = foreGroundPos;
                            }

                            LoadBackgroundMusic(sr);

                            break;
                        }
                    }

                    LoadZBoundaries();
                    //###################### REFERENCES ######################
                    DeleteChilds(referencesHolder);
                    // Characters

                    foreach (var context in Game.Instance.GameState
                        .GetElementReferences(scene.getId())
                        .Where(tc => !tc.IsRemoved())
                        .Checked())
                    {
                        InstanceElement(context);
                    }

                    //###################### ACTIVEAREAS ######################
                    DeleteChilds(activeAreasHolder);

                    foreach (var activeArea in scene.getActiveAreas().NotRemoved()
                        .Where(a => ConditionChecker.check(a.getConditions())))
                    {
                        InstanceRectangle<ActiveArea>(activeArea);
                    }

                    //###################### EXITS ######################
                    DeleteChilds(exitsHolder);

                    foreach (var exit in scene.getExits()
                        .Where(e => e.isHasNotEffects() || ConditionChecker.check(e.getConditions())))
                    {
                        InstanceRectangle<Exit>(exit);
                    }


                    if (!Game.Instance.GameState.IsFirstPerson && scene.isAllowPlayerLayer())
                    {
                        var playerContext = Game.Instance.GameState.PlayerContext;

                        //###################### BARRIERS ######################
                        var barriers = scene.getBarriers().FindAll(b => ConditionChecker.check(b.getConditions())).ToArray();

                        var trajectory = scene.getTrajectory();
                        if (trajectory == null)
                        {
                            barriers = barriers.ToList().ConvertAll(b => {
                                Barrier r = b.Clone() as Barrier;
                                r.setValues(r.getX(), 0, r.getWidth(), backgroundTexture.height);
                                return r;
                            }).ToArray();

                            trajectory = new Trajectory();
                            var width = backgroundTexture ? backgroundTexture.width : Screen.width;
                            trajectory.addNode("leftSide", 0, playerContext.getY(), playerContext.Scale);
                            trajectory.addNode("rightSide", width, playerContext.getY(), playerContext.Scale);
                            trajectory.addSide("leftSide", "rightSide", width);
                        }

                        trajectoryHandler = new TrajectoryHandler(TrajectoryHandler.CreateBlockedTrajectory(trajectory, barriers));

                        Representable player = GameObject.Instantiate(playerPrefab, referencesHolder).GetComponent<Representable>();
                        player.Element = Game.Instance.GameState.Player;
                        player.Context = playerContext;

                        var scenePositioner = player.gameObject.AddComponent<ScenePositioner>();
                        scenePositioner.Scene = this;
                        scenePositioner.Representable = player;
                        scenePositioner.Context = playerContext;
                        // Force the start
                        player.SendMessage("Start");
                        
                        ready = true;
                    }
                    else
                    {
                        ready = true;
                    }

                    var playerFollower = FindObjectOfType<PlayerFollower>();
                    if (playerFollower)
                    {
                        playerFollower.SettleInstant();
                    }

                    break;
                case GeneralScene.GeneralSceneSceneType.SLIDESCENE:
                    InventoryManager.Instance.Show = false;
                    Slidescene ssd = (Slidescene)SceneData;
                    currentSlide = 0;
                    foreach (ResourcesUni r in ssd.getResources())
                    {
                        if (ConditionChecker.check(r.getConditions()))
                        {
                            this.slides = Game.Instance.ResourceManager.getAnimation(r.getAssetPath(Slidescene.RESOURCE_TYPE_SLIDES));
                            SetSlide(0);

                            LoadBackgroundMusic(r);

                            ready = true;
                            break;
                        }
                    }
                    break;
            }
            firstRender = false;
        }

        private void LoadBackgroundMusic(ResourcesUni sr)
        {
            var musicPath = sr.getAssetPath(Scene.RESOURCE_TYPE_MUSIC);
            if (!string.IsNullOrEmpty(musicPath))
            {
                AudioClip audioClip = Game.Instance.ResourceManager.getAudio(musicPath);
                var audioSource = GetComponent<AudioSource>();
                audioSource.clip = audioClip;
                audioSource.loop = true;
                audioSource.Play();
            }
        }

        private static void DeleteChilds(Transform parent)
        {
            if (parent != null)
            {
                foreach (Transform child in parent)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }

        private void InstanceElement(ElementReference context)
        {
            Element element = Game.Instance.GameState.FindElement<Element>(context.getTargetId());
            if(element == null)
            {
                Debug.LogError("Unable to find target element: " + context.getTargetId());
                return;
            }

            GameObject basePrefab;
            Transform parent;
            if (element.GetType() == typeof(Atrezzo))
            {
                basePrefab = atrezzoPrefab;
                parent = referencesHolder;
            }
            else if (element.GetType() == typeof(NPC))
            {
                basePrefab = characterPrefab;
                parent = referencesHolder;
            }
            else if (element.GetType() == typeof(Item))
            {
                basePrefab = objectPrefab;
                parent = referencesHolder;
            }
            else
            {
                return;
            }

            GameObject ret = Instantiate(basePrefab, parent);
            var representable = ret.GetComponent<Representable>();
            representable.Context = context;
            representable.Element = element;

            var scenePositioner = ret.AddComponent<ScenePositioner>();
            scenePositioner.Scene = this;
            scenePositioner.Context = context;
            scenePositioner.Representable = representable;
        }

        private void InstanceRectangle<T>(Rectangle context) where T : Rectangle
        {
            if (context == null)
            {
                return;
            }

            GameObject basePrefab;
            Transform parent;

            if (typeof(T) == typeof(ActiveArea))
            {
                basePrefab = activeAreaPrefab;
                parent = activeAreasHolder;
            }
            else if(typeof(T) == typeof(Exit))
            {
                basePrefab = exitPrefab;
                parent = exitsHolder;
            }else
            {
                return;
            }

            GameObject ret = GameObject.Instantiate(basePrefab, parent);
            ret.GetComponent<Area>().Element = context;

            Transform trans = ret.GetComponent<Transform>();

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
            if (background)
            {
                var size = texture ? new Vector2(texture.width, texture.height) : new Vector2(Screen.width, Screen.height);
                if (texture)
                {
                    background.GetComponent<Renderer>().material.mainTexture = texture;
                }
                background.localScale = ToWorldSize(size);
                background.localPosition = ToWorldPosition(Vector2.zero, size, ScenePivot, 20);
                transform.localScale = (Vector3) (Vector2.one * (PixelsSceneHeight / size.y)) + new Vector3(0, 0, 1);
                var playerFollower = FindObjectOfType<PlayerFollower>();
                if (playerFollower)
                {
                    playerFollower.Background = background.gameObject;

                    if (firstRender)
                    {
                        playerFollower.Init();
                    }

                    playerFollower.SettleInstant();
                }
            }

        }

        private bool wasDisabled;
        private void SetSlide(int i)
        {
            if (slides != null && i < slides.frames.Count)
            {
                var texture = slides.frames[i].Image;

                if (slides.Animation.isUseTransitions() && i > 0)
                {
                    var transition = slides.Animation.getTranstionForFrame(currentSlide);
                    transitionManager.PrepareTransition(transition, texture);
                    wasDisabled = uAdventureRaycaster.Instance.Disabled;
                    uAdventureRaycaster.Instance.Disabled = true;
                    transitionManager.DoTransition((_, t) =>
                    {
                        this.currentSlide = i;
                        SetBackground(t);
                        uAdventureRaycaster.Instance.Disabled = wasDisabled;
                    });
                }
                else
                {
                    this.currentSlide = i;
                    SetBackground(texture);
                }
            }
        }

        private bool ProgressSlide()
        {
            var canProgress = currentSlide + 1 < this.slides.frames.Count;

            if (canProgress)
            {
                SetSlide(currentSlide + 1);
            }

            return canProgress;
        }

        public InteractuableResult Interacted()
        {
            return Interacted(null);
        }

        public InteractuableResult Interacted(PointerEventData pointerData)
        {
            InteractuableResult res = InteractuableResult.IGNORES;
            
            switch (SceneData.getType())
            {
                case GeneralScene.GeneralSceneSceneType.SCENE:
                    var scene = SceneData as Scene;
                    if (!Game.Instance.GameState.IsFirstPerson && scene.isAllowPlayerLayer())
                    {
                        RaycastHit hitInfo;
                        background.GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(pointerData.position), out hitInfo, float.MaxValue);
                        var texPos = PixelsSize;
                        var texCoord = hitInfo.textureCoord;
                        texCoord.y = 1 - texCoord.y;
                        texPos.Scale(texCoord);
                        var positioner = PlayerMB.Instance.GetComponent<ScenePositioner>();
                        var accesible = TrajectoryHandler.GetAccessibleTrajectory(positioner.Position, trajectoryHandler);
                        PlayerMB.Instance.GetComponent<Mover>().Move(accesible.closestPoint(texPos));
                    }
                    break;
                case GeneralScene.GeneralSceneSceneType.SLIDESCENE:
                    if (ProgressSlide())
                    {
                        res = InteractuableResult.REQUIRES_MORE_INTERACTION;
                    }
                    else
                    {
                        res = FinishCutscene((Cutscene)SceneData);
                    }
                    break;
                case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
                    var videoscene = (Videoscene)SceneData;
                    if (movieplayer == MovieState.NOT_MOVIE
                        || movieplayer == MovieState.STOPPED
                        || (movieplayer == MovieState.PLAYING && !movie.isPlaying())
                        || videoscene.isCanSkip())
                    {
                        movie.Stop();
                        movieplayer = MovieState.STOPPED;
                        if (movieplayer == MovieState.PLAYING)
                        {
                            TrackerAsset.Instance.Accessible.Skipped(SceneData.getId(), AccessibleTracker.Accessible.Cutscene);
                        }
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
                default: // By default Cutscene.GOBACK
                    var previousTarget = Game.Instance.GameState.PreviousChapterTarget;
                    if (previousTarget == null)
                    {
                        var possibleTargets = Game.Instance.GameState.GetObjects<IChapterTarget>();
                        previousTarget = possibleTargets.FirstOrDefault(t => t.getId() != this.SceneData.getId());
                    }
                    if(previousTarget != null)
                    {
                        triggerScene = new TriggerSceneEffect(previousTarget.getId(), int.MinValue, int.MinValue, 
                            float.MinValue, cutscene.getTransitionTime(), (int)cutscene.getTransitionType());
                    }
                    break;
                case Cutscene.NEWSCENE:
                    triggerScene = new TriggerSceneEffect(cutscene.getTargetId(), int.MinValue, int.MinValue,
                        float.MinValue, cutscene.getTransitionTime(), (int)cutscene.getTransitionType());
                    break;
                case Cutscene.ENDCHAPTER:
                    // TODO: When we add more chapters, we must trigger the next chapter instead of quiting que aplication
                    GUIManager.Instance.ExitApplication();
                    break;
            }

            if (triggerScene != null)
            {
                var effects = new Effects { triggerScene };
                var cutsceneEffects = cutscene.getEffects();
                if (cutsceneEffects != null)
                {
                    effects.AddRange(cutsceneEffects);
                }

                Game.Instance.Execute(new EffectHolder(effects));
            }

            return res;
        }

        private static void ColorChilds(Transform transform, Color color)
        {
            foreach (Transform t1 in transform)
            {
                if (t1.name != "Exits")
                {
                    if (t1.GetComponent<Renderer>() != null)
                    {
                        t1.GetComponent<Renderer>().material.color = color;
                    }

                    foreach (Transform t2 in t1)
                    {
                        if (t2.GetComponent<Renderer>() != null)
                        {
                            t2.GetComponent<Renderer>().material.color = color;
                        }
                    }
                }
            }
        }

        // VIDEOSCENE FUNCTIONS

        public void OnPointerClick(PointerEventData eventData)
        {
            MenuMB.Instance.hide();
            Interacted(eventData);
        }

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

        private static void MoveCamera(Vector2 delta)
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
            if (SceneData is Scene)
            {
                data.Use();
            }
        }
    }
}