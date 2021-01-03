
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;
using UnityEngine;

namespace uAdventure.Editor
{
    public interface IElementManagerDataControl<in TD>
        where TD : DataControl
    {
        bool CanAddElement(TD parent, int type);

        bool AddElement(TD parent, int type, string id);
        
        int[] ElementTypes { get; }

    }

    public interface IElementFactory<out TT>
        where TT : DataControl
    {
        int[] ElementTypes { get; }

        TT CreateDataControl(object o);

        TT CreateElement(int type, string id, params object[] extraParams);

        bool HandlesType(Type t);

        bool RequiresId(int type);

        bool ReferencesId(int type);

        void GatherExtraParameters(int type, Action<object[]> callback);

        Type[] ValidReferencedTypes(int type);
    }

    public class DefaultElementFactory<TT> : IElementFactory<TT>
        where TT : DataControl
    {
        public class ElementCreator
        {
            public class TypeDescriptor
            {
                public Type ContentType { get; set; }
                public int Type { get; set; }
                public bool RequiresId { get; set; }
                public bool ReferencesId { get; set; }
                public Type[] ValidReferenceTypes { get; set; }
                public Action<Action<object>>[] ExtraParameters { get; set; }
            }

            public delegate TT CreateDataControlDelegate(object o);
            public CreateDataControlDelegate CreateDataControl;

            public delegate TT CreateElementDelegate(int type, string id, params object[] extraParams);
            public CreateElementDelegate CreateElement;

            public TypeDescriptor[] TypeDescriptors { get; set; }

        }

        private readonly ElementCreator[] elementCreators;

        public DefaultElementFactory(params ElementCreator[] elementCreators)
        {
            this.elementCreators = elementCreators;
        }

        public int[] ElementTypes
        {
            get { return elementCreators.SelectMany(ec => ec.TypeDescriptors.Select(td => td.Type)).ToArray(); }
        }

        public bool HandlesType(Type t)
        {
            return elementCreators.Any(ec => ec.TypeDescriptors.Any(td => td.ContentType == t));
        }

        public TT CreateDataControl(object o)
        {
            var dataControl = elementCreators.First(ec => ec.TypeDescriptors.Any(td => td.ContentType == o.GetType())).CreateDataControl(o);
            var hasId = o as HasId;
            if (dataControl != null && hasId != null)
            {
                Controller.Instance.IdentifierSummary.addId(o.GetType(), hasId.getId());
            }

            return dataControl;
        }

        public TT CreateElement(int type, string id, params object[] extraParams)
        {
            var ET = elementCreators
                .SelectMany(ec => ec.TypeDescriptors.Select(td => new { ElementCreator = ec, TypeDescriptor = td }))
                .Where(a => a.TypeDescriptor.Type == type)
                .FirstOrDefault();

            if(ET == null)
            {
                throw new Exception(string.Format("No element creator found for type {0}!", type));
            }

            if (ET.TypeDescriptor.RequiresId && !string.IsNullOrEmpty(id) && !Controller.Instance.isElementIdValid(id))
            {
                id = Controller.Instance.makeElementValid(id);
            }

            var dataControl = ET.ElementCreator.CreateElement(type, id, extraParams);

            if (ET.TypeDescriptor.RequiresId && dataControl != null)
            {
                var hasId = dataControl.getContent() as HasId;
                if (hasId != null)
                {
                    Controller.Instance.IdentifierSummary.addId(hasId.GetType(), hasId.getId());
                }
            }

            return dataControl;
        }

        public bool RequiresId(int type)
        {
            var typeDescriptor = GetTypeDescriptor(type);
            return typeDescriptor != null && typeDescriptor.RequiresId;
        }

        public bool ReferencesId(int type)
        {
            var typeDescriptor = GetTypeDescriptor(type);
            return typeDescriptor != null && typeDescriptor.ReferencesId;
        }

        public void GatherExtraParameters(int type, Action<object[]> callback)
        {
            var typeDescriptor = GetTypeDescriptor(type);
            if (typeDescriptor != null && typeDescriptor.ExtraParameters != null)
            {
                new ParameterGatherer(typeDescriptor.ExtraParameters, callback).DoGathering();
            }
            else
            {
                callback(new object[] { });
            }
        }


        public Type[] ValidReferencedTypes(int type)
        {
            var typeDescriptor = GetTypeDescriptor(type);
            return typeDescriptor != null ? typeDescriptor.ValidReferenceTypes : null;
        }

        private ElementCreator.TypeDescriptor GetTypeDescriptor(int type)
        {
            return elementCreators.SelectMany(ec => ec.TypeDescriptors).First(td => td.Type == type);
        }

        private class ParameterGatherer
        {
            private readonly IEnumerator parametersEnumerator;
            private readonly object[] results;
            private readonly Action<object[]> callback;
            private int currentParameter;


            public ParameterGatherer(Action<Action<object>>[] parameters, Action<object[]> callback)
            {
                parametersEnumerator = parameters.GetEnumerator();
                results = new object[parameters.Length];
                this.callback = callback;
            }

            public void DoGathering()
            {
                parametersEnumerator.Reset();
                currentParameter = 0;
                MoveNext();
            }

            private void MoveNext()
            {
                if (parametersEnumerator.MoveNext())
                {
                    var nextParam = parametersEnumerator.Current as Action<Action<object>>;
                    nextParam(AddResultAndContinue);
                }
                else
                {
                    callback(results.ToArray());
                }
            }

            private void AddResultAndContinue(object result)
            {
                results[currentParameter] = result;
                currentParameter++;
                MoveNext();
            }
        }
    }

