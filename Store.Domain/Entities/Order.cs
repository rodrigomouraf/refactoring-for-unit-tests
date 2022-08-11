using Store.Domain.Enums;
using Flunt.Validations;

namespace Store.Domain.Entities
{
    public class Order : Entity
    {
        private IList<OrderItem> _items;
        public Order(Customer customer, decimal deliveryFee, Discount discount)
        {
            AddNotifications(
                new Contract()
                    .Requires()
                    .IsNotNull(customer, "Customer", "Cliente inválido")
            );

            Customer = customer;
            Date = DateTime.Now;
            Number = Guid.NewGuid().ToString().Substring(0, 8);
            DeliveryFee = deliveryFee;
            Status = EOrderStatus.WaitingPayment;
            DeliveryFee = deliveryFee;
            Discount = discount;
            _items = new List<OrderItem>();
        }

        public Customer Customer { get; private set; }
        public DateTime Date { get; private set; }
        public decimal DeliveryFee { get; private set; }
        public string Number { get; private set; }
        public EOrderStatus Status { get; private set; }
        public Discount Discount { get; private set; }
        public IReadOnlyCollection<OrderItem> Items { get { return _items.ToArray(); } }

        public void AddItem(Product product, int quantity)
        {
            var item = new OrderItem(product, quantity);
            if(item.Valid)
                _items.Add(item);
        }

        public decimal Total()
        {
            decimal total = 0;
            foreach (var item in _items)
            {
                total += item.Total();
            }

            total += DeliveryFee;
            total -= Discount != null ? Discount.Value() : 0;

            return total;
        }

        public void Pay(decimal amount)
        {
            if(amount == Total())
                this.Status = EOrderStatus.WaitingDelivery;
        }

        public void Cancel()
        {
            Status = EOrderStatus.Canceled;
        }

    }
}