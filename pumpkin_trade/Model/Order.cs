using System;

using PumpkinTrade.Infrastructure.Enums;

namespace PumpkinTrade.Model
{
    public class Order
    {
        public Order() {
            Id = Guid.NewGuid();
            DatePlaced = DateTime.Now;
        }
        public Order(decimal price, Guid clientId): this()
        {
            Price = price;
            ClientId = clientId;
        }
        public Guid Id { get; }
        public OrderTypes OrderType { get; protected set; }
        public Order ComplementaryOrder { get; private set; } = null;
        public decimal Price { get; }
        public DateTime DatePlaced { get; }
        public Guid? ClientId { get; protected set; } = null;

        public State State { get; private set; } = State.Open;

        public void CloseOrder(Order complementaryOrder)
        {
            ComplementaryOrder = complementaryOrder;
            ComplementaryOrder.ComplementaryOrder = this;
            if (DatePlaced > complementaryOrder.DatePlaced)
            {
                ComplementaryOrder.State = State.ClosedAsSecondary;
                if (OrderType == OrderTypes.Buy) State = State.ClosedAsPrimaryBuy;
                else State = State.ClosedAsPrimarySale;
            }
            else
            {
                State = State.ClosedAsSecondary;
                if (ComplementaryOrder.OrderType == OrderTypes.Buy) ComplementaryOrder.State = State.ClosedAsPrimaryBuy;
                else ComplementaryOrder.State = State.ClosedAsPrimarySale;
            }
        }
    }
}
