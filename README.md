# Master Parallel Consumer Kafka --- Đang viết chưa xong
MasterKafka là một dự án xử lý Apache Kafka được phát triển một cách chuyên nghiệp bởi HungNV165.
Dự án này là kết quả của sự nghiên cứu sâu và triển khai một cách khoa học, được tính toán thiết kế để đáp ứng các yêu cầu cao cấp về xử lý dữ liệu lớn trên nền tảng Kafka.

**Thông tin Broker Kafka đã có sẵn trên VPS có thể sử dụng.**   
**Lưu ý:** Mạng công ty cần maphost mới truy cập được Kafdrop và Portainer - Để dùng sever có sẵn thì dùng mạng thường hoặc VPN 

1. MapHost by pass proxy (mạng công ty):  
    ```bash
    34.171.40.194 master-kafka-01
	```	

2. Kafdrop:  
    ```bash
    http://34.171.40.194:9091/
	```	
	
3. Portainer:  
    ```bash
    http://34.171.40.194:9000/
	
	Username: admin
	Password: Provanhung77
	```	

3. BootstrapServers (mạng công ty không truy cập được, mạng thường và VPN thì có thể):  
    ```bash
    34.171.40.194:9092
	```	
	
## Cài đặt Kafka bằng docker compose:  

**Để sử dụng được Kafka cần có 3 container cần thiết sau:**   

1. **Zookeeper** - Quản lý Kafka
2. **Kafka** - Kafka broker
3. **Kafdrop** - Công cụ theo dõi và quản lý cần thiết cho việc load test  

Tạo file docker Compose có tên `docker-compose.yaml` như sau:  


```bash
version: '3'
services:
  zookeeper:
    image: wurstmeister/zookeeper
    container_name: zookeeper
    networks:
      - kafka-net
    ports:
      - "2181:2181"

  kafka:
    image: wurstmeister/kafka
    container_name: kafka
    networks:
      - kafka-net
    ports:
      - "9092:9092"
    environment:
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://localhost:9092
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT
      KAFKA_LISTENERS: PLAINTEXT://0.0.0.0:9092
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181

  kafdrop:
    image: obsidiandynamics/kafdrop
    container_name: kafdrop
    ports:
      - "9091:9000"
    environment:
      KAFKA_BROKERCONNECT: localhost:9092
      JVM_OPTS: "-Xms32M -Xmx64M"
    depends_on:
      - kafka

networks:
  kafka-net:
    driver: bridge

```

 **Chạy Docker Compose:**  
  **Lưu ý:** Để trình chạy file docker compose không gặp lỗi thì chú ý các mục sau:  

1. Chú ý `port` đã bị chiếm trong hệ thống chưa: `2181`, `9092`, `9091`.  
2. Chú ý `container_name` đã có chưa: `zookeeper`, `kafka`, `kafdrop`.  
3. Chú ý `networks` đã có chưa: `kafka-net`.  
4. Chú ý cấu hình các `environment` (biến môi trường) phù hợp.  
 
 Mở terminal và di chuyển đến thư mục chứa tệp docker-compose.yml, sau đó chạy lệnh:  
 
 ```bash
  docker-compose up -d
 ```
 
  Truy cập Kafdrop:  
 
 ```bash
  http://localhost:9091
 ```
 
 	
**Lưu lý các biến môi trường sau:**   

1. **Kafka container:**  
 `KAFKA_ADVERTISED_LISTENERS` Nếu dùng host VPS thì để IP như sau:      
 `KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://34.171.40.194:9092`.   
 Với localhost: `KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://localhost:9092`
 
2. **Kafdrop container:**  
 `KAFKA_BROKERCONNECT` Lưu ý nếu dùng VPS thì để IP như sau:  
 `KAFKA_BROKERCONNECT: 34.171.40.194:9092`. 
 Với localhost thì `KAFKA_BROKERCONNECT: localhost:9092`.

 
## Cài đặt Kafka bằng docker run từng container (cách khác nếu không muốn chạy compose):

0. Tạo networks:  
    ```bash
    docker network create kafka-net
	```	

1. Zookeeper container:
    ```bash
	docker run -d --name zookeeper --network kafka-net -p 2181:2181 wurstmeister/zookeeper
	```	
	1.1. Zookeeper container Đối với macbook M1:
    ```bash
    docker run -d --name zookeeper --network kafka-net -p 2181:2181 arm64v8/zookeeper
    ``` 

1. Kafka container:
    ```bash
    docker run -d --name kafka --network kafka-net -p 9092:9092 -e \
    KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://localhost:9092 -e \
    KAFKA_LISTENER_SECURITY_PROTOCOL_MAP=PLAINTEXT:PLAINTEXT -e \
    KAFKA_LISTENERS=PLAINTEXT://0.0.0.0:9092 -e \
    KAFKA_ZOOKEEPER_CONNECT=zookeeper:2181 wurstmeister/kafka
    
    ```
	
1. Kafdrop container:
    ```bash
    docker run -d --name kafdrop -p 9091:9000 -e
	KAFKA_BROKERCONNECT=localhost:9092 -
	e JVM_OPTS="-Xms32M -Xmx64M" obsidiandynamics/kafdrop
    ```
	
	
