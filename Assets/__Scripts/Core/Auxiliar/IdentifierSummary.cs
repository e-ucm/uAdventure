using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

/**
 * This class holds the summary of all the identifiers present on the script.
 */
public class IdentifierSummary {

    /**
         * List of all identifiers in the script.
         */
    private List<string> globalIdentifiers;

    /**
     * List of all scene identifiers in the chapter (including playable scenes
     * and cutscenes).
     */
    private List<string> generalSceneIdentifiers;

    /**
     * List of all ccutscene identifiers in the script.
     */
    private List<string> sceneIdentifiers;

    /**
     * List of all identifiers of cutscenes in the script.
     */
    private List<string> cutsceneIdentifiers;

    /**
     * List of all book identifiers in the script.
     */
    private List<string> bookIdentifiers;

    /**
     * List of all item identifiers in the script.
     */
    private List<string> itemIdentifiers;

    /**
     * List of all atrezzo item identifiers in the script.
     */
    private List<string> atrezzoIdentifiers;

    /**
     * List of all NPC identifiers in the script.
     */
    private List<string> npcIdentifiers;

    /**
     * List of all conversation identifiers in the script.
     */
    private List<string> conversationIdentifiers;

    /**
     * List of all assessment profile identifiers in the script and its associated assessment rules. 
     */
    private Dictionary<string, List<string>> assessmentIdentifiers;

    /**
     * List of all adaptation profile identifiers in the script and its associated adaptation rules.
     */
    private Dictionary<string, List<string>> adaptationIdentifiers;

    /**
     * List of all global states identifiers in the script.
     */
    private List<string> globalStateIdentifiers;

    /**
     * List of all macro identifiers in the script.
     */
    private List<string> macroIdentifiers;

    private List<string> activeAreaIdentifiers;

    /**
     * Constructor.
     * 
     * @param chapter
     *            Chapter data which will provide the identifiers
     */
    public IdentifierSummary(Chapter chapter)
    {

        // Create the lists
        globalIdentifiers = new List<string>();
        generalSceneIdentifiers = new List<string>();
        sceneIdentifiers = new List<string>();
        cutsceneIdentifiers = new List<string>();
        bookIdentifiers = new List<string>();
        itemIdentifiers = new List<string>();
        atrezzoIdentifiers = new List<string>();
        npcIdentifiers = new List<string>();
        conversationIdentifiers = new List<string>();
        assessmentIdentifiers = new Dictionary<string, List<string>>();
        adaptationIdentifiers = new Dictionary<string, List<string>>();
        globalStateIdentifiers = new List<string>();
        macroIdentifiers = new List<string>();
        activeAreaIdentifiers = new List<string>();

        // Fill all the lists
        loadIdentifiers(chapter);
    }

    /**
     * Reloads the identifiers with the given chapter data.
     * 
     * @param chapter
     *            Chapter data which will provide the identifiers
     */
    public void loadIdentifiers(Chapter chapter)
    {

        // Clear the lists
        globalIdentifiers.Clear();
        generalSceneIdentifiers.Clear();
        sceneIdentifiers.Clear();
        cutsceneIdentifiers.Clear();
        bookIdentifiers.Clear();
        itemIdentifiers.Clear();
        atrezzoIdentifiers.Clear();
        npcIdentifiers.Clear();
        conversationIdentifiers.Clear();
        globalStateIdentifiers.Clear();
        macroIdentifiers.Clear();
        activeAreaIdentifiers.Clear();
        assessmentIdentifiers.Clear();
        adaptationIdentifiers.Clear();

        // Add scene IDs
        foreach (Scene scene in chapter.getScenes())
        {
            addSceneId(scene.getId());
            foreach (ActiveArea activeArea in scene.getActiveAreas())
            {
                if (activeArea.getId() != null && !activeArea.getId().Equals(""))
                    addActiveAreaId(activeArea.getId());
            }
        }

        // Add cutscene IDs
        foreach (Cutscene cutscene in chapter.getCutscenes())
            addCutsceneId(cutscene.getId());

        // Add book IDs
        foreach (Book book in chapter.getBooks())
            addBookId(book.getId());

        // Add item IDs
        foreach (Item item in chapter.getItems())
            addItemId(item.getId());

        // Add atrezzo items IDs
        foreach (Atrezzo atrezzo in chapter.getAtrezzo())
            addAtrezzoId(atrezzo.getId());

        // Add NPC IDs
        foreach (NPC npc in chapter.getCharacters())
            addNPCId(npc.getId());

        // Add conversation IDs
        foreach (Conversation conversation in chapter.getConversations())
            addConversationId(conversation.getId());

        // Add global state IDs
        foreach (GlobalState globalState in chapter.getGlobalStates())
            addGlobalStateId(globalState.getId());

        // Add macro IDs
        foreach (Macro macro in chapter.getMacros())
            addMacroId(macro.getId());

        // Add assessment rules ids and asssessmnet profiles ids
        foreach (AssessmentProfile profile in chapter.getAssessmentProfiles())
        {
            string name = profile.getName();
            addAssessmentProfileId(name);
            foreach (AssessmentRule rule in profile.getRules())
                this.addAssessmentRuleId(rule.getId(), name);
        }

        // Add adaptation rules ids and asssessmnet profiles ids
        foreach (AdaptationProfile profile in chapter.getAdaptationProfiles())
        {
            string name = profile.getName();
            addAdaptationProfileId(name);
            foreach (AdaptationRule rule in profile.getRules())
                this.addAdaptationRuleId(rule.getId(), name);
        }

    }

