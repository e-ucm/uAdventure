using System;
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
            return "InitialScene:" + initialScene + "\nScenes: " + CollectionPrinter.PrintCollection(scenes) + "\nCutscenes: " + CollectionPrinter.PrintCollection(cutscenes) + "\nBooks: " +
                   CollectionPrinter.PrintCollection(books) + "\nItems: " + CollectionPrinter.PrintCollection(items) + "\nAtrezzo: " + CollectionPrinter.PrintCollection(atrezzo) + "\n Player:" + player + "\nCharacters: " +
                   CollectionPrinter.PrintCollection(characters) + "\nConversations:" + CollectionPrinter.PrintCollection(conversations) + "\nTimers: " + CollectionPrinter.PrintCollection(timers) + "\nFlags: " + CollectionPrinter.PrintCollection(flags) + "\nVars " +
                   CollectionPrinter.PrintCollection(vars) + "\nGlobalStates: " + CollectionPrinter.PrintCollection(globalStates) + "\nMacros: " + CollectionPrinter.PrintCollection(macros);
        }

        /**
         * Identifier of the initial scene.
         */
        private string initialScene;

        /**
         * List of playable scenes.
         */
        private List<Scene> scenes;

        /**
         * List of cutscenes.
         */
        private List<Cutscene> cutscenes;

        /**
         * List of books.
         */
        private List<Book> books;

        /**
         * List of items (objects).
         */
        private List<Item> items;

        /**
         * List of atrezzo items (non interactive objects)
         */
        private List<Atrezzo> atrezzo;

        /**
         * The player.
         */
        private Player player;

        /**
         * List of characters.
         */
        private List<NPC> characters;

        /**
         * List of conversations.
         */
        private List<Conversation> conversations;

        /**
         * The list of timers (advanced options)
         */
        private List<Timer> timers;

        /**
         * List of the flags present in the game
         */
        private List<string> flags;

        /**
         * List of the vars present in the game
         */
        private List<string> vars;

        /**
         * List of global states
         */
        private List<GlobalState> globalStates;

        /**
         * List of macros
         */
        private List<Macro> macros;

        /**
         * List of completables
         */
        private List<Completable> completables;

        /**
         * Empty constructor. Sets values to null and creates empty lists.
         */

        // ------------------------------
        // EXTENSIONS
        // ------------------------------

        Dictionary<Type, IList> extensionObjects;


        public Chapter() : base()
        {
            // Create lists
            scenes = new List<Scene>();
            cutscenes = new List<Cutscene>();
            books = new List<Book>();
            items = new List<Item>();
            atrezzo = new List<Atrezzo>();
            player = new Player();
            characters = new List<NPC>();
            conversations = new List<Conversation>();
            timers = new List<Timer>();
            flags = new List<string>();
            vars = new List<string>();
            globalStates = new List<GlobalState>();
            macros = new List<Macro>();
            completables = new List<Completable>();

            extensionObjects = new Dictionary<Type, IList>();
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

            // Create lists
            scenes = new List<Scene>();
            cutscenes = new List<Cutscene>();
            books = new List<Book>();
            items = new List<Item>();
            atrezzo = new List<Atrezzo>();
            player = new Player();
            characters = new List<NPC>();
            conversations = new List<Conversation>();
            timers = new List<Timer>();
            // Add the scene
            scenes.Add(new Scene(sceneId));
            globalStates = new List<GlobalState>();
            macros = new List<Macro>();
            flags = new List<string>();
            vars = new List<string>();
            completables = new List<Completable>();

            extensionObjects = new Dictionary<Type, IList>();
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

        public GeneralScene getInitialGeneralScene()
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

        /**
         * Returns the list of playable scenes in the game.
         * 
         * @return List of playable scenes
         */

        public List<Scene> getScenes()
        {

            return scenes;
        }

        /**
         * Returns the list of cutscenes in the game.
         * 
         * @return List of cutscenes
         */

        public List<Cutscene> getCutscenes()
        {

            return cutscenes;
        }

        /**
         * Returns the list of books in the game
         * 
         * @return the list of books in the game
         */

        public List<Book> getBooks()
        {

            return books;
        }

        /**
         * Returns the list of items in the game
         * 
         * @return the list of items in the game
         */

        public List<Item> getItems()
        {

            return items;
        }

        /**
         * Returns the list of atrezzo items in the game
         * 
         * @return the list of atrezzo items in the game
         */

        public List<Atrezzo> getAtrezzo()
        {

            return atrezzo;
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

            return characters;
        }

        /**
         * Returns the list of conversations in the game
         * 
         * @return the list of conversations in the game
         */

        public List<Conversation> getConversations()
        {

            return conversations;
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

            scenes.Add(scene);
        }

        /**
         * Adds a cutscene to the list of cutscenes in the game.
         * 
         * @param cutscene
         *            The cutscene to add
         */

        public void addCutscene(Cutscene cutscene)
        {

            cutscenes.Add(cutscene);
        }

        /**
         * Adds a book to the list of book in the game
         * 
         * @param book
         *            the book to add
         */

        public void addBook(Book book)
        {

            books.Add(book);
        }

        /**
         * Adds an item to the list of items in the game
         * 
         * @param item
         *            the item to add
         */

        public void addItem(Item item)
        {

            items.Add(item);
        }

        /**
         * Adds an atrezzo item to the list of atrezzo items in the game
         * 
         * @param atrezzo
         *            the atrezzo item to add
         */

        public void addAtrezzo(Atrezzo atrezzo)
        {

            this.atrezzo.Add(atrezzo);
        }

        /**
         * Adds a global state to the list of global states in the game
         * 
         * @param globalState
         *            the global state to add
         */

        public void addGlobalState(GlobalState globalState)
        {

            globalStates.Add(globalState);
        }

        /**
         * Adds a macro to the list of macros in the game
         * 
         * @param macro
         *            the macro to add
         */

        public void addMacro(Macro macro)
        {

            macros.Add(macro);
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

            characters.Add(npc);
        }

        /**
         * Adds a conversation to the list of conversation in the game
         * 
         * @param conversation
         *            the new conversation
         */

        public void addConversation(Conversation conversation)
        {

            conversations.Add(conversation);
        }

        /**
         * Adds a timer to the list of timers in the game
         * 
         * @param timer
         *            the new timer
         */

        public void addTimer(Timer timer)
        {

            timers.Add(timer);
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

            foreach (Scene scene in scenes)
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

            foreach (Cutscene scene in cutscenes)
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

            foreach (Item item in items)
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

            foreach (Atrezzo at in atrezzo)
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

            foreach (NPC npc in characters)
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

            foreach (GlobalState gs in globalStates)
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

            foreach (Macro m in macros)
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

            return timers;
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

            this.timers = timers;
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
            foreach (Scene scene in scenes)
            {
                generalScenes.Add(scene);
            }
            foreach (Cutscene cutscene in cutscenes)
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

            foreach (Book book in books)
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

            foreach (Conversation conversation in conversations)
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

            return globalStates;
        }

        /**
         * @param globalStates
         *            the globalStates to set
         */

        public void setGlobalStates(List<GlobalState> globalStates)
        {

            this.globalStates = globalStates;
        }

        /**
         * @return the macros
         */

        public List<Macro> getMacros()
        {

            return macros;
        }

        /**
         * @param macros
         *            the macros to set
         */

        public void setMacros(List<Macro> macros)
        {

            this.macros = macros;
        }

        /**
         * set the completables
         */

        public void setCompletabes(List<Completable> completables)
        {
            this.completables = completables;
        }

        /**
         * get all the completables
         * @return the completables
         */

        public List<Completable> getCompletabes()
        {
            return this.completables;
        }

        /**
         * get all the completables
         * @param id
         * the id of the completable to find
         * @return a completable that matches the id
         */

        public Completable getCompletable(string id)
        {
            Completable selectedCompletable = null;

            foreach (Completable completable in completables)
                if (completable.getId().Equals(id))
                    selectedCompletable = completable;

            return selectedCompletable;
        }

        /**
         * adds a completable
         * @param completable
         * the completable to add into the list of completables
         */

        public void addCompletable(Completable completable)
        {
            this.completables.Add(completable);
        }

        /// <summary>
        /// Obtain extension objects by type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public List<T> getObjects<T>()
        {
            if (!extensionObjects.ContainsKey(typeof(T)))
                extensionObjects.Add(typeof(T), new List<T>());

            return extensionObjects[typeof(T)] as List<T>;
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
            if (atrezzo != null)
            {
                c.atrezzo = new List<Atrezzo>();
                foreach (Atrezzo a in atrezzo)
                    c.atrezzo.Add((Atrezzo)a.Clone());
            }
            if (books != null)
            {
                c.books = new List<Book>();
                foreach (Book b in books)
                    c.books.Add((Book)b.Clone());
            }
            if (characters != null)
            {
                c.characters = new List<NPC>();
                foreach (NPC n in characters)
                    c.characters.Add((NPC)n.Clone());
            }
            if (conversations != null)
            {
                c.conversations = new List<Conversation>();
                foreach (Conversation cc in conversations)
                    c.conversations.Add((Conversation)cc.Clone());
            }
            if (cutscenes != null)
            {
                c.cutscenes = new List<Cutscene>();
                foreach (Cutscene cs in cutscenes)
                    c.cutscenes.Add((Cutscene)cs.Clone());
            }
            if (flags != null)
            {
                c.flags = new List<string>();
                foreach (string s in flags)
                    c.flags.Add(s);
            }
            if (globalStates != null)
            {
                c.globalStates = new List<GlobalState>();
                foreach (GlobalState gs in globalStates)
                    c.globalStates.Add((GlobalState)gs.Clone());
            }
            c.initialScene = (initialScene != null ? initialScene : null);
            if (items != null)
            {
                c.items = new List<Item>();
                foreach (Item i in items)
                    c.items.Add((Item)i.Clone());
            }
            if (macros != null)
            {
                c.macros = new List<Macro>();
                foreach (Macro m in macros)
                    c.macros.Add((Macro)m.Clone());
            }
            c.player = (player != null ? (Player)player.Clone() : null);
            if (scenes != null)
            {
                c.scenes = new List<Scene>();
                foreach (Scene s in scenes)
                    c.scenes.Add((Scene)s.Clone());
            }
            if (timers != null)
            {
                c.timers = new List<Timer>();
                foreach (Timer t in timers)
                    c.timers.Add((Timer)t.Clone());
            }
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