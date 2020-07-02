
using System.Collections.Generic;

using uAdventure.Core;
using System;

namespace uAdventure.Editor
{
    /**
     * This abstract class checks the operations that can be performed onto the data
     * elements of the script. Each class must hold the element that is going to be
     * checked.
     */
    public abstract class DataControl : Searchable, IObservable<DataControl>
    {
        private readonly List<DataControlDisposable> disposables;

        /**
         * Link to the main controller.
         */
        protected Controller controller;

        protected bool justCreated;

        /**
         * Constructor.
         */
        protected DataControl()
        {
            disposables = new List<DataControlDisposable>();
            controller = Controller.Instance;
        }

        /**
         * Returns the content element of the data controller.
         * 
         * @return Element being controlled
         */
        public abstract System.Object getContent();

        /**
         * Returns an array of int which holds the types of data that can be added
         * as children to the given data type.
         * 
         * @return Array of data types
         */
        public abstract int[] getAddableElements();

        /**
         * Returns whether the element accepts new elements at this time.
         * 
         * @return True if new elements can be added, false otherwise
         */
        public virtual bool canAddElements()
        {

            int[] addableElements = getAddableElements();
            if (addableElements == null)
            {
                return false;
            }

            bool canAddElements = false;

            // If at least one type of element can be added, return true
            foreach (int type in addableElements)
            {
                canAddElements = canAddElements || canAddElement(type);
            }

            return canAddElements;
        }

        /**
         * Returns wheter the element accepts new elements of a given type.
         * 
         * @param type
         *            Type of element we want to check.
         * @return True if the element can be added, false otherwise
         */
        public abstract bool canAddElement(int type);

        /**
         * Returns whether the element can be deleted.
         * 
         * @return True if the element can be deleted, false otherwise
         */
        public abstract bool canBeDeleted();

        /**
         * Returns whether the element can be duplicated.
         * 
         * @return True if the element can be duplicated, false otherwise
         */
        public abstract bool canBeDuplicated();

        /**
         * Returns whether the element can be moved in the structure in which it's
         * placed.
         * 
         * @return True if the element can be moved, false otherwise
         */
        public abstract bool canBeMoved();

        /**
         * Returns whether the element can be renamed.
         * 
         * @return True if the element can be renamed, false otherwises
         */
        public abstract bool canBeRenamed();

        /**
         * Adds a new element of the given type to the element.
         * 
         * @param type
         *            The new type of element we want to add
         * @return True if the element was added succesfully, false otherwise
         */
        public abstract bool addElement(int type, string id);

        public virtual string getDefaultId(int type)
        {
            return "id";
        }

        /**
         * Deletes a given element from the current element.
         * 
         * @param dataControl
         *            Data controller which contains the element
         * @return True if the element was deleted, false otherwise
         */
        public abstract bool deleteElement(DataControl dataControl, bool askConfirmation);

        /**
         * Duplicates a given element from the current element.
         * 
         * @param dataControl
         *            Data controller which contains the element
         * @return True if the element was deleted, false otherwise
         */
        public virtual bool duplicateElement(DataControl dataControl)
        {

            return false;
        }

        /**
         * Moves a given element to the previous position in the structure of the
         * current element.
         * 
         * @param dataControl
         *            Data controller which contains the element
         * @return True if the element was moved, false otherwise
         */
        public abstract bool moveElementUp(DataControl dataControl);

        /**
         * Moves a given element to the previous position in the structure of the
         * current element.
         * 
         * @param dataControl
         *            Data controller which contains the element
         * @return True if the element was moved, false otherwise
         */
        public abstract bool moveElementDown(DataControl dataControl);

        /**
         * Asks the user for a new ID for the element if newName is null or user it
         * otherwise.
         * 
         * @param newName
         * @return The old name if the element was renamed, null otherwise
         */
        public abstract string renameElement(string newName);

        /**
         * Updates the given flag summary, adding the flag references contained in
         * the elements. This method works recursively.
         * 
         * @param varFlagSummary
         *            Flag summary to update. It is important to point that the main
         *            element must clear the flag summary first, in order to provide
         *            clean data
         */
        public abstract void updateVarFlagSummary(VarFlagSummary varFlagSummary);

