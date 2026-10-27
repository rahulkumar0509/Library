Designing a high-performance, low-latency Order Management System (OMS) in **.NET** deployed on **Azure** requires optimizing both the cloud topology (HLD) and the C# runtime pipeline (LLD).

Target metrics for this design:

* **Order Acceptance Latency (P99):** $< 15\text{ ms}$
* **Order Status Query Latency (P99):** $< 3\text{ ms}$
* **Write Throughput:** $20,000+\text{ orders/sec}$ peak

---

## 1. High-Level Design (HLD): Cloud Architecture

An OMS handles two distinct workloads: **Hot Write Path** (Order Placement, Validation, Inventory Reservation) and **Read Path** (Order Status, Tracking). We apply **CQRS** and **Event Sourcing** to decouple write throughput from read scaling.

```
                                [ Client / Front-End ]
                                          |
                        (HTTPS / HTTP/3 via Anycast IP)
                                          |
                             [ Azure Front Door (WAF) ]
                                          |
                           [ Azure API Management ]
                            (JWT, Rate Limiting)
                                          |
                    +---------------------+---------------------+
                    |                                           |
           (gRPC / HTTP/2)                             (HTTP/2 REST)
                    |                                           |
         [ Order Ingestion API ]                      [ Order Read API ]
         (AKS - .NET 8/9 Service)                   (AKS - .NET 8/9 Service)
                    |                                           |
     +--------------+--------------+                            |
     |                             |                            |
(Fast Lock)                  (Async Queue)                (Hot Cache Read)
     |                             |                            |
[ Azure Cache ]           [ Azure Event Hubs ]                  |
[ for Redis   ]            (Partitioned Stream)                 |
(Active-Active)                    |                            |
                            [ Order Worker ]                     |
                            (Batch Processor)                   |
                                   |                            |
                    +--------------+                            |
                    |                                           |
           (Direct TCP Mode)                             (Read Replica)
                    |                                           |
       [ Azure Cosmos DB (NoSQL) ] <--- Change Feed ---> [ Azure Redis L2 ]
       (Multi-Region Write Store)                        (Materialized Views)

```

### Component Breakdown

| Tier | Azure Service | Key Configuration & Purpose |
| --- | --- | --- |
| **Edge Routing** | **Azure Front Door** | Anycast routing, edge TLS termination, DDoS protection, global failover. |
| **API Gateway** | **Azure APIM (Premium)** | Localized token verification (JWT validation without network hop), rate limiting (Token Bucket). |
| **Compute** | **Azure Kubernetes Service (AKS)** | .NET 8/9 Linux containers running on **CBL-Mariner** (optimized minimal kernel) with Horizontal Pod Autoscaler (HPA) targeting CPU + custom Event Hub lag metrics. |
| **Async Messaging** | **Azure Event Hubs (Premium)** | Partitioned event log for order ingestion spikes. `PartitionKey = CustomerId` guarantees ordered processing without bottlenecking global throughput. |
| **In-Memory Cache** | **Azure Cache for Redis Enterprise** | **L2 Cache**: Dual purpose—holds inventory counters for atomic decrements (`DECRBY` via Lua scripts) and serves hot order status queries. |
| **Hot Database** | **Azure Cosmos DB (NoSQL)** | **Primary Write Store**: Configured with Multi-Region Writes and Session/Bounded Staleness consistency. Partitioned by `/customerId` or `/orderId`. |
| **Analytics / Audit** | **Azure Data Explorer (ADX)** | Cosmos DB Change Feed streams raw events to ADX for real-time operational metrics and order history analytics. |

---

## 2. Low-Level Design (LLD): .NET Pipeline Optimizations

To achieve sub-15ms P99 latency, the .NET application code must eliminate CPU thread contention, heap allocations, and unnecessary network round-trips.

### A. Zero-Allocation & Memory Pipeline