    /**
     * Returns if the given id exists or not.
     * 
     * @param id
     *            Id to be checked
     * @return True if the id exists, false otherwise
     */
    public bool existsId(string id)
    {

        return globalIdentifiers.Contains(id);
    }

    /**
     * Returns whether the given identifier is a scene or not.
     * 
     * @param sceneId
     *            Scene identifier
     * @return True if the identifier belongs to a scene, false otherwise
     */
    public bool isScene(string sceneId)
    {

        return sceneIdentifiers.Contains(sceneId);
    }

    /**
     * Returns whether the given identifier is a conversation or not.
     * 
     * @param sceneId
     *            Scene identifier
     * @return True if the identifier belongs to a scene, false otherwise
     */
    public bool isConversation(string convId)
    {

        return conversationIdentifiers.Contains(convId);
    }

    /**
     * Returns an array of general scene identifiers.
     * 
     * @return Array of general scene identifiers
     */
    public string[] getGeneralSceneIds()
    {

        return generalSceneIdentifiers.ToArray();
    }

    /**
     * Returns an array of scene identifiers.
     * 
     * @return Array of scene identifiers
     */
    public string[] getSceneIds()
    {

        return sceneIdentifiers.ToArray();
    }

    /**
     * Returns an array of cutscene identifiers.
     * 
     * @return Array of cutscene identifiers
     */
    public string[] getCutsceneIds()
    {

        return cutsceneIdentifiers.ToArray();
    }

    /**
     * Returns all scenes ids (scenes and cutsecene)
     * 
     * @return Array of cutscene plus scene identifiers
     */
    public string[] getAllSceneIds()
    {

        List<string> allScenes = new List<string>(cutsceneIdentifiers);
        allScenes.AddRange(new List<string>(sceneIdentifiers));
        return allScenes.ToArray();
    }

    /**
     * Returns an array of book identifiers.
     * 
     * @return Array of book identifiers
     */
    public string[] getBookIds()
    {

        return bookIdentifiers.ToArray();
    }

    /**
     * Returns an array of item identifiers.
     * 
     * @return Array of item identifiers
     */
    public string[] getItemIds()
    {

        return itemIdentifiers.ToArray();
    }

    /**
     * Returns an array of atrezzo item identifiers.
     * 
     * @return Array of atrezzo item identifiers
     */
    public string[] getAtrezzoIds()
    {

        return atrezzoIdentifiers.ToArray();
    }

    /**
     * Returns an array of NPC identifiers.
     * 
     * @return Array of NPC identifiers
     */
    public string[] getNPCIds()
    {

        return npcIdentifiers.ToArray();
    }

    /**
     * Returns an array of conversation identifiers.
     * 
     * @return Array of conversation identifiers
     */
    public string[] getConversationsIds()
    {

        return conversationIdentifiers.ToArray();
    }

