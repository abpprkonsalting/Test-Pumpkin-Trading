using System;

namespace PumpkinTrade.Model
{
    public class SellOrder: Order
    {
        public SellOrder() { }
        public SellOrder(decimal price,Guid clientId): base(price)
        {
            SellerId = clientId;
        }
    }
}
