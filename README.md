## Eventual consistency proof of concept
### Run solution
* Run rabbit mq and managment using docker
```
docker run -d --hostname my-rabbit --name shop -p 8080:15672 -p 5672:5672 rabbitmq:3-management
```
* Run all applications in the solution

### Architecture overview


### Roles
* Customer Client: Send events to the "events" exchange, simulate user activity
* Orders Client: Consumed "customer.updated.event" and "customer.removed.event" do actions with them
* Payment Client: Consumed "customer.updated.event" and "customer.removed.event" but processed only "customer.removed.event"

### Cases
* After message publish to "events" exchange, queues "orders-service", "payment-service" automaticaly get them.
* "orders-service", "payment-service" queues are durable, if one of the Clients is down all messages persisted in the queue, after restart all unprocessed messages processed.
* if message processing terminated in the middle of process, message returns back to the queue and processed again after restart (manual ask). What to do with message order? How to do message process indepotency?

### Queues configuration
#### events exchange
* type: fanout
* durable: true
* autodelete: false 

#### payment-service queue
* durable: true
* autodelete: false 
* exclusive: false
* autoAsk: false

#### order-service queue
* durable: true
* autodelete: false 
* exclusive: false
* autoAsk: false
