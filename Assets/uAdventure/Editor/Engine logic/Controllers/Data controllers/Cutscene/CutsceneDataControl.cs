using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using uAdventure.Core;
using System;
using Xasu.HighLevel;

namespace uAdventure.Editor
{
    public class CutsceneDataControl : DataControlWithResources, IxAPIConfigurable
    {
        /**
           * Contained cutscene data.
           */
        private Cutscene cutscene;

        /**
         * Holds the type of the cutscene.
         */
        private int cutsceneType;

        /**
         * Constructor.
         * 
         * @param cutscene
         *            Contained cutscene data
         */
        public CutsceneDataControl(Cutscene cutscene)
        {

            this.cutscene = cutscene;
            this.resourcesList = cutscene.getResources();

            switch (cutscene.getType())
            {
                case GeneralScene.GeneralSceneSceneType.SLIDESCENE:
                    cutsceneType = Controller.CUTSCENE_SLIDES;
                    break;
                case GeneralScene.GeneralSceneSceneType.VIDEOSCENE:
                    cutsceneType = Controller.CUTSCENE_VIDEO;
                    break;
            }

            selectedResources = 0;

            // Add a new resource if the list is empty
            if (resourcesList.Count == 0)
            {
                resourcesList.Add(new ResourcesUni());
            }

            // Create the subcontrollers
            resourcesDataControlList = new List<ResourcesDataControl>();
            foreach (ResourcesUni resources in resourcesList)
            {
                resourcesDataControlList.Add(new ResourcesDataControl(resources, cutsceneType));
            }

            xApiOptions = new Dictionary<string, List<string>>();

            var accessibleOptions = Enum.GetValues(typeof(AccessibleTracker.AccessibleType))
                .Cast<AccessibleTracker.AccessibleType>()
                .Select(v => v.ToString().ToLower())
                .ToList();

            xApiOptions.Add("accesible", accessibleOptions);

            var alternativeOptions = Enum.GetValues(typeof(AlternativeTracker.AlternativeType))
                .Cast<AlternativeTracker.AlternativeType>()
                .Select(v => v.ToString().ToLower())
                .ToList();

            xApiOptions.Add("alternative", alternativeOptions);
        }

        /**
         * Returns the type of the contained cutscene.
         * 
         * @return Type of the contained cutscene
         */
        public int getType()
        {

            return cutsceneType;
        }

        /**
         * Returns the id of the contained cutscene.
         * 
         * @return If of the contained cutscene
         */
        public string getId()
        {

            return cutscene.getId();
        }

        /**
         * Returns the documentation of the scene.
         * 
         * @return Cutscene's documentation
         */
        public string getDocumentation()
        {

            return cutscene.getDocumentation();
        }

        /**
         * Returns the name of the cutscene.
         * 
         * @return Cutscene's name
         */
        public string getName()
        {

            return cutscene.getName();
        }

        /**
         * Sets the new documentation of the cutscene.
         * 
         * @param documentation
         *            Documentation of the cutscene
         */
        public void setDocumentation(string documentation)
        {

            controller.AddTool(new ChangeDocumentationTool(cutscene, documentation));
        }

        /**
         * Sets the new name of the cutscene.
         * 
         * @param name
         *            Name of the cutscene
         */
        public void setName(string name)
        {

            controller.AddTool(new ChangeNameTool(cutscene, name));
        }

        public string getTargetId()
        {

            return cutscene.getTargetId();
        }

        public void setTargetId(string targetId)
        {

            cutscene.setTargetId(targetId);
        }


        public override System.Object getContent()
        {

            return cutscene;
        }


        public override int[] getAddableElements()
        {

            return new int[] { Controller.RESOURCES, Controller.NEXT_SCENE, Controller.END_SCENE };
        }


