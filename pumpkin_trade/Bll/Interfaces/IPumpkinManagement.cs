using System;
using System.Collections.Generic;

using PumpkinTrade.Model;

namespace PumpkinTrade.Bll.Interfaces
{
    interface IPumpkinManagement
    {
        Order SellPumpkin(decimal price, uint clientId);
        Order BuyPumpkin(decimal price, uint clientId);
        List<String> GetTrades();

    }
}