        /**
         * Returns if the data structure pending from the element is valid or not.
         * This method works recursively.
         * 
         * @param currentPath
         *            string with the path to the given element (including the
         *            element)
         * @param incidences
         *            List to store the incidences in the elements. Null if no
         *            incidences track must be stored
         * @return True if the data structure pending from the element is valid,
         *         false otherwise
         */
        public abstract bool isValid(string currentPath, List<string> incidences);

        /**
         * Counts all the references to a given asset. This method works
         * recursively.
         * 
         * @param assetPath
         *            Path to the asset (relative to the ZIP), without suffix in
         *            case of an animation or set of slides
         * @return Number of references to the given asset
         */
        public abstract int countAssetReferences(string assetPath);

        /**
         * Produces a list with all the referenced assets in the data control. This
         * method works recursively.
         * 
         * @param assetPaths
         *            The list with all the asset references. The list will only
         *            contain each asset path once, even if it is referenced more
         *            than once.
         * @param assetTypes
         *            The types of the assets contained in assetPaths.
         */
        public abstract void getAssetReferences(List<string> assetPaths, List<int> assetTypes);

        /**
         * Searchs all the references to the given path, and deletes them. This
         * method works recursively.
         * 
         * @param assetPath
         *            Path to the asset (relative to the ZIP), without suffix in
         *            case of an animation or set of slides
         */
        public abstract void deleteAssetReferences(string assetPath);

        /**
         * Counts all the references to a given identifier. This method works
         * recursively.
         * 
         * @param id
         *            Identifier to which the references must be found
         * @return Number of references to the given identifier
         */
        public abstract int countIdentifierReferences(string id);

        /**
         * Searchs all the references to a given identifier, and replaces them with
         * another one. This method works recursively.
         * 
         * @param oldId
         *            Identifier to be replaced
         * @param newId
         *            Identifier to replace the old one
         */
        public abstract void replaceIdentifierReferences(string oldId, string newId);

        /**
         * Searchs all the references to a given identifier and deletes them. This
         * method works recursively.
         * 
         * @param id
         *            Identifier to be deleted
         */
        public abstract void deleteIdentifierReferences(string id);

        public void setJustCreated(bool justCreated)
        {

            this.justCreated = justCreated;
        }

        public bool isJustCreated()
        {

            return justCreated;
        }

        protected override List<Searchable> getPath(Searchable dataControl)
        {

            if (dataControl == this)
            {
                List<Searchable> path = new List<Searchable>();
                path.Add(this);
                return path;
            }
            return getPathToDataControl(dataControl);
        }

        protected string performRenameElement<T>(string newId) where T : HasId
        {
            var elem = ((T)getContent());
            string oldId = elem.getId();

            // If some value was typed and the identifiers are different
            if (!controller.isElementIdValid(newId))
                newId = controller.makeElementValid(newId);

            elem.setId(newId);
            controller.replaceIdentifierReferences(oldId, newId);
            controller.IdentifierSummary.deleteId<T>(oldId);
            controller.IdentifierSummary.addId<T>(newId);
            Changed();

            return newId;
        }

        public abstract List<Searchable> getPathToDataControl(Searchable dataControl);

		public virtual void MoveElement(DataControl element, int fromPos, int toPos){
			var direction = fromPos > toPos ? -1 : 1;
			if (fromPos != -1) {
				while (fromPos != toPos) {
					if (direction > 0) moveElementDown (element);
					else moveElementUp (element);
					fromPos += direction;
				}
			}
		}

        protected void Changed()
        {
            disposables.ForEach(d => d.Observer.OnNext(this));
        }

        public System.IDisposable Subscribe(IObserver<DataControl> observer)
        {
            return new DataControlDisposable(this, observer);
        }

        public sealed class DataControlDisposable : System.IDisposable
        {
            private readonly IObserver<DataControl> observer;
            private readonly DataControl toObserve;

            public DataControlDisposable(DataControl toObserve, IObserver<DataControl> observer)
            {
                this.toObserve = toObserve;
                toObserve.disposables.Add(this);
                this.observer = observer;
            }

            ~DataControlDisposable()
            {
                Dispose();
            }

            public void Dispose()
            {
                toObserve.disposables.Remove(this);
            }

            public IObserver<DataControl> Observer { get { return observer; } }
        }
    }
}