1. **Source-Generated JSON:** Eliminate reflection overhead during payload parsing.
```csharp
[JsonSerializable(typeof(OrderRequest))]
[JsonSourceGenerationOptions(PropertyNamingPolicy = JsonKnownNamingPolicy.CamelCase)]
public partial class OrderJsonContext : JsonSerializerContext { }

```


2. **Buffer Pooling (`ArrayPool<T>` & `Span<T>`):** Avoid allocating string or byte arrays when processing order payloads or payload signatures:
```csharp
public async ValueTask ProcessPayloadAsync(ReadOnlyMemory<byte> rawBuffer)
{
    // Use ReadOnlySpan to slice memory without allocating on the heap
    ReadOnlySpan<byte> span = rawBuffer.Span;
    // Parse headers/metadata directly from memory buffer
}

```


3. **`ValueTask<T>` on Hot Execution Paths:** Use `ValueTask` instead of `Task` for methods that complete synchronously on cache hits:
```csharp
public async ValueTask<OrderState?> GetOrderAsync(string orderId)
{
    // If present in L1 In-Memory Cache, return synchronously (Zero Task allocation)
    if (_l1Cache.TryGetValue(orderId, out OrderState state))
        return state;

    return await FetchFromRedisAsync(orderId);
}

```



---

### B. High-Throughput In-Memory Queuing with `System.Threading.Channels`

To avoid thread locks during order ingestion, buffer incoming orders in an bounded, lock-free, in-memory `Channel` before batching them out to Azure Event Hubs:

```csharp
public class OrderIngestionBuffer
{
    private readonly Channel<OrderCommand> _channel;

    public OrderIngestionBuffer(int capacity = 50000)
    {
        var options = new BoundedChannelOptions(capacity)
        {
            SingleReader = true,         // Allows single background worker loop optimizations
            SingleWriter = false,        // Multiple API controllers writing concurrently
            FullMode = BoundedChannelFullMode.Wait
        };
        _channel = Channel.CreateBounded<OrderCommand>(options);
    }

    public ValueTask EnqueueOrderAsync(OrderCommand order, CancellationToken ct) 
        => _channel.Writer.WriteAsync(order, ct);

    public ChannelReader<OrderCommand> Reader => _channel.Reader;
}

```

---

### C. Azure Client SDK Hardening

#### 1. Azure Cosmos DB Client Tuning

By default, the Cosmos DB SDK routes requests through an HTTP gateway and returns full documents on writes. We bypass this:

```csharp
CosmosClientOptions options = new()
{
    // Bypasses HTTP gateway, connects directly to backend data nodes over TCP
    ConnectionMode = ConnectionMode.Direct,
    
    // Avoids echoing full JSON body back over the wire on creation (saves network bandwith)
    EnableContentResponseOnWrite = false, 
    
    // Custom System.Text.Json serializer to avoid Newtonsoft overhead
    Serializer = new CosmosSystemTextJsonSerializer(OrderJsonContext.Default)
};

```

#### 2. Service Bus / Event Hubs Batching

Never publish orders one by one. Accumulate events into an `EventDataBatch` and flush:

```csharp
public async Task FlushBatchAsync(IEnumerable<OrderCommand> orders)
{
    using EventDataBatch batch = await _eventHubClient.CreateBatchAsync();
    foreach (var order in orders)
    {
        var bytes = JsonSerializer.SerializeToUtf8Bytes(order, OrderJsonContext.Default.OrderCommand);
        if (!batch.TryAdd(new EventData(bytes)))
        {
            // Batch full -> send and create a new batch
            await _eventHubClient.SendAsync(batch);
            break;
        }
    }
}

```

---

## 3. Resiliency & Consistency Pattern

### Atomic Inventory Reservation (Preventing Overbooking)

To reserve stock without heavy SQL table locks:

1. Maintain inventory counts inside **Azure Redis Enterprise**.
2. Run an atomic Lua script to validate and decrement stock in a single Redis engine step ($< 1\text{ ms}$):

```lua
-- KEYS[1]: Item ID, ARGV[1]: Quantity
local current_stock = tonumber(redis.call('get', KEYS[1]))
if current_stock and current_stock >= tonumber(ARGV[1]) then
    return redis.call('decrby', KEYS[1], ARGV[1])
else
    return -1 -- Insufficient stock
end

```

