using System;
using System.Collections.Generic;
using System.Linq;
using uAdventure.Runner;
using UnityEngine;

namespace uAdventure.Core
{
    public class TrackerConfig
    {
        public enum TraceFormat { CSV, XAPI };
        public enum StorageType { LOCAL, NET };

        private bool rawCopy = true;
        private int flushInterval = 3;
        private TraceFormat traceFormat = TraceFormat.CSV;
        private StorageType storageType = StorageType.LOCAL;

        private string host = null;
        private string trackingCode = null;

        private bool debug = false;

        //#################################################
        //#################### GETTERS ####################
        //#################################################
        #region getters
        public bool getRawCopy()
        {
            return this.rawCopy;
        }
        public int getFlushInterval()
        {
            return this.flushInterval;
        }
        public TraceFormat getTraceFormat()
        {
            return this.traceFormat;
        }
        public StorageType getStorageType()
        {
            return this.storageType;
        }
        public string getHost()
        {
            return this.host;
        }
        public string getTrackingCode()
        {
            return this.trackingCode;
        }
        public bool getDebug()
        {
            return this.debug;
        }
        #endregion getters
        //#################################################
        //#################### SETTERS ####################
        //#################################################
        #region setters
        public void setRawCopy(bool rawCopy)
        {
            this.rawCopy = rawCopy;
        }
        public void setFlushInterval(int flushInterval)
        {
            this.flushInterval = flushInterval;
        }
        public void setTraceFormat(TraceFormat traceFormat)
        {
            this.traceFormat = traceFormat;
        }
        public void setStorageType(StorageType storageType)
        {
            this.storageType = storageType;
        }
        public void setHost(string host)
        {
            this.host = host;
        }
        public void setTrackingCode(string trackingCode)
        {
            this.trackingCode = trackingCode;
        }
        public void setDebug(bool debug)
        {
            this.debug = debug;
        }
        #endregion setters
        //#################################################
        //#################################################
        //#################################################
    }
}
