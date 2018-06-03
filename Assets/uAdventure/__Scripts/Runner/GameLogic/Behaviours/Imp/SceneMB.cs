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
    public class SceneMB : MonoBehaviour, IRunnerChapterTarget, IPointerClickHandler, IBeginDragHandler, IEndDragHandler, IDragHandler, IConfirmWantsDrag
    {
        private class HeightLayerComparer : IComparer<Representable>
        {
            public Dictionary<Representable, float> Heights { get; set; }
            public int Compare(Representable r1, Representable r2)
            {
                var order = (int)(Heights[r1] - Heights[r2]);
                if (Mathf.Approximately(order, 0))
                    order = r1.Context.getLayer() - r2.Context.getLayer();
                return order;
            }
        }

        private enum MovieState { NOT_MOVIE, LOADING, PLAYING, STOPPED };

        // Constants
        public static readonly Vector2 ScenePivot = new Vector2(0, 1f);
        public static readonly Vector2 DefaultPivot = Vector2.one / 2f;
        public const float VideoSceneRatio = 4f / 3f;
        public const float PixelsToWorld = 0.1f;
        public const float PixelsSceneHeight = 600f;
        public const float WorldSceneHeight = PixelsSceneHeight * PixelsToWorld;

        // Properties
        public GameObject exitPrefab, activeAreaPrefab, characterPrefab, objectPrefab, atrezzoPrefab, playerPrefab;
        private Transform exitsHolder, activeAreasHolder, referencesHolder, background, foreground;
        private bool interactuable = false;
        private GeneralScene sceneData;
        private bool ready = false;
        private bool dragging = false;
        private Vector2 endDragSpeed = Vector2.zero;
        private Dictionary<ElementReference, ElementReference> localContexts = new Dictionary<ElementReference, ElementReference>();
        private HeightLayerComparer comparer;
        private Dictionary<Representable, float> heights;
        private SortedList<Representable, float> finalOrder;

        // Movements
        private float minZ;
        private float maxZ;
        private TrajectoryHandler trajectoryHandler;
        private float[] layerY = new float[0];

        // Transitions
        private bool fading = false;
        private float totalTime = 0f, currentTime = 0f;
        private float resistance = 5000f;


        // Slides Properties
        private eAnim slides;
        private int currentSlide;

        // Movie Properties
        private MovieHolder movie;
        private MovieState movieplayer = MovieState.NOT_MOVIE;


        public GeneralScene SceneData
        {
            get { return sceneData; }
            set { sceneData = value; }
        }

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

        private void Awake()
        {
            var comparer = new HeightLayerComparer();
            heights = new Dictionary<Representable, float>();
            finalOrder = new SortedList<Representable, float>(comparer);
            comparer.Heights = heights;
        }

        // Use this for initialization
        void Start()
        {
            this.gameObject.name = sceneData.getId();
            RenderScene();
        }

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

        private void LateUpdate()
        {
            if (!referencesHolder || referencesHolder.childCount == 0)
            {
                return;
            }

            // First we categorize the childs
            // (This is not necesary if the childs wont change)
            var staticChilds = new List<Representable>();
            heights.Clear();
            finalOrder.Clear();

            for (int i = 0; i < referencesHolder.childCount; i++)
            {
                var representable = referencesHolder.GetChild(i).GetComponent<Representable>();
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
            var maxY = staticChilds.First().getPosition().y;
            foreach(var child in staticChilds)
            {
                // Since we're iterating backwards we carry the minimum to the higher layers
                maxY = Mathf.Max(maxY, child.getPosition().y);
                // Finally we insert the elements: The layered elements use the simulated Y, whereas the dynamic ones use their real Y.
                heights[child] = maxY;
                finalOrder[child] = maxY;
            }
            
            // And then we apply the Z coordinate
            var zStep = (maxZ - minZ) / finalOrder.Count;
            var count = 0;
            foreach(var kv in finalOrder)
            {
                kv.Key.Z = minZ + zStep * count;
                count++;
            }
        }

        void FixedUpdate()
        {
            if (fading)
            {
                currentTime -= Time.deltaTime;
                float alpha = currentTime / totalTime;
                ColorChilds(new Color(1, 1, 1, alpha));
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
                totalTime = time;
                currentTime = time;
                fading = true;
                this.transform.position = new Vector3(this.transform.position.x, this.transform.position.y, this.transform.position.z - 10);
            }
            else
                GameObject.DestroyImmediate(this.gameObject);
        }

        private void LoadParents()
        {
            this.background = this.transform.Find("Background");
            this.foreground = this.transform.Find("Foreground");
            this.exitsHolder = this.transform.Find("Exits");
            this.activeAreasHolder = this.transform.Find("ActiveAreas");
            this.referencesHolder = this.transform.Find("References");
        }

        private void LoadZBoundaries()
        {
            // Space for references goes from background to the references holder
            minZ = 1; // background.localPosition.z;
            maxZ = -1; // referencesHolder.localPosition.z;
        }



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
            LoadParents();
            switch (sceneData.getType())
            {
                case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
                    InventoryManager.Instance.Show = false;
                    movie = Game.Instance.ResourceManager.getVideo(((Videoscene)sceneData).getResources()[0].getAssetPath(Videoscene.RESOURCE_TYPE_VIDEO));
                    movieplayer = MovieState.LOADING;
                    SetBackground(movie.Movie);
                    break;
                case GeneralScene.GeneralSceneSceneType.SCENE:
                    InventoryManager.Instance.Show = true;
                    Scene scene = (Scene)sceneData;
                    Texture2D backgroundTexture = null;
                    foreach (ResourcesUni sr in scene.getResources())
                    {
                        if (ConditionChecker.check(sr.getConditions()))
                        {
                            backgroundTexture = Game.Instance.ResourceManager.getImage(sr.getAssetPath(Scene.RESOURCE_TYPE_BACKGROUND));
                            SetBackground(backgroundTexture);

                            if (sr.getAssetPath(Scene.RESOURCE_TYPE_FOREGROUND) != "")
                            {
                                Texture2D foregroundTexture = Game.Instance.ResourceManager.getImage(sr.getAssetPath(Scene.RESOURCE_TYPE_FOREGROUND));

                                foreground.GetComponent<Renderer>().material.SetTexture("_MainTex", backgroundTexture);
                                foreground.GetComponent<Renderer>().material.SetTexture("_AlphaTex", foregroundTexture);

                                foreground.localScale = background.localScale;
                                var foreGroundPos = background.localPosition;
                                foreGroundPos.z = 1;
                                foreground.localPosition = foreGroundPos;
                            }

                            break;
                        }
                    }

                    LoadZBoundaries();
                    //###################### REFERENCES ######################
                    DeleteChilds(referencesHolder);
                    // Characters
                    foreach (ElementReference context in scene.getCharacterReferences())
                        if (!Game.Instance.GameState.getRemovedElements().Contains(context.getTargetId()))
                            InstanceElement<NPC>(context);

                    // Items
                    foreach (ElementReference context in scene.getItemReferences())
                        if(!Game.Instance.GameState.getRemovedElements().Contains(context.getTargetId()))
                            InstanceElement<Item>(context);

                    // Atrezzos
                    foreach (ElementReference context in scene.getAtrezzoReferences())
                        if (!Game.Instance.GameState.getRemovedElements().Contains(context.getTargetId()))
                            InstanceElement<Atrezzo>(context);

                    //###################### ACTIVEAREAS ######################
                    DeleteChilds(activeAreasHolder);

                    foreach (ActiveArea ad in scene.getActiveAreas())
                        if (!Game.Instance.GameState.getRemovedElements().Contains(ad.getId()))
                            if (ConditionChecker.check(ad.getConditions()))
                                InstanceRectangle<ActiveArea>(ad);

                    //###################### EXITS ######################
                    DeleteChilds(exitsHolder);

                    foreach (Exit exit in scene.getExits())
                        if (exit.isHasNotEffects() || ConditionChecker.check(exit.getConditions()))
                            InstanceRectangle<Exit>(exit);


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
                            var width = backgroundTexture ? (int)backgroundTexture.width : Screen.width;
                            trajectory.addNode("leftSide", 0, playerContext.getY(), playerContext.getScale());
                            trajectory.addNode("rightSide", width, playerContext.getY(), playerContext.getScale());
                            trajectory.addSide("leftSide", "rightSide", width);
                        }

                        trajectoryHandler = new TrajectoryHandler(TrajectoryHandler.CreateBlockedTrajectory(trajectory, barriers));
                        /*GameObject.Destroy(this.transform.FindChild ("Player"));*/

                        Representable player = GameObject.Instantiate(playerPrefab, referencesHolder).GetComponent<Representable>();
                        player.Element = Game.Instance.GameState.Player;
                        player.Context = playerContext;
                        player.Scene = this;
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
                    Slidescene ssd = (Slidescene)sceneData;
                    currentSlide = 0;
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

        private void DeleteChilds(Transform parent)
        {
            if (parent != null)
            {
                foreach (Transform child in parent)
                {
                    GameObject.Destroy(child.gameObject);
                }
            }
        }

        private void InstanceElement<T>(ElementReference context) where T : Element
        {
            if (!ConditionChecker.check(context.getConditions()))
                return;

            if (!localContexts.ContainsKey(context))
            {
                ElementReference new_context = new ElementReference(context.getTargetId(), context.getX(), context.getY());
                new_context.setScale(context.getScale());
                new_context.setLayer(context.getLayer());
                new_context.setInfluenceArea(context.getInfluenceArea());
                new_context.setConditions(context.getConditions());
                localContexts.Add(context, new_context);
            }

            ElementReference localContext = localContexts[context];

            GameObject basePrefab;
            Transform parent;
            Element element;

            if (typeof(T) == typeof(Atrezzo))
            {
                basePrefab = atrezzoPrefab;
                parent = referencesHolder;
                element = Game.Instance.GameState.getAtrezzo(localContext.getTargetId());
            }
            else if (typeof(T) == typeof(NPC))
            {
                basePrefab = characterPrefab;
                parent = referencesHolder;
                element = Game.Instance.GameState.getCharacter(localContext.getTargetId());
            }
            else if (typeof(T) == typeof(Item))
            {
                basePrefab = objectPrefab;
                parent = referencesHolder;
                element = Game.Instance.GameState.getObject(localContext.getTargetId());
            }
            else
            {
                return;
            }

            GameObject ret = Instantiate(basePrefab, parent);
            var representable = ret.GetComponent<Representable>();
            representable.Scene = this;
            representable.Context = localContext;
            representable.Element = element;
        }

        private void InstanceRectangle<T>(Rectangle context) where T : Rectangle
        {
            if (context == null)
                return;

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
                var size = new Vector2(texture.width, texture.height);
                background.GetComponent<Renderer>().material.mainTexture = texture;
                background.localScale = ToWorldSize(size);
                background.localPosition = ToWorldPosition(Vector2.zero, size, ScenePivot, 20);
                transform.localScale = (Vector3) (Vector2.one * (PixelsSceneHeight / size.y)) + new Vector3(0, 0, 1);
            }

        }

        private void SetSlide(int i)
        {
            if (slides != null && i < slides.frames.Count)
            {
                this.currentSlide = i;
                SetBackground(slides.frames[currentSlide].Image);
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

        public InteractuableResult Interacted(PointerEventData pointerData = null)
        {
            InteractuableResult res = InteractuableResult.IGNORES;
            
            switch (SceneData.getType())
            {
                case GeneralScene.GeneralSceneSceneType.SCENE:
                    var scene = sceneData as Scene;
                    if (!Game.Instance.GameState.IsFirstPerson && scene.isAllowPlayerLayer())
                    {
                        RaycastHit hitInfo;
                        background.GetComponent<Collider>().Raycast(Camera.main.ScreenPointToRay(pointerData.position), out hitInfo, float.MaxValue);
                        var texPos = PixelsSize;
                        var texCoord = hitInfo.textureCoord;
                        texCoord.y = 1 - texCoord.y;
                        texPos.Scale(texCoord);
                        var accesible = TrajectoryHandler.GetAccessibleTrajectory(PlayerMB.Instance.GetComponent<Representable>().getPosition(), trajectoryHandler);
                        PlayerMB.Instance.GetComponent<Mover>().Move(accesible.closestPoint(texPos));
                    }
                    break;
                case GeneralScene.GeneralSceneSceneType.SLIDESCENE:
                    if (ProgressSlide()) res = InteractuableResult.REQUIRES_MORE_INTERACTION;
                    else res = FinishCutscene((Cutscene)SceneData);
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
                            TrackerAsset.Instance.Accessible.Skipped(SceneData.getId(), AccessibleTracker.Accessible.Cutscene);
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
                        previousTarget = possibleTargets.FirstOrDefault(t => t.getId() != this.SceneData.getId());
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
            var cutsceneEffects = ((Cutscene)SceneData).getEffects();
            if (cutsceneEffects != null)
            {
                e.AddRange(cutsceneEffects);
            }

            Game.Instance.Execute(new EffectHolder(e));

            return res;
        }

        private void ColorChilds(Color color)
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
            if (SceneData is Scene)
                data.Use();
        }
    }
}