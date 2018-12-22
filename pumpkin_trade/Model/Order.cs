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
        public uint? BuyerId { get; protected set; } = null;
        public uint? SellerId { get; protected set; } = null;
        public TradeType TradeType { get; private set; } = TradeType.NoTrade;

        public void CloseOrder(Order complementaryOrder)
        {
            if (BuyerId != null)
            {
                SellerId = complementaryOrder.SellerId;
                if (DatePlaced > complementaryOrder.DatePlaced) TradeType = TradeType.Bought;
            }    
            else
            {
                BuyerId = complementaryOrder.BuyerId;
                if (DatePlaced > complementaryOrder.DatePlaced) TradeType = TradeType.Sold;
            }
            ComplementaryOrderId = complementaryOrder.Id;
        }

    }
}
