using System;
using TinCan;
using Xasu.Config;

namespace Xasu.Processors.Formatter
{
    public static class TraceFormatter
    {
        public static string Format(Statement statement, TraceFormats format, TCAPIVersion version)
        {
            switch (format)
            {
                case TraceFormats.XAPI:
                    return statement.ToJSON(version);
                default:
                case TraceFormats.CSV:
                    throw new NotSupportedException("CSV is not available in this version!");
            }
        }
    }
}
