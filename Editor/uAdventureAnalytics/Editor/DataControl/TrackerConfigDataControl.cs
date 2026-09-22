using System;
using System.Collections.Generic;
using uAdventure.Core;
using uAdventure.Editor;

namespace uAdventure.Analytics
{
    public class TrackerConfigDataControl : DataControl
    {
        private readonly TrackerConfig trackerConfig;

        public TrackerConfigDataControl(TrackerConfig trackerConfig)
        {
            this.trackerConfig = trackerConfig;
        }

        //#################################################
        //#################### GETTERS ####################
        //#################################################
        #region getters
        public bool getRawCopy()
        {
            return this.trackerConfig.getRawCopy();
        }
        public int getFlushInterval()
        {
            return this.trackerConfig.getFlushInterval();
        }
        public TrackerConfig.TraceFormat getTraceFormat()
        {
            return this.trackerConfig.getTraceFormat();
        }
        public TrackerConfig.StorageType getStorageType()
        {
            return this.trackerConfig.getStorageType();
        }
        public string getHost()
        {
            return this.trackerConfig.getHost();
        }
        public string getTrackingCode()
        {
            return this.trackerConfig.getTrackingCode();
        }
        public bool getDebug()
        {
            return this.trackerConfig.getDebug();
        }
        #endregion getters
        //#################################################
        //#################### SETTERS ####################
        //#################################################
        #region setters
        public void setRawCopy(bool rawCopy)
        {
            controller.AddTool(new ChangeBooleanValueTool(trackerConfig, rawCopy, "getRawCopy",
                "setRawCopy"));
        }
        public void setFlushInterval(int flushInterval)
        {
            controller.AddTool(new ChangeIntegerValueTool(trackerConfig, flushInterval, "getFlushInterval",
                "setFlushInterval"));
        }
        public void setTraceFormat(TrackerConfig.TraceFormat traceFormat)
        {
            controller.AddTool(ChangeEnumValueTool.Create(trackerConfig, traceFormat, "getTraceFormat",
                "setTraceFormat"));
        }
        public void setStorageType(TrackerConfig.StorageType storageType)
        {
            controller.AddTool(ChangeEnumValueTool.Create(trackerConfig, storageType, "getStorageType",
                "setStorageType"));
        }
        public void setHost(string host)
        {
            controller.AddTool(new ChangeStringValueTool(trackerConfig, host, "getHost", "setHost"));
        }
        public void setTrackingCode(string trackingCode)
        {
            controller.AddTool(new ChangeStringValueTool(trackerConfig, trackingCode, "getTrackingCode",
                "setTrackingCode"));
        }
        public void setDebug(bool debug)
        {
            controller.AddTool(new ChangeBooleanValueTool(trackerConfig, debug, "getDebug",
                "setDebug"));
        }
        #endregion setters
        //#################################################
        //#################################################
        //#################################################

        #region dataControl


        public override object getContent()
        {
            return trackerConfig;
        }

        public override int[] getAddableElements()
        {
            return null;
        }

        public override bool canAddElement(int type)
        {
            return false;
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

        public override string renameElement(string newName)
        {
            return null;
        }

        public override void updateVarFlagSummary(VarFlagSummary varFlagSummary)
        {
            throw new NotImplementedException();
        }

        public override bool isValid(string currentPath, List<string> incidences)
        {
            return true;
        }

        public override int countAssetReferences(string assetPath)
        {
            return 0;
        }

        public override void getAssetReferences(List<string> assetPaths, List<int> assetTypes)
        {
        }

        public override void deleteAssetReferences(string assetPath)
        {
        }

        public override int countIdentifierReferences(string id)
        {
            return 0;
        }

        public override void replaceIdentifierReferences(string oldId, string newId)
        {
        }

        public override void deleteIdentifierReferences(string id)
        {
        }

        public override List<Searchable> getPathToDataControl(Searchable dataControl)
        {
            return dataControl == this ? new List<Searchable> { this } : null;
        }

        public override void recursiveSearch()
        {

        }
        #endregion dataControl
    }
}