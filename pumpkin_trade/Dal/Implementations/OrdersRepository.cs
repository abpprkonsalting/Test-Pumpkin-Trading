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

        public OrdersRepository()
        {
            Orders = new List<Order>();
        }
        public Order Add(Order order)
        {
            lock (OrdersLock)
            {
                Orders.Add(order);
                if (order.OrderType == OrderTypes.Sale)
                {
                    return GetBestMatchingOrderByPriceAndDate(order, Criteria.Max);
                }
                return GetBestMatchingOrderByPriceAndDate(order, Criteria.Min);
            }
        }

        public List<Order> GetTrades()
        {
            return Orders.Where(ord => ord.State == State.ClosedAsPrimaryBuy || ord.State == State.ClosedAsPrimarySale).OrderBy(ord => ord.DatePlaced).ToList();
        }

        private Order GetBestMatchingOrderByPriceAndDate(Order order, Criteria criteria)
        {
            var notClosedOrders = Orders.FindAll(ord => ord.Id != order.Id && ord.State == State.Open).ToList();
            if (!notClosedOrders.Any()) return order;
            var qualifyingOrders = order.OrderType == OrderTypes.Sale ? notClosedOrders.Where(ord => ord.OrderType == OrderTypes.Buy && ord.Price >= order.Price).ToList() :
                                                                        notClosedOrders.Where(ord => ord.OrderType == OrderTypes.Sale && ord.Price < order.Price).ToList();
            if (!qualifyingOrders.Any()) return order;
            var orderedOrders = order.OrderType == OrderTypes.Sale ? qualifyingOrders.OrderBy(ord => ord.Price).ToList() :
                                                                     qualifyingOrders.OrderByDescending(ord => ord.Price).ToList();
            var groupedByPrice = orderedOrders.GroupBy(ord => ord.Price);
            var groupsOrderedByPrice = groupedByPrice.OrderBy(group => group.Key);
            var matchingGroup = order.OrderType == OrderTypes.Sale ? groupsOrderedByPrice.Last() : groupsOrderedByPrice.First();
            Order selected = null;
            if (matchingGroup.Count() > 1)
            {
                var orderedByDate = matchingGroup.OrderBy(ord => ord.DatePlaced);
                selected = orderedByDate.First();
            }
            else
            {
                selected = matchingGroup.First();
            }
            selected.CloseOrder(order);
            if (selected.State != State.ClosedAsSecondary) return selected;
            else return order;
        }
    }
}
