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
        public Order(decimal price): this()
        {
            Price = price;
        }
        public Guid Id { get; }
        public Guid? ComplementaryOrderId { get; private set; } = null;
        public decimal Price { get; }
        public DateTime DatePlaced { get; }
        public Guid? BuyerId { get; protected set; } = null;
        public Guid? SellerId { get; protected set; } = null;
        public State State { get; private set; } = State.Open;

        public void CloseOrder(Order complementaryOrder)
        {
            if (BuyerId != null)
            {
                SellerId = complementaryOrder.SellerId;
                if (DatePlaced > complementaryOrder.DatePlaced)
                {
                    State = State.ClosedAsPrimaryBuy;
                    complementaryOrder.State = State.ClosedAsSecondary;
                }
            }    
            else
            {
                BuyerId = complementaryOrder.BuyerId;
                if (DatePlaced > complementaryOrder.DatePlaced)
                {
                    State = State.ClosedAsPrimarySale;
                    complementaryOrder.State = State.ClosedAsSecondary;
                }
            }
            ComplementaryOrderId = complementaryOrder.Id;
        }

    }
}
