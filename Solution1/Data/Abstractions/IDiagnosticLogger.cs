using System;

namespace TP.ConcurrentProgramming.Data.Abstractions
{
    public interface IDiagnosticLogger : IDisposable
    {
        long DroppedRecords { get; }

        bool TryLog(BallDiagnosticRecord record);
    }
}
