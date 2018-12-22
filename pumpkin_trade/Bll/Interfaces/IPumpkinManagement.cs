using System;
using System.Collections.Generic;

using PumpkinTrade.Model;

namespace PumpkinTrade.Bll.Interfaces
{
    interface IPumpkinManagement
    {
        Order SellPumpkin(decimal price, Guid clientId);
        Order BuyPumpkin(decimal price, Guid clientId);
        List<String> GetTrades();
        Guid AddClient(String name);
    }
}