    /**
     * Returns an array of global state identifiers.
     * 
     * @return Array of global state identifiers
     */
    public string[] getGlobalStatesIds()
    {

        return globalStateIdentifiers.ToArray();
    }

    /**
     * Returns an array of macro identifiers.
     * 
     * @return Array of macro identifiers
     */
    public string[] getMacroIds()
    {

        return macroIdentifiers.ToArray();
    }

    /**
     * Returns an array of global state identifiers.
     * 
     * @return Array of global state identifiers
     */
    public string[] getGlobalStatesIds(string[] exceptions)
    {

        List<string> globalStateIds = new List<string>();
        foreach (string id in this.globalStateIdentifiers)
        {
            bool found = false;
            foreach (string exception in exceptions)
            {
                found |= exception.Equals(id);
                if (found)
                {
                    break;
                }
            }
            if (!found)
            {
                globalStateIds.Add(id);
            }
        }
        return globalStateIds.ToArray();
    }

    /**
     * Returns an array of macro identifiers.
     * 
     * @return Array of macro identifiers
     */
    public string[] getMacroIds(string exception)
    {

        List<string> macroIds = new List<string>();
        foreach (string id in this.macroIdentifiers)
        {
            if (!id.Equals(exception))
                macroIds.Add(id);
        }
        return macroIds.ToArray();
    }

    /**
     * Adds a new scene id.
     * 
     * @param sceneId
     *            New scene id
     */
    public void addSceneId(string sceneId)
    {

        globalIdentifiers.Add(sceneId);
        generalSceneIdentifiers.Add(sceneId);
        sceneIdentifiers.Add(sceneId);
    }

    /**
     * Adds a new cutscene id.
     * 
     * @param cutsceneId
     *            New cutscene id
     */
    public void addCutsceneId(string cutsceneId)
    {

        globalIdentifiers.Add(cutsceneId);
        generalSceneIdentifiers.Add(cutsceneId);
        cutsceneIdentifiers.Add(cutsceneId);
    }

    /**
     * Adds a new book id.
     * 
     * @param bookId
     *            New book id
     */
    public void addBookId(string bookId)
    {

        globalIdentifiers.Add(bookId);
        bookIdentifiers.Add(bookId);
    }

    /**
     * Adds a new item id.
     * 
     * @param itemId
     *            New item id
     */
    public void addItemId(string itemId)
    {

        globalIdentifiers.Add(itemId);
        itemIdentifiers.Add(itemId);
    }

    /**
     * Adds a new atrezzo item id.
     * 
     * @param atrezzoId
     *            New atrezzo item id
     */
    public void addAtrezzoId(string atrezzoId)
    {

        globalIdentifiers.Add(atrezzoId);
        atrezzoIdentifiers.Add(atrezzoId);
    }

    /**
     * Adds a new NPC id.
     * 
     * @param npcId
     *            New NPC id
     */
    public void addNPCId(string npcId)
    {

        globalIdentifiers.Add(npcId);
        npcIdentifiers.Add(npcId);
    }

    /**
     * Adds a new conversation id.
     * 
     * @param conversationId
     *            New conversation id
     */
    public void addConversationId(string conversationId)
    {

        globalIdentifiers.Add(conversationId);
        conversationIdentifiers.Add(conversationId);
    }

    /**
     * Adds a new global state id.
     * 
     * @param globalStateId
     *            New conversation id
     */
    public void addGlobalStateId(string globalStateId)
    {

        globalIdentifiers.Add(globalStateId);
        globalStateIdentifiers.Add(globalStateId);
    }

    /**
     * Adds a new macro id.
     * 
     * @param macroId
     *            New macro id
     */
    public void addMacroId(string macroId)
    {

        globalIdentifiers.Add(macroId);
        macroIdentifiers.Add(macroId);
    }

    /**
     * Adds a new assessment rule id. 
     * The id has the name of its profile and the name of the rule :  "profile.assRuleId"
     * 
     * @param assRuleId
     *            New assessment rule id
     * @param profile
     *            The name of profile which contains the rule
     */
    public void addAssessmentRuleId(string assRuleId, string profile)
    {

        globalIdentifiers.Add(profile + "." + assRuleId);
        this.assessmentIdentifiers[profile].Add(assRuleId);
    }

