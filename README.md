# Test-Pumpkin-Trading

The application is based on two models:

1- Client:
  
  This one contains two fields quite self-explanatories, Id and Name.

2- Order: 

  This is more complex, having two childs: SellOrder & BuyOrder. Whenever a client place an order (for buy or for sale), 
  an instance of the corresponding child model is created. The distintion between them is made by the existence (or not) of value in one
  of two exclusive fields of the parent model, named respectivelly as BuyerId and SellerId. An order placed as a Sale will be instanstiated
  with SellerId = < X value > and BuyerId as null, and viceversa.
  
  This model is designed in such a way that all fields are accesible for reading, but protected for writting. Fields can only be modified
  by the correct operations triggered by the program logic, never directly.
  
For interacting with the models there were created two "repositories" in a Data Access Layer (Dal), one for each.

Both "repos" contains a collection (List) of it's corresponding model, and methods to interact with that collection.

Methods of Clients repo (all public):

  - Add item.
  - Find by Id.
  - Check existence by Id.
  
Methods of Order repo:

  Public ones:
  
    - Add.
    - Get Trades
    
  Private:
  
    - GetBestMatchingOrderByPriceAndDate: This is a method that is called after an Order is placed, and what it does is to filter the 
      collection of Orders and selecting the apropiate one to match the placed order. If there is a match then both orders are closed
      and linked, if not the order is just added to the queue in an open state.
 
Any order can have one of 4 states: Open, ClosedAsSecondary, ClosedAsPrimarySale, ClosedAsPrimaryBuy.

When the pair of orders that constitute a Trade operation are marked they are always done in pairs, a primary and a secondary. The
criteria for determining which one is the primary is the last one arriving to the queue.

In any case an order is never removed from the repo because the "GetTrades" method what does is to filter the closed orders that are 
marked as been "PrimarySale" or "PrimaryBuy", and then delivering that collection to the Bll method that create the report of trades.

The interaction with the library is done in the Bll, where there are the three requested methods in the task description:

- SellPumpkin: used by the client to place a sale order. If there is not a corresponding buy order that match the sale, then the return
  value is own "sale order" without been closed. If there was a match then the return value is the corresponding buy order, in this case
  closed.
  
- BuyPumpkin: The same logic that SellPumpkin but opposite.

- GetTrades return a collection of the closed trade operations in human readable format.

And finally an accessory method for adding clients.


The solution contains a project with the unit tests, named "pumpkin_trade_tests". All the test cases proposed in the task description
are covered and run succesfully.

The thread safety is guaranteed by the use of locks each time the collections in the repos are accessed. As every operation returns
always with a result and leave all orders in a defined state, blocking the access to the collection is enough. That means that any 
operation will always take place with all orders in a safe state.

Component state is protected from illegal modifications by the use of encapsulation in the model and only exposing the methods for add
orders and getting the report.

Adding a parameter for amount of pumpkins could be achieved adding a field "quantity" to the order and changing the logic for matching
orders; but here could be two cases:

1- A buy/sale order for more than a pumpking could be satisfied by several sale/buy orders that sum all together match the proposed 
price by the stablished rules.

  This case is more complex, and it's implementation would requiere the use of logic not only based on filtering in the repository 
  methods. In fact it could imply different approaches to the logic of matching orders.

2- A buy/sale order for more than a pumpking could be satisfied only by a sale/buy order that match exactly the same amount.

  This case is more simple, but less real. It would only need to add to the compare of order the new parameter.
  
Expiration time for the order could be implemented including in the filter of qualifiedOrders (OrdersRepository.cs line 41) a condition
to exclude the orders that exceed an established timespan (it could be done too in line 39, but conceptually it is better in 41).

Example:

var qualifyingOrders = order.SellerId != null ? notClosedOrders.Where(ord => ord.BuyerId != null && 
                       ord.Price >= order.Price && DateTime.Now - order.DatePlaced < new TimeSpan(10, 0, 0, 0)
                       ).OrderBy(ord => ord.Price).ToList() :
                       notClosedOrders.Where(ord => ord.SellerId != null && ord.Price < order.Price && 
                       DateTime.Now - order.DatePlaced < new TimeSpan(10, 0, 0, 0)).OrderByDescending(ord => ord.Price).ToList();

It could be implemented too adding a field to the order, and marking the order as "expired" the first time the check is done and it
proceed, but then it will need another filter.

