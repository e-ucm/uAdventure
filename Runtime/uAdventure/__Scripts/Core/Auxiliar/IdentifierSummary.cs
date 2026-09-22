using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;

namespace uAdventure.Core
{
    /**
     * This class holds the summary of all the identifiers present on the script.
     */
    public class IdentifierSummary
    {


		private Dictionary<string, System.Type> globalIdentifiers;
		private Dictionary<System.Type, List<string>> typeGroups;

        /**
         * List of all assessment profile identifiers in the script and its associated assessment rules. 
         */
        private Dictionary<string, List<string>> assessmentIdentifiers;

        /**
         * List of all adaptation profile identifiers in the script and its associated adaptation rules.
         */
        private Dictionary<string, List<string>> adaptationIdentifiers;

        /**
         * Constructor.
         * 
         * @param chapter
         *            Chapter data which will provide the identifiers
         */
        public IdentifierSummary(Chapter chapter)
        {
            // Create the lists
			globalIdentifiers = new Dictionary<string, System.Type>(StringComparer.InvariantCultureIgnoreCase);
			typeGroups = new Dictionary<System.Type, List<string>>();

            assessmentIdentifiers = new Dictionary<string, List<string>> ();
			adaptationIdentifiers = new Dictionary<string, List<string>> ();

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

            assessmentIdentifiers.Clear();
            adaptationIdentifiers.Clear();

            // Add scene IDs
            foreach (Scene scene in chapter.getScenes())
            {
                addId<Scene>(scene.getId());
                foreach (ActiveArea activeArea in scene.getActiveAreas())
                {
                    if (activeArea.getId() != null && !activeArea.getId().Equals(""))
                        addId<ActiveArea>(activeArea.getId());
                }
            }            
            
            // Add conversation IDs
            foreach (Conversation conversation in chapter.getConversations())
            {
                addId<Conversation>(conversation.getId());
            }

            foreach (var o in chapter.getObjects().FindAll (t => t is HasId)) {
				var h = o as HasId;
				addId(o.GetType (), h.getId ());
			}


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
			return globalIdentifiers.ContainsKey(id); 
        }

		public bool isType<T>(string id){

			return globalIdentifiers.ContainsKey (id) && typeof(T).IsAssignableFrom(globalIdentifiers[id]);
        }

        public Type getType(string id)
        {
            return existsId(id) ? globalIdentifiers[id] : null;
        }


        public void addId(System.Type t, string id){
			if(!globalIdentifiers.ContainsKey (id))
            {
                globalIdentifiers.Add(id, t);
            }

            t = GroupableTypeAttribute.GetGroupType(t);

            if (!typeGroups.ContainsKey (t))
            {
                typeGroups.Add(t, new List<string>());
            }

			if (!typeGroups [t].Contains (id))
            {
                typeGroups[t].Add(id);
            }
        }

		public void addId<T>(string id){
			addId (typeof(T), id);
		}

        public string[] getIds(Type t)
        {
            if (typeGroups.ContainsKey(t))
            {
                return typeGroups[t].ToArray();
            }
            else
            {
                return new string[0];
            }
        }

        public string[] getIds<T>()
        {
            if (typeof(T).IsInterface)
            {
                return groupIds(typeof(T));
            }

            return getIds(typeof(T)); 
        }

        public void deleteId<T>(string id)
        {
            deleteId(typeof(T), id);
        }

        public void deleteId(Type t, string id){
			if (globalIdentifiers.ContainsKey(id))
            {
                globalIdentifiers.Remove(id);

                t = GroupableTypeAttribute.GetGroupType(t);

                if (typeGroups.ContainsKey(t) && typeGroups[t].Contains(id))
                {
                    typeGroups[t].Remove(id);
                }
            }
		}

        /**
         * Returns an array of global state identifiers.
         * 
         * @return Array of global state identifiers
         */
        public string[] getGlobalStatesIds(string[] exceptions)
        {

            List<string> globalStateIds = new List<string>();
			foreach (string id in getIds<GlobalState> ())
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
			addId<AssessmentRule> (profile + "." + assRuleId);
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
			addId<AdaptationRule> (profile + "." + adpRuleId);
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
			addId<AssessmentProfile> (assProfileId);
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
			addId<AdaptationProfile> (adaptProfileId);
            this.adaptationIdentifiers.Add(adaptProfileId, new List<string>());
        }

        /**
         * Deleted an assessment rule id
         * 
         * @param id
         *            Assessment rule id to be deleted
         */
        public void deleteAssessmentRuleId(string id, string profile)
        {
			deleteId<AssessmentRule> (profile + "." + id);
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
			deleteId<AdaptationRule> (profile + "." + id);
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
			deleteId<AssessmentProfile> (id);
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
			deleteId<AdaptationProfile> (id);
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

		[Obsolete("Use idType<T> instead")]
        public bool isGlobalStateId(string id)
		{
			return isType<GlobalState> (id);
        }

		[Obsolete("Use idType<T> instead")]
        public bool isMacroId(string id)
		{
			return isType<Macro> (id);
        }

		[Obsolete("Use addId<T> instead")]
        public void addActiveAreaId(string id)
        {
			addId<ActiveArea> (id);
        }

		[Obsolete("Use deleteId<T> instead")]
        public void deleteActiveAreaId(string id)
		{
			deleteId<ActiveArea> (id);
        }

        /**
         * Get a list of all ids of the items and active areas
         * 
         * @return ids of the items and activeAreas
         */
		[Obsolete("Use getIds<T> instead")]
        public string[] getItemAndActiveAreaIds()
        {
			List<string> set = new List<string>(getIds<Item> ());
			set.AddRange(getIds<ActiveArea> ());
            return set.ToArray();
        }

        /**
         * Get a list of all ids of the items, active areas and npcs
         * 
         * @return ids of the items, activeAreas and npcs
         */
		[Obsolete("Use getIds<T> instead")]
        public string[] getItemActiveAreaNPCIds()
		{
			List<string> set = new List<string>(getIds<Item> ());
			set.AddRange(getIds<ActiveArea> ());
			set.AddRange(getIds<NPC>());
            return set.ToArray();
        }

		public string[] groupIds<T>() 
		{
			return groupIds (typeof(T));
		}

		public string[] groupIds(System.Type t){
			var complete = new List<string> ();
			foreach (var kv in typeGroups)
				if (t.IsAssignableFrom (kv.Key))
					complete.AddRange (kv.Value);

			return complete.ToArray ();
		}

        public string[] combineIds(System.Type[] types)
        {
            var elementCount = types.Select(t => getIds(t).Length).Sum();

            // If the list has elements, show the dialog with the options
            if (elementCount > 0)
            {
                var elements = new string[elementCount];
                var totalCopied = 0;
                foreach (var t in types)
                {
                    var typeElements = getIds(t);
                    System.Array.Copy(typeElements, 0, elements, totalCopied, typeElements.Length);
                    totalCopied += typeElements.Length;
                }
                return elements;
            }

            return new string[0];
        }
    }
}