    /**
     * Adds a new adaptation rule id. 
     * The id has the name of its profile and the name of the rule :  "profile.assRuleId"
     * 
     * @param assRuleId
     *            New adaptation rule id
     *            
     * @param profile
     *            The name of profile which contains the rule
     */
    public void addAdaptationRuleId(string adpRuleId, string profile)
    {

        globalIdentifiers.Add(profile + "." + adpRuleId);
        this.adaptationIdentifiers[profile].Add(adpRuleId);
    }

    /**
     * Adds a new assessment profile id.
     * 
     * @param assProfileId
     *            New assessment profile id
     */
    public void addAssessmentProfileId(string assProfileId)
    {

        globalIdentifiers.Add(assProfileId);
        this.assessmentIdentifiers.Add(assProfileId, new List<string>());
    }

    /**
     * Adds a new adaptation profile id.
     * 
     * @param adaptProfileId
     *            New adaptation profile id
     */
    public void addAdaptationProfileId(string adaptProfileId)
    {

        globalIdentifiers.Add(adaptProfileId);
        this.adaptationIdentifiers.Add(adaptProfileId, new List<string>());
    }

    /**
     * Deletes a new scene id.
     * 
     * @param sceneId
     *            Scene id to be deleted
     */
    public void deleteSceneId(string sceneId)
    {

        globalIdentifiers.Remove(sceneId);
        generalSceneIdentifiers.Remove(sceneId);
        sceneIdentifiers.Remove(sceneId);
    }

    /**
     * Deletes a new cutscene id.
     * 
     * @param cutsceneId
     *            Cutscene id to be deleted
     */
    public void deleteCutsceneId(string cutsceneId)
    {

        globalIdentifiers.Remove(cutsceneId);
        generalSceneIdentifiers.Remove(cutsceneId);
        cutsceneIdentifiers.Remove(cutsceneId);
    }

    /**
     * Deletes a new book id.
     * 
     * @param bookId
     *            Book id to be deleted
     */
    public void deleteBookId(string bookId)
    {

        globalIdentifiers.Remove(bookId);
        bookIdentifiers.Remove(bookId);
    }

    /**
     * Deletes a new item id.
     * 
     * @param itemId
     *            Item id to be deleted
     */
    public void deleteItemId(string itemId)
    {

        globalIdentifiers.Remove(itemId);
        itemIdentifiers.Remove(itemId);
    }

    /**
     * Deletes a new atrezzo item id.
     * 
     * @param atrezzoId
     *            atrezzo item id to be deleted
     */
    public void deleteAtrezzoId(string atrezzoId)
    {

        globalIdentifiers.Remove(atrezzoId);
        atrezzoIdentifiers.Remove(atrezzoId);
    }

    /**
     * Deletes a new NPC id.
     * 
     * @param npcId
     *            NPC id to be deleted
     */
    public void deleteNPCId(string npcId)
    {

        globalIdentifiers.Remove(npcId);
        npcIdentifiers.Remove(npcId);
    }

    /**
     * Deletes a new conversation id.
     * 
     * @param conversationId
     *            Conversation id to be deleted
     */
    public void deleteConversationId(string conversationId)
    {

        globalIdentifiers.Remove(conversationId);
        conversationIdentifiers.Remove(conversationId);
    }

    /**
     * Deletes a new conversation id.
     * 
     * @param globalStateId
     *            Conversation id to be deleted
     */
    public void deleteGlobalStateId(string globalStateId)
    {

        globalIdentifiers.Remove(globalStateId);
        globalStateIdentifiers.Remove(globalStateId);
    }

    /**
     * Deletes a macro id.
     * 
     * @param macroId
     *            Macro id to be deleted
     */
    public void deleteMacroId(string macroId)
    {

        globalIdentifiers.Remove(macroId);
        macroIdentifiers.Remove(macroId);
    }

    /**
     * Deleted an assessment rule id
     * 
     * @param id
     *            Assessment rule id to be deleted
     */
    public void deleteAssessmentRuleId(string id, string profile)
    {

        globalIdentifiers.Remove(profile + "." + id);
        assessmentIdentifiers[profile].Remove(id);
    }

