using System;
using System.Collections.Generic;

using PumpkinTrade.Bll.Interfaces;
using PumpkinTrade.Dal.Implementations;
using PumpkinTrade.Model;
using PumpkinTrade.Infrastructure.Enums;

namespace PumpkinTrade.Bll.Implementations
{
    public class PumpkinManagement : IPumpkinManagement
    {
        OrdersRepository _ordersRepository;
        ClientsRepository _clientsRepository;

        public PumpkinManagement()
        {
            _ordersRepository = new OrdersRepository();
            _clientsRepository = new ClientsRepository();
        }

        public Order SellPumpkin(decimal price, Guid clientId)
        {
            if (!_clientsRepository.Exist(clientId)) return null;
            var newSellOrder = new SellOrder(price, clientId);
            return _ordersRepository.Add(newSellOrder);
        }
        public Order BuyPumpkin(decimal price, Guid clientId)
        {
            if (!_clientsRepository.Exist(clientId)) return null;
            var newBuyOrder = new BuyOrder(price, clientId);
            return _ordersRepository.Add(newBuyOrder);
        }
        public List<String> GetTrades()
        {
            var trades = _ordersRepository.GetTrades();
            var outPut = new List<String>();
            trades.ForEach(trade =>
            {
                var seller = _clientsRepository.GetById((Guid)trade.SellerId);
                var buyer = _clientsRepository.GetById((Guid)trade.BuyerId);
                outPut.Add(trade.State == State.Sale ? seller.Name + " sold a pumpkin to " + buyer.Name + " for " + trade.Price + " Euros." :
                                                               buyer.Name + " bought a pumpkin from " + seller.Name + " for " + trade.Price + " Euros.");
            });
            return outPut;
            
        }
        public Guid AddClient(String name)
        {
            return _clientsRepository.Add(name);
        }
    }
}
