using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using Animation = uAdventure.Core.Animation;

namespace uAdventure.Editor
{
    /**
     * Microcontroller for the resources data.
     */
    public class ResourcesDataControl : DataControl
    {
        /**
            * Contained resources.
            */
        private ResourcesUni resources;

        /**
         * The assets information of the resources.
         */
        private AssetInformation[] assetsInformation;

        private int[][] assetsGroups = null;

        private string[] groupsInfo = null;

        /**
         * Conditions controller.
         */
        private ConditionsController conditionsController;

        private int resourcesType;

        private Dictionary<string, string> imageIconMap;

        /**
         * Contructor.
         * 
         * @param resources
         *            Resources of the data control structure
         * @param resourcesType
         *            Type of the resources
         */
        public ResourcesDataControl(ResourcesUni resources, int resourcesType)
        {

            this.resources = resources;
            this.resourcesType = resourcesType;

            // Initialize the assetsInformation, depending on the assets type
            switch (resourcesType)
            {
                case Controller.SCENE:
                    assetsInformation = new AssetInformation[] { new AssetInformation("Resources.DescriptionSceneBackground", "background", true, AssetsConstants.CATEGORY_BACKGROUND, AssetsController.FILTER_JPG), new AssetInformation("Resources.DescriptionSceneForeground", "foreground", false, AssetsConstants.CATEGORY_BACKGROUND, AssetsController.FILTER_PNG), /*new AssetInformation( TextConstants.getText( "Resources.DescriptionSceneHardMap" ), "hardmap", false, AssetsController.CATEGORY_BACKGROUND, AssetsController.FILTER_PNG ), */new AssetInformation("Resources.DescriptionSceneMusic", "bgmusic", false, AssetsConstants.CATEGORY_AUDIO, AssetsController.FILTER_NONE) };

                    if (!resources.existAsset("background"))
                        resources.addAsset("background", SpecialAssetPaths.ASSET_EMPTY_BACKGROUND);
                    break;
                case Controller.CUTSCENE_SLIDES:
                    assetsInformation = new AssetInformation[] { new AssetInformation("Resources.DescriptionSlidesceneSlides", "slides", true, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_JPG), new AssetInformation("Resources.DescriptionSceneMusic", "bgmusic", false, AssetsConstants.CATEGORY_AUDIO, AssetsController.FILTER_NONE) };

                    if (!resources.existAsset("slides"))
                        resources.addAsset("slides", SpecialAssetPaths.ASSET_EMPTY_ANIMATION);
                    break;
                case Controller.ACTION_CUSTOM:
                case Controller.ACTION_CUSTOM_INTERACT:
                    assetsInformation = new AssetInformation[] { new AssetInformation("Resources.DescriptionButtonNormal", "buttonNormal", true, AssetsConstants.CATEGORY_BUTTON, AssetsController.FILTER_PNG), new AssetInformation("Resources.DescriptionButtonOver", "buttonOver", true, AssetsConstants.CATEGORY_BUTTON, AssetsController.FILTER_PNG), new AssetInformation("Resources.DescriptionButtonSound", "buttonSound", false, AssetsConstants.CATEGORY_AUDIO, AssetsController.FILTER_NONE)/*, new AssetInformation( "Resources.DescriptionButtonPressed" , "buttonPressed", true, AssetsConstants.CATEGORY_BUTTON, AssetsController.FILTER_PNG )*/, new AssetInformation("Resources.DescriptionActionAnimation", "actionAnimation", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_NONE), new AssetInformation("Resources.DescriptionActionAnimationLeft", "actionAnimationLeft", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_NONE) };
                    assetsGroups = new int[][] { new int[] { 0, 1, 2 }, new int[] { 3, 4 } };
                    groupsInfo = new string[] { "Resources.Button", "Resources.Animations" };

                    // For each asset, if it has not been declared attach the empty animation
                    string[] assets = new string[] { "buttonNormal", "buttonOver", "buttonPressed" };
                    foreach (string asset in assets)
                    {
                        if (!resources.existAsset(asset))
                            resources.addAsset(asset, SpecialAssetPaths.ASSET_EMPTY_ANIMATION);
                    }

                    break;
                case Controller.CUTSCENE_VIDEO:
                    assetsInformation = new AssetInformation[] { new AssetInformation("Resources.DescriptionVideoscenes", "video", true, AssetsConstants.CATEGORY_VIDEO, AssetsController.FILTER_NONE) };
                    break;
                case Controller.BOOK:
                    assetsInformation = new AssetInformation[] { new AssetInformation(  "Resources.DescriptionBookBackground" , "background", true, AssetsConstants.CATEGORY_BACKGROUND, AssetsController.FILTER_JPG ),
                        new AssetInformation( "Resources.ArrowLeftNormal", "arrowLeftNormal", false, AssetsConstants.CATEGORY_ARROW_BOOK, AssetsController.FILTER_PNG),
                        new AssetInformation( "Resources.ArrowRightNormal", "arrowRightNormal", false, AssetsConstants.CATEGORY_ARROW_BOOK, AssetsController.FILTER_PNG),
                        new AssetInformation( "Resources.ArrowLeftOver", "arrowLeftOver", false, AssetsConstants.CATEGORY_ARROW_BOOK, AssetsController.FILTER_PNG),
                        new AssetInformation( "Resources.ArrowRightOver", "arrowRightOver", false, AssetsConstants.CATEGORY_ARROW_BOOK, AssetsController.FILTER_PNG) };

                    if (!resources.existAsset("background"))
                        resources.addAsset("background", SpecialAssetPaths.ASSET_EMPTY_BACKGROUND);
                    break;
                case Controller.ITEM:
                    assetsInformation = new AssetInformation[] { new AssetInformation(  "Resources.DescriptionItemImage" , "image", true, AssetsConstants.CATEGORY_IMAGE, AssetsController.FILTER_NONE ), new AssetInformation( "Resources.DescriptionItemIcon" , "icon", false, AssetsConstants.CATEGORY_ICON, AssetsController.FILTER_NONE )
                                                            ,  new AssetInformation( "Resources.DescriptionItemImageOver" , "imageover", false, AssetsConstants.CATEGORY_IMAGE, AssetsController.FILTER_NONE )};
                    imageIconMap = new Dictionary<string, string>();
                    imageIconMap.Add("icon", "image");
                    if(!resources.existAsset("image"))
                        resources.addAsset("image", SpecialAssetPaths.ASSET_EMPTY_IMAGE);
                    if (!resources.existAsset("icon"))
                        resources.addAsset("icon", SpecialAssetPaths.ASSET_EMPTY_ICON);
                    break;
                case Controller.PLAYER:
                case Controller.NPC:
                    assetsInformation = new AssetInformation[] { new AssetInformation( "Resources.DescriptionCharacterAnimationStandUp" , "standup", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationStandDown" , "standdown", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ), 
                        //check if is 3rd or 1st person game to set the asset as necessary
                        new AssetInformation( "Resources.DescriptionCharacterAnimationStandRight" , "standright", Controller.Instance.PlayTransparent?false:true, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationStandLeft" , "standleft", Controller.Instance.PlayTransparent?false:true, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),

                        new AssetInformation( "Resources.DescriptionCharacterAnimationSpeakUp", "speakup", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationSpeakDown" , "speakdown", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationSpeakRight", "speakright", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationSpeakLeft" , "speakleft", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationUseRight" , "useright", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationUseLeft", "useleft", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationWalkUp", "walkup", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationWalkDown" , "walkdown", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationWalkRight" , "walkright", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ),
                        new AssetInformation( "Resources.DescriptionCharacterAnimationWalkLeft" , "walkleft", false, AssetsConstants.CATEGORY_ANIMATION, AssetsController.FILTER_PNG ) };
                    assetsGroups = new int[][] { new int[] { 0, 1, 2, 3 }, new int[] { 4, 5, 6, 7 }, new int[] { 8, 9 }, new int[] { 10, 11, 12, 13 } };
                    groupsInfo = new string[] { "Resources.StandingAnimations", "Resources.SpeakingAnimations", "Resources.UsingAnimations", "Resources.WalkingAnimations" };
                    
                    foreach (string asset in new string[]
                    {
                        "standup", "standdown", "standright", "standleft", "speakup", "speakdown", "speakright", "speakleft",
                        "useright", "useleft", "walkup", "walkdown", "walkright", "walkleft"
                    })
                    {
                        if (!resources.existAsset(asset))
                            resources.addAsset(asset, SpecialAssetPaths.ASSET_EMPTY_ANIMATION);
                    }

                    break;
                case Controller.ATREZZO:
                    assetsInformation = new AssetInformation[] { new AssetInformation("Resources.DescriptionItemImage", "image", true, AssetsConstants.CATEGORY_IMAGE, AssetsController.FILTER_NONE) };
                    if (!resources.existAsset("image"))
                        resources.addAsset("image", SpecialAssetPaths.ASSET_EMPTY_IMAGE);
                    break;
                case Controller.CONVERSATION_DIALOGUE_LINE:
                case Controller.CONVERSATION_OPTION_LINE:
                    assetsInformation = new AssetInformation[] {
                        new AssetInformation("Resources.DescriptionLineImage", "image", false, AssetsConstants.CATEGORY_IMAGE, AssetsController.FILTER_NONE),
                        new AssetInformation("Resources.DescriptionLineAudio", "audio", false, AssetsConstants.CATEGORY_AUDIO, AssetsController.FILTER_MP3),
                        new AssetInformation("Resources.DescriptionLineTTS", "tts", false, AssetsConstants.CATEGORY_BOOL, AssetsController.FILTER_NONE)
                    };
                    break;
            }

            // Create subcontrollers
            conditionsController = new ConditionsController(resources.getConditions(), Controller.RESOURCES, "");
        }

        public int getResourcesType()
        {

            return resourcesType;
        }

        /**
         * Returns the conditions microcontroller.
         * 
         * @return Conditions microcontroller
         */
        public ConditionsController getConditions()
        {

            return conditionsController;
        }

        /**
         * Returns the number of assets that the resources block has.
         * 
         * @return Number of assets
         */
        public int getAssetCount()
        {

            return assetsInformation.Length;
        }

        /**
         * Returns the name of the asset in the given position.
         * 
         * @param index
         *            Index of the asset
         * @return Name of the asset
         */
        public string getAssetName(int index)
        {
            return assetsInformation[index].name;
        }

        /**
         * Returns the description of the asset in the given position.
         * 
         * @param index
         *            Index of the asset
         * @return Description of the asset
         */
        public string getAssetDescription(int index)
        {
            return TC.get(assetsInformation[index].description);
        }

        /**
         * Returns the category of the asset in the given position.
         * 
         * @param index
         *            Index of the asset
         * @return Category of the asset
         */
        public int getAssetCategory(int index)
        {

            return assetsInformation[index].category;
        }

        /**
         * Returns the filter of the asset in the given position.
         * 
         * @param index
         *            Index of the asset
         * @return Filter of the asset
         */
        public int getAssetFilter(int index)
        {

            return assetsInformation[index].filter;
        }

        /**
         * Returns the relative path of the given asset.
         * 
         * @param asset
         *            Name of the asset
         * @return The path to the resource if present, null otherwise
         */
        public string getAssetPath(string asset)
        {

            return resources.getAssetPath(asset);
        }

        /**
         * Returns the relative path of the given asset (used for display).
         * 
         * @param index
         *            Index of the asset
         * @return The path to the resource if present, null otherwise
         */
        public string getAssetPath(int index)
        {

            return resources.getAssetPath(assetsInformation[index].name);
        }

        /**
         * Shows a dialog to choose a new path for the given asset.
         * 
         * @param index
         *            Index of the asset
         */
        public void editAssetPath(int index)
        {
            controller.AddTool(new SelectResourceTool(resources, assetsInformation, resourcesType, index));
        }

        /**
         * Deletes the path of the given asset.
         * 
         * @param index
         *            Index of the asset
         */
        public void deleteAssetPath(int index)
        {
            controller.AddTool(new DeleteResourceTool(resources, assetsInformation, index));
        }

        public override System.Object getContent()
        {

            return resources;
        }


        public override int[] getAddableElements()
        {

            return new int[] { };
        }


        public override bool canAddElement(int type)
        {

            return false;
        }


        public override bool canBeDeleted()
        {

            return true;
        }


        public override bool canBeMoved()
        {

            return true;
        }


        public override bool canBeRenamed()
        {

            return true;
        }


        public override bool addElement(int type, string id)
        {

            return false;
        }


        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {

            return false;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            return false;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            return false;
        }


        public override string renameElement(string name)
        {
            resources.setName(name);
            return name;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Update the flag summary with the conditions
            ConditionsController.updateVarFlagSummary(varFlagSummary, resources.getConditions());
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            if (assetsInformation == null)
            {
                return valid;
            }

            // Check the assets that are necessary
            foreach (AssetInformation assetInformation in assetsInformation)
            {
                // If the asset is necessary and the value is null, set to invalid
                if (assetInformation.assetNecessary && resources.getAssetPath(assetInformation.name) == null)
                {
                    valid = false;

                    // Store the incidence
                    if (incidences != null)
                    {
                        incidences.Add(currentPath + " >> " +
                                       TC.get("Operation.AdventureConsistencyErrorResources",
                                           assetInformation.name));
                    }
                }
            }

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Search in the types of the resources
            foreach (string type in resources.getAssetTypes())
                if (resources.getAssetPath(type).Equals(assetPath))
                    count++;

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Search in the assetsInformation
            foreach (var assetInfo in assetsInformation)
            {
                if (!string.IsNullOrEmpty(resources.getAssetPath(assetInfo.name)) && assetInfo.category != AssetsConstants.CATEGORY_BOOL)
                {
                    string assetPath = resources.getAssetPath(assetInfo.name);
                    int assetType = assetInfo.category;

                    // Search that the assetPath has not been previously added
                    bool add = true;
                    foreach (string asset in assetPaths)
                    {
                        if (asset.Equals(assetPath))
                        {
                            add = false;
                            break;
                        }
                    }
                    if (add)
                    {
                        int last = assetPaths.Count;
                        assetPaths.Insert(last, assetPath);
                        assetTypes.Insert(last, assetType);
                    }
                }
            }
        }


        public override void deleteAssetReferences(string assetPath)
        {
            controller.AddTool(new DeleteAssetReferencesInResources(resources, assetPath));
        }


        public override int countIdentifierReferences(string id)
        {

            return this.conditionsController.countIdentifierReferences(id);
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            conditionsController.replaceIdentifierReferences(oldId, newId);
        }


        public override void deleteIdentifierReferences(string id)
        {

            conditionsController.deleteIdentifierReferences(id);
        }


        public override bool canBeDuplicated()
        {

            return true;
        }

        /**
         * Method that is invoked only by the "Edit" button
         * 
         * @param filename
         * @param index
         */
        public void setAssetPath(string filename, string destinyAssetName, int index)
        {
            controller.AddTool(new EditResourceTool(resources, assetsInformation, index, filename, destinyAssetName));
        }


        public override void recursiveSearch()
        {

            // check(this.getConditions(), Language.GetText("Search.Conditions"));
            //for (int i = 0; i < this.getAssetCount(); i++)
            //{
            //    check(this.getAssetDescription(i), Language.GetText("Search.AssetDescription"));
            //    check(this.getAssetPath(i), Language.GetText("Search.AssetPath"));
            //    //Added v1.4
            //    if (this.getAssetPath(i) != null && this.getAssetPath(i).toLowerCase().endsWith(".eaa"))
            //    {
            //        Animation animation = Loader.loadAnimation(AssetsController.getInputStreamCreator(), this.getAssetPath(i), new EditorImageLoader());
            //        for (Frame frame:animation.getFrames())
            //        {
            //            check(frame.getUri(), Language.GetText("Search.AssetPath"));
            //            check(frame.getSoundUri(), Language.GetText("Search.AssetPath"));
            //        }
            //    }
            //}
        }

        public int getAssetGroupCount()
        {

            if (assetsGroups == null)
                return 1;
            else
                return assetsGroups.Length;
        }

        public string getGroupInfo(int i)
        {
            return TC.get(groupsInfo[i]);
        }

        public int getGroupAssetCount(int selectedIndex)
        {

            return assetsGroups[selectedIndex].Length;
        }

        public int getAssetIndex(int group, int asset)
        {

            if (assetsGroups == null || assetsGroups.Length == 0)
                return asset;
            return assetsGroups[group][asset];
        }

        public bool isIconFromImage(int i)
        {

            if (imageIconMap == null)
                return false;
            return imageIconMap[assetsInformation[i].name] != null;
        }

        public int getOriginalImage(int i)
        {

            string name = imageIconMap[assetsInformation[i].name];
            for (int j = 0; j < assetsInformation.Length; j++)
            {
                if (assetsInformation[j].name.Equals(name))
                    return j;
            }
            return -1;
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return null;
        }

        /**
         * This method creates the frames of the animation from the images belonging
         * to the previous animation format.
         * 
         * @param assetPath
         *            The path to the previous animation
         */
        public static void framesFromImages(Animation animation, string assetPath)
        {
            framesFromImages(animation, assetPath, false);

        }

        /**
         * This method creates the frames of the animation from the images belonging
         * to the previous animation format. 
         * 
         * @param assetPath
         *            The path to the previous animation
         * @param changeFormat
         *            Change the frame creation setting the time per frame and wait user interaction.
         */
        public static void framesFromImages(Animation animation, string assetPath, bool changeFormat)
        {

            //animation.getFrames().Clear();
            //animation.getTransitions().Clear();
            //animation.getTransitions().Add(new Transition());
            //animation.getTransitions().Add(new Transition());

            //int i = 1;
            //Sprite currentSlide = null;
            //bool end = false;

            //while (!end)
            //{
            //    string file = assetPath + "_" + leadingZeros(i) + ".jpg";
            //    currentSlide = AssetsController.getImage(file);
            //    if (currentSlide == null)
            //    {
            //        file = assetPath + "_" + leadingZeros(i) + ".png";
            //        currentSlide = AssetsController.getImage(file);
            //    }
            //    if (currentSlide == null)
            //        end = true;
            //    else {
            //        if (!changeFormat)
            //            animation.addFrame(-1, new Frame(animation.getImageLoaderFactory(), file));
            //        else
            //            animation.addFrame(-1, new Frame(animation.getImageLoaderFactory(), file, 100, true));
            //    }
            //    i++;
            //}
        }

        /**
         * @param n
         *            number to convert to a string
         * @return a 2 character string with value n
         */
        private static string leadingZeros(int n)
        {

            string s;
            if (n < 10)
                s = "0";
            else
                s = "";
            s = s + n;
            return s;
        }

        public string getName()
        {
            return this.resources.getName();
        }

        /**
         * This method change the assets path without using a tool for it. That is necessary when old animations are changed at LO generation
         */
        public void changeAssetPath(int index, string path)
        {
            resources.deleteAsset(assetsInformation[index].name);
            resources.addAsset(assetsInformation[index].name, path);
        }

        /**
         * Adding resource without using tool. 
         */
        public void addAsset(string name, string path)
        {
            // used to convert old animation at LO export time
            resources.addAsset(name, path);
        }


        public AssetInformation[] getAssetsInformation()
        {

            return assetsInformation;
        }
    }
}