    /**
     * Deleted an adaptation rule id
     * 
     * @param id
     *            adaptation rule id to be deleted
     */
    public void deleteAdaptationRuleId(string id, string profile)
    {

        globalIdentifiers.Remove(profile + "." + id);
        adaptationIdentifiers[profile].Remove(id);
    }

    /**
     * Deleted an assessment profile id
     * 
     * @param id
     *            Assessment profile id to be deleted
     */
    public void deleteAssessmentProfileId(string id)
    {

        globalIdentifiers.Remove(id);
        assessmentIdentifiers.Remove(id);
    }

    /**
     * Deleted an adaptation profile id
     * 
     * @param id
     *            adaptation profile id to be deleted
     */
    public void deleteAdaptationProfileId(string id)
    {

        globalIdentifiers.Remove(id);
        adaptationIdentifiers.Remove(id);

    }

    /**
     * Rename the adaptation profile and all of its associated rules
     * 
     * @param oldName
     * 		The old profile's name.
     * @param newName
     * 		The new profile's name.
     */
    public void renameAdaptationProfile(string oldName, string newName)
    {
        // delete all adaptation rules ids associated to deleted profile
        List<string> adp = adaptationIdentifiers[oldName];
        for (int i = 0; i < adp.Count; i++)
        {
            this.deleteAdaptationRuleId(adp[i], oldName);
        }
        // delete the profile ID
        this.deleteAdaptationProfileId(oldName);

        // Add the new adaptation ID and its rules ids
        this.addAdaptationProfileId(newName);
        for (int i = 0; i < adp.Count; i++)
        {
            this.addAdaptationRuleId(adp[i], newName);
        }
    }

    /**
     * Rename the asssessment profile and all of its associated rules
     * 
     * @param oldName
     * 		The old profile's name.
     * @param newName
     * 		The new profile's name.
     */
    public void renameAssessmentProfile(string oldName, string newName)
    {
        // delete all adaptation rules ids associated to deleted profile
        List<string> adp = assessmentIdentifiers[oldName];
        for (int i = 0; i < adp.Count; i++)
        {
            this.deleteAssessmentRuleId(adp[i], oldName);
        }
        // delete the profile ID
        this.deleteAssessmentProfileId(oldName);

        // Add the new assessment ID and its rules ids
        this.addAssessmentProfileId(newName);
        for (int i = 0; i < adp.Count; i++)
        {
            this.addAssessmentRuleId(adp[i], newName);
        }
    }

    public bool isAdaptationProfileId(string id)
    {

        return this.adaptationIdentifiers.Keys.Contains(id);
    }

    public bool isAdaptationRuleId(string id, string profile)
    {

        return this.adaptationIdentifiers[profile].Contains(id);
    }

    public bool isAssessmentProfileId(string id)
    {

        return this.assessmentIdentifiers.Keys.Contains(id);
    }

    public bool isAssessmentRuleId(string id, string profile)
    {

        return this.assessmentIdentifiers[profile].Contains(id);
    }

    public bool isGlobalStateId(string id)
    {

        return globalStateIdentifiers.Contains(id);
    }

    public bool isMacroId(string id)
    {

        return macroIdentifiers.Contains(id);
    }

    public void addActiveAreaId(string id)
    {

        globalIdentifiers.Add(id);
        activeAreaIdentifiers.Add(id);
    }

    public void deleteActiveAreaId(string id)
    {

        if (activeAreaIdentifiers.Contains(id))
        {
            globalIdentifiers.Remove(id);
            activeAreaIdentifiers.Remove(id);
        }
    }

    /**
     * Get a list of all ids of the items and active areas
     * 
     * @return ids of the items and activeAreas
     */
    public string[] getItemAndActiveAreaIds()
    {
        List<string> set = new List<string>(itemIdentifiers);
        set.AddRange(activeAreaIdentifiers);
        return set.ToArray();
    }

    /**
     * Get a list of all ids of the items, active areas and npcs
     * 
     * @return ids of the items, activeAreas and npcs
     */
    public string[] getItemActiveAreaNPCIds()
    {
        List<string> set = new List<string>(itemIdentifiers);
        set.AddRange(activeAreaIdentifiers);
        set.AddRange(npcIdentifiers);
        return set.ToArray();
    }
}
