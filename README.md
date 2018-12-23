# Test-Pumpkin-Trading

The application is based on two models:

1- Client:
  
  This one contains two fields quite self-explanatories, Id and Name.

2- Order: 

  This is more complex, having two childs: SellOrder & BuyOrder. Whenever a client place an order (for buy or for sale), an instance of the corresponding child model is created. The distinction between them is made by a field in the model (OrderType).
  
  This model is designed in such a way that all fields are accesible for reading, but protected for writting. Fields can only be modified by the correct operations triggered by the program logic, never directly.
  
For interacting with the models were created two "repositories" in a Data Access Layer (Dal), one for each model.

Both "repos" contains a collection (List) of it's corresponding model, and methods to interact with that collection.

Methods of Clients repo (all public):

  - Add item.
  - Find by Id.
  - Check existence by Id.
  
Methods of Orders repo:

  Public ones:
  
   Add.
   Get Trades.
    
  Private:
  
  GetBestMatchingOrderByPriceAndDate: This method is called after an Order is placed, and what it does is to filter the collection of Orders and selecting the apropiate one to match the placed order. If there is a match, then both orders are closed and linked, if not, the order remains in open state.
 
Any order can have one of 4 states: Open, ClosedAsSecondary, ClosedAsPrimarySale, ClosedAsPrimaryBuy.

When the pair of orders that constitute a Trade operation are marked they are always done in pairs, a primary and a secondary. The criteria for determining which one is the primary is the last one arriving to the queue.

In any case, an order is never removed from the repo because the "GetTrades" method what does is to filter the closed orders that are marked as been "PrimarySale" or "PrimaryBuy", and then return that collection to the Bll method that create the report of trades.

The interaction with the library is done in the Bll, where exist the three requested methods in the task description:

- SellPumpkin: used by the client to place a sale order. If there is not a corresponding buy order that match the sale, then the return value is the own "sale order" without been closed. If there was a match, then the return value is the corresponding buy order, in this case closed. Access to the original “sale order” remains possible because both orders are linked throught their respectives “ComplementaryOrder” fields; i.e.: The ComplementaryOrder field of the “sale order” is the corresponding “buy order”, and viceversa. 
  
- BuyPumpkin: The same logic that SellPumpkin but opposite.

- GetTrades return a collection of the closed trade operations in human readable format. A trade operation, as concept, is composed by both orders (the sale and it’s corresponding buy), but the way of characterizing the operation as a sale/buy is to select the last order of the pair placed. The other one remains in the collection but marked otherwise as “secondary”. 

Finally there is an accessory method for adding clients.

The solution contains a project with the unit tests, named "pumpkin_trade_tests". All the test cases proposed in the task description
are covered and run succesfully.

The thread safety is guaranteed by the use of locks each time the collections in the repos are accessed. As every operation returns always with a result, and keep all orders in a defined state, blocking the access to the collection is enough. Any concurrent operation that could take place meanwhile there is one of them in course, will have to wait until this last one finish. That means that any operation will always take place with all orders in a safe state.

Component state is protected from illegal modifications by the use of encapsulation in the model and only exposing the methods for adding orders and getting the report.

Adding a parameter for amount of pumpkins could be achieved adding a field "quantity" to the order and changing the logic for matching orders; but here there could be two cases:

1- A buy/sale order for more than a pumpking could be satisfied by several sale/buy orders that summed all together match the proposed price by the established rules.

  This case is the more complex one, and it's implementation would requiere the use of logic not only based on methods that filter in the repository. In fact, it could imply different approaches to the logic of matching orders.

2- A buy/sale order for more than one pumpking could be satisfied only by a sale/buy order that match exactly the same amount of pumkings.

  This case is simpler, but less real. It would only need to add to the filters this new parameter for amount of pumpkings.
  
Expiration time for the order could be implemented including in the filter of qualifiedOrders (OrdersRepository.cs line 41) a condition to exclude the orders that exceed an established timespan (it could be done too in line 39, but conceptually it is better in 41. The first filter is for selecting the opened orders).

Example:

var qualifyingOrders = order.SellerId != null ? notClosedOrders.Where(ord => ord.BuyerId != null && ord.Price >= order.Price && DateTime.Now - order.DatePlaced < new TimeSpan(10, 0, 0,0)).ToList() : notClosedOrders.Where(ord => ord.SellerId != null && ord.Price < order.Price && DateTime.Now - order.DatePlaced < new TimeSpan(10, 0, 0,0)).ToList();

It could be implemented also adding a field to the order, and marking the order as "expired" the first time the check is done and it proceed, but then it will need another filter.
