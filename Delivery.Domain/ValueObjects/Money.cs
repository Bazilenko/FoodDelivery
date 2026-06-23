
using Delivery.Domain.Common;
using Delivery.Domain.Exceptions;

namespace Delivery.Domain.Value_Objects
{
    public class Money : ValueObject
    {
        public decimal Amount { get; private set; }
        public string Currency { get ; private set; }
        public Money(decimal amount, string currency = "UAH")
        {
            if (amount < 0)
                throw new InvalidMoneyException(amount);

            if (string.IsNullOrWhiteSpace(currency) || currency.Length != 3)
                throw new DomainException("Currency must be a 3-letter ISO code.", "InvalidCurrency");

            Amount = decimal.Round(amount, 2);
            Currency = currency.ToUpperInvariant();
        }
        public Money Add(Money other)
        {
            if (Currency != other.Currency)
                throw new DifferentCurrencyException();

            return new Money(Amount + other.Amount, Currency);
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return Amount;
            yield return Currency;
        }

    }
}
