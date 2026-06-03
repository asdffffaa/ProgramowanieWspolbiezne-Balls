using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;
using System.Text;
using TP.ConcurrentProgramming.Data.Abstractions;
using TP.ConcurrentProgramming.Data.Diagnostics;
using TP.ConcurrentProgramming.Data.Models;

namespace TP.ConcurrentProgramming.DataTest
{
    [TestClass]
    public class DiagnosticTests
    {
        [TestMethod]
        public void DiagnosticRecord_ShouldSerializeToAsciiText()
        {
            BallDiagnosticRecord record = new BallDiagnosticRecord(
                DateTimeOffset.Parse("2026-06-02T20:00:00Z"),
                7,
                10.5,
                20.25,
                3,
                -4);

            string line = record.ToAsciiLine();

            foreach (char character in line)
            {
                Assert.IsTrue(character <= 127);
            }

            StringAssert.Contains(line, "\"ballId\":7");
            StringAssert.Contains(line, "\"positionX\":10.5");
        }

        [TestMethod]
        public void DataApi_SaveDiagnosticSnapshots_ShouldNotChangeBallBehavior()
        {
            RejectingDiagnosticLogger logger = new RejectingDiagnosticLogger();
            IDataApi dataApi = new DataApi(new BallRepository(), logger);
            IBall ball = dataApi.CreateBall(20, 10, 15, 2, 3);

            dataApi.Add(ball);

            BallSnapshot beforeLogging = ball.Snapshot();

            dataApi.SaveDiagnosticSnapshots();

            BallSnapshot afterLogging = ball.Snapshot();

            Assert.AreEqual(beforeLogging.PositionX, afterLogging.PositionX);
            Assert.AreEqual(beforeLogging.PositionY, afterLogging.PositionY);
            Assert.AreEqual(beforeLogging.VelocityX, afterLogging.VelocityX);
            Assert.AreEqual(beforeLogging.VelocityY, afterLogging.VelocityY);
            Assert.AreEqual(1, logger.DroppedRecords);
        }

        [TestMethod]
        public void AsyncFileDiagnosticLogger_ShouldWriteDiagnosticsWithoutExternalPackage()
        {
            string filePath = Path.Combine(
                Path.GetTempPath(),
                "balls-diagnostics-" + Guid.NewGuid().ToString("N") + ".jsonl");

            using (AsyncFileDiagnosticLogger logger = new AsyncFileDiagnosticLogger(filePath))
            {
                logger.TryLog(new BallDiagnosticRecord(
                    DateTimeOffset.UtcNow,
                    1,
                    2,
                    3,
                    4,
                    5));
            }

            string text = File.ReadAllText(filePath, Encoding.ASCII);

            StringAssert.Contains(text, "\"ballId\":1");
            StringAssert.Contains(text, "\"velocityY\":5");

            File.Delete(filePath);
        }

        private sealed class RejectingDiagnosticLogger : IDiagnosticLogger
        {
            private long _droppedRecords;

            public long DroppedRecords => _droppedRecords;

            public bool TryLog(BallDiagnosticRecord record)
            {
                _droppedRecords++;
                return false;
            }

            public void Dispose()
            {
            }
        }
    }
}
