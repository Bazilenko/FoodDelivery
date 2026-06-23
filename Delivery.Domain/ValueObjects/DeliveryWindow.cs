using Delivery.Domain.Common;
using Delivery.Domain.Exceptions;

namespace Delivery.Domain.ValueObjects
{
    public class DeliveryWindow : ValueObject
    {
        public DateTime Start { get; }

        public DateTime End { get; }

        public DeliveryWindow(DateTime start, DateTime end)
        {
            if (end <= start)
                throw new DomainException("End time must be greater than start time.", "TimeRestriction");
            if (end - start > TimeSpan.FromHours(1))
                throw new DomainException("Delivery window too long (max 1 hour).", "TimeRestriction");

            Start = start.ToUniversalTime();
            End = end.ToUniversalTime();
        }

        public bool IsWithinWindow(DateTime time)
        {
            var t = time.ToUniversalTime();
            return t >= Start && t <= End;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Start;
            yield return End;
        }

        public override string ToString() => $"{Start:HH:mm} - {End:HH:mm}";
    }
}
