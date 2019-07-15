using System.Collections.Generic;
using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.QR
{
    public class QRCodeDataControl : DataControl
    {
        private readonly QR qr;
        private readonly ConditionsController conditionsController;
        private readonly EffectsController effectsController;

        public QRCodeDataControl(QR qr)
        {
            this.qr = qr;
            this.conditionsController = new ConditionsController(qr.Conditions);
            this.effectsController = new EffectsController(qr.Effects);
        }

        public override bool addElement(int type, string id)
        {
            return false;
        }

        public override bool canAddElement(int type)
        {
            return false;
        }

        public override bool canBeDeleted()
        {
            return true;
        }

        public override bool canBeDuplicated()
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

        public override int countAssetReferences(string assetPath)
        {
            return EffectsController.countAssetReferences(assetPath, effectsController.getEffectsDirectly());
        }

        public override int countIdentifierReferences(string id)
        {
            return EffectsController.countIdentifierReferences(id, effectsController.getEffectsDirectly());
        }

        public override void deleteAssetReferences(string assetPath)
        {
            EffectsController.deleteAssetReferences(assetPath, effectsController.getEffectsDirectly());
        }

        public override bool deleteElement(DataControl dataControl, bool askConfirmation)
        {
            return false;
        }

        public override void deleteIdentifierReferences(string id)
        {
            EffectsController.deleteIdentifierReferences(id, effectsController.getEffectsDirectly());
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
            EffectsController.getAssetReferences(assetPaths, assetTypes, effectsController.getEffectsDirectly());
        }

        public override object getContent()
        {
            return qr;
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return null;
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return EffectsController.isValid(currentPath + " >> " + TC.get("Element.Effects"), incidences, Effects.getEffectsDirectly());
        }

        public override bool moveElementDown(DataControl dataControl)
        {
            return false;
        }

        public override bool moveElementUp(DataControl dataControl)
        {
            return false;
        }

        public override void recursiveSearch()
        {
            check(this.Id, Language.GetText("Search.ID"));
            check(this.Content, Language.GetText("Search.Title"));
            check(this.Documentation, Language.GetText("Search.Documentation"));
            check(this.Conditions, TC.get("Search.Conditions"));
            for (int i = 0; i < this.Effects.getEffectCount(); i++)
            {
                check(this.Effects.getEffectInfo(i), TC.get("Search.Effect"));
                check(this.Effects.getConditionController(i), TC.get("Search.Conditions"));
            }
        }

        public override string renameElement(string newName)
        {
            string oldQRId = qr.getId();
            string references = controller.countIdentifierReferences(oldQRId).ToString();

            // Ask for confirmation
            if (newName != null || controller.ShowStrictConfirmDialog(TC.get("Operation.RenameItemTitle"), TC.get("Operation.RenameElementWarning", new string[] { oldQRId, references })))
            {

                // Show a dialog asking for the new item id
                if (newName == null)
                {
                    controller.ShowInputDialog(TC.get("Operation.RenameItemTitle"), TC.get("Operation.RenameItemMessage"), oldQRId, (o, s) => performRenameElement(s));
                }
                else
                {
                    return performRenameElement(newName);
                }
            }

            return null;
        }

        private string performRenameElement(string newQRId)
        {
            string oldQRId = qr.getId();


            // If some value was typed and the identifiers are different
            if (!controller.isElementIdValid(newQRId))
            {
                newQRId = controller.makeElementValid(newQRId);
            }

            qr.setId(newQRId);
            controller.replaceIdentifierReferences(oldQRId, newQRId);
            controller.IdentifierSummary.deleteId<QR>(oldQRId);
            controller.IdentifierSummary.addId<QR>(newQRId);
            controller.DataModified();

            return newQRId;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
            EffectsController.replaceIdentifierReferences(oldId, newId, effectsController.getEffectsDirectly());
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            EffectsController.updateVarFlagSummary(varFlagSummary, effectsController.getEffectsDirectly());
            ConditionsController.updateVarFlagSummary(varFlagSummary, conditionsController.Conditions);
        }

        // ###################################################

        public ConditionsController Conditions
        {
            get { return conditionsController; }
        }

        public EffectsController Effects
        {
            get { return effectsController; }
        }

        public string Id
        {
            get { return qr.Id; }
        }

        public string Documentation
        {
            get { return qr.Documentation; }
            set { controller.AddTool(new ChangeDocumentationTool(qr, value)); }
        }

        public string Content
        {
            get { return qr.Content; }
            set { controller.AddTool(new ChangeValueTool<QR, string>(qr, value, "Content")); }
        }
    }
}
