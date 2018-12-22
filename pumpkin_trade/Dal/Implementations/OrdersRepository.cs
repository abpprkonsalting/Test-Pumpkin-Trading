using System;
using System.Collections.Generic;
using System.Linq;
using PumpkinTrade.Model;
using PumpkinTrade.Infrastructure.Enums;


namespace PumpkinTrade.Dal.Implementations
{
    public class OrdersRepository
    {
        private List<Order> Orders;
        private readonly object OrdersLock = new object();

        public Order Add(Order order)
        {
            lock (OrdersLock)
            {
                Orders.Add(order);
                if (order.SellerId != null)
                {
                    return GetBestMatchingOrderByPriceAndDate(order, Criteria.Max);
                }
                return GetBestMatchingOrderByPriceAndDate(order, Criteria.Min);
            }
        }

        public List<Order> GetTrades()
        {
            return Orders.Where(ord => ord.TradeType != TradeType.NoTrade).OrderBy(ord => ord.DatePlaced).ToList();
        }

        private Order GetBestMatchingOrderByPriceAndDate(Order order, Criteria criteria)
        {
            var notClosedOrders = Orders.FindAll(ord => ord.Id != order.Id &&
                                                        ord.TradeType == TradeType.NoTrade).ToList();
            if (!notClosedOrders.Any()) return null;
            var qualifyingOrders = order.SellerId != null ? notClosedOrders.Where(ord => ord.BuyerId != null && 
                                                                                        ord.Price >= order.Price).OrderBy(ord => ord.Price).ToList() :
                                                        notClosedOrders.Where(ord => ord.SellerId != null &&
                                                                                        ord.Price < order.Price).OrderByDescending(ord => ord.Price).ToList();
            if (!qualifyingOrders.Any()) return null;
            var groupedByPrice = qualifyingOrders.GroupBy(ord => ord.Price);
            var matchingGroup = groupedByPrice.SelectMany(elem => elem.Where(a => a.Price == elem.Max(c => c.Price)));
            var orderedByDate = matchingGroup.OrderBy(ord => ord.DatePlaced);
            var selected = orderedByDate.First();
            selected.CloseOrder(order);
            order.CloseOrder(selected);
            if (selected.TradeType != TradeType.NoTrade) return selected;
            else return order;
        }
    }
}
