using Delivery.Domain.Common;
using Delivery.Domain.Exceptions;

namespace Delivery.Domain.Entities
{
    public class Courier : BaseEntity
    {
        public string Name { get; private set; }
        public string Email { get; private set; }
        public string PhoneNumber { get; private set; }

        public string UserId { get; private set; }

        private Courier() { }

        public Courier(string name, string email, string phoneNumber, string userId)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new DomainException("Name cannot be empty.", "InvalidValue");

            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            UserId = userId;
        }

        public void Update(string name, string email, string phoneNumber)
        {
            Name = name;
            Email = email;
            PhoneNumber = phoneNumber;
            Touch();
        }
    }
}

