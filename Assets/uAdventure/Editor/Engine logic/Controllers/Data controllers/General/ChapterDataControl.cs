using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;
using System.Linq;

namespace uAdventure.Editor
{
    /**
     * Controller for the main element of the script.
     * 
     */
    public class ChapterDataControl : DataControl
    {


        /**
         * Chapter data contained.
         */
        private Chapter chapter;

        /**
         * Scenes list data controller.
         */
        private ScenesListDataControl scenesListDataControl;

        /**
         * Cutscenes list data controller.
         */
        private CutscenesListDataControl cutscenesListDataControl;

        /**
         * Books list data controller.
         */
        private BooksListDataControl booksListDataControl;

        /**
         * Items list data controller.
         */
        private ItemsListDataControl itemsListDataControl;

        /**
         * Atrezzo items list data controller.
         */
        private AtrezzoListDataControl atrezzoListDataControl;

        /**
         * Player data controller.
         */
        private PlayerDataControl playerDataControl;

        /**
         * NPCs list data controller.
         */
        private NPCsListDataControl npcsListDataControl;

        /**
         * Conversations list data controller.
         */
        private ConversationsListDataControl conversationsListDataControl;

        private List<DataControl> extraDataControls = new List<DataControl>();

        public void RegisterExtraDataControl(DataControl dataControl)
        {
            extraDataControls.Add(dataControl);
        }

        /**
         * Assessment file data controller
         */
        // private AssessmentProfilesDataControl assessmentProfilesDataControl;

        /**
         * Adaptation file data controller
         */
        // private AdaptationProfilesDataControl adaptationProfilesDataControl;

        /**
         * Advanced features data controller (timers, global states and macros)
         */
        private AdvancedFeaturesDataControl advancedFeaturesDataControl;

        /**
         * Constructor.
         * 
         * @param chapter
         *            Contained chapter data
         */
        public ChapterDataControl(Chapter chapter)
        {

            update(chapter);
        }

        /**
         * Updates the data contained in the data control with a new chapter. This
         * method is essential for some undo/redo tools
         * 
         * @param chaper
         */
        public void update(Chapter chapter)
        {

            this.chapter = chapter;

            // Create the subcontrollers
            playerDataControl = new PlayerDataControl(chapter.getPlayer());
			scenesListDataControl = new ScenesListDataControl(chapter.getObjects<Scene> (), this.getPlayer().getPreviewImage());
			cutscenesListDataControl = new CutscenesListDataControl(chapter.getObjects<Cutscene> ());
			booksListDataControl = new BooksListDataControl(chapter.getObjects<Book> ());
			itemsListDataControl = new ItemsListDataControl(chapter.getObjects<Item> ());
			atrezzoListDataControl = new AtrezzoListDataControl(chapter.getObjects<Atrezzo> ());
			npcsListDataControl = new NPCsListDataControl(chapter.getObjects<NPC> ());
			conversationsListDataControl = new ConversationsListDataControl(chapter.getObjects<Conversation> ());
			TimersListDataControl timersListDataControl = new TimersListDataControl(chapter.getObjects<Timer> ());
			GlobalStateListDataControl globalStatesListDataControl = new GlobalStateListDataControl(chapter.getObjects<GlobalState> ());
			MacroListDataControl macrosListDataControl = new MacroListDataControl(chapter.getObjects<Macro> ());
            advancedFeaturesDataControl = new AdvancedFeaturesDataControl();
            advancedFeaturesDataControl.setTimerListDataControl(timersListDataControl);
            advancedFeaturesDataControl.setGlobalStatesListDataContorl(globalStatesListDataControl);
            advancedFeaturesDataControl.setMacrosListDataControl(macrosListDataControl);
            // assessmentProfilesDataControl = new AssessmentProfilesDataControl(chapter.getAssessmentProfiles());
            //adaptationProfilesDataControl = new AdaptationProfilesDataControl(chapter.getAdaptationProfiles());
        }

