using UnityEngine;
using System.Collections;
using System.Collections.Generic;

using uAdventure.Core;

namespace uAdventure.Editor
{
    public class ChapterListDataControl
    {
        /**
            * Controller for the chapters of the adventure.
            */
        private List<ChapterDataControl> chapterDataControlList;

        /**
         * The list of toolManagers (for undo/redo)
         */
        private List<ChapterToolManager> chapterToolManagers;

        /**
         * Stores the current selected Chapter
         */
        private int selectedChapter;

        /**
         * Summary of identifiers.
         */
        private IdentifierSummary identifierSummary;

        /**
         * Summary of flags.
         */
        private VarFlagSummary varFlagSummary;

        /**
         * The list of chapters
         */
        private List<Chapter> chapters;

        public ChapterListDataControl()
        {

            varFlagSummary = new VarFlagSummary();
            chapterDataControlList = new List<ChapterDataControl>();
            chapterToolManagers = new List<ChapterToolManager>();
            setSelectedChapterInternal(-1);
            chapters = new List<Chapter>();
        }

        public ChapterListDataControl(List<Chapter> chapters) : this()
        {
            foreach (Chapter chapter in chapters)
            {
                chapterDataControlList.Add(new ChapterDataControl(chapter));
                chapterToolManagers.Add(new ChapterToolManager());
                //Debug.Log(chapter);
            }
            if (chapters.Count > 0)
                setSelectedChapterInternal(0);
            this.chapters = chapters;
        }

        public List<ChapterDataControl> getChapters()
        {
            return chapterDataControlList;
        }

        public void setSelectedChapterInternal(int newSelectedChapter)
        {

            this.selectedChapter = newSelectedChapter;
            if (selectedChapter == -1)
            {
                if (chapterDataControlList.Count > 0)
                {
                    selectedChapter = 0;
                    if (identifierSummary == null)
                        identifierSummary = new IdentifierSummary(getSelectedChapterData());
                    else
                        identifierSummary.loadIdentifiers(getSelectedChapterData());

                    if (varFlagSummary == null)
                        varFlagSummary = new VarFlagSummary();
                    getSelectedChapterDataControl().updateVarFlagSummary(varFlagSummary);
                }
                else
                {
                    identifierSummary = null;
                    varFlagSummary = new VarFlagSummary();
                }
            }
            else
            {
                identifierSummary = new IdentifierSummary(getSelectedChapterData());
                //if( varFlagSummary == null )
                //delete all flags and vars before add those included in the chapter through updateVarFlagSummary methods
                varFlagSummary = new VarFlagSummary();
                getSelectedChapterDataControl().updateVarFlagSummary(varFlagSummary);
            }
        }

        /**
         * Returns the data of the selected chapter.
         * 
         * @return Selected chapter data
         */
        public Chapter getSelectedChapterData()
        {
            if (chapterDataControlList == null || selectedChapter < 0 || selectedChapter >= chapterDataControlList.Count)
                return null;

            return (Chapter)chapterDataControlList[selectedChapter].getContent();
        }

        /**
         * Adds a new data control with the new chapter. It also updates
         * automatically the selectedChapter, pointing to this new one
         * 
         * @param chapter
         */
        public void addChapterDataControl(Chapter newChapter)
        {

            chapters.Add(newChapter);
            chapterDataControlList.Add(new ChapterDataControl(newChapter));
            chapterToolManagers.Add(new ChapterToolManager());
            setSelectedChapterInternal(chapterDataControlList.Count - 1);
        }

        /**
         * Adds a new data control with the new chapter in the given position. It
         * also updates automatically the selectedChapter, pointing to this new one
         * 
         * @param chapter
         */
        public void addChapterDataControl(int index, Chapter newChapter)
        {

            chapters.Insert(index, newChapter);
            chapterDataControlList.Insert(index, new ChapterDataControl(newChapter));
            chapterToolManagers.Insert(index, new ChapterToolManager());
            setSelectedChapterInternal(index);
        }

        /**
         * Deletes the selected chapter data control. It also updates automatically
         * the selectedChapter if necessary
         * 
         * @param chapter
         */
        public ChapterDataControl removeChapterDataControl()
        {

            return removeChapterDataControl(selectedChapter);
        }

        /**
         * Deletes the chapter data control in the given position. It also updates
         * automatically the selectedChapter if necessary
         * 
         * @param chapter
         */
        public ChapterDataControl removeChapterDataControl(int index)
        {

            chapters.RemoveAt(index);
            ChapterDataControl removed = chapterDataControlList[index];
            chapterDataControlList.RemoveAt(index);
            chapterToolManagers.RemoveAt(index);
            setSelectedChapterInternal(selectedChapter - 1);
            return removed;
        }

        /**
         * Returns the index of the chapter currently selected.
         * 
         * @return Index of the selected chapter
         */
        public int getSelectedChapter()
        {

            return selectedChapter;
        }

