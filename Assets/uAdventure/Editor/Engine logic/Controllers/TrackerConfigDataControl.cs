using System;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Core;

namespace uAdventure.Editor
{
    public class TrackerConfigDataControl
    {
        private TrackerConfig trackerConfig;

        public TrackerConfigDataControl(AdventureData adventureData)
        {
            this.trackerConfig = adventureData.getTrackerConfig();
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
            this.trackerConfig.setRawCopy(rawCopy);
        }
        public void setFlushInterval(int flushInterval)
        {
            this.trackerConfig.setFlushInterval(flushInterval);
        }
        public void setTraceFormat(TrackerConfig.TraceFormat traceFormat)
        {
            this.trackerConfig.setTraceFormat(traceFormat);
        }
        public void setStorageType(TrackerConfig.StorageType storageType)
        {
            this.trackerConfig.setStorageType(storageType);
        }
        public void setHost(string host)
        {
            this.trackerConfig.setHost(host);
        }
        public void setTrackingCode(string trackingCode)
        {
            this.trackerConfig.setTrackingCode(trackingCode);
        }
        public void setDebug(bool debug)
        {
            this.trackerConfig.setDebug(debug);
        }
        #endregion setters
        //#################################################
        //#################################################
        //#################################################
    }
}