## Công cụ quản lý container (option):  

Portainer là một công cụ quản lý Docker dựa trên giao diện web, giúp bạn dễ dàng quản lý và giám sát các container Docker trên một hoặc nhiều máy chủ. 
Portainer cung cấp một giao diện người dùng đồ họa thân thiện, 
cho phép người quản trị và người phát triển tương tác với Docker mà không cần sử dụng các lệnh dòng lệnh phức tạp.

**Portainer:**  

1. Cài đặt Portainer:
    ```bash
    docker run -d -p 9000:9000 --name=portainer --restart=always -v
	/var/run/docker.sock:/var/run/docker.sock -v portainer_data:/data portainer/portainer-ce
    ```
	
2. chạy Portainer:
    ```bash
    http://localhost:9000
    ```
	
## Truy cập và quản lý topic Kafka:  
 
 Để sử dụng được kafka cũng ta cần kiểm tra xem container đã run chưa và cấu hình thêm topic cho nó. Cấu hình các partitions để xử linh hoạt hơn.
 Nếu không rành gõ lệnh các bạn có thể cấu hình trực tiếp bằng `Kafdrop` 

1. Exec vào contaier:  
    ```bash
    docker exec -it kafka /bin/bash
	```	

2. Tạo topic:  
    ```bash
    kafka-topics.sh --create --topic topic-events1 --bootstrap-server localhost:9092
	```	
	
3. Tạo topic và partition:  
    ```bash
    kafka-topics.sh --create --topic topic-events2 --partitions 5 --replication-factor 1
	--bootstrap-server localhost:9092
	```	
	
4. Tăng số lượng partitions:  
    ```bash
    kafka-topics.sh --bootstrap-server localhost:9092 --topic topic-events2 --alter --partitions 3
	```		

5. Xem mô tả topic:  
    ```bash
    kafka-topics.sh --bootstrap-server localhost:9092 --topic topic-events2 --describe
	```	
6. Producer:  
    ```bash
    kafka-console-producer.sh --topic topic-events2 --bootstrap-server localhost:9092
	```		

7. Consumer:  
    ```bash
    kafka-console-consumer.sh --topic topic-events2 --from-beginning --bootstrap-server localhost:9092
	```	
	
## Xử lý code Producer:

 Mô tả ở đây: 

Concep:

 
1. Curl push mesage:  
    ```bash
    curl --location 'http://localhost:5001/Home/Privacy' \
     --header 'Content-Type: application/json' \
     --data-raw '{
       "id": 1,
       "Name": "hung"
     }'
	```	
1. Producer hàng loạt: 

    ```csharp
	var config = new ProducerConfig
	{
		BootstrapServers = "localhost:9092"
	};

	var topics = new List<string>() { "topic-events1", "topic-events2" };

	Parallel.For(0, topics.Count, i =>
	{
		var topic = topics[i];
		var numMessages = 20000;

		for (int j = 1; j <= numMessages; j++)
		{
			var message = new Message<Null, string>
			{
				Value = $"message {j} for {topic}"
			};

			// Gọi hàm produce message theo từng topic
			_messageBroker.ProducePushMessage(topic, config, message, message.Value);
		}
	});

	```	
		
2. Code xử lý code : 

    ```csharp
    public class KafkaProducer : IKafkaProducer
    {
        public async Task<bool> ProducePushMessage(string topic, ProducerConfig config, object objRequest)
        {
            var log = new StringBuilder();
            using var producer = new ProducerBuilder<Null, string>(config).Build();
            try
            {
                var jsonObj = JsonConvert.SerializeObject(objRequest);
                var message = new Message<Null, string> { Value = jsonObj };
                var result = await producer.ProduceAsync(topic, message);

                log.AppendLine($"Input: {jsonObj}");
                log.AppendLine($"Delivered: {JsonConvert.SerializeObject(result.Value)} to: {result.TopicPartitionOffset}");
                return true;
            }
            catch (ProduceException<Null, string> e)
            {
                log.AppendLine($"Delivery failed: {e.Error.Reason}");
                return false;
            }
            finally
            {
                Console.WriteLine(log);
                LoggingHelper.SetLogStep(log.ToString());
            }
        }
    }
	```	
	
	Giải thích
	
	
## Xử lý code Consumer:

 Mô tả ở đây: 

 Concep:

 
1. Curl push mesage:  
    ```bash
    curl --location 'http://localhost:5001/Home/Privacy' \
     --header 'Content-Type: application/json' \
     --data-raw '{
       "id": 1,
       "Name": "hung"
     }'
	```	
	
1. Hướng dẫn sử dụng : 

    ```csharp	
            var topic5 = "events5";
            // More Topic

            _consumerManager.AddConsumer(topic5, 
                message => { _orderProcess.CreateOrderProcess(message, topic5); return Task.CompletedTask; }, 
                KafkaConfiguration.ConsumerConfig); // Configuration can be changed
	```	

	Giải thích