        /**
         * Returns the selected chapter data controller.
         * 
         * @return The selected chapter data controller
         */
        public ChapterDataControl getSelectedChapterDataControl()
        {

            if (chapterDataControlList.Count != 0)
                return chapterDataControlList[selectedChapter];
            else
                return null;
        }

        public void addPlayerToAllScenesChapters()
        {

            foreach (ChapterDataControl dataControl in chapterDataControlList)
            {
                dataControl.getScenesList().addPlayerToAllScenes();
            }
        }

        public void addPlayerToAllScenesSelectedChapter()
        {

        }

        public void deletePlayerToAllScenesChapters()
        {

            foreach (ChapterDataControl dataControl in chapterDataControlList)
            {
                dataControl.getScenesList().deletePlayerToAllScenes();
            }
        }

        public void deletePlayerToAllScenesSelectedChapter()
        {

        }

        public bool isValid(string currentPath, List<string> incidences)
        {

            bool valid = true;
            foreach (ChapterDataControl dataControl in chapterDataControlList)
            {
                valid &= dataControl.isValid(currentPath, incidences);
            }

            return valid;
        }

        public bool isAnyChapterSelected()
        {

            return selectedChapter != -1;
        }

        /**
         * @return the identifierSummary
         */
        public IdentifierSummary getIdentifierSummary()
        {

            return identifierSummary;
        }

        /**
         * @return the varFlagSummary
         */
        public VarFlagSummary getVarFlagSummary()
        {

            return varFlagSummary;
        }

        public bool replaceSelectedChapter(Chapter newChapter)
        {

            int chapter = this.getSelectedChapter();
            chapters[getSelectedChapter()] = newChapter;
            chapterDataControlList[chapter] = new ChapterDataControl(newChapter);
            identifierSummary = new IdentifierSummary(newChapter);
            identifierSummary.loadIdentifiers(getSelectedChapterData());

            return true;
        }

        //public bool hasScorm12Profiles(AdventureDataControl adventureData)
        //{

        //    bool hasProfiles = true;
        //    foreach (ChapterDataControl dataControl in chapterDataControlList)
        //    {
        //        if (dataControl.hasAdaptationProfile())
        //        {
        //            AdaptationProfileDataControl adpProfile = dataControl.getSelectedAdaptationProfile();
        //            hasProfiles &= adpProfile.isScorm12();
        //        }
        //        if (dataControl.hasAssessmentProfile())
        //        {
        //            AssessmentProfileDataControl assessmentProfile = dataControl.getSelectedAssessmentProfile();
        //            hasProfiles &= assessmentProfile.isScorm12();
        //        }
        //    }
        //    return hasProfiles;
        //}

        //public bool hasScorm2004Profiles(AdventureDataControl adventureData)
        //{

        //    bool hasProfiles = true;
        //    for (ChapterDataControl dataControl : chapterDataControlList)
        //    {
        //        if (dataControl.hasAdaptationProfile())
        //        {
        //            AdaptationProfileDataControl adpProfile = dataControl.getSelectedAdaptationProfile();
        //            hasProfiles &= adpProfile.isScorm2004();
        //        }
        //        if (dataControl.hasAssessmentProfile())
        //        {
        //            AssessmentProfileDataControl assessmentProfile = dataControl.getSelectedAssessmentProfile();
        //            hasProfiles &= assessmentProfile.isScorm2004();
        //        }
        //    }
        //    return hasProfiles;
        //}

        public void updateVarFlagSummary()
        {

            getSelectedChapterDataControl().updateVarFlagSummary(varFlagSummary);
        }

        /**
         * Moves the selected chapter to the previous position of the chapter's
         * list.
         */
        public bool moveChapterUp()
        {

            return moveChapterUp(selectedChapter);
        }

        /**
         * Moves the selected chapter to the previous position of the chapter's
         * list.
         */
        public bool moveChapterUp(int index)
        {

            // If the chapter can be moved
            if (index > 0)
            {
                // Move the chapter and update the selected chapter
                Chapter c = chapters[index];
                chapters.RemoveAt(index);
                chapters.Insert(index - 1, c);
                // Move the chapter and update the selected chapter
                ChapterDataControl con = chapterDataControlList[index];
                chapterDataControlList.RemoveAt(index);
                chapterDataControlList.Insert(index - 1, con);

                ChapterToolManager man = chapterToolManagers[index];
                chapterToolManagers.RemoveAt(index);
                chapterToolManagers.Insert(index - 1, man);

                setSelectedChapterInternal(index - 1);
                return true;
            }
            return false;
        }

        /**
         * Moves the selected chapter to the next position of the chapter's list.
         * 
         */
        public bool moveChapterDown()
        {

            return moveChapterDown(selectedChapter);
        }