        public override bool canAddElement(int type)
        {
            switch (type)
            {
                case Controller.RESOURCES: return true;
            }

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


        public override bool addElement(int type, string selectedScene)
        {

            bool elementAdded = false;

            // If the element is a resources block
            if (type == Controller.RESOURCES)
            {
                elementAdded = Controller.Instance.AddTool(new AddResourcesBlockTool(resourcesList, resourcesDataControlList, cutsceneType, this));
            }

            return elementAdded;
        }


        public override bool moveElementUp(DataControl dataControl)
        {

            bool elementMoved = false;

            // If the element to move is a resources block
            if (resourcesList.Contains((ResourcesUni)dataControl.getContent()))
            {
                int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

                if (elementIndex > 0)
                {
                    ResourcesUni r = resourcesList[elementIndex];
                    resourcesList.RemoveAt(elementIndex);
                    resourcesList.Insert(elementIndex - 1, r);

                    ResourcesDataControl d = resourcesDataControlList[elementIndex];
                    resourcesDataControlList.RemoveAt(elementIndex);
                    resourcesDataControlList.Insert(elementIndex - 1, d);
                    controller.DataModified();
                    elementMoved = true;
                }
            }

            return elementMoved;
        }


        public override bool moveElementDown(DataControl dataControl)
        {

            bool elementMoved = false;

            // If the element to move is a resources block
            if (resourcesList.Contains((ResourcesUni)dataControl.getContent()))
            {
                int elementIndex = resourcesList.IndexOf((ResourcesUni)dataControl.getContent());

                if (elementIndex < resourcesList.Count - 1)
                {
                    ResourcesUni r = resourcesList[elementIndex];
                    resourcesList.RemoveAt(elementIndex);
                    resourcesList.Insert(elementIndex + 1, r);

                    ResourcesDataControl d = resourcesDataControlList[elementIndex];
                    resourcesDataControlList.RemoveAt(elementIndex);
                    resourcesDataControlList.Insert(elementIndex + 1, d);
                    controller.DataModified();
                    elementMoved = true;
                }
            }

            return elementMoved;
        }


        public override string renameElement(string name)
        {
            string oldCutsceneId = cutscene.getId();
            string references = controller.countIdentifierReferences(oldCutsceneId).ToString();

            // Ask for confirmation
            if (name != null || controller.ShowStrictConfirmDialog(TC.get("Operation.RenameCutsceneTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldCutsceneId, references })))
            {

                // Show a dialog asking for the new cutscnee id
                string newCutsceneId = name;
                if (name == null)
                    controller.ShowInputDialog(TC.get("Operation.RenameCutsceneTitle"), TC.get("Operation.RenameCutsceneMessage"), oldCutsceneId, (o,s) => performRenameElement(s));
                else
                {
                    controller.DataModified();
                    return performRenameElement(newCutsceneId);
                }
            }
                
			return null;
        }

        private string performRenameElement(string newCutsceneId)
        {
            string oldCutsceneId = cutscene.getId();

            // If some value was typed and the identifiers are different
            if (!controller.isElementIdValid(newCutsceneId))
                newCutsceneId = controller.makeElementValid(newCutsceneId);

            cutscene.setId(newCutsceneId);
            controller.replaceIdentifierReferences(oldCutsceneId, newCutsceneId);
            controller.IdentifierSummary.deleteId<Cutscene>(oldCutsceneId);
            controller.IdentifierSummary.addId<Cutscene>(newCutsceneId);
            return newCutsceneId;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // Iterate through each next scene
            EffectsController.updateVarFlagSummary(varFlagSummary, cutscene.getEffects());
            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                resourcesDataControl.updateVarFlagSummary(varFlagSummary);
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Iterate through the resources
            for (int i = 0; i < resourcesDataControlList.Count; i++)
            {
                string resourcesPath = currentPath + " >> " + Controller.RESOURCES + " #" + (i + 1);
                valid &= resourcesDataControlList[i].isValid(resourcesPath, incidences);
            }

            valid &= EffectsController.isValid(currentPath + " >> " + TC.get("Element.Effects"), incidences, cutscene.getEffects());

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                count += resourcesDataControl.countAssetReferences(assetPath);

            count += EffectsController.countAssetReferences(assetPath, cutscene.getEffects());

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                resourcesDataControl.getAssetReferences(assetPaths, assetTypes);

            EffectsController.getAssetReferences(assetPaths, assetTypes, cutscene.getEffects());

        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Iterate through the resources
            foreach (ResourcesDataControl resourcesDataControl in resourcesDataControlList)
                resourcesDataControl.deleteAssetReferences(assetPath);

            EffectsController.deleteAssetReferences(assetPath, cutscene.getEffects());
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            if (cutscene.getTargetId() != null)
            {
                if (cutscene.getTargetId().Equals(id))
                    count++;
            }

            count += EffectsController.countIdentifierReferences(id, cutscene.getEffects());

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            if (cutscene.getNext() == Cutscene.NEWSCENE && cutscene.getTargetId().Equals(oldId))
                cutscene.setTargetId(newId);

            EffectsController.replaceIdentifierReferences(oldId, newId, cutscene.getEffects());
        }


        public override void deleteIdentifierReferences(string id)
        {

            EffectsController.deleteIdentifierReferences(id, cutscene.getEffects());
            if (cutscene.getNext() == Cutscene.NEWSCENE && cutscene.getTargetId().Equals(id))
                cutscene.setNext(Cutscene.GOBACK);
        }


        public override bool canBeDuplicated()
        {

            return true;
        }


        public override void recursiveSearch()
        {

            //check(this.getId(), "ID");
            //check(this.getDocumentation(), Language.GetText("Search.Documentation"));
            //check(this.getName(), Language.GetText("Search.Name"));
            //check(this.getTargetId(), Language.GetText("Search.NextScene"));

            //EffectsController effecContr = new EffectsController(cutscene.getEffects());
            //for (int i = 0; i < this.getEffects().getEffectCount(); i++)
            //{
            //    check(effecContr.getEffectInfo(i), Language.GetText("Search.Effect"));
            //    check(effecContr.getConditionController(i), Language.GetText("Search.Conditions"));
            //}
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            return getPathFromChild(dataControl, resourcesDataControlList.Cast<System.Object>().ToList());
        }

        /**
         * Returns the path to the selected preview image.
         * 
         * @return Path to the image, null if not present
         */
        public string getPreviewImage()
        {
            if (cutsceneType == Controller.CUTSCENE_SLIDES)
            {
                string previewAnimationPath = resourcesDataControlList[selectedResources].getAssetPath("slides");
                if (!string.IsNullOrEmpty(previewAnimationPath) && !previewAnimationPath.EndsWith(SpecialAssetPaths.ASSET_EMPTY_ANIMATION))
                {
                    var incidences = new List<Incidence>();
                    var animation = Loader.LoadAnimation(previewAnimationPath, Controller.ResourceManager, incidences);
                    return animation != null && animation.getFrames().Count > 0 ? animation.getFrame(0).getUri() : null;
                }
                else
                {
                    return SpecialAssetPaths.ASSET_EMPTY_BACKGROUND;
                }
            }
            else
            {
                return "img/icons/video.png";
            }
        }

        public string getPathToSlides()
        {
            return resourcesDataControlList[selectedResources].getAssetPath("slides");
        }

        public void setPathToSlides(string path)
        {
            resourcesDataControlList[selectedResources].addAsset("slides", path);
        }

        public string getPathToVideo()
        {
            return resourcesDataControlList[selectedResources].getAssetPath("video");
        }

        public void setPathToVideo(string path)
        {
            resourcesDataControlList[selectedResources].addAsset("video", path);
        }

        internal string getPathToMusic()
        {
            return resourcesDataControlList[selectedResources].getAssetPath("music");
        }

        public void setPathToMusic(string path)
        {
            resourcesDataControlList[selectedResources].addAsset("music", path);
        }


        public int getNext()
        {

            return cutscene.getNext();
        }

        public void setNext(int next)
        {
			var chapterTargets = controller.IdentifierSummary .groupIds<IChapterTarget> ();
            Controller.Instance.AddTool(new ChangeIntegerValueTool(cutscene, next, "getNext", "setNext"));
            if (cutscene.getTargetId() == null || cutscene.getTargetId().Equals(""))
			{
				if(chapterTargets.Length > 0)
					cutscene.setTargetId(chapterTargets[0]);
            }
            else
            {
                bool exists = false;
				for (int i = 0; i < chapterTargets.Length; i++)
                {
					if (chapterTargets[i].Equals(cutscene.getTargetId()))
                        exists = true;
                }
                if (!exists)
					cutscene.setTargetId(chapterTargets[0]);
            }
        }

        public string getNextSceneId()
        {

            return cutscene.getTargetId();
        }

        public void setNextSceneId(string targetId)
        {

            cutscene.setTargetId(targetId);
        }

        public bool hasDestinyPosition()
        {

            return cutscene.hasPlayerPosition();
        }

        public TransitionType getTransitionType()
        {

            return cutscene.getTransitionType();
        }

        public int getTransitionTime()
        {

            return cutscene.getTransitionTime();
        }

        public void setTransitionTime(int value)
        {

            Controller.Instance.AddTool(new ChangeIntegerValueTool(cutscene, value, "getTransitionTime", "setTransitionTime"));
        }

        public void setTransitionType(TransitionType value)
        {

            Controller.Instance.AddTool(ChangeEnumValueTool.Create(cutscene, value, "getTransitionType", "setTransitionType"));
        }

        /**
         * Toggles the destiny position. If the next scene has a destiny position
         * deletes it, if it doesn't have one, set initial values for it.
         */
        public void toggleDestinyPosition()
        {

            if (cutscene.hasPlayerPosition())
                controller.AddTool(new ChangeNSDestinyPositionTool(cutscene, int.MinValue, int.MinValue));
            else
                controller.AddTool(new ChangeNSDestinyPositionTool(cutscene, AssetsImageDimensions.BACKGROUND_MAX_WIDTH / 2, AssetsImageDimensions.BACKGROUND_MAX_HEIGHT / 2));
        }

        /**
         * Returns the X coordinate of the destiny position
         * 
         * @return X coordinate of the destiny position
         */
        public int getDestinyPositionX()
        {

            return cutscene.getPositionX();
        }

        /**
         * Returns the Y coordinate of the destiny position
         * 
         * @return Y coordinate of the destiny position
         */
        public int getDestinyPositionY()
        {

            return cutscene.getPositionY();
        }

        /**
         * Sets the new destiny position of the next scene.
         * 
         * @param positionX
         *            X coordinate of the destiny position
         * @param positionY
         *            Y coordinate of the destiny position
         */
        public void setDestinyPosition(int positionX, int positionY)
        {

            controller.AddTool(new ChangeNSDestinyPositionTool(cutscene, positionX, positionY));
        }

        public EffectsController getEffects()
        {

            return new EffectsController(cutscene.getEffects());
        }

        public bool isVideoscene()
        {
            return (cutscene is Videoscene);
        }

        /**
         * Identify if the videoscene can be skiped. IMPORTANT!! this method only will check if can be skipped if the cutscene is a video scene, if not, 
         * it always return false
         * 
         * @return
         */
        public bool getCanSkip()
        {
            if (isVideoscene())
                return ((Videoscene)cutscene).isCanSkip();
            else
                return false;
        }

        public void setCanSkip(bool canSkip)
        {
            if (isVideoscene())
            {
                ((Videoscene) cutscene).setCanSkip(canSkip);
            }
        }



        private readonly Dictionary<string, List<string>> xApiOptions;

        public List<string> getxAPIValidTypes(string @class)
        {
            return xApiOptions[@class];
        }

        public List<string> getxAPIValidClasses()
        {
            return xApiOptions.Keys.ToList();
        }

        public string getxAPIType()
        {
            return cutscene.getXApiType();
        }

        public string getxAPIClass()
        {
            return cutscene.getXApiClass();
        }

        public void setxAPIType(string type)
        {
            if (!xApiOptions.ContainsKey(getxAPIClass()) || !xApiOptions[getxAPIClass()].Contains(type))
            {
                return;
            }

            controller.AddTool(new ChangeStringValueTool(cutscene, type, "getXApiType", "setXApiType"));
        }

        public void setxAPIClass(string @class)
        {
            if (!xApiOptions.ContainsKey(@class))
            {
                return;
            }

            controller.AddTool(new ChangeStringValueTool(cutscene, @class, "getXApiClass", "setXApiClass"));
        }
    }
}