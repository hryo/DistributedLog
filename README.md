# DistributedLog

Replicate messages asynchronously from one master to multiple replicas. 
To launch: docker-compose -f .\docker-compose.yml up --build

Master: http://localhost:5000/swagger
Replica1: http://localhost:5001/swagger
Replica2: http://localhost:5002/swagger

Implemented: 
1. Sends messages only to active replicas
2. Healthchecks happens every 2 seconds
3. If message can't be delivered to replica master will retry forever with exponensial backoffs. 
4. ReplicationFactor - when 1 - waits only when message will be stored in master, 2 - waits replication to one replica, 3 - waits replication to both replicas
5. each replica can be update with next configurations: storageDelaySeconds - artifishial delay to test how master waits replicas, isHealthy - replica will throw errors without saving the messages, throwAfterStorageFinished - replica will throw error after message was stored(to test messages deduplication)

Implementation: 
When master receives text message it stores it assigns uniqueu id to it(Guid) and stores it to in memory storage(threadsafe queue).
Next it wraps message in awaitable ReplicationMessage wich propagates next to ReplicationTarget and then will await on that message Completion Task till the moment when required number of targets will replicate message. 
Each target has inmemory Channel(helps us to keep messages in order) and separate Task that is constantly listens for new messages from this Channel. 
When Target managed to deliver message to replica(received 200 from replica's http endpoint) target marks ReplicationMessage as Processed. 

Replica storage: to keep messages in order but achieve dedubplication we use two types of collections:
ConcurrentQueue - to keep messages ordered(here we store only message ids) and ConcurrentDictionary - hash map to verify if message was already stored or not. 