    /*public class ListElementManager<TD, TT> : IElementManagerDataControl<TD>
        where TD : DataControl
        where TT : DataControl
    {


        private ElementFactoryView[] elementFactoryViews;

        public ListElementManager(params ElementFactoryView[] elementFactoryViews)
        {
            this.elementFactoryViews = elementFactoryViews;
        }

        public int[] ElementTypes
        {
            get { return elementFactoryViews.Select(efv => efv.ElementFactory).SelectMany(ef => ef.ElementTypes).ToArray(); }
        }

        public bool AddElement(TD parent, int type, string id)
        {
            throw new NotImplementedException();
        }

        public bool CanAddElement(TD parent, int type)
        {
            throw new NotImplementedException();
        }
    }*/

    public class ListDataControl<TD, TT> : DataControl
        where TD : DataControl 
        where TT : DataControl
    {

        public class ElementFactoryView
        {
            public ElementFactoryView()
            {
                Titles = new Dictionary<int, string>();
                DefaultIds = new Dictionary<int, string>();
                Messages = new Dictionary<int, string>();
                Errors = new Dictionary<int, string>();
            }

            public Dictionary<int, string> Titles { get; set; }
            public Dictionary<int, string> DefaultIds { get; set; }
            public Dictionary<int, string> Messages { get; set; }
            public Dictionary<int, string> Errors { get; set; }
            public IElementFactory<TT> ElementFactory { get; set; }

        }

        private readonly TD parent;
        private readonly IList elements;
        private readonly ElementFactoryView[] elementFactoryViews;
        private readonly List<TT> dataControls;
        public bool CanDeleteLastElement { get; set; } = true;

        public ListDataControl(TD parent, IList elements, params ElementFactoryView[] elementFactoryViews)
        {
            this.parent = parent;
            this.elements = elements;
            this.elementFactoryViews = elementFactoryViews;
            this.dataControls = new List<TT>();
            foreach (var e in elements)
            {
                var dataControl = CreateDataControl(e);
                if (dataControl == null)
                {
                    Debug.LogWarning("Cannot create DataControl for " + e.ToString());
                }

                dataControls.Add(dataControl);
            }
        }

        private ElementFactoryView GetFactoryFor(int type)
        {
            return elementFactoryViews.FirstOrDefault(efv => efv.ElementFactory.ElementTypes.Contains(type));
        }

        private TT CreateDataControl(object o)
        {
            var factory = elementFactoryViews.FirstOrDefault(efv => efv.ElementFactory.HandlesType(o.GetType()));
            return factory == null ? null : factory.ElementFactory.CreateDataControl(o);
        }

        public TT this[int index]
        {
            get { return DataControls[index]; }
        }

        public List<TT> DataControls
        {
            get { return dataControls; }
        }


        public override bool addElement(int type, string id)
        {
            var elementFactoryView = GetFactoryFor(type);
            if (elementFactoryView == null)
            {
                return false;
            }

            var elementFactory = elementFactoryView.ElementFactory;

            if (string.IsNullOrEmpty(id) && (elementFactory.RequiresId(type) || elementFactory.ReferencesId(type)))
            {
                var title = elementFactoryView.Titles[type].Traslate();
                var message = elementFactoryView.Messages[type].Traslate();
                if (elementFactory.RequiresId(type))
                {
                    if (elementFactoryView.DefaultIds.ContainsKey(type))
                    {
                        controller.ShowInputIdDialog(title, message, Controller.Instance.makeElementValid(elementFactoryView.DefaultIds[type]), (o, s) => continueAddingElement(type, s));
                    }
                    else
                    {
                        controller.ShowInputIdDialog(title, message, "", (o, s) => continueAddingElement(type, s));
                    }
                }
                else if (elementFactory.ReferencesId(type))
                {
                    var ids = controller.IdentifierSummary.combineIds(elementFactory.ValidReferencedTypes(type));
                    controller.ShowInputDialog(title, message, ids.Cast<object>().ToArray(), (o, s) => continueAddingElement(type, s));
                }

                return true;
            }



            return continueAddingElement(type, id);
        }

        private bool continueAddingElement(int type, string id)
        {
            var elementFactoryView = GetFactoryFor(type);
            if (elementFactoryView == null)
            {
                return false;
            }

            var r = false;
            elementFactoryView.ElementFactory.GatherExtraParameters(type, objects =>
                {
                    r = performAddElement(type, id, objects);
                });

            return r;
        }

        private bool performAddElement(int type, string id, params object[] extraParams)
        {
            var elementFactoryView = GetFactoryFor(type);
            if (elementFactoryView == null)
            {
                return false;
            }

            var dataControl = elementFactoryView.ElementFactory.CreateElement(type, id, extraParams);
            return dataControl != null && controller.AddTool(new AddRemoveElementTool(parent, elements, dataControls, dataControl, false));
        }

        public override bool canAddElement(int type)
        {
            var elementFactoryView = GetFactoryFor(type);
            if (elementFactoryView == null)
            {
                return false;
            }

            return elementFactoryView.ElementFactory.ElementTypes.Contains(type);
        }

        public override bool canBeDeleted()
        {
            return false;
        }

        public override bool canBeDuplicated()
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

        public override int countAssetReferences(string assetPath)
        {
            return dataControls.Sum(d => d.countAssetReferences(assetPath));
        }

        public override int countIdentifierReferences(string id)
        {
            return dataControls.Sum(d => d.countIdentifierReferences(id));
        }

        public override void deleteAssetReferences(string assetPath)
        {
            dataControls.ForEach(d => d.deleteAssetReferences(assetPath));
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            if(!CanDeleteLastElement && elements.Count == 1)
            {
                controller.ShowErrorDialog("Operation.CantDeleteLastElementTitle".Traslate(), "Operation.CantDeleteLastElementMessage".Traslate());
                return false;
            }

            var toRemove = dataControl as TT;
            if (toRemove == null)
            {
                return false;
            }

            var hasId = dataControl.getContent() as HasId;

            if (hasId != null && askConfirmation && !controller.ShowStrictConfirmDialog("Operation.DeleteElementTitle".Traslate(),
                    "Operation.DeleteElementWarning".Traslate(hasId.getId(), controller.countIdentifierReferences(hasId.getId()).ToString())))
            {
                return false;
            }

            return controller.AddTool(new AddRemoveElementTool(parent, elements, dataControls, toRemove, true));
        }

        public override void deleteIdentifierReferences(string id)
        {
            dataControls.ForEach(d => d.deleteIdentifierReferences(id));

            for (var i = dataControls.Count - 1; i >= 0; i--)
            {
                var targetsId = dataControls[i].getContent() as HasTargetId;
                if (targetsId != null && targetsId.getTargetId() == id)
                {
                    elements.RemoveAt(i);
                    dataControls.RemoveAt(i);
                }
            }
        }

        public override int[] getAddableElements()
        {
            return elementFactoryViews.SelectMany(efv => efv.ElementFactory.ElementTypes).ToArray();
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            dataControls.ForEach(d => d.getAssetReferences(assetPaths, assetTypes));
        }

        public override object getContent()
        {
            return elements;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return getPathFromChild(dataControl, DataControls.Cast<object>().ToList());
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            var allValid = true;

            // With the foreach we force to check all possible incidences
            dataControls.ForEach(d => allValid &= d.isValid(currentPath, incidences));

            return allValid;
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            var toMove = dataControl as TT;
            return toMove != null && controller.AddTool(new MoveElementTool(parent, elements, dataControls, toMove, false));
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            var toMove = dataControl as TT;
            return toMove != null && controller.AddTool(new MoveElementTool(parent, elements, dataControls, toMove, true));
        }

        public override void recursiveSearch()
        {
            dataControls.ForEach(d => d.recursiveSearch());
        }

        public override string renameElement(string newName)
        {
            return null;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            dataControls.ForEach(d => d.replaceIdentifierReferences(oldId, newId));
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            dataControls.ForEach(d => d.updateVarFlagSummary(varFlagSummary));
        }

        private class AddRemoveElementTool : Tool
        {
            private readonly TD parent;
            private readonly IList elements;
            private readonly List<TT> dataControls;
            private readonly TT dataControl;
            private readonly bool isRemove;

            public AddRemoveElementTool(TD parent, IList elements, List<TT> dataControls, TT dataControl, bool isRemove)
            {
                this.parent = parent;
                this.elements = elements;
                this.dataControls = dataControls;
                this.dataControl = dataControl;
                this.isRemove = isRemove;
            }

            public override bool doTool()
            {
                return isRemove ? remove() : add();
            }

            public override bool canUndo()
            {
                return true;
            }

            public override bool undoTool()
            {
                return isRemove ? add() : remove();
            }

            public override bool canRedo()
            {
                return true;
            }

            public override bool redoTool()
            {
                return doTool();
            }

            public override bool combine(Tool other)
            {
                return false;
            }

            private bool add()
            {
                var content = dataControl.getContent();

                dataControls.Add(dataControl);
                elements.Add(content);

                var hasId = content as HasId;
                if (hasId != null)
                {
                    Controller.Instance.IdentifierSummary.addId(hasId.GetType(), hasId.getId());
                }

                if (isRemove) // This means is undoing
                {
                    // Update references to var and flags in case the element contains them
                    Controller.Instance.updateVarFlagSummary();
                }

                return true;
            }

            private bool remove()
            {
                var content = dataControl.getContent();

                dataControls.Remove(dataControl);
                elements.Remove(content);

                var hasId = content as HasId;
                if (hasId != null)
                {
                    Controller.Instance.deleteIdentifierReferences(hasId.getId());
                    Controller.Instance.IdentifierSummary.deleteId(hasId.GetType(), hasId.getId());
                }

                // Update references to var and flags in case the element contains them
                Controller.Instance.updateVarFlagSummary();

                return true;
            }
        }
        private class MoveElementTool : Tool
        {
            private readonly TD parent;
            private readonly IList elements;
            private readonly List<TT> dataControls;
            private readonly TT dataControl;
            private int times;

            public MoveElementTool(TD parent, IList elements, List<TT> dataControls, TT dataControl, bool up)
            {
                this.parent = parent;
                this.elements = elements;
                this.dataControls = dataControls;
                this.dataControl = dataControl;
                this.times = up ? -1 : 1;
            }

            public override bool doTool()
            {
                return move(times);
            }

            public override bool canUndo()
            {
                return true;
            }

            public override bool undoTool()
            {
                return move(-times);
            }

            public override bool canRedo()
            {
                return true;
            }

            public override bool redoTool()
            {
                return doTool();
            }

            public override bool combine(Tool other)
            {
                var otherMove = other as MoveElementTool;
                if (otherMove == null || parent != otherMove.parent || !elements.Equals(otherMove.elements) || 
                    dataControls != otherMove.dataControls ||  dataControl != otherMove.dataControl ||
                    Math.Abs(otherMove.times + times) != Math.Abs(otherMove.times) + Math.Abs(times))
                {
                    return false;
                }

                times += otherMove.times;

                return true;
            }

            private bool move(int times)
            {
                var content = dataControl.getContent();

                var index = dataControls.IndexOf(dataControl);
                if (index == -1)
                {
                    return false;
                }

                var pos = index + times;

                if (!pos.InRange(0, dataControls.Count - 1))
                {
                    return false;
                }

                dataControls.RemoveAt(index);
                elements.RemoveAt(index);

                dataControls.Insert(pos, dataControl);
                elements.Insert(pos, content);

                return true;
            }
        }
    }
}
