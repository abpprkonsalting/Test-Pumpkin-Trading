using System;

namespace PumpkinTrade.Model
{
    public class BuyOrder : Order
    {
        public BuyOrder() { }
        public BuyOrder(decimal price, Guid clientId) : base(price)
        {
            BuyerId = clientId;
        }
    }
}