3. If Redis succeeds, enqueue the order to **Event Hubs**. If the downstream worker fails to write to Cosmos DB, trigger a compensating transaction (Saga Pattern) to restore Redis inventory.

---

performance optimization
    -- Asynchronous Programming
    𝟗 𝐊𝐞𝐲 𝐒𝐭𝐫𝐚𝐭𝐞𝐠𝐢𝐞𝐬 𝐭𝐨 𝐁𝐨𝐨𝐬𝐭 𝐀𝐏𝐈 𝐏𝐞𝐫𝐟𝐨𝐫𝐦𝐚𝐧𝐜𝐞 💡 

        𝐔𝐬𝐞 𝐂𝐚𝐜𝐡𝐢𝐧𝐠:
        Store frequently accessed data in memory so you don’t have to repeatedly fetch it from the database or other slow sources. This drastically cuts down on response time.

        𝐌𝐢𝐧𝐢𝐦𝐢𝐳𝐞 𝐏𝐚𝐲𝐥𝐨𝐚𝐝 𝐒𝐢𝐳𝐞:
        Send only the necessary data in responses. Avoid sending large, unneeded chunks of data by filtering fields or compressing the payload, which reduces bandwidth usage and speeds up responses.

        𝐔𝐬𝐞 𝐀𝐬𝐲𝐧𝐜𝐡𝐫𝐨𝐧𝐨𝐮𝐬 𝐏𝐫𝐨𝐜𝐞𝐬𝐬𝐢𝐧𝐠:
        Use asynchronous methods for tasks that don’t need an immediate response (like sending emails or processing large data sets). This keeps the API responsive while the heavy work happens in the background.

        𝐋𝐨𝐚𝐝 𝐁𝐚𝐥𝐚𝐧𝐜𝐢𝐧𝐠:
        Distribute incoming API requests across multiple servers to ensure no single server is overloaded. This improves availability and handles more traffic efficiently.

        𝐎𝐩𝐭𝐢𝐦𝐢𝐳𝐞 𝐃𝐚𝐭𝐚 𝐅𝐨𝐫𝐦𝐚𝐭𝐬:
        Use lightweight data formats like JSON or Protocol Buffers instead of XML. Smaller data formats reduce the time spent parsing and transmitting data.

        𝐂𝐨𝐧𝐧𝐞𝐜𝐭𝐢𝐨𝐧 𝐏𝐨𝐨𝐥𝐢𝐧𝐠:
        Reuse existing connections to the database or other services rather than opening a new one for each request. Connection pooling significantly reduces the overhead of establishing connections.

        𝐔𝐬𝐞 𝐂𝐨𝐧𝐭𝐞𝐧𝐭 𝐃𝐞𝐥𝐢𝐯𝐞𝐫𝐲 𝐍𝐞𝐭𝐰𝐨𝐫𝐤𝐬 (𝐂𝐃𝐍𝐬):
        For APIs serving static content (like images or scripts), use CDNs to deliver content faster by caching it closer to the user’s location, reducing latency.

        𝐈𝐦𝐩𝐥𝐞𝐦𝐞𝐧𝐭 𝐀𝐏𝐈 𝐆𝐚𝐭𝐞𝐰𝐚𝐲:
        An API Gateway can help routing requests, handling authentication, rate limiting, and caching. By offloading these tasks from your API, you can improve its overall performance.

        𝐀𝐯𝐨𝐢𝐝 𝐎𝐯𝐞𝐫𝐟𝐞𝐭𝐜𝐡𝐢𝐧𝐠 𝐚𝐧𝐝 𝐔𝐧𝐝𝐞𝐫𝐟𝐞𝐭𝐜𝐡𝐢𝐧𝐠:
        Design your API endpoints to return just the right amount of data. GraphQL, for example, allows clients to request exactly what they need, avoiding overfetching and underfetching issues common in REST APIs.
