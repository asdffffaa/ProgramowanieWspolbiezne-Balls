using System;
using System.Collections.Generic;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Diagnostics;

namespace TP.ConcurrentProgramming.Data.Models
{
    public class DataApi : IDataApi
    {
        private readonly IBallRepository _ballRepository;
        private readonly IDiagnosticLogger _diagnosticLogger;

        public DataApi(IBallRepository ballRepository)
            : this(ballRepository, new NullDiagnosticLogger())
        {
        }

        public DataApi(IBallRepository ballRepository, IDiagnosticLogger diagnosticLogger)
        {
            _ballRepository = ballRepository;
            _diagnosticLogger = diagnosticLogger;
        }

        public long DroppedDiagnosticRecords => _diagnosticLogger.DroppedRecords;

        public IReadOnlyList<IBall> GetAll()
        {
            return _ballRepository.GetAll();
        }

        public IReadOnlyList<BallSnapshot> GetSnapshots()
        {
            List<BallSnapshot> snapshots = new List<BallSnapshot>();

            foreach (IBall ball in _ballRepository.GetAll())
            {
                snapshots.Add(ball.Snapshot());
            }

            return snapshots.AsReadOnly();
        }

        public void Add(IBall ball)
        {
            _ballRepository.Add(ball);
        }

        public void Clear()
        {
            _ballRepository.Clear();
        }

        public void SaveDiagnosticSnapshots()
        {
            DateTimeOffset timestamp = DateTimeOffset.UtcNow;

            foreach (BallSnapshot snapshot in GetSnapshots())
            {
                _diagnosticLogger.TryLog(snapshot.ToDiagnosticRecord(timestamp));
            }
        }

        public IBall CreateBall(
            double diameter,
            double positionX,
            double positionY,
            double velocityX,
            double velocityY
        )
        {
            return new Ball(
                diameter,
                new Position(positionX, positionY),
                new Vector(velocityX, velocityY)
            );
        }
    }
}
