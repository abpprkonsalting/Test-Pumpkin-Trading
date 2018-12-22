
namespace PumpkinTrade.Model
{
    public class BuyOrder : Order
    {
        public BuyOrder() { }
        public BuyOrder(decimal price, uint clientId) : base(price)
        {
            BuyerId = clientId;
        }
    }
}
