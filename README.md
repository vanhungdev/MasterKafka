# Master Parallel Consumer Kafka

MasterKafka là một dự án xử lý Apache Kafka được phát triển một cách chuyên nghiệp bởi HungNV165.
Dự án này là kết quả của sự nghiên cứu sâu và triển khai một cách khoa học, được tính toán thiết kế để đáp ứng các yêu cầu cao cấp về xử lý dữ liệu lớn trên nền tảng Kafka.


## Cài Đặt Kafka bằng docker Compose:  

**Để sử dụng được Kafka cần có 3 container cần thiết sau:**   

1. **Zookeeper** - Quản lý Kafka
2. **Kafka** - Kafka Broker
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
      KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://34.171.40.194:9092
      KAFKA_LISTENER_SECURITY_PROTOCOL_MAP: PLAINTEXT:PLAINTEXT
      KAFKA_LISTENERS: PLAINTEXT://0.0.0.0:9092
      KAFKA_ZOOKEEPER_CONNECT: zookeeper:2181

  kafdrop:
    image: obsidiandynamics/kafdrop
    container_name: kafdrop
    ports:
      - "9091:9000"
    environment:
      KAFKA_BROKERCONNECT: 34.171.40.194:9092
      JVM_OPTS: "-Xms32M -Xmx64M"
    depends_on:
      - kafka

networks:
  kafka-net:
    driver: bridge

```

**Cài đặt kafka bằng docker run từng contaier:**   

0. Tạo networks:  
    ```bash
    docker network create kafka-net
	```	

1. zookeeper contaier
    ```docker run -d --name zookeeper --network kafka-net -p 2181:2181 wurstmeister/zookeeper
	```	
	
1. Kafka contaier
    ```bash
    docker run -d --name kafka --network kafka-net -p 9092:9092 -e
	KAFKA_ADVERTISED_LISTENERS=PLAINTEXT://34.171.40.194:9092 -e
	KAFKA_LISTENER_SECURITY_PROTOCOL_MAP=PLAINTEXT:PLAINTEXT -e
	KAFKA_LISTENERS=PLAINTEXT://0.0.0.0:9092 -e
	KAFKA_ZOOKEEPER_CONNECT=zookeeper:2181 wurstmeister/kafka
    ```
	
1. kafdrop contaier
    ```bash
    docker run -d --name kafdrop -p 9091:9000 -e
	KAFKA_BROKERCONNECT=34.171.40.194:9092 -
	e JVM_OPTS="-Xms32M -Xmx64M" obsidiandynamics/kafdrop
    ```
	
	
**Lưu lý các biến môi trường sau:**   

1. **Kafka container:**  
 `KAFKA_ADVERTISED_LISTENERS` nếu dùng host VPS thì để ip như sau:
 `KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://34.171.40.194:9092`  
 localhost: `KAFKA_ADVERTISED_LISTENERS: PLAINTEXT://localhost:9092`
 
2. **Kafdrop container:**  
 `KAFKA_BROKERCONNECT` lưu ý nếu dùng VPS:
 `KAFKA_BROKERCONNECT: 34.171.40.194:9092` dùng ip của VPS

## Cài Đặt Kafka bằng docker Compose:  

Để cài đặt Akka.Streams.Kafka, bạn có thể sử dụng NuGet. Chạy lệnh sau trong Package Manager Console:

