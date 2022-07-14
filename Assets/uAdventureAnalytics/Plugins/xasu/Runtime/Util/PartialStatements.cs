using System.Collections.Generic;
using System.Linq;
using TinCan;
using Xasu.Processors;

namespace Xasu.Util
{
    /// <summary>
    /// Partial statements are a Statement extension that allows the statement to be enqueued but not sent until completed.
    /// Depending on the configuration, the Statement will block the entire queue until it is completed or not.
    /// 
    /// This behavior can be configured in the "tracker.conf" file.
    /// 
    ///  i.e.: "partial-statements-block"=true/false
    ///  
    /// A partial statement can be used to preserve trace order in creation and submission (for systems that may require it).
    /// Use cases include when the interaction that raised the trace causes some results in the (very) near future, but
    /// they are not available at the moment of the trace creation. 
    /// </summary>
    public static class PartialStatements
    {
        // If the statement is in the set, it is partial.
        private static HashSet<Statement> _partialStatements;

        public static bool IsPartial(this Statement statement)
        {
            if(_partialStatements == null)
            {
                _partialStatements = new HashSet<Statement>();
            }

            return _partialStatements.Contains(statement);
        }

        public static void SetPartial(this Statement statement)
        {
            if (_partialStatements == null)
            {
                _partialStatements = new HashSet<Statement>();
            }

            if (!_partialStatements.Contains(statement))
            {
                _partialStatements.Add(statement);
            }
        }

        public static void Complete(this Statement statement)
        {
            if (_partialStatements == null)
            {
                _partialStatements = new HashSet<Statement>();
            }

            if (_partialStatements.Contains(statement))
            {
                _partialStatements.Remove(statement);
            }
        }

        public static void CompleteAllStatements()
        {
            _partialStatements.Clear();
        }

        public static IEnumerable<Statement> Completed(this IEnumerable<Statement> statements)
        {
            return statements.TakeWhile(s => !s.IsPartial());
        }

        internal static IEnumerable<TraceTask> Completed(this IEnumerable<TraceTask> statements)
        {
            return statements.TakeWhile(s => !s.statement.IsPartial());
        }
    }
}