        /**
         * Moves the selected chapter to the next position of the chapter's list.
         * 
         */
        public bool moveChapterDown(int index)
        {

            // If the chapter can be moved
            if (index < chapterDataControlList.Count - 1)
            {
                // Move the chapter and update the selected chapter
                Chapter c = chapters[index];
                chapters.RemoveAt(index);
                chapters.Insert(index + 1, c);
                // Move the chapter and update the selected chapter
                ChapterDataControl con = chapterDataControlList[index];
                chapterDataControlList.RemoveAt(index);
                chapterDataControlList.Insert(index + 1, con);

                setSelectedChapterInternal(index + 1);
                return true;
            }
            return false;
        }

        /**
         * Counts all the references to a given asset in the entire script.
         * 
         * @param assetPath
         *            Path of the asset (relative to the ZIP), without suffix in
         *            case of an animation or set of slides
         * @return Number of references to the given asset
         */
        public int countAssetReferences(string assetPath)
        {

            int count = 0;

            // Search in all the chapters
            foreach (ChapterDataControl chapterDataControl in chapterDataControlList)
            {
                count += chapterDataControl.countAssetReferences(assetPath);
            }
            return count;
        }

        /**
         * Gets a list with all the assets referenced in the chapter along with the
         * types of those assets
         * 
         * @param assetPaths
         * @param assetTypes
         */
        public void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {

            foreach (ChapterDataControl chapterDataControl in chapterDataControlList)
            {
                chapterDataControl.getAssetReferences(assetPaths, assetTypes);
            }
        }

        /**
         * Deletes a given asset from the script, removing all occurrences.
         * 
         * @param assetPath
         *            Path of the asset (relative to the ZIP), without suffix in
         *            case of an animation or set of slides
         */
        public void deleteAssetReferences(string assetPath)
        {

            // Delete the asset in all the chapters
            foreach (ChapterDataControl chapterDataControl in chapterDataControlList)
                chapterDataControl.deleteAssetReferences(assetPath);
        }

        /**
         * Deletes a given identifier from the script, removing all occurrences.
         * 
         * @param id
         *            Identifier to be deleted
         */
        public void deleteIdentifierReferences(string id)
        {

            if (getSelectedChapterDataControl() != null)
                getSelectedChapterDataControl().deleteIdentifierReferences(id);
            //  else
            //    this.identifierSummary.deleteAssessmentRuleId( id );
        }

        public bool addTool(Tool tool)
        {

            bool done = true;
            if (!tool.GetDoesClone())
            {
                done = chapterToolManagers[getSelectedChapter()].addTool(tool);
            }
            else
            {
                if (done = tool.doTool())
                {
                    chapterToolManagers[getSelectedChapter()].clear();
                    chapterToolManagers[getSelectedChapter()].addTool(false, tool);
                }
                else
                    chapterToolManagers[getSelectedChapter()].addTool(false, tool);
            }

            return done;
        }

        public void undoTool()
        {

            chapterToolManagers[getSelectedChapter()].undoTool();
        }

        public void redoTool()
        {

            chapterToolManagers[getSelectedChapter()].redoTool();
        }

        public void pushLocalToolManager()
        {

            chapterToolManagers[getSelectedChapter()].pushLocalToolManager();
        }

        public void popLocalToolManager()
        {

            chapterToolManagers[getSelectedChapter()].popLocalToolManager();
        }

        /**
         * Returns an array with the chapter titles.
         * 
         * @return Array with the chapter titles
         */
        public string[] getChapterTitles()
        {

            List<string> chapterNames = new List<string>();

            // Add the chapter titles
            foreach (ChapterDataControl chapterDataControl in chapterDataControlList)
            {
                Chapter chapter = (Chapter)chapterDataControl.getContent();
                chapterNames.Add(chapter.getTitle());
            }

            return chapterNames.ToArray();
        }

        public int getChaptersCount()
        {

            return chapters.Count;
        }

        public bool exitsChapter(string chapterTitle)
        {

            // look the chapter titles
            foreach (ChapterDataControl chapterDataControl in chapterDataControlList)
            {
                Chapter chapter = (Chapter)chapterDataControl.getContent();
                if (chapter.getTitle().Equals(chapterTitle))
                    return true;
            }
            return false;
        }

        /**
         * Private method that fills the flags and vars structures of the chapter
         * data before passing on the information to the game engine for running
         */
        public void updateVarsFlagsForRunning()
        {

            // Update everyChapter
            foreach (ChapterDataControl chapterDataControl in chapterDataControlList)
            {
                VarFlagSummary tempSummary = new VarFlagSummary();
                chapterDataControl.updateVarFlagSummary(tempSummary);
                tempSummary.clean();
                Chapter chapter = (Chapter)chapterDataControl.getContent();
                // Update flags
                foreach (string flag in tempSummary.getFlags())
                {
                    chapter.addFlag(flag);
                }
                // Update vars
                foreach (string var in tempSummary.getVars())
                {
                    chapter.addVar(var);
                }
            }
        }

        ////////DEBUGGING OPTIONS
        /**
         * @return the chapterToolManagers
         */
        public List<ChapterToolManager> getChapterToolManagers()
        {

            return chapterToolManagers;
        }
    }
}