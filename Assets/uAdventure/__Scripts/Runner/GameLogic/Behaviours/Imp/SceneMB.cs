using UnityEngine;
using System.Collections.Generic;

using uAdventure.Core;
using UnityEngine.EventSystems;
using System.Linq;
using Xasu;
using Xasu.HighLevel;

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

        // Music synchronization
        private static AudioClip lastClip;
        private static float lastSecond;

        // Properties
        [SerializeField]
        protected GameObject exitPrefab, activeAreaPrefab, characterPrefab, objectPrefab, atrezzoPrefab, playerPrefab;
        private Transform exitsHolder, activeAreasHolder, referencesHolder, background, foreground;
        private bool interactuable;
        private bool ready;
        private bool firstRender = true;
        private bool dragging;
        private Vector2 endDragSpeed;
        private Texture2D backgroundTexture;
        private Dictionary<ScenePositioner, float> heights;
        private SortedList<ScenePositioner, float> finalOrder;
        private bool wasDisabled;
        private EffectHolder finishingEffects;

        // Movements
        private float minZ;
        private float maxZ;
        private TrajectoryHandler trajectoryHandler;

        // Transitions
        private TransitionManager transitionManager;
        private readonly float resistance = 500f;


        // Slides Properties
        private eAnim slides;
        private int currentSlide;

        // Movie Properties
        private MovieHolder movie;
        private MovieState movieplayer = MovieState.NOT_MOVIE;

        private bool isPinching = false;
        private static float OriginalOrthoSize, LastOrthoSize;
        private const float MinOrthoSize = 10, MaxOrthoSize = 30;

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
            this.transitionManager = GetComponent<TransitionManager>();
            Game.Instance.GameState.OnConditionChanged += OnConditionChanged;
        }

        // Use this for initialization
        protected void Start()
        {
            this.gameObject.name = SceneData.getId();

            if (OriginalOrthoSize == 0)
            {
                LastOrthoSize = OriginalOrthoSize = Camera.main.orthographicSize;
            }

            Camera.main.orthographicSize = LastOrthoSize;
        }

        private bool touch0Set, touch1Set;
        private Vector2 touchPosition0, touchPosition1;
        private Vector2 touchDelta0, touchDelta1;

        protected void Update()
        {
            if (!dragging && endDragSpeed != Vector2.zero)
            {
                MoveCamera(endDragSpeed * -Time.deltaTime);
                var lastSpeed = endDragSpeed;
                endDragSpeed -= (endDragSpeed.normalized * resistance) * Time.deltaTime;
                if (lastSpeed.sqrMagnitude < endDragSpeed.sqrMagnitude)
                {
                    endDragSpeed = Vector2.zero;
                }
            }

            HandleZoom();
        }

        private void HandleZoom()
        {
            if (!SceneData.AllowsZoom)
            {
                return;
            }

            SimulateTouches();
            if ((uAdventureRaycaster.Instance.Override == null || uAdventureRaycaster.Instance.Override == gameObject) && (Input.touchCount >= 2 || IsSimulatingTouches()))
            {
                GetTouchInputs(out Touch touch0, out Touch touch1);

                if (!isPinching)
                {
                    isPinching = true;
                    uAdventureRaycaster.Instance.Override = this.gameObject;
                }

                // This is a camera movement zoom approach
                if ((touch0.phase == TouchPhase.Moved || touch0.phase == TouchPhase.Stationary) &&
                    (touch1.phase == TouchPhase.Moved || touch1.phase == TouchPhase.Stationary))
                {
                    var cam = GameObject.FindObjectOfType<Camera>();

                    // First we get the old touch positions to compare them with the new ones
                    var oldTouch0 = touch0.position - touch0.deltaPosition;
                    var oldTouch1 = touch1.position - touch1.deltaPosition;

                    // The camera moves as much as the center moves
                    var previousTouchCenter = (cam.ScreenToWorldPoint(oldTouch1) + cam.ScreenToWorldPoint(oldTouch0)) / 2f;
                    var touchCenter = (cam.ScreenToWorldPoint(touch1.position) + cam.ScreenToWorldPoint(touch0.position)) / 2f;
                    var touchCenterMovement = touchCenter - previousTouchCenter;

                    // Second, we calculate the camera growth based on the touch distance difference
                    var oldDist = (oldTouch1 - oldTouch0).magnitude;
                    var currentDist = (touch1.position - touch0.position).magnitude;
                    var growthRatio = currentDist / oldDist;

                    // The zoom moves towards the center proportionally to the growth
                    // To do it, we calculate the vector from the camera center to the touch center.
                    // This vector is scaled inversely to the camera growth so the camera moves towards this point when zooming.
                    // (for instance, if the camera doubles the zoom, it will also move towards the touch center half the distance)
                    var cameraCenter = cam.transform.position;
                    var touchCenterToCameraVector = cameraCenter - touchCenter;
                    var newTouchCenterToCameraVector = touchCenterToCameraVector / growthRatio;

                    // Finally we calculate the new orthographic size inverse to the camera growth
                    // (If the ratio of growth > 1 the ortho size decreases, zooming )
                    var newOrtho = cam.orthographicSize / growthRatio;
                    cam.orthographicSize = Mathf.Clamp(newOrtho, MinOrthoSize, MaxOrthoSize);

                    // And we adjust the camera position based on:
                    //  - The touch center (the point between the touches)
                    //  - Plus the new scaled camera vector
                    //  - Minus the movement of the touches (similar to dragging from the center point)
                    cam.transform.position = touchCenter + newTouchCenterToCameraVector - touchCenterMovement;
                }
            }
            else if (isPinching)
            {
                uAdventureRaycaster.Instance.Override = null;
                isPinching = false;
            }


            if ((uAdventureRaycaster.Instance.Override == null || uAdventureRaycaster.Instance.Override == gameObject) && Input.mouseScrollDelta.y != 0)
            {
                var cam = GameObject.FindObjectOfType<Camera>();
                cam.orthographicSize = Mathf.Clamp(cam.orthographicSize - Input.mouseScrollDelta.y, MinOrthoSize, MaxOrthoSize);
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
                var zStep = SectionSpace / finalOrder.Count;
                var count = 0;
                foreach (var kv in finalOrder)
                {
                    kv.Key.Z = zStep * count;
                    count++;
                }
            }
        }

        protected float SectionSpace { get { return (maxZ - minZ) / (float)transform.childCount; } }

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
            var audioSource = GetComponent<AudioSource>();
            lastClip = null;
            lastSecond = 0;
            if (audioSource && audioSource.clip)
            {
                lastClip = audioSource.clip;
                lastSecond = audioSource.time;
            }

            var emptytexture = new Texture2D(1, 1);
            foreground.GetComponent<Renderer>().material.SetTexture("_MainTex", emptytexture);
            foreground.GetComponent<Renderer>().material.SetTexture("_AlphaTex", emptytexture);

            Game.Instance.GameState.OnConditionChanged -= OnConditionChanged;
            GameObject.DestroyImmediate(this.gameObject);

            Scene scene = SceneData as Scene;
            if(scene != null)
            {
                foreach (ResourcesUni sr in scene.getResources())
                {
                    if (ConditionChecker.check(sr.getConditions()))
                    {
                        Game.Instance.ResourceManager.ClearImage(sr.getAssetPath(Scene.RESOURCE_TYPE_BACKGROUND));
                    }
                }
            }

            if (Camera.main)
            {
                Camera.main.orthographic = true;
                Camera.main.orthographicSize = OriginalOrthoSize;
            }

            onDestroy();
        }

        private void OnConditionChanged(string condition, int value)
        {
            switch (SceneData.getType())
            {
                default:
                    // Nothing to do in the other scenes
                    break;
                case GeneralScene.GeneralSceneSceneType.SCENE:
                    Scene scene = (Scene)SceneData;
                    RefreshBackground(scene);
                    if (!Game.Instance.GameState.IsFirstPerson && scene.isAllowPlayerLayer())
                    {
                        RefreshPlayerAndTrajectory(scene);
                    }
                    break;
            }
        }

        private void LoadParents()
        {
            var childs = Enumerable.Range(0, this.transform.childCount).Select(n => transform.GetChild(n));
            LoadZBoundaries();
            var step = (maxZ - minZ) / ((int)childs.Count());

            for(int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                child.transform.localPosition = new Vector3(child.transform.localPosition.x, child.transform.localPosition.y, minZ + step * i);
                switch (child.name)
                {
                    case "Background":
                        this.background = child;
                        if (background)
                        {
                            this.transitionManager.UseMaterial(background.GetComponent<MeshRenderer>().material);
                        }
                        break;
                    case "Foreground":
                        this.foreground = child;
                        break;
                    case "Exits":
                        this.exitsHolder = child;
                        break;
                    case "ActiveAreas":
                        this.activeAreasHolder = child;
                        break;
                    case "References":
                        this.referencesHolder = child;
                        break;
                }
            }
        }

        private void LoadZBoundaries()
        {
            // Space for references goes from background to the references holder
            minZ = 10; 
            maxZ = 0; 
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
                    ready = true;
                    break;
                case GeneralScene.GeneralSceneSceneType.SCENE:
                    Scene scene = (Scene)SceneData;
                    InventoryManager.Instance.Show = !SceneData.HideInventory;
                    RefreshBackground(scene);

                    LoadZBoundaries();
                    //###################### REFERENCES ######################
                    DeleteChilds(referencesHolder);
                    // Characters

                    foreach (var context in Game.Instance.GameState
                        .GetElementReferences(scene.getId()))
                    {
                        InstanceElement(context);
                    }
                    // The references are ordered in lateupdate

                    //###################### ACTIVEAREAS ######################
                    DeleteChilds(activeAreasHolder);

                    foreach (var activeArea in scene.getActiveAreas())
                    {
                        InstanceRectangle<ActiveArea>(activeArea);
                    }

                    OrderElementsZ(activeAreasHolder, SectionSpace);

                    //###################### EXITS ######################
                    DeleteChilds(exitsHolder);

                    foreach (var exit in scene.getExits())
                    {
                        InstanceRectangle<Exit>(exit);
                    }

                    OrderElementsZ(exitsHolder, SectionSpace);


                    //###################### THIRD PERSON ######################
                    if (!Game.Instance.GameState.IsFirstPerson && scene.isAllowPlayerLayer())
                    {
                        RefreshPlayerAndTrajectory(scene);
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
                    foreach (ResourcesUni r in ssd.getResources().Checked())
                    {
                        this.slides = Game.Instance.ResourceManager.getAnimation(r.getAssetPath(Slidescene.RESOURCE_TYPE_SLIDES));
                        SetSlide(0);

                        LoadBackgroundMusic(r);

                        ready = true;
                        break;
                    }
                    break;
            }
            firstRender = false;
        }

        private void RefreshPlayerAndTrajectory(Scene scene)
        {
            var playerContext = Game.Instance.GameState.PlayerContext;

            //###################### BARRIERS ######################
            var barriers = scene.getBarriers().FindAll(b => ConditionChecker.check(b.getConditions())).ToArray();

            var trajectory = scene.getTrajectory();
            if (trajectory == null)
            {
                barriers = barriers.ToList().ConvertAll(b =>
                {
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

            if (!ready)
            {
                Representable player = GameObject.Instantiate(playerPrefab, referencesHolder).GetComponent<Representable>();
                player.Element = Game.Instance.GameState.Player;
                player.Context = playerContext;

                var scenePositioner = player.gameObject.AddComponent<ScenePositioner>();
                scenePositioner.Scene = this;
                scenePositioner.Representable = player;
                scenePositioner.Context = playerContext;
                // Force the start
                player.SendMessage("Start");
            }
        }

        private void RefreshBackground(Scene scene)
        {
            backgroundTexture = null;
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
        }

        private void OrderElementsZ(Transform holder, float sectionSpace)
        {
            var space = sectionSpace / holder.childCount;
            for (int i = 0; i < holder.childCount; i++)
            {
                var t = holder.GetChild(i).transform;
                var p = t.localPosition;
                t.localPosition = new Vector3(p.x, p.y, space * i);
            }
        }

        private void LoadBackgroundMusic(ResourcesUni sr)
        {
            var musicPath = sr.getAssetPath(Scene.RESOURCE_TYPE_MUSIC);
            if (!string.IsNullOrEmpty(musicPath))
            {
                AudioClip audioClip = Game.Instance.ResourceManager.getAudio(musicPath);
                var audioSource = GetComponent<AudioSource>();
                if(audioSource.clip != audioClip)
                {
                    audioSource.clip = audioClip;
                    audioSource.loop = true;
                    if(lastClip == audioClip)
                    {
                        audioSource.time = lastSecond;
                    }
                    audioSource.Play();
                }
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

        private GameObject InstanceElement(ElementReference context)
        {
            Element element = Game.Instance.GameState.FindElement<Element>(context.getTargetId());
            if(element == null)
            {
                Debug.LogError("Unable to find target element: " + context.getTargetId());
                return null;
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
                return null;
            }

            GameObject ret = Instantiate(basePrefab, parent);
            var representable = ret.GetComponent<Representable>();
            representable.Context = context;
            representable.Element = element;

            var scenePositioner = ret.AddComponent<ScenePositioner>();
            scenePositioner.Scene = this;
            scenePositioner.Context = context;
            scenePositioner.Representable = representable;

            return ret;
        }

        private GameObject InstanceRectangle<T>(Rectangle context) where T : Rectangle
        {
            if (context == null)
            {
                return null;
            }

            GameObject basePrefab;
            Transform parent;

            if (typeof(T) == typeof(ActiveArea))
            {
                basePrefab = activeAreaPrefab;
                parent = activeAreasHolder;
            }
            else if (typeof(T) == typeof(Exit))
            {
                basePrefab = exitPrefab;
                parent = exitsHolder;
            }
            else
            {
                return null;
            }

            GameObject ret = GameObject.Instantiate(basePrefab, parent);
            ret.GetComponent<Area>().Element = context;

            Transform trans = ret.GetComponent<Transform>();

            Vector2 tmpPos = new Vector2(context.getX(), context.getY());
            Vector2 tmpSize = new Vector2(context.getWidth(), context.getHeight());

            // ActiveArea pivot starts from top-left corner (Y = 1, X = 0)
            trans.localPosition = ToWorldPosition(tmpPos, tmpSize, new Vector2(0, 1f), 0);
            trans.localScale = ToWorldSize(tmpSize);

            return ret;
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
                    res = InteractuableResult.REQUIRES_MORE_INTERACTION;
                    if (movieplayer == MovieState.NOT_MOVIE
                        || movieplayer == MovieState.STOPPED
                        || (movieplayer == MovieState.PLAYING && !movie.isPlaying())
                        || videoscene.isCanSkip())
                    {
                        if (movieplayer == MovieState.PLAYING && XasuTracker.Instance.Status.State != TrackerState.Uninitialized && videoscene.isCanSkip())
                        {
                            AccessibleTracker.Instance.Skipped(SceneData.getId(), AccessibleTracker.AccessibleType.Cutscene);
                        }
                        movie.Stop();
                        movieplayer = MovieState.STOPPED;
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

            if(finishingEffects == null)
            {
                switch ((cutscene).getNext())
                {
                    default: // By default Cutscene.GOBACK
                        var previousTarget = Game.Instance.GameState.PreviousChapterTarget;
                        if (previousTarget == null)
                        {
                            var possibleTargets = Game.Instance.GameState.GetObjects<IChapterTarget>();
                            previousTarget = possibleTargets.FirstOrDefault(t => t.getId() != this.SceneData.getId());
                        }
                        if (previousTarget != null)
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
                        Game.Instance.Quit();
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
                    finishingEffects = new EffectHolder(effects);
                }
            }
            
            if(finishingEffects != null)
            {
                if (Game.Instance.isSomethingRunning())
                {
                    if (finishingEffects.execute())
                    {
                        res = InteractuableResult.REQUIRES_MORE_INTERACTION;
                    }
                    else
                    {
                        res = InteractuableResult.IGNORES;
                    }
                }
                else
                {
                    Game.Instance.Execute(finishingEffects);
                }
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
            if(PlayerMB.Instance == null && !isPinching)
            {
                eventData.Use();
                dragging = true;
            }
        }

        

        public void OnEndDrag(PointerEventData eventData)
        {
            if (dragging && !isPinching)
            {
                dragging = false;
                eventData.Use();
                endDragSpeed = -MoveCamera(-eventData.delta) / Time.deltaTime;
            }
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (dragging && !isPinching)
            {
                eventData.Use();
                MoveCamera(-eventData.delta);
            }
        }

        private Vector2 MoveCamera(Vector2 delta)
        {
            var cameraPos = Camera.main.transform.position;
            var screenCameraPosition = Camera.main.WorldToScreenPoint(cameraPos);
            screenCameraPosition.x += delta.x;
            screenCameraPosition.y += delta.y;
            var newPos = Camera.main.ScreenToWorldPoint(screenCameraPosition);
            Camera.main.transform.position = new Vector3(newPos.x, newPos.y, cameraPos.z);
            PlayerFollower.FixInside(Camera.main.transform, background.gameObject, cameraPos.z);
            return Camera.main.transform.position - cameraPos;
        }

        public void OnConfirmWantsDrag(PointerEventData data)
        {
            if (SceneData is Scene && data.button == PointerEventData.InputButton.Left)
            {
                data.Use();
            }
        }

        // Touch Simulation for editor:
        //  - Maintain control pressed.
        //  - Left or right click fixes a touch point (0 or 1 respectively) 
        //  - Left or right drag moves the touch point (only works when the other touch is set too)
        //  - Releasing control also releases both touches

        private void GetTouchInputs(out Touch touch0, out Touch touch1)
        {
            touch0 = touch0Set && Application.isEditor
                ? new Touch
                {
                    phase = TouchPhase.Moved,
                    position = touchPosition0,
                    deltaPosition = touchDelta0
                }
                : Input.GetTouch(0);
            touch1 = touch1Set && Application.isEditor
                ? new Touch
                {
                    phase = TouchPhase.Moved,
                    position = touchPosition1,
                    deltaPosition = touchDelta1
                }
                : Input.GetTouch(1);
        }

        private bool IsSimulatingTouches()
        {
            return (touch0Set && touch1Set);
        }

        private void SimulateTouches()
        {
            if (!Application.isEditor)
            {
                return;
            }

            if (Input.GetKey(KeyCode.LeftControl))
            {
                var mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
                if (Input.GetMouseButton(0))
                {
                    if (!touch0Set)
                    {
                        touchDelta0 = Vector2.zero;
                        touch0Set = true;
                    }
                    else
                    {
                        touchDelta0 = mousePos - touchPosition0;
                    }
                    touchPosition0 = mousePos;
                }
                if (Input.GetMouseButton(1))
                {
                    if (!touch1Set)
                    {
                        touchDelta1 = Vector2.zero;
                        touch1Set = true;
                    }
                    else
                    {
                        touchDelta1 = mousePos - touchPosition1;
                    }
                    touchPosition1 = mousePos;
                }
            }
            else
            {
                touch0Set = false;
                touch1Set = false;
            }
        }
    }
}