using System;

namespace Catalog.Dal.Entities
{
    public abstract class BaseEntity : IEquatable<BaseEntity>
    {
        public int Id { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public bool IsDeleted { get; set; } = false;

        public static bool operator ==(BaseEntity? first, BaseEntity? second)
        {
            if (ReferenceEquals(first, second))
                return true;

            if (first is null || second is null)
                return false;

            return first.Equals(second);
        }

        public static bool operator !=(BaseEntity? first, BaseEntity? second)
        {
            return !(first == second);
        }

        public override bool Equals(object? obj)
        {
            if (obj is null)
                return false;

            if (ReferenceEquals(this, obj))
                return true;

            if (obj.GetType() != GetType())
                return false;

            if (obj is not BaseEntity other)
                return false;

            if (Id == 0 || other.Id == 0)
                return false;

            return Id == other.Id;
        }

        public bool Equals(BaseEntity? other)
        {
            if (other is null)
                return false;

            if (ReferenceEquals(this, other))
                return true;

            if (other.GetType() != GetType())
                return false;

            if (Id == 0 || other.Id == 0)
                return false;

            return Id == other.Id;
        }

        public override int GetHashCode()
        {
            if (Id == 0)
                return base.GetHashCode();

            return (GetType().ToString() + Id).GetHashCode();
        }
    }
}