using System;
using System.Linq;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace uAdventure.Core
{
    /**
     * This class hold the data of a chapter in eAdventure.
     */

    public class Chapter : ChapterSummary, HasTargetId, ICloneable
    {
        public override string ToString()
        {
            return ""/*"InitialScene:" + initialScene + "\nScenes: " + CollectionPrinter.PrintCollection(scenes) + "\nCutscenes: " + CollectionPrinter.PrintCollection(cutscenes) + "\nBooks: " +
                   CollectionPrinter.PrintCollection(books) + "\nItems: " + CollectionPrinter.PrintCollection(items) + "\nAtrezzo: " + CollectionPrinter.PrintCollection(atrezzo) + "\n Player:" + player + "\nCharacters: " +
                   CollectionPrinter.PrintCollection(characters) + "\nConversations:" + CollectionPrinter.PrintCollection(conversations) + "\nTimers: " + CollectionPrinter.PrintCollection(timers) + "\nFlags: " + CollectionPrinter.PrintCollection(flags) + "\nVars " +
                   CollectionPrinter.PrintCollection(vars) + "\nGlobalStates: " + CollectionPrinter.PrintCollection(globalStates) + "\nMacros: " + CollectionPrinter.PrintCollection(macros)*/;
        }

        /**
         * Identifier of the initial scene.
         */
        private string initialScene;

		private List<string> flags, vars;

		/// <summary>
		/// The player.
		/// </summary>
		private Player player;

        /**
         * Empty constructor. Sets values to null and creates empty lists.
         */

        // ------------------------------
        // EXTENSIONS
        // ------------------------------

        Dictionary<Type, IList> extensionObjects;


        public Chapter() : base()
        {
			extensionObjects = new Dictionary<Type, IList>();
			flags = new List<string> ();
			vars = new List<string> ();
            getObjects<NPC>();
            player = new Player();
        }

        /**
         * Constructor with title for the chapter. Sets empty values and creates
         * empty lists, plus one scene.
         * 
         * @param title
         *            Title for the chapter
         * @param sceneId
         *            Identifier for the scene
         */

        public Chapter(string title, string sceneId) : base(title)
        {
            initialScene = sceneId;
			extensionObjects = new Dictionary<Type, IList>();
			flags = new List<string> ();
			vars = new List<string> ();
            getObjects<NPC>();
            player = new Player();
        }

        /**
         * Returns the initial scene identifier.
         * 
         * @return Initial scene identifier
         */

        public string getTargetId()
        {
            return initialScene;
        }

        /**
         * Returns the initial scene
         * 
         * @return the initial scene
         */

        private GeneralScene getInitialGeneralScene()
        {

            GeneralScene initialGeneralScene = null;
            if (initialScene != null)
            {
                initialGeneralScene = getGeneralScene(initialScene);
            }
            if (initialGeneralScene == null)
            {
                // Return the FIRST initial scene stored
                for (int i = 0; i < getGeneralScenes().Count && initialGeneralScene == null; i++)
                    if (getGeneralScenes()[i].isInitialScene())
                        initialGeneralScene = getGeneralScenes()[i];

                // If there is no initial scene, return the first scene
                if (initialGeneralScene == null)
                    initialGeneralScene = getGeneralScenes()[0];
            }

            return initialGeneralScene;
        }

        public IChapterTarget getInitialChapterTarget()
        {
            if(initialScene != null && initialScene != "")
            {
                // return the initial scene
                return (IChapterTarget) getObjects().Find(o => o is IChapterTarget && (o as IChapterTarget).getId() == initialScene);
            }
            else
            {
                // return the first scene possible
                return (IChapterTarget) getObjects().Find(o => o is IChapterTarget);
            }
        }

        /**
         * Returns the list of playable scenes in the game.
         * 
         * @return List of playable scenes
         */

        public List<Scene> getScenes()
        {

			return getObjects<Scene> ();
        }

        /**
         * Returns the list of cutscenes in the game.
         * 
         * @return List of cutscenes
         */

        public List<Cutscene> getCutscenes()
        {
			return getObjects<Cutscene> ();
        }

        /**
         * Returns the list of books in the game
         * 
         * @return the list of books in the game
         */

        public List<Book> getBooks()
		{
			return getObjects<Book> ();
        }

        /**
         * Returns the list of items in the game
         * 
         * @return the list of items in the game
         */

        public List<Item> getItems()
		{
			return getObjects<Item> ();
        }

        /**
         * Returns the list of atrezzo items in the game
         * 
         * @return the list of atrezzo items in the game
         */

        public List<Atrezzo> getAtrezzo()
		{
			return getObjects<Atrezzo> ();
        }

        /**
         * Returns the player of the game
         * 
         * @return the player of the gam
         */

        public Player getPlayer()
        {
            return player;
        }

        /**
         * Returns the list of characters in the game
         * 
         * @return the list of characters in the game
         */

        public List<NPC> getCharacters()
		{
			return getObjects<NPC> ();
        }

        /**
         * Returns the list of conversations in the game
         * 
         * @return the list of conversations in the game
         */

        public List<Conversation> getConversations()
		{
			return getObjects<Conversation> ();
        }

        /**
         * Changes the initial scene of the chapter.
         * 
         * @param initialScene
         *            New initial scene identifier
         */

        public void setTargetId(string initialScene)
        {
            this.initialScene = initialScene;
        }

        /**
         * Adds a scene to the list of playable scenes in the game.
         * 
         * @param scene
         *            the scene to add
         */

        public void addScene(Scene scene)
        {
			getScenes ().Add(scene);
        }

        /**
         * Adds a cutscene to the list of cutscenes in the game.
         * 
         * @param cutscene
         *            The cutscene to add
         */

        public void addCutscene(Cutscene cutscene)
        {
			getCutscenes ().Add(cutscene);
        }

        /**
         * Adds a book to the list of book in the game
         * 
         * @param book
         *            the book to add
         */

        public void addBook(Book book)
        {
			getBooks ().Add (book);
        }

        /**
         * Adds an item to the list of items in the game
         * 
         * @param item
         *            the item to add
         */

        public void addItem(Item item)
        {
			getItems().Add (item);
        }

        /**
         * Adds an atrezzo item to the list of atrezzo items in the game
         * 
         * @param atrezzo
         *            the atrezzo item to add
         */

        public void addAtrezzo(Atrezzo atrezzo)
        {
			getAtrezzo ().Add (atrezzo);
        }

        /**
         * Adds a global state to the list of global states in the game
         * 
         * @param globalState
         *            the global state to add
         */

        public void addGlobalState(GlobalState globalState)
		{
			getGlobalStates ().Add (globalState);
        }

        /**
         * Adds a macro to the list of macros in the game
         * 
         * @param macro
         *            the macro to add
         */

        public void addMacro(Macro macro)
		{
			getMacros ().Add (macro);
        }

        /**
         * Changes the player in the game
         * 
         * @param player
         *            the new player
         */

        public void setPlayer(Player player)
        {

            this.player = player;
        }

        /**
         * Adds a character to the list of characters in the game
         * 
         * @param npc
         *            the new character
         */

        public void addCharacter(NPC npc)
		{
			getCharacters ().Add (npc);
        }

        /**
         * Adds a conversation to the list of conversation in the game
         * 
         * @param conversation
         *            the new conversation
         */

        public void addConversation(Conversation conversation)
		{
			getConversations ().Add (conversation);
        }

        /**
         * Adds a timer to the list of timers in the game
         * 
         * @param timer
         *            the new timer
         */

        public void addTimer(Timer timer)
		{
			getTimers ().Add (timer);
        }

        /**
         * Returns a scene with the given id.
         * 
         * @param sceneId
         *            Scene id
         * @return Scene requested, null if it was not found
         */

        public Scene getScene(string sceneId)
        {

            Scene selectedScene = null;

			foreach (Scene scene in getScenes ())
                if (scene.getId().Equals(sceneId))
                    selectedScene = scene;

            return selectedScene;
        }

        /**
         * Returns a cutscene with the given id.
         * 
         * @param sceneId
         *            Scene id
         * @return Scene requested, null if it was not found
         */

        public Cutscene getCutscene(string sceneId)
        {

            Cutscene selectedScene = null;

			foreach (Cutscene scene in getCutscenes())
                if (scene.getId().Equals(sceneId))
                {
                    selectedScene = scene;
                    break;
                }

            return selectedScene;
        }

        /**
         * Returns an item with the given id.
         * 
         * @param itemId
         *            Item id
         * @return Item requested, null if it was not found
         */

        public Item getItem(string itemId)
        {

            Item selectedItem = null;

			foreach (Item item in getItems ())
                if (item.getId().Equals(itemId))
                    selectedItem = item;

            return selectedItem;
        }

        /**
         * Returns an atrezzo item with the given id.
         * 
         * @param atrezzoId
         *            Atrezzo id
         * @return Atrezzo item requested, null if it was not found
         */

        public Atrezzo getAtrezzo(string atrezzoId)
        {

            Atrezzo selectedAtrezzo = null;

			foreach (Atrezzo at in getObjects<Atrezzo> ())
                if (at.getId().Equals(atrezzoId))
                    selectedAtrezzo = at;

            return selectedAtrezzo;
        }

        /**
         * Returns a character with the given id.
         * 
         * @param npcId
         *            Character id
         * @return Character requested, null if it was not found
         */

        public NPC getCharacter(string npcId)
        {

            NPC selectedNPC = null;

			foreach (NPC npc in getCharacters())
                if (npc.getId().Equals(npcId))
                    selectedNPC = npc;

            return selectedNPC;
        }

        /**
         * Returns a global state with the given id.
         * 
         * @param globalStateId
         *            Global State id
         * @return GlobalState requested, null if it was not found
         */

        public GlobalState getGlobalState(string globalStateId)
        {

            GlobalState selectedGlobalState = null;

			foreach (GlobalState gs in getGlobalStates ())
                if (gs.getId().Equals(globalStateId))
                    selectedGlobalState = gs;

            return selectedGlobalState;
        }

        /**
         * Returns a macro with the given id.
         * 
         * @param macroId
         *            Macro id
         * @return Macro requested, null if it was not found
         */

        public Macro getMacro(string macroId)
        {

            Macro selectedMacro = null;

			foreach (Macro m in getMacros ())
                if (m.getId().Equals(macroId))
                    selectedMacro = m;

            return selectedMacro;
        }

        /**
         * Returns the list of timers (blocks of effects ruled by conditions which
         * will get executed each TIME seconds
         * 
         * @return The list of timers
         */

        public List<Timer> getTimers()
        {
			return getObjects<Timer> ();
        }

        /**
         * Set the list of timers
         * 
         * @param timers
         *            The new list of timers
         * @see #getTimers()
         */

        public void setTimers(List<Timer> timers)
        {
			this.extensionObjects [typeof(Timer)] = timers;
        }

        /**
         * Returns the list of flags in the game
         * 
         * @return the list of flags in the game
         */

        public List<string> getFlags()
        {
            return flags;
        }

        /**
         * Returns the list of vars in the game
         * 
         * @return the list of vars in the game
         */

        public List<string> getVars()
        {
            return vars;
        }

        /**
         * Adds a flag to the list of flags in the game
         * 
         * @param flag
         *            the flag to add
         */

        public void addFlag(string flag)
        {

            if (!flags.Contains(flag))
                flags.Add(flag);
        }

        /**
         * Adds a var to the list of Vars in the game
         * 
         * @param Var
         *            the var to add
         */

        public void addVar(string var)
        {

            if (!vars.Contains(var))
                vars.Add(var);
        }

        /**
         * Returns the scene with the given id. If the scene is not found, null is
         * returned
         * 
         * @param generalSceneId
         *            the id of the scene to find
         * @return the scene with the given id
         */

        public GeneralScene getGeneralScene(string generalSceneId)
        {

            GeneralScene scene = getScene(generalSceneId);
            if (scene == null)
                scene = getCutscene(generalSceneId);

            return scene;
        }

        /**
         * Returns the list of general scenes in the game
         * 
         * @return the list of general scenes in the game
         */

        public List<GeneralScene> getGeneralScenes()
        {

            List<GeneralScene> generalScenes = new List<GeneralScene>();
			foreach (Scene scene in getScenes ())
            {
                generalScenes.Add(scene);
            }
			foreach (Cutscene cutscene in getCutscenes ())
            {
                generalScenes.Add(cutscene);
            }
            return generalScenes;
        }

        /**
         * Returns an book with the given id.
         * 
         * @param bookId
         *            book id
         * @return book requested, null if it was not found
         */

        public Book getBook(string bookId)
        {

            Book selectedbook = null;

			foreach (Book book in getBooks ())
                if (book.getId().Equals(bookId))
                    selectedbook = book;

            return selectedbook;
        }

        /**
         * Returns a Conversation with the given id.
         * 
         * @param ConversationId
         *            Conversation id
         * @return Conversation requested, null if it was not found
         */

        public Conversation getConversation(string conversationId)
        {

            Conversation selectedConversation = null;

			foreach (Conversation conversation in getConversations())
                if (conversation.getId().Equals(conversationId))
                    selectedConversation = conversation;

            return selectedConversation;
        }

        /**
         * Returns true if the argumented id matches to a cutscene
         */

        public bool isCutscene(string id)
        {

            return getCutscene(id) != null;
        }

        /**
         * @return the globalStates
         */

        public List<GlobalState> getGlobalStates()
		{

			return getObjects<GlobalState> ();
        }

        /**
         * @param globalStates
         *            the globalStates to set
         */

        public void setGlobalStates(List<GlobalState> globalStates)
		{
			this.extensionObjects [typeof(GlobalState)] = globalStates;
        }

        /**
         * @return the macros
         */

        public List<Macro> getMacros()
        {

			return getObjects<Macro> ();
        }

        /**
         * @param macros
         *            the macros to set
         */

        public void setMacros(List<Macro> macros)
		{
			this.extensionObjects [typeof(Macro)] = macros;
        }

		public object findObject(string id){
			/*return from o in getObjects ()
			       where (o is HasId) && ((o as HasId).getId () == id)
			       select o;*/

			return getObjects ()
				.FindAll (o => o is HasId)
				.ConvertAll (o => o as HasId)
				.Find (o => o.getId () == id);
		}

		public T findObject<T>(string id){
			return getObjects<T> ()
				.FindAll (o => o is HasId)
				.Find (o => (o as HasId).getId () == id);
		}

        /// <summary>
        /// Obtain extension objects by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> getObjects<T>()
        {
            if (!extensionObjects.ContainsKey(typeof(T)))
            {
                var exists = new List<T>(getObjects().FindAll(o => o is T).Cast<T>());
                if (exists.Count == 0)
                    extensionObjects.Add(typeof(T), new List<T>());
                else
                    return exists;
            }

            return extensionObjects[typeof(T)] as List<T>;
        }

        public List<object> getObjects()
        {

            // Let's return all the objects
            List<object> allObjects = new List<object>();
            
            allObjects.Add(player);
            allObjects.AddRange(flags.Cast<object>());
            allObjects.AddRange(vars.Cast<object>());

            foreach (var kp in extensionObjects)
            {
                allObjects.AddRange(kp.Value.Cast<object>());
            }

            return allObjects;
        }

        public IList getObjects(Type t)
        {
            var listType = typeof(List<>).MakeGenericType(t);
                
            if (!extensionObjects.ContainsKey(t))
                extensionObjects.Add(t, Activator.CreateInstance(listType) as IList);

            return extensionObjects[t] as IList;
        }

        public List<Type> getObjectTypes()
        {
            return new List<Type>(extensionObjects.Keys);
        }


        public override object Clone()
        {
            Chapter c = (Chapter)base.Clone();
            if (flags != null)
            {
                c.flags = new List<string>();
                foreach (string s in flags)
                    c.flags.Add(s);
            }
            c.initialScene = (initialScene != null ? initialScene : null);
            c.player = (player != null ? (Player)player.Clone() : null);
            if (vars != null)
            {
                c.vars = new List<string>();
                foreach (string s in vars)
                    c.vars.Add(s);
            }
            if (assessmentProfiles != null)
            {
                c.assessmentProfiles = new List<AssessmentProfile>();
                foreach (AssessmentProfile ap in assessmentProfiles)
                    c.assessmentProfiles.Add(ap);
            }

            if (adaptationProfiles != null)
            {
                c.adaptationProfiles = new List<AdaptationProfile>();
                foreach (AdaptationProfile ap in adaptationProfiles)
                    c.adaptationProfiles.Add(ap);
            }

            if (extensionObjects != null)
            {
                c.extensionObjects = new Dictionary<Type, IList>();
                foreach (var eo in extensionObjects)
                {
                    var listType = typeof(List<>).MakeGenericType(eo.Key);
                    var l = Activator.CreateInstance(listType) as IList;
                    
                    foreach(var o in eo.Value)
                    {
                        if (o is ICloneable)
                            l.Add((o as ICloneable).Clone());
                        else
                            l.Add(o);
                    }

                    c.extensionObjects.Add(eo.Key, l);
                }
            }

            return c;
        }
    }
}