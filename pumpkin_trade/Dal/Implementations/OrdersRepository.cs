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
                if (order.SellerId != null)
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
            var qualifyingOrders = order.SellerId != null ? notClosedOrders.Where(ord => ord.BuyerId != null && 
                                                                                        ord.Price >= order.Price).OrderBy(ord => ord.Price).ToList() :
                                                        notClosedOrders.Where(ord => ord.SellerId != null &&
                                                                                        ord.Price < order.Price).OrderByDescending(ord => ord.Price).ToList();
            if (!qualifyingOrders.Any()) return order;
            var groupedByPrice = qualifyingOrders.GroupBy(ord => ord.Price);
            var groupsOrderedByPrice = order.SellerId != null ? groupedByPrice.OrderBy(group => group.Key) : groupedByPrice.OrderByDescending(group => group.Key);
            var matchingGroup = groupsOrderedByPrice.Last();
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
            order.CloseOrder(selected);
            if (selected.State != State.ClosedAsSecondary) return selected;
            else return order;
        }
    }
}
