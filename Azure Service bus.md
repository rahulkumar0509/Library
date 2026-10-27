We use Azure service bus to define Queue system which can be used to asynchronously send message from one service to another service. For e.g., OrderService send Payment request to PaymentService. 

In one case like Big Basket, we place the order and sends the payment request and do not wait for it.
Once payment is done it will confirm back to order service, if it fails then user gets COD or retry payment option.

This is Asynchronous call, Order service sends message to Azure service bus Queue. 
Payment service have one backgroundService which keeps listeneing to any event from same queue.

## Setup 


ReplyTo versus separate response queue
Read dead letter queue
MessageId

Store MessageId and 

Negative scenarios
=============================
1. Message did not reach Product Service

use retry:
    var client =
new ServiceBusClient(
 connectionString,
 new ServiceBusClientOptions
 {
    RetryOptions =
    {
       MaxRetries = 5,
       Delay = TimeSpan.FromSeconds(2)
    }
 });

 
2. Product Service is down
Data is still in queue and when product service is back all the data comes to product services.

3. Duplicate Messages:
    Store processed message IDs in Table.,
    check for exisitng Messages id in consumer whenveer the events triggers.
    if(exists)
    {
        await args.CompleteMessageAsync(args.Message);
        return;
    }
    process the message
    add the new message id.

4. DLQ
Dead letter queue
define new ServiceBusProcessorOptions
{
    MaxConcurrentCalls = 5
}
Max Delivery Count = 10 // no of attmepts to deliver message.
after 10 retry it will move the message to dead letter queue
