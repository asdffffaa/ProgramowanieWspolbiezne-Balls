using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Diagnostics
{
    public sealed class NullDiagnosticLogger : IDiagnosticLogger
    {
        public long DroppedRecords => 0;

        public bool TryLog(BallDiagnosticRecord record)
        {
            return true;
        }

        public void Dispose()
        {
        }
    }
}
