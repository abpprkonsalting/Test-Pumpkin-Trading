using System;

using PumpkinTrade.Infrastructure.Enums;

namespace PumpkinTrade.Model
{
    public class SellOrder: Order
    {
        public SellOrder(decimal price,Guid clientId): base(price, clientId)
        {
            OrderType = OrderTypes.Sale;
        }
    }
}