        /**
         * Returns the title of the chapter.
         * 
         * @return Chapter's title
         */
        public string getTitle()
        {

            return chapter.getTitle();
        }

        /**
         * Returns the description of the chapter.
         * 
         * @return Chapter's description
         */
        public string getDescription()
        {

            return chapter.getDescription();
        }

        /**
         * Returns the name to the assessment profile of the chapter.
         * 
         * @return Name to the assessment profile of the chapter
         */
        public string getAssessmentName()
        {

            return chapter.getAssessmentName();
        }

        /**
         * Returns the name to the adaptation profile of the chapter.
         * 
         * @return Name to the adaptation profile of the chapter
         */
        public string getAdaptationName()
        {

            return chapter.getAdaptationName();
        }

        /**
         * Returns the initial scene identifier of the chapter.
         * 
         * @return Initial scene identifier
         */
        public string getInitialScene()
        {

            return chapter.getTargetId();
        }

        /**
         * Returns the scenes list controller.
         * 
         * @return Scenes list controller
         */
        public ScenesListDataControl getScenesList()
        {

            return scenesListDataControl;
        }

        /**
         * Returns the cutscenes list controller.
         * 
         * @return Cutscenes list controller
         */
        public CutscenesListDataControl getCutscenesList()
        {

            return cutscenesListDataControl;
        }

        /**
         * Returns the books list controller.
         * 
         * @return Books list controller
         */
        public BooksListDataControl getBooksList()
        {

            return booksListDataControl;
        }

        /**
         * Returns the items list controller.
         * 
         * @return Items list controller
         */
        public ItemsListDataControl getItemsList()
        {

            return itemsListDataControl;
        }

        /**
         * Returns the atrezzo items list controller.
         * 
         * @return Atrezzo list controller
         */
        public AtrezzoListDataControl getAtrezzoList()
        {

            return atrezzoListDataControl;
        }

        /**
         * Returns the player controller.
         * 
         * @return Player controller
         */
        public PlayerDataControl getPlayer()
        {

            return playerDataControl;
        }

        /**
         * Returns the NPCs list controller.
         * 
         * @return NPCs list controller
         */
        public NPCsListDataControl getNPCsList()
        {

            return npcsListDataControl;
        }

        /**
         * Returns the conversations list controller.
         * 
         * @return Conversations list controller
         */
        public ConversationsListDataControl getConversationsList()
        {

            return conversationsListDataControl;
        }

        /**
         * Returns the list of timers controller
         * 
         * @return Timers list controller
         */
        public TimersListDataControl getTimersList()
        {

            return this.advancedFeaturesDataControl.getTimersList();
        }

        /**
         * Sets the new title of the chapter.
         * 
         * @param title
         *            Title of the chapter
         */
        public void setTitle(string title)
        {

            ChangeTitleTool tool = new ChangeTitleTool(chapter, title);
            controller.AddTool(tool);
        }

        /**
         * Sets the new description of the chapter.
         * 
         * @param description
         *            Description of the chapter
         */
        public void setDescription(string description)
        {

            ChangeDescriptionTool tool = new ChangeDescriptionTool(chapter, description);
            controller.AddTool(tool);
        }

        /**
         * Sets the new assessment file for the chapter, showing a dialog to the
         * user.
         */
        //public void setAssessmentPath(string profileName)
        //{
        //    Controller.getInstance().addTool(new ChangeSelectedProfileTool(chapter, ChangeSelectedProfileTool.MODE_ASSESSMENT, profileName));
        //}

        /**
         * Sets the new adaptation file for the chapter, showing a dialog to the
         * user.
         * @param profileName 
         */
        //public void setAdaptationPath(string profileName)
        //{
        //    Controller.getInstance().addTool(new ChangeSelectedProfileTool(chapter, ChangeSelectedProfileTool.MODE_ADAPTATION, profileName));
        //}

