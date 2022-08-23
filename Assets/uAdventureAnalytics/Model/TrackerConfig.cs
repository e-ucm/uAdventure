

namespace uAdventure.Analytics
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
        private string basePath = null;
        private string loginEndpoint = null;
        private string startEndpoint = null;
        private string trackEndpoint = null;
        private string backupEndpoint = null;
        private string backupFileName = null;

        private bool useBearerOnTrackEndpoint = false;

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
        public string getBasePath()
        {
            return this.basePath;
        }
        public string getLoginEndpoint()
        {
            return this.loginEndpoint;
        }
        public string getStartEndpoint()
        {
            return this.startEndpoint;
        }
        public string getTrackEndpoint()
        {
            return this.trackEndpoint;
        }
        public string getBackupEndpoint()
        {
            return this.backupEndpoint;
        }
        public string getBackupFileName()
        {
            return this.backupFileName;
        }
        public bool getUseBearerOnTrackEndpoint()
        {
            return this.useBearerOnTrackEndpoint;
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
        public void setBasePath(string basePath)
        {
            this.basePath = basePath;
        }
        public void setLoginEndpoint(string loginEndpoint)
        {
            this.loginEndpoint = loginEndpoint;
        }
        public void setStartEndpoint(string startEndpoint)
        {
            this.startEndpoint = startEndpoint;
        }
        public void setTrackEndpoint(string trackEndpoint)
        {
            this.trackEndpoint = trackEndpoint;
        }
        public void setBackupEndpoint(string backupEndpoint)
        {
            this.backupEndpoint = backupEndpoint;
        }
        public void setBackupFileName(string backupFileName)
        {
            this.backupFileName = backupFileName;
        }
        public void setUseBearerOnTrackEndpoint(bool useBearerOnTrackEndpoint)
        {
            this.useBearerOnTrackEndpoint = useBearerOnTrackEndpoint;
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
