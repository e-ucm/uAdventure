using System;
using System.Collections.Generic;
using UnityEngine;

namespace uAdventure.Core
{
    /**
     * This class holds the data of a scene of any type in eAdventure.
     */
    public abstract class GeneralScene : IChapterTarget, Documented, Named, HasId, ICloneable
    {

        public enum GeneralSceneSceneType { SCENE = 0, SLIDESCENE = 1, VIDEOSCENE = 2 };

        /**
         * xApi Class
         */
        protected string xapiClass = "accesible";

        /**
         * xApi Class type
         */
        protected string xapiType = "area";

        /**
         * xApi completable ends in
         */
        protected string xapiEndsIn;

        /**
         * xApi completable score.
         */
        protected string xapiScore;

        /**
         * Type of the scene.
         */
        private GeneralSceneSceneType type;

        /**
         * Id of the scene.
         */
        private string id;

        /**
         * Name of the scene.
         */
        private string name;

        /**
         * Documentation of the scene.
         */
        private string documentation;

        /**
         * List of resources for the scene.
         */
        private List<ResourcesUni> resources;

        /**
         * Stores if the scene should be loaded at the begining
         */
        private bool initialScene;

        /**
         * Stores if the scene should hide the inventory
         */
        private bool hideInventory;

        /**
         * Stores if the scene should allow the player to zoom in
         */
        private bool allowsZoom;

        /**
         * Stores if the user can save in the scene or not
         */
        private bool saveGameAllowed = true;

        public bool HideInventory
        {
            get
            {
                return hideInventory;
            }

            set
            {
                hideInventory = value;
            }
        }

        public bool AllowsZoom
        {
            get
            {
                return allowsZoom;
            }

            set
            {
                allowsZoom = value;
            }
        }

        /**
         * Creates a new GeneralScene.
         * 
         * @param type
         *            the type of the scene
         * @param id
         *            the id of the scene
         */
        protected GeneralScene(GeneralSceneSceneType type, string id)
        {

            this.type = type;
            this.id = id;
            name = "";
            documentation = null;
            resources = new List<ResourcesUni>();
        }

        /**
         * Returns the xapi class.
         * 
         * @return the xapi class.
         */
        public string getXApiClass()
        {
            return xapiClass == null ? "" : xapiClass;
        }
        /**
         * Returns the xapi type of the scene.
         * 
         * @return the xapi type of the scene
         */
        public string getXApiType()
        {
            return xapiType == null ? "" : xapiType;
        }

        /**
         * Returns the type of the scene.
         * 
         * @return the type of the scene
         */
        public GeneralSceneSceneType getType()
        {

            return type;
        }

        /**
         * Returns the id of the scene.
         * 
         * @return the id of the scene
         */
        public string getId()
        {
            
            return id == null ? "" : id;
        }

        /**
         * Returns the name of the scene.
         * 
         * @return the name of the scene
         */
        public string getName()
        {

            return name == null ? "" : name;
        }

        /**
         * Returns the documentation of the scene.
         * 
         * @return the documentation of the scene
         */
        public string getDocumentation()
        {

            return documentation == null ? "" : documentation;
        }

        /**
         * Returns the list of resources of this scene.
         * 
         * @return the list of resources of this scene
         */
        public List<ResourcesUni> getResources()
        {

            return resources;
        }

        /**
         * Returns the xapi class.
         * 
         * @return the xapi class.
         */
        public void setXApiClass(string xapiClass)
        {
            this.xapiClass = xapiClass;
        }
        /**
         * Returns the xapi type of the scene.
         * 
         * @return the xapi type of the scene
         */
        public void setXApiType(string xapiType)
        {

            this.xapiType = xapiType;
        }

        /**
         * Sets the a new identifier for the general scene.
         * 
         * @param id
         *            New identifier
         */
        public void setId(string id)
        {

            this.id = id;
        }

        /**
         * Changes the name of this scene.
         * 
         * @param name
         *            the new name
         */
        public void setName(string name)
        {

            this.name = name;
        }

        /**
         * Changes the documentation of this scene.
         * 
         * @param documentation
         *            The new documentation
         */
        public void setDocumentation(string documentation)
        {

            this.documentation = documentation;
        }

        /**
         * Adds some resources to the list of resources.
         * 
         * @param resources
         *            the resources to add
         */
        public void addResources(ResourcesUni resources)
        {

            this.resources.Add(resources);
        }

        /**
         * Returns whether this is the initial scenene
         * 
         * @return true if this is the initial scnene, false otherwise
         */
        public bool isInitialScene()
        {

            return initialScene;
        }

        /**
         * Changes whether this is the initial scene
         * 
         * @param initialScene
         *            true if this is the initial scene, false otherwise
         */
        public void setInitialScene(bool initialScene)
        {

            this.initialScene = initialScene;
        }

        public virtual object Clone()
        {
            GeneralScene gs = (GeneralScene)this.MemberwiseClone();
            gs.documentation = (documentation != null ? documentation : null);
            gs.id = (id != null ? id : null);
            gs.initialScene = initialScene;
            gs.name = (name != null ? name : null);
            if (resources != null)
            {
                gs.resources = new List<ResourcesUni>();
                foreach (ResourcesUni r in resources)
                    gs.resources.Add((ResourcesUni)r.Clone());
            }
            gs.type = type;
            return gs;
        }

        public void setAllowsSavingGame(bool allowsSavingGame)
        {
            saveGameAllowed = allowsSavingGame;
        }

        public bool allowsSavingGame()
        {
            return saveGameAllowed;
        }

        /*
        @Override
        public Object clone() throws CloneNotSupportedException
        {

            GeneralScene gs = (GeneralScene) super.clone( );
            gs.documentation = ( documentation != null ? new string(documentation ) : null );
            gs.id = ( id != null ? new string(id ) : null );
            gs.initialScene = initialScene;
            gs.name = ( name != null ? new string(name ) : null );
            if( resources != null ) {
                gs.resources = new List<Resources>( );
                for( Resources r : resources )
                    gs.resources.add( (Resources) r.clone( ) );
            }
    gs.type = type;
            return gs;
        }*/

    }
}