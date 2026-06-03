using System;
using System.Globalization;
using System.Text;

namespace TP.ConcurrentProgramming.Data.Abstractions
{
    public readonly struct BallDiagnosticRecord
    {
        public BallDiagnosticRecord(
            DateTimeOffset timestamp,
            int ballId,
            double positionX,
            double positionY,
            double velocityX,
            double velocityY)
        {
            Timestamp = timestamp;
            BallId = ballId;
            PositionX = positionX;
            PositionY = positionY;
            VelocityX = velocityX;
            VelocityY = velocityY;
        }

        public DateTimeOffset Timestamp { get; }
        public int BallId { get; }
        public double PositionX { get; }
        public double PositionY { get; }
        public double VelocityX { get; }
        public double VelocityY { get; }

        public string ToAsciiLine()
        {
            StringBuilder builder = new StringBuilder();
            builder.Append("{\"timestamp\":\"");
            builder.Append(Timestamp.UtcDateTime.ToString("O", CultureInfo.InvariantCulture));
            builder.Append("\",\"ballId\":");
            builder.Append(BallId.ToString(CultureInfo.InvariantCulture));
            builder.Append(",\"positionX\":");
            builder.Append(PositionX.ToString("R", CultureInfo.InvariantCulture));
            builder.Append(",\"positionY\":");
            builder.Append(PositionY.ToString("R", CultureInfo.InvariantCulture));
            builder.Append(",\"velocityX\":");
            builder.Append(VelocityX.ToString("R", CultureInfo.InvariantCulture));
            builder.Append(",\"velocityY\":");
            builder.Append(VelocityY.ToString("R", CultureInfo.InvariantCulture));
            builder.Append('}');
            return builder.ToString();
        }
    }
}
