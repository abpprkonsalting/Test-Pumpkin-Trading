using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PumpkinTrade.Bll.Implementations;
using PumpkinTrade.Model;
using PumpkinTrade.Infrastructure.Enums;

namespace pumpkin_trade_tests
{
    [TestClass]
    public class UnitTest1
    {

        [TestMethod]
        public void TestCreateBuyOrder()
        {
            PumpkinManagement pumpkinManagement = new PumpkinManagement();
            Guid clientAId = pumpkinManagement.AddClient("A");
            Order newBuyOrder = pumpkinManagement.BuyPumpkin(25,clientAId);
            Guid buyerId = (Guid)newBuyOrder.BuyerId;
            decimal orderPrice = newBuyOrder.Price;
            Type t = typeof(Guid);
            Assert.IsInstanceOfType(buyerId,t, "wrong clientId type");
            Assert.AreEqual((decimal)25, orderPrice, "wrong price for order");
        }

        [TestMethod]
        public void TestCreateSellOrder()
        {
            PumpkinManagement pumpkinManagement = new PumpkinManagement();
            Guid clientBId = pumpkinManagement.AddClient("B");
            Order newSellOrder = pumpkinManagement.SellPumpkin(25,clientBId);
            Guid sellerId = (Guid) newSellOrder.SellerId;
            decimal orderPrice = newSellOrder.Price;
            Type t = typeof(Guid);
            Assert.IsInstanceOfType(sellerId, t, "wrong clientId type");
            Assert.AreEqual((decimal)25, orderPrice, "wrong price for order");
        }

        [TestMethod]
        public void TestBuyThenSell()
        {
            PumpkinManagement pumpkinManagement = new PumpkinManagement();
            Guid clientAId = pumpkinManagement.AddClient("A");
            Guid clientBId = pumpkinManagement.AddClient("B");
            Order newBuyOrder = pumpkinManagement.BuyPumpkin(25,clientAId);
            Order newSellOrder = pumpkinManagement.SellPumpkin(25,clientBId);
            var buyOrderId = newBuyOrder.Id;
            var buyOrderComplementaryOrderId = newBuyOrder.ComplementaryOrderId;
            var sellOrderId = newSellOrder.Id;
            var sellOrderComplementaryOrderId = newSellOrder.ComplementaryOrderId;
            Assert.AreEqual(buyOrderId, sellOrderComplementaryOrderId, "sellOrder not complementary to buyOrder");
            Assert.AreEqual(sellOrderId, buyOrderComplementaryOrderId, "buyOrder not complementary to sellOrder");

        }

        [TestMethod]
        public void MultipleBuysAndSells()
        {
            PumpkinManagement pumpkinManagement = new PumpkinManagement();
            Guid clientAId = pumpkinManagement.AddClient("A");
            Guid clientBId = pumpkinManagement.AddClient("B");
            Guid clientCId = pumpkinManagement.AddClient("C");
            Guid clientDId = pumpkinManagement.AddClient("D");
            Guid clientEId = pumpkinManagement.AddClient("E");
            Guid clientFId = pumpkinManagement.AddClient("F");
            Guid clientGId = pumpkinManagement.AddClient("G");
            Order clientAOrder = pumpkinManagement.BuyPumpkin(10,clientAId);
            Order clientBOrder = pumpkinManagement.BuyPumpkin(11, clientBId);
            Order clientCOrder = pumpkinManagement.SellPumpkin(15, clientCId);
            Order clientDOrder = pumpkinManagement.SellPumpkin(9, clientDId);
            Order clientEOrder = pumpkinManagement.BuyPumpkin(10, clientEId);
            Order clientFOrder = pumpkinManagement.SellPumpkin(10, clientFId);
            Order clientGOrder = pumpkinManagement.BuyPumpkin(100, clientGId);

            var finalReport = pumpkinManagement.GetTrades();

            Assert.AreEqual(clientDOrder.ComplementaryOrderId, clientBOrder.Id, "clientBOrder not complementary to ClientDOrder");
            Assert.AreEqual(clientDOrder.State, State.ClosedAsPrimarySale, "clientDOrder not marked as sold");
            Assert.AreEqual("D sold a pumpkin to B for 9 Euros.", finalReport[0], "wrong report for sale of D");

            Assert.AreEqual(clientFOrder.ComplementaryOrderId, clientAOrder.Id, "clientFOrder not complementary to ClientAOrder");
            Assert.AreEqual(clientFOrder.State, State.ClosedAsPrimarySale, "clientFOrder not marked as sold");
            Assert.AreEqual("F sold a pumpkin to A for 10 Euros.", finalReport[1], "wrong report for sale of F");

            Assert.AreEqual(clientGOrder.ComplementaryOrderId, clientCOrder.Id, "clientGOrder not complementary to ClientCOrder");
            Assert.AreEqual(clientGOrder.State, State.ClosedAsPrimaryBuy, "clientGOrder not marked as bought");
            Assert.AreEqual("G bought a pumpkin from C for 100 Euros.", finalReport[2], "wrong report for bought of G");
        }
    }
}
