
namespace PumpkinTrade.Model
{
    public class SellOrder: Order
    {
        public SellOrder() { }
        public SellOrder(decimal price, uint clientId): base(price)
        {
            SellerId = clientId;
        }
    }
}