        /**
         * Sets the new initial scene identifier for the chapter.
         * 
         * @param initialScene
         *            Initial scene identifier
         */
        public void setInitialScene(string initialScene)
        {

            Controller.Instance.AddTool(new ChangeTargetIdTool(chapter, initialScene));
        }

        /**
         * Deletes the assessment file reference of the chapter.
         */
        //public void deleteAssessmentPath()
        //{

        //    Controller.getInstance().addTool(new SetNoSelectedProfileTool(chapter, SetNoSelectedProfileTool.MODE_ASSESSMENT));
        //}

        /**
         * Deletes the adaptation file reference of the chapter.
         */
        //public void deleteAdaptationPath()
        //{

        //    Controller.getInstance().addTool(new SetNoSelectedProfileTool(chapter, SetNoSelectedProfileTool.MODE_ADAPTATION));
        //}

        public override System.Object getContent()
        {

            return chapter;
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

            return false;
        }


        public override bool canBeMoved()
        {

            return false;
        }


        public override bool canBeRenamed()
        {

            return false;
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

            return null;
        }


        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {

            // First of all, clear the summary
            varFlagSummary.clearReferences();

            // Update the summary with the elements
            scenesListDataControl.updateVarFlagSummary(varFlagSummary);
            cutscenesListDataControl.updateVarFlagSummary(varFlagSummary);
            itemsListDataControl.updateVarFlagSummary(varFlagSummary);
            atrezzoListDataControl.updateVarFlagSummary(varFlagSummary);
            npcsListDataControl.updateVarFlagSummary(varFlagSummary);
            playerDataControl.updateVarFlagSummary(varFlagSummary);
            conversationsListDataControl.updateVarFlagSummary(varFlagSummary);
            advancedFeaturesDataControl.updateVarFlagSummary(varFlagSummary);
            extraDataControls.ForEach(d => d.updateVarFlagSummary(varFlagSummary));
        }


        public override bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;

            // Set the current path
            currentPath = getTitle();
            string playerPath = currentPath + " >> " + Controller.PLAYER;

            // Spread the call to the rest of te elements
            valid &= scenesListDataControl.isValid(currentPath, incidences);
            valid &= cutscenesListDataControl.isValid(currentPath, incidences);
            valid &= booksListDataControl.isValid(currentPath, incidences);
            valid &= itemsListDataControl.isValid(currentPath, incidences);
            valid &= atrezzoListDataControl.isValid(currentPath, incidences);
            valid &= playerDataControl.isValid(playerPath, incidences);
            valid &= npcsListDataControl.isValid(currentPath, incidences);
            valid &= conversationsListDataControl.isValid(currentPath, incidences);
            valid &= advancedFeaturesDataControl.isValid(currentPath, incidences);
            valid &= extraDataControls.All(d => d.isValid(currentPath, incidences));

            return valid;
        }


