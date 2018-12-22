using System;

using PumpkinTrade.Infrastructure.Enums;

namespace PumpkinTrade.Model
{
    public class BuyOrder : Order
    {
        public BuyOrder(decimal price, Guid clientId) : base(price, clientId)
        {
            OrderType = OrderTypes.Buy;
        }
    }
}
