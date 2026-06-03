using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TP.ConcurrentProgramming.Data.Abstractions;

namespace TP.ConcurrentProgramming.Data.Diagnostics
{
    public sealed class AsyncFileDiagnosticLogger : IDiagnosticLogger
    {
        private readonly ConcurrentQueue<string> _pendingLines = new ConcurrentQueue<string>();
        private readonly AutoResetEvent _lineAvailable = new AutoResetEvent(false);
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly Task _writerTask;
        private readonly string _filePath;
        private readonly int _capacity;
        private int _pendingCount;
        private long _droppedRecords;
        private bool _disposed;

        public AsyncFileDiagnosticLogger(string filePath, int capacity = 4096)
        {
            if (capacity <= 0)
            {
                throw new ArgumentException("Capacity must be greater than zero.", nameof(capacity));
            }

            _filePath = filePath;
            _capacity = capacity;
            _writerTask = Task.Run(WritePendingLines);
        }

        public long DroppedRecords => Interlocked.Read(ref _droppedRecords);

        public bool TryLog(BallDiagnosticRecord record)
        {
            if (_disposed)
            {
                return false;
            }

            int newCount = Interlocked.Increment(ref _pendingCount);

            if (newCount > _capacity)
            {
                Interlocked.Decrement(ref _pendingCount);
                Interlocked.Increment(ref _droppedRecords);
                return false;
            }

            _pendingLines.Enqueue(record.ToAsciiLine());
            _lineAvailable.Set();
            return true;
        }

        public void Dispose()
        {
            if (_disposed)
            {
                return;
            }

            _disposed = true;
            _cancellationTokenSource.Cancel();
            _lineAvailable.Set();

            try
            {
                _writerTask.Wait(TimeSpan.FromSeconds(2));
            }
            catch (AggregateException)
            {
            }

            _lineAvailable.Dispose();
            _cancellationTokenSource.Dispose();
        }

        private void WritePendingLines()
        {
            string? directory = Path.GetDirectoryName(_filePath);

            if (!string.IsNullOrWhiteSpace(directory))
            {
                Directory.CreateDirectory(directory);
            }

            using StreamWriter writer = new StreamWriter(_filePath, append: true, Encoding.ASCII);

            while (!_cancellationTokenSource.IsCancellationRequested || !_pendingLines.IsEmpty)
            {
                while (_pendingLines.TryDequeue(out string? line))
                {
                    Interlocked.Decrement(ref _pendingCount);
                    writer.WriteLine(line);
                }

                writer.Flush();
                _lineAvailable.WaitOne(25);
            }
        }
    }
}