        public override int countAssetReferences(string assetPath)
        {

            int count = 0;
            // Add the references from the elements
            count += scenesListDataControl.countAssetReferences(assetPath);
            count += cutscenesListDataControl.countAssetReferences(assetPath);
            count += booksListDataControl.countAssetReferences(assetPath);
            count += itemsListDataControl.countAssetReferences(assetPath);
            count += atrezzoListDataControl.countAssetReferences(assetPath);
            count += playerDataControl.countAssetReferences(assetPath);
            count += npcsListDataControl.countAssetReferences(assetPath);
            count += conversationsListDataControl.countAssetReferences(assetPath);
            count += advancedFeaturesDataControl.countAssetReferences(assetPath);
            count += extraDataControls.Sum(d => d.countAssetReferences(assetPath));

            return count;
        }


        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            scenesListDataControl.getAssetReferences(assetPaths, assetTypes);
            cutscenesListDataControl.getAssetReferences(assetPaths, assetTypes);
            booksListDataControl.getAssetReferences(assetPaths, assetTypes);
            itemsListDataControl.getAssetReferences(assetPaths, assetTypes);
            atrezzoListDataControl.getAssetReferences(assetPaths, assetTypes);
            playerDataControl.getAssetReferences(assetPaths, assetTypes);
            npcsListDataControl.getAssetReferences(assetPaths, assetTypes);
            conversationsListDataControl.getAssetReferences(assetPaths, assetTypes);
            advancedFeaturesDataControl.getAssetReferences(assetPaths, assetTypes);
            extraDataControls.ForEach(d => d.getAssetReferences(assetPaths, assetTypes));
        }


        public override void deleteAssetReferences(string assetPath)
        {

            // Delete the asset references in the chapter
            scenesListDataControl.deleteAssetReferences(assetPath);
            cutscenesListDataControl.deleteAssetReferences(assetPath);
            booksListDataControl.deleteAssetReferences(assetPath);
            itemsListDataControl.deleteAssetReferences(assetPath);
            atrezzoListDataControl.deleteAssetReferences(assetPath);
            playerDataControl.deleteAssetReferences(assetPath);
            npcsListDataControl.deleteAssetReferences(assetPath);
            conversationsListDataControl.deleteAssetReferences(assetPath);
            advancedFeaturesDataControl.deleteAssetReferences(assetPath);
            extraDataControls.ForEach(d => d.deleteAssetReferences(assetPath));
        }


        public override int countIdentifierReferences(string id)
        {

            int count = 0;

            // Count the initial scene
            if (chapter.getTargetId().Equals(id))
                count++;

            // Spread the call to the rest of the elements
            count += scenesListDataControl.countIdentifierReferences(id);
            count += cutscenesListDataControl.countIdentifierReferences(id);
            count += itemsListDataControl.countIdentifierReferences(id);
            count += atrezzoListDataControl.countIdentifierReferences(id);
            count += npcsListDataControl.countIdentifierReferences(id);
            count += conversationsListDataControl.countIdentifierReferences(id);
            count += advancedFeaturesDataControl.countIdentifierReferences(id);
            count += extraDataControls.Sum(d => d.countIdentifierReferences(id));

            return count;
        }


        public override void replaceIdentifierReferences(string oldId, string newId)
        {

            // If the initial scene identifier has changed, update it
            if (chapter.getTargetId().Equals(oldId))
                chapter.setTargetId(newId);

            // Spread the call to the rest of the elements
            scenesListDataControl.replaceIdentifierReferences(oldId, newId);
            cutscenesListDataControl.replaceIdentifierReferences(oldId, newId);
            itemsListDataControl.replaceIdentifierReferences(oldId, newId);
            atrezzoListDataControl.replaceIdentifierReferences(oldId, newId);
            npcsListDataControl.replaceIdentifierReferences(oldId, newId);
            conversationsListDataControl.replaceIdentifierReferences(oldId, newId);
            advancedFeaturesDataControl.replaceIdentifierReferences(oldId, newId);
            extraDataControls.ForEach(d => d.replaceIdentifierReferences(oldId, newId));
        }


        public override void deleteIdentifierReferences(string id)
        {

            // If the initial scene has been deleted, change the value to the first one in the scenes list
			if (chapter.getTargetId ().Equals (id)) {
				var newTarget = (IChapterTarget)controller.SelectedChapterDataControl .getObjects ().Find (o => o is IChapterTarget);
				if(newTarget!= null)
					chapter.setTargetId(newTarget.getId ());
			}
            // Spread the call to the rest of the elements
            scenesListDataControl.deleteIdentifierReferences(id);
            cutscenesListDataControl.deleteIdentifierReferences(id);
            itemsListDataControl.deleteIdentifierReferences(id);
            atrezzoListDataControl.deleteIdentifierReferences(id);
            npcsListDataControl.deleteIdentifierReferences(id);
            conversationsListDataControl.deleteIdentifierReferences(id);
            advancedFeaturesDataControl.deleteIdentifierReferences(id);
            extraDataControls.ForEach(d => d.deleteIdentifierReferences(id));
        }


        public override bool canBeDuplicated()
        {

            return true;
        }

        /**
         * @return the globalStatesListDataControl
         */
        public GlobalStateListDataControl getGlobalStatesListDataControl()
        {

            return advancedFeaturesDataControl.getGlobalStatesListDataControl();
        }

        /**
         * @return the globalStatesListDataControl
         */
        public MacroListDataControl getMacrosListDataControl()
        {

            return advancedFeaturesDataControl.getMacrosListDataControl();
        }

        public List<object> getObjects()
        {
            return chapter.getObjects();
        }

        public List<T> getObjects<T>()
        {
            return chapter.getObjects<T>();
        }

        public override void recursiveSearch()
        {

            check(this.getAdaptationName(), Language.GetText("Search.AdaptationPath"));
            check(this.getAssessmentName(), Language.GetText("Search.AssessmentPath"));
            check(this.getDescription(), Language.GetText("Search.Description"));
            check(this.getInitialScene(), Language.GetText("Search.InitialScene"));
            check(this.getTitle(), Language.GetText("Search.Title"));
            this.getAtrezzoList().recursiveSearch();
            this.getBooksList().recursiveSearch();
            this.getConversationsList().recursiveSearch();
            this.getCutscenesList().recursiveSearch();
            this.getItemsList().recursiveSearch();
            this.getNPCsList().recursiveSearch();
            this.getPlayer().recursiveSearch();
            this.getScenesList().recursiveSearch();
            extraDataControls.ForEach(d => d.recursiveSearch());
        }

        /**
         * Returns the assessment profile that is actually selected
         * 
         * @return
         */
        //public AssessmentProfileDataControl getSelectedAssessmentProfile()
        //{

        //    return assessmentProfilesDataControl.getProfileByPath(chapter.getAssessmentName());
        //}

        /**
         * Returns the adaptation profile that is actually selected
         * 
         * @return
         */
        //public AdaptationProfileDataControl getSelectedAdaptationProfile()
        //{

        //    return adaptationProfilesDataControl.getProfileByPath(chapter.getAdaptationName());
        //}

        /**
         * @return the assessmentProfilesDataControl
         */
        //public AssessmentProfilesDataControl getAssessmentProfilesDataControl()
        //{

        //    return assessmentProfilesDataControl;
        //}

        /**
         * @return the adaptationProfilesDataControl
         */
        //public AdaptationProfilesDataControl getAdaptationProfilesDataControl()
        //{

        //    return adaptationProfilesDataControl;
        //}

        /**
         * Check if chapter has adaptation profile
         * 
         * @return
         */
        public bool hasAdaptationProfile()
        {

            return chapter.hasAdaptationProfile();
        }

        /**
         * Check if chapter has assessment profile
         * 
         * @return
         */
        public bool hasAssessmentProfile()
        {

            return chapter.hasAssessmentProfile();
        }


        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {

            List<Searchable> path;
            path = getPathFromChild(dataControl, scenesListDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, cutscenesListDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, booksListDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, itemsListDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, atrezzoListDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, npcsListDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, playerDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, conversationsListDataControl);
            if (path != null)
                return path;
            path = getPathFromChild(dataControl, advancedFeaturesDataControl);
            if (path != null)
                return path;

            foreach (var extraDataControl in extraDataControls)
            {
                path = getPathFromChild(dataControl, extraDataControl);
                if (path != null)
                    return path;
            }
            //path = getPathFromChild(dataControl, assessmentProfilesDataControl);
            //if (path != null)
            //    return path;
            //path = getPathFromChild(dataControl, adaptationProfilesDataControl);
            //if (path != null)
            //    return path;
            return null;
        }

        public AdvancedFeaturesDataControl getAdvancedFeaturesController()
        {

            return this.advancedFeaturesDataControl;
        